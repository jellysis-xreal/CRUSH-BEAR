using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Information")] [SerializeField]
    private uint waveNum = 0; // Wave number
    [SerializeField] private WaveType currentWave; // 진행 중인 Wave Type
    [SerializeField] private WaveState currentState;
    public float waveTime = 0.0f; // 흘러간 Wave Time
    [SerializeField] public float waveBeat = 0.0f; // 흘러간 Wave beat
    private WaveState beforeState;
    private float _oneBeat;
    private float _beat;
    private int _beatNum = 0;
    
    [Header("Music Information")] public uint waveMusicGUID; // 현재 세팅된 Music의 GUID
    public DataManager.MusicData CurMusicData; // 현재 세팅된 Music data

    [Header("setting")] [SerializeField] private GameObject RightInteraction;
    [SerializeField] private GameObject LeftInteraction;
    [SerializeField] private NodeInstantiator_minha nodeInstantiator;

    // 기획에 따른 변수
    private int waveTypeNum = 3; // Wave Type 갯수
    [SerializeField] private List<GameObject> _toppingArea = new List<GameObject>();

    // wave 일시정지 기능을 구현하기 위한 변수
    private bool IsWait = false;
    private float timerDuration = 2f;
    private float waitTimer;
    private bool hasSetted = false;
    private bool IsPause = false;

    // wave 전환을 위한 변수
    private enum WaveState
    {
        Init,       //새로운 Wave로 Enter 하는 중
        Waiting,    //잠시 대기 중
        Playing     //Wave 진행 중
    }


    public void Init()
    {
        Debug.Log("Initialize WaveManager");

        // Wave Num
        waveNum = 0;
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
    }

    public void Test()
    {
        // currentWave = WaveType.Hitting;
        // SetThisWave();
    }

    public WaveType GetWaveType()
    {
        return currentWave;
    }

    /// <summary>
    /// Wave 전환 시 사용함.
    /// Random으로 다음 wave를 지정하고, 세팅합니다.
    /// </summary>
    public void NextWaveSetting()
    {
        // Random으로 다음 wave를 지정
        //currentWave = GetRandomWave();
        currentWave = WaveType.Hitting; // TODO: 임시설정. For Test

        // Wave 세팅
        SetWavePlayer(); // Player의 Interact 세팅
        // TODO: Scene 내의 점수판 세팅
        // TODO: Scene 내의 조명 세팅
        waveMusicGUID = 0; // TODO: 임시로 GUID 0번으로 세팅
        CurMusicData = GameManager.Data.GetMusicData(waveMusicGUID); //받아올 Music Data 세팅
        _oneBeat = 60.0f / CurMusicData.BPM;
        _beat = _oneBeat;
        
        nodeInstantiator.InitToppingPool(currentWave); //Topping Pool 세팅
    }

    public void NextWaveStart()
    {
        Debug.Log("[WAVE] Wave Start");
        waveNum++;
        waveTime = 0.0f;
        GameManager.Sound.PlayWaveMusic(waveMusicGUID); //음악 start
        // 노드는 Time.timeScale == 1일 경우 자동으로 Update 됨.
    }

    public Vector3 GetSpawnPosition(int index)
    {
        return _toppingArea[(int)currentWave].transform.GetChild(index).transform.position;
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
        switch (currentState)
        {
            case WaveState.Init:
                // TODO: Wave를 지정하는 효과 ON(240108)
                NextWaveSetting();
                Debug.Log("[WAVE] Wave Initialize, 잠시 대기하는 중입니다.");
                beforeState = WaveState.Init;
                currentState = WaveState.Waiting;
                break;
            
            case WaveState.Playing:
                waveTime += Time.deltaTime;
                beforeState = WaveState.Playing;
                break;

            case WaveState.Waiting:
                SetPauseWave();
                ContinueWave();
                break;
        }
    }

    
    [ContextMenu("DEBUG/SetPauseWave()")] //TODO: For Test, 이후 제거하기
    public void SetPauseWave()
    {
        Debug.Log("[WAVE] Wave Pause");
        // Wave 진행을 일시정지 시킵니다.
        if (!IsPause)
        {
            IsPause = true;
            Time.timeScale = 0;
        }
    }

    [ContextMenu("DEBUG/ContinueWave()")] //TODO: For Test, 이후 제거하기
    public void ContinueWave()
    {
        Debug.Log("[WAVE] Wave Continue");
        // __초 뒤에 Wave 일시정지를 해제합니다.
        if (IsPause)
        {
            if (beforeState == WaveState.Init)
                StartCoroutine(InitWaitSeconds(5.0f));
            else if (beforeState == WaveState.Playing)
                StartCoroutine(WaitSeconds(3.0f));
            
            currentState = WaveState.Playing;
        }
    }

    IEnumerator InitWaitSeconds(float sec)
    {
        yield return new WaitForSecondsRealtime(sec);
        NextWaveStart();
        CallContinueSetting();
    }

    IEnumerator WaitSeconds(float sec)
    {
        yield return new WaitForSecondsRealtime(sec);
        CallContinueSetting();
    }
    

    private void CallContinueSetting()
    {
        Debug.Log("[WAVE] Wave 일시중지 해제함");
        IsPause = false;
        Time.timeScale = 1;
    }

    public void SetIsPause(bool _isPause)
    {
        IsPause = _isPause;
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

    public void UpdateBeat()
    {
        if (waveTime > _beat)
        {
            //Debug.Log("[WAVE BEAT] " + _beatNum + "beat");
            _beatNum++;
            _beat += _oneBeat;
        }
    }
}
