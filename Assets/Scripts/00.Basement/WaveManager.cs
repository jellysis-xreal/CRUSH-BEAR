using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    [FormerlySerializedAs("waveNum")]
    [Header("Wave Information")] 
    // [SerializeField] private uint currenWaveNum = 0; // Wave number
    // [SerializeField] private uint endWaveNum = 0; // 진행할 웨이브 전체 숫자.
    public uint currenWaveNum = 0; // Wave number
    public uint endWaveNum = 0; // 진행할 웨이브 전체 숫자.
    [SerializeField] private WaveType currentWave; // 진행 중인 Wave Type
    [SerializeField] private WaveState currentState;
    public float waveTime = 0.0f; // 흘러간 Wave Time
    [SerializeField] public float waveBeat = 0.0f; // 흘러간 Wave beat
    private WaveState beforeState;
    private float _oneBeat;
    private float _beat;
    private int _beatNum = 0;
    private Coroutine _waitBeforePlayingCoroutine;
    private Coroutine _waitAfterPlayingCoroutine;
    
    [Header("Music Information")] public uint waveMusicGUID; // 현재 세팅된 Music의 GUID
    public DataManager.MusicData CurMusicData; // 현재 세팅된 Music data

    [Header("setting")] [SerializeField] private GameObject RightInteraction;
    [SerializeField] private GameObject LeftInteraction;
    [SerializeField] private NodeInstantiator_minha nodeInstantiator;
    [SerializeField] private GameObject nodeArrivalArea;
    [SerializeField] private GameObject nodeArrivalUI;

    // 기획에 따른 변수
    [SerializeField] private int waveTypeNum = 3; // Wave Type 갯수
    [SerializeField] private List<GameObject> _toppingArea = new List<GameObject>();

    // wave 일시정지 기능을 구현하기 위한 변수
    // 일단 사용하지 않아서 주석처리
    /*private bool _isWait = false;
    private float _timerDuration = 2f;
    private float _waitTimer;
    private bool _hasSet = false;*/
    private bool _isPause = false;

    // wave 전환을 위한 변수
    public enum WaveState
    {
        Init,       //새로운 Wave로 Enter 하는 중
        Waiting,    //잠시 대기 중
        Playing,     //Wave 진행 중
        End
    }


    public void Init()
    {
        Debug.Log("Initialize WaveManager");

        // Wave Num
        waveTime = 0.0f;

        RightInteraction = Utils.FindChildByRecursion(GameManager.Player.RightController.transform, "Interaction")
            .gameObject;
        LeftInteraction = Utils.FindChildByRecursion(GameManager.Player.LeftController.transform, "Interaction")
            .gameObject;

        // Topping이 생성될 위치 초기화. **Hierarchy 주의**
        for (int i = 0; _toppingArea.Count < waveTypeNum && i < waveTypeNum; i++)
        {
            //Debug.Log(this.transform.GetChild(0).GetChild(i).gameObject.name);
            _toppingArea.Add(this.transform.GetChild(0).GetChild(i).gameObject);
        }
        
        // Topping이 도착할 위치 초기화. **Hierarchy 주의**
        nodeArrivalArea = transform.GetChild(1).gameObject;
        
        // Topping이 이동하는 Arrival UI 초기화. **Hierarchy 주의**
        nodeArrivalUI = transform.GetChild(2).gameObject;

    }
    
    public WaveType GetWaveType()
    {
        return currentWave;
    }

    public Vector3 GetSpawnPosition(int index)
    {
        int TypeNum = (int)currentWave;
        return _toppingArea[TypeNum].transform.GetChild(index).transform.position;
    }

    public Vector3 GetArrivalPosition(int index)
    {
        int TypeNum = (int)currentWave;
        Transform CurArrive = nodeArrivalArea.transform.GetChild(TypeNum);
        return CurArrive.GetChild(index).transform.position;
    }

    private WaveType GetRandomWave()
    {
        WaveType temp;
        // 진행하던 wave와 중복되지 않도록 random하게 돌림
        do
        {
            temp = (WaveType)Random.Range(0, 3);
        } while (temp != currentWave);

        return temp;
    }

    private void SetWavePlayer()
    {
        RightInteraction.transform.GetChild(0).gameObject.SetActive(false);
        RightInteraction.transform.GetChild(1).gameObject.SetActive(false);
        RightInteraction.transform.GetChild(2).gameObject.SetActive(false);

        LeftInteraction.transform.GetChild(0).gameObject.SetActive(false);
        LeftInteraction.transform.GetChild(1).gameObject.SetActive(false);
        LeftInteraction.transform.GetChild(2).gameObject.SetActive(false);

        int TypeNum = (int)currentWave;
        Debug.Log($"TypeNum : {TypeNum}");
        RightInteraction.transform.GetChild(TypeNum).gameObject.SetActive(true);
        LeftInteraction.transform.GetChild(TypeNum).gameObject.SetActive(true);
    }

    private void SetWavePlay()
    {
        int TypeNum = (int)currentWave;
        
        // Node 도착 지점 설정
        nodeArrivalArea.transform.GetChild(0).gameObject.SetActive(false);
        nodeArrivalArea.transform.GetChild(1).gameObject.SetActive(false);
        nodeArrivalArea.transform.GetChild(2).gameObject.SetActive(false);
        nodeArrivalArea.transform.GetChild(TypeNum).gameObject.SetActive(true);
        
        // Node Line UI 설정
        nodeArrivalUI.transform.GetChild(0).gameObject.SetActive(false);
        nodeArrivalUI.transform.GetChild(1).gameObject.SetActive(false);
        nodeArrivalUI.transform.GetChild(2).gameObject.SetActive(false);
        nodeArrivalUI.transform.GetChild(TypeNum).gameObject.SetActive(true);
    }

    private void Update()
    {
        // 현재 Wave manager가 작동하는 상황이라면, Wave State를 업데이트 합니다.
        if (GameManager.Instance.currentGameState == GameState.Waving)
        {
            UpdateWaveState();
            UpdateBeat();
        }
    }

    private void UpdateWaveState()
    {
        // Init -> Waiting -> Playing(노래(wave) 재생 중..) -> Waiting(노래(wave) 종료) -> Init -> 반복하다 비트 끝나면 End
        switch (currentState)
        {
            case WaveState.Init:
                // TODO: Wave를 지정하는 효과 ON(240108)
                NextWaveSetting();
                Debug.Log("[WAVE] Wave Initialize(next wave setting), 잠시 대기하는 중입니다.");
                beforeState = WaveState.Init;
                currentState = WaveState.Waiting;
                Debug.Log("[Wave] Change wave State Init to Waiting!");
                break;
            
            case WaveState.Playing:
                waveTime += Time.deltaTime;
                beforeState = WaveState.Playing;
                // current == WaveState.Playing -> Waiting 바꾸는 코드, UpdateBeat()에 구현
                break;

            case WaveState.Waiting:
                SetPauseWave();
                ContinueWave(beforeState);
                beforeState = WaveState.Waiting;
                break;
            case WaveState.End:
                
                
                break;
        }
    }
    /// <summary>
    /// Wave 전환 시 사용함.
    /// Random으로 다음 wave를 지정하고, 세팅합니다.
    /// </summary>
    public void NextWaveSetting()
    {
        // Random으로 다음 wave를 지정
        //currentWave = GetRandomWave();
        // TODO: 임시설정. For Test
        waveTime = 0;
        _beatNum = 0;
        currenWaveNum++;

        if (currenWaveNum % 2 == 0) currentWave =WaveType.Punching; 
        else currentWave = WaveType.Hitting;
        // currentWave = WaveType.Hitting; // 

        // Wave 세팅
        SetWavePlayer(); // Player의 Interact 세팅
        
        // TODO: Scene 내의 점수판, 조명, 노드 도착지점 세팅
        SetWavePlay();

        // 음악 세팅
        waveMusicGUID = 0; // TODO: 임시로 GUID 0번으로 세팅
        CurMusicData = GameManager.Data.GetMusicData(waveMusicGUID); //받아올 Music Data 세팅
        Debug.Log($"[Wave] : received Music Data. Music GUID {CurMusicData.GUID}");
        _oneBeat = 60.0f / CurMusicData.BPM;
        _beat = _oneBeat;
        
        nodeInstantiator.InitToppingPool(currentWave); //Topping Pool 세팅
    }


    [ContextMenu("DEBUG/SetPauseWave()")] //TODO: For Test, 이후 제거하기
    public void SetPauseWave()
    {
        if (!_isPause)
        {
            Debug.Log("[WAVE] Wave Pause");
            // Wave 진행을 일시정지 시킵니다.
            
            _isPause = true;
            Time.timeScale = 0;
            // timeScale 변경 필요
        }
    }

    [ContextMenu("DEBUG/ContinueWave()")] //TODO: For Test, 이후 제거하기
    public void ContinueWave(WaveState waveState)
    {
        if (_isPause)
        {
            // Debug.Log("[WAVE] Wave Continue");
            // __초 뒤에 Wave 일시정지를 해제합니다.
            
            if (beforeState == WaveState.Init && _waitBeforePlayingCoroutine == null)
                _waitBeforePlayingCoroutine = StartCoroutine(WaitBeforePlaying(5.0f, waveState));
                
            else if (beforeState == WaveState.Playing && _waitAfterPlayingCoroutine == null)
                _waitAfterPlayingCoroutine = StartCoroutine(WaitAfterPlaying(3.0f, waveState));
            
            //currentState = WaveState.Playing; 
            // Waiting -> Playing state 관리 
        }
    }
    
    // Init -> Waiting -> Playing(노래(wave) 재생 중..) -> Waiting(노래(wave) 종료) -> Init -> 반복하다 비트 끝나면 End
    // Waiting -> Playing
    IEnumerator WaitBeforePlaying(float sec, WaveState waveState)
    {
        Debug.Log($"[Wave] State : Waiting -> Playing Wait {sec}s. (이제 Wave 시작한다? 세팅 후에 게임 시작 전 대기 시간을 가짐. 플레이어 준비 시간.) ");
        yield return new WaitForSecondsRealtime(sec);
        CallContinueSetting(waveState);
        _waitBeforePlayingCoroutine = null;
    }
    // Waiting -> Init -> Playing 
    IEnumerator WaitAfterPlaying(float sec, WaveState waveState)
    {
        Debug.Log($"[Wave] State : Playing -> Waiting Wait {sec}s. (이제 Wave 끝났다? 다음 Wave 시작 전 혹은 게임 종료 전 대기 시간) ");
        yield return new WaitForSecondsRealtime(sec);
        CallContinueSetting(waveState);
        _waitBeforePlayingCoroutine = null;
    }
    

    private void CallContinueSetting(WaveState waveState)
    {
        Debug.Log("[WAVE] Wave 일시중지 해제함");
        _isPause = false;
        Time.timeScale = 1;
        
        // 진행된 wave가 최종 wave 수와 같아지면 게임 종료, wave가 남았다면 Next Wave Start
        if (currenWaveNum == endWaveNum) EndGame();
        else if(currenWaveNum < endWaveNum && waveState == WaveState.Init) NextWaveStart();
        else if(currenWaveNum < endWaveNum && waveState == WaveState.Playing) NextWaveInit();
    }
    private void NextWaveStart()
    {
        Debug.Log("[WAVE] Wave Start");
        currentState = WaveState.Playing;
        waveTime = 0.0f;
        GameManager.Sound.PlayWaveMusic(waveMusicGUID); //음악 start
        // 노드는 Time.timeScale == 1일 경우 자동으로 Update 됨.
    }

    private void NextWaveInit()
    {
        Debug.Log("[WAVE] Next Wave Init");
        currentState = WaveState.Init;
    }

    private void EndGame()
    {
        // 모든 웨이브가 종료되었을 때 호출.
        currentState = WaveState.End;
        Debug.Log("게임 종료!");
    }
    public void SetIsPause(bool _isPause)
    {
        this._isPause = _isPause;
        if (_isPause)
        {
            // 소리 끄기
        }
        else
        {
            // 소리 3초 후 틀기
            Debug.Log("Resume the game after 3 sec...");

        }
    }
    
    // Update에서 반복, 비트가 남았을 경우 계속 진행(beatNum, beat값 수정), 모든 비트가 마무리된 경우 currentState -> Waiting으로 전환 
    public void UpdateBeat()
    {
        if (waveTime > _beat) // 조건 : 1beat 시간이 흘렀을 경우 한 번 호출
        {
            //Debug.Log("[WAVE BEAT] " + _beatNum + "beat");
            
            // 존재 비트 모두 플레이 했을 때 State : Playing -> Waiting으로 전환
            if (CurMusicData.BeatNum == _beatNum)
            {
                Debug.Log($"[Wave] : Detected All Beat is Done");
                currentState = WaveState.Waiting;
            }
            
            _beatNum++;
            _beat += _oneBeat;
        }
    }
}
