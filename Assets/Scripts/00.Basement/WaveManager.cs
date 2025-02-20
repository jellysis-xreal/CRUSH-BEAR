﻿using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using EnumTypes;
using UnityEngine.Serialization;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;
using TMPro;
using System.Threading;
using Unity.VisualScripting;

public class WaveManager : MonoBehaviour
{
    [FormerlySerializedAs("waveNum")]
    [Header("----+ Wave Setting +----")] 
    public WaveDifficulty waveDifficulty = WaveDifficulty.Easy;
    public uint endWaveNum = 0; // 진행할 웨이브 전체 숫자.
    public WaveType firstWaveType = WaveType.Punching;
    public SpriteAtlas waveSpriteAtlas;
    
    [Header("----+ Wave Information +----")] 
    // [SerializeField] private uint currenWaveNum = 0; // Wave number
    // [SerializeField] private uint endWaveNum = 0; // 진행할 웨이브 전체 숫자.
    public uint currenWaveNum = 0; // Wave number
    public WaveType currentWave; // 진행 중인 Wave Type
    public float waveTime = 0.0f; // 흘러간 Wave Time
    [SerializeField] public int currentBeatNum = 0;
    [SerializeField] private WaveState beforeState;
    [SerializeField] private WaveState currentState;
    [SerializeField] public int loadedBeatNum = 0; // 로딩된 Wave beat
    [HideInInspector] public WaveType beforeWave; // 이전 Wave Type

    private float _oneBeat;
    private float _beat;
   
    private UniTask _waitBeforePlayingCoroutine;
    private UniTask _waitAfterPlayingCoroutine;
    private CancellationTokenSource _waitBeforePlayingCTS, _waitAfterPlayingCTS;
    
    [Header("----+ Music Information +----")] 
    public uint waveMusicGUID; // 현재 세팅된 Music의 GUID
    public DataManager.MusicData CurMusicData; // 현재 세팅된 Music data
    public int stageID;

    [Header("----+ setting +----")]
    //[SerializeField] private GameObject RightInteraction;
    //[SerializeField] private GameObject LeftInteraction;
    public NodeInstantiator nodeInstantiator;
    [SerializeField] private GameObject nodeArrivalArea;
    [SerializeField] private GameObject nodeArrivalUI;
    [SerializeField] public GameObject[] timerCanvas;
    [SerializeField] public GameObject[] waveUICanvas;
    public Image[] waveUIImages;
    private int countdownTime = 4;
    private TextMeshProUGUI timer;
    private GameObject _settingUI; // 캐싱을 위한 변수

    // 기획에 따른 변수
    private int waveTypeNum = 3; // Wave Type 종류의 갯수
    [SerializeField] private List<GameObject> _toppingArea = new List<GameObject>();

    // Wave 전환 사이의 Wait Time
    public IndicatorController indicatorController;
    private bool _isPause = false;
    private bool _IsManagerInit = false;
    
    // controller speed setting을 위함
    private float _curSettingTime = 0.0f;
    private float _settingTime = 5.0f;

    private Transform _rightInteraction, _rightType1, _rightType2, _rightType3;
    private Transform _leftInteraction, _leftType1, _leftType2, _leftType3;
    
    // wave 전환을 위한 변수
    public enum WaveState
    {
        Init,       //새로운 Wave로 Enter 하는 중
        Waiting,    //잠시 대기 중
        Playing,     //Wave 진행 중
        Pause,
        End,
        CheckResult
    }
    
    public enum WaveDifficulty
    {
        Easy,
        Hard
    }

    public void Init()
    {
        //Debug.Log("Initialize WaveManager");
        waveUIImages = new Image[waveUICanvas.Length];
        waveUIImages[0] = waveUICanvas[0].GetComponent<Image>();
        waveUIImages[1] = waveUICanvas[1].GetComponent<Image>();
        waveUIImages[2] = waveUICanvas[2].GetComponent<Image>();
        
        // Wave Num
        waveTime = 0.0f;
        
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

        _settingUI = GameObject.FindWithTag("SettingUI");
        indicatorController = GameObject.FindWithTag("Indicator").GetComponent<IndicatorController>();
        
        _rightInteraction = GameManager.Player.RightInteraction.transform;
        _rightType1 = _rightInteraction.GetChild(0);
        _rightType2 = _rightInteraction.GetChild(1);
        _rightType3 = _rightInteraction.GetChild(2);
        
        _leftInteraction = GameManager.Player.LeftInteraction.transform;
        _leftType1 = _leftInteraction.GetChild(0);
        _leftType2 = _leftInteraction.GetChild(1);
        _leftType3 = _leftInteraction.GetChild(2);

        /*// 난이도 설정
        switch (waveDifficulty)
        {
            case WaveDifficulty.Easy:
                currenWaveNum = 0;
                endWaveNum = 4;
                waveMusicGUID = 0;
                break;
            
            case WaveDifficulty.Hard:
                currenWaveNum = 2;
                endWaveNum = 4;
                waveMusicGUID = 2;
                break;
        }*/
        currenWaveNum = 0;
        endWaveNum = 4;
        waveMusicGUID = 0;
        
        currentState = WaveState.Init;
        beforeState = WaveState.Init;

        _isPause = false;
        _IsManagerInit = false;
        _curSettingTime = 0.0f;
        _settingTime = 5.0f;
    }
    
    public WaveType GetWaveType()
    {
        return currentWave;
    }

    public Vector3 GetSpawnPosition(int index)
    {
        //if (index < 0 || index > 3)
        //[XMC]Debug.Log("[ERROR] " + index);
        int TypeNum = (int)currentWave;
        return _toppingArea[TypeNum].transform.GetChild(index).transform.position;

        /*if (TypeNum >= 0 && TypeNum <= 3)
        {
        }
        else
        {
            return Vector3.zero;
        }*/
    }   

    public Vector3 GetArrivalPosition(int index)
    {
        //if (index < 0 || index > 3)
        //[XMC]Debug.Log("[ERROR] " + index);
        int TypeNum = (int)currentWave;
        Transform CurArrive = nodeArrivalArea.transform.GetChild(TypeNum);
        return CurArrive.GetChild(index).transform.position;
    }

    public Transform GetWaveScoreUI()
    {
        int TypeNum = (int)currentWave;
        return nodeArrivalUI.transform.GetChild(TypeNum);
    }
    
    public void SetWavePlayer()
    {
        _rightType1.gameObject.SetActive(false);
        _rightType2.gameObject.SetActive(false);
        _rightType3.gameObject.SetActive(false);

        _leftType1.gameObject.SetActive(false);
        _leftType2.gameObject.SetActive(false);
        _leftType3.gameObject.SetActive(false);

        int TypeNum = (int)currentWave;
        
        _rightInteraction.GetChild(TypeNum).gameObject.SetActive(true);
        _leftInteraction.GetChild(TypeNum).gameObject.SetActive(true);
    }

    public void SetWavePlayer(WaveType type)
    {
        _rightType1.gameObject.SetActive(false);
        _rightType2.gameObject.SetActive(false);
        _rightType3.gameObject.SetActive(false);

        _leftType1.gameObject.SetActive(false);
        _leftType2.gameObject.SetActive(false);
        _leftType3.gameObject.SetActive(false);
        
        _rightInteraction.GetChild((int)type).gameObject.SetActive(true);
        _leftInteraction.GetChild((int)type).gameObject.SetActive(true);
    }

    private void SetWavePlay()
    {
        int TypeNum = (int)currentWave;
        //[XMC]Debug.Log($"TypeNum : {TypeNum}");
        // Node 도착 지점 설정
        nodeArrivalArea.transform.GetChild(0).gameObject.SetActive(false);
        nodeArrivalArea.transform.GetChild(1).gameObject.SetActive(false);
        nodeArrivalArea.transform.GetChild(2).gameObject.SetActive(false);
        nodeArrivalArea.transform.GetChild(TypeNum).gameObject.SetActive(true);
        
        // PlayScene Node UI 설정
        GameManager.Player.PlaySceneUIInit(TypeNum);
        
        // Wave indicator 세팅
        waveUIImages[TypeNum].sprite = waveSpriteAtlas.GetSprite("wave_"+currenWaveNum.ToString());
        
        indicatorController.SetWaveIndicator(currenWaveNum, beforeWave, currentWave);
    }

    public void SetWaveTutorial()
    {
        nodeArrivalArea.transform.GetChild(0).gameObject.SetActive(false);
        nodeArrivalArea.transform.GetChild(1).gameObject.SetActive(false);
        nodeArrivalArea.transform.GetChild(2).gameObject.SetActive(false);
        nodeArrivalArea.transform.GetChild((int)currentWave).gameObject.SetActive(true);
    }
    public void FinishWavePlay()
    {
        int TypeNum = (int)currentWave;
        // Node 도착 지점 설정
        nodeArrivalArea.transform.GetChild(0).gameObject.SetActive(false);
        nodeArrivalArea.transform.GetChild(1).gameObject.SetActive(false);
        nodeArrivalArea.transform.GetChild(2).gameObject.SetActive(false);
        
        // PlayScene Node UI 설정
        GameManager.Player.FinishSceneUI();
    }
    
    // Fixedupdate -> 프레임
    private void Update() // 프레임에 의존적
    {
        if (GameManager.Instance.currentGameState != GameState.Waving)
            return;
        
        if (_curSettingTime < _settingTime)
        {
            _curSettingTime += Time.deltaTime;
        }
        else if (!_IsManagerInit)
        {
            if (_settingUI == null)
                _settingUI = GameObject.FindWithTag("SettingUI");
            _settingUI.SetActive(false);
            _IsManagerInit = true;
        }

        // 현재 Wave manager가 작동하는 상황이라면, Wave State를 업데이트 합니다.
        if (GameManager.Instance.currentGameState == GameState.Waving && _IsManagerInit)
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
                //[XMC]Debug.Log("[WAVE] Wave Initialize(next wave setting), 잠시 대기하는 중입니다.");
                NextWaveSetting();
                beforeState = WaveState.Init;
                currentState = WaveState.Waiting;

                // else if (beforeState == WaveState.Waiting)
                // {
                //     beforeState = WaveState.Init;
                //     currentState = WaveState.Playing;
                // }
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

            case WaveState.Pause:
                //[XMC]Debug.Log("[WAVE] : current State : Pause");
                beforeState = WaveState.Pause;
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
        currentBeatNum = 0;
        currenWaveNum++;
        Debug.Log(currenWaveNum);
        if (currenWaveNum > endWaveNum)
            return;
        
        waveMusicGUID++;
        // 음악 세팅
        // waveMusicGUID = 1; // TODO: 임시로 GUID 1번으로 세팅
        CurMusicData = GameManager.Data.GetMusicData(waveMusicGUID); //받아올 Music Data 세팅
        //Debug.Log($"[Wave] : received Music Data. Music GUID {CurMusicData.GUID}");
        _oneBeat = 60.0f / CurMusicData.BPM;
        GameManager.Instance.Metronome.secondsPerBeat = _oneBeat;
        _beat = _oneBeat;


        beforeWave = currentWave;
        currentWave = (WaveType)CurMusicData.WaveType;
        //Debug.Log(beforeWave);
        //Debug.Log(currentWave);
        // Wave 세팅
        SetWavePlayer(); // Player의 Interact 세팅
        
        // TODO: Scene 내의 점수판, 조명, 노드 도착지점 세팅
        SetWavePlay();
        GameManager.Instance.Metronome.Init(CurMusicData.BPM, waveMusicGUID);
        nodeInstantiator.InitToppingPool(currentWave); //Topping Pool 세팅
    }


    [ContextMenu("DEBUG/SetPauseWave()")] //TODO: For Test, 이후 제거하기
    public void SetPauseWave()
    {
        if (!_isPause)
        {
            //[XMC]Debug.Log("[WAVE] Wave Pause");
            // Wave 진행을 일시정지 시킵니다.
            SetIsPause(true);
        }
        
    }

    public void SetWaveType(WaveType waveType)
    {
        currentWave = waveType;
    }
    [ContextMenu("DEBUG/ContinueWave()")] //TODO: For Test, 이후 제거하기
    public void ContinueWave(WaveState waveState)
    {
        if (_isPause)
        {
            // Debug.Log("[WAVE] Wave Continue");
            // __초 뒤에 Wave 일시정지를 해제합니다.

            if (beforeState == WaveState.Init)
            {
                _waitBeforePlayingCTS = new CancellationTokenSource();
                _waitBeforePlayingCoroutine = WaitBeforePlaying(5, waveState, _waitBeforePlayingCTS.Token);
                _waitBeforePlayingCoroutine.Forget();
            }
            else if (beforeState == WaveState.Playing)
            {
                countdownTime = 2;
                _waitAfterPlayingCTS = new CancellationTokenSource();
                _waitAfterPlayingCoroutine = WaitAfterPlaying(3, waveState, _waitAfterPlayingCTS.Token);
                _waitAfterPlayingCoroutine.Forget();
            }

            // currentState = WaveState.Playing; 
            // Waiting -> Playing state 관리 
        }
    }
    
    [ContextMenu("DEBUG/CountDown()")] //TODO: For Test, 이후 제거하기
    public void CountDown()
    {
        //[XMC]Debug.Log("[TEST] Countdown Start!");
        StartCoroutine(CountdownToStart());
    }
    
    // Init -> Waiting -> Playing(노래(wave) 재생 중..) -> Waiting(노래(wave) 종료) -> Init -> 반복하다 비트 끝나면 End
    // Waiting -> Playing
    async UniTask WaitBeforePlaying(int sec, WaveState waveState, CancellationToken token)
    {
        //[XMC]Debug.Log($"[Wave] State : Waiting -> Playing Wait {sec}s. (이제 Wave 시작한다? 세팅 후에 게임 시작 전 대기 시간을 가짐. 플레이어 준비 시간.) ");

        // CMS: Count down starts
        // CMS TODO: 이거 WaitBefore After 합쳐도 되면 중복이라 합치고 싶은데 확인 부탁드려여2
        GameManager.Score.setTXT();
        int idx = (int)currentWave;
        countdownTime = sec;

        timerCanvas[idx].SetActive(true);
        timerCanvas[idx].transform.GetChild(0).gameObject.SetActive(true);
        timerCanvas[idx].transform.GetChild(1).gameObject.SetActive(false);

        timer = timerCanvas[idx].transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        while(countdownTime > 0)
        {
            GameManager.Sound.PlayEffect_Countdown(countdownTime, true);
            if (countdownTime == 1)
            {
                timerCanvas[idx].transform.GetChild(0).gameObject.SetActive(false) ; timerCanvas[idx].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                timer.text = (countdownTime - 1).ToString();
            }
            await UniTask.Delay(TimeSpan.FromSeconds(1), ignoreTimeScale: true, cancellationToken: token);
            //yield return new WaitForSecondsRealtime(1f);
            countdownTime--;
        }
        timerCanvas[idx].SetActive(false);
        // CMS: Count down ends

        CallContinueSetting(waveState);
        
        if (_waitBeforePlayingCTS != null)
        {
            _waitBeforePlayingCTS.Cancel();
            _waitBeforePlayingCTS.Dispose();
        }
        
        countdownTime = 4;
    }
    // Waiting -> Init -> Playing 
    async UniTask WaitAfterPlaying(int sec, WaveState waveState, CancellationToken token)
    {
        //[XMC]Debug.Log($"[Wave] State : Playing -> Waiting Wait {sec}s. (이제 Wave 끝났다? 다음 Wave 시작 전 혹은 게임 종료 전 대기 시간) ");

        GameManager.Instance.Metronome.SetGameEnd();
        
        // 게임 종료 시, Wave 종료 UI 호출
        if (currenWaveNum + 1 > endWaveNum)
        {
            _isPause = false;
            Time.timeScale = 1;
            EndGame();
            return;
        }

        // CMS: Count down starts
        // CMS TODO: 이거 WaitBefore After 합쳐도 되면 중복이라 합치고 싶은데 확인 부탁드려여2
        int idx = (int)currentWave;
        countdownTime = sec;

        timerCanvas[idx].SetActive(true);
        timerCanvas[idx].transform.GetChild(0).gameObject.SetActive(true);
        timerCanvas[idx].transform.GetChild(1).gameObject.SetActive(false);

        timer = timerCanvas[idx].transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        while(countdownTime > 0)
        {
            // TODO: 임시로... 240216 JMH
            // 게임이 쭉 이어질 때, 이게 2번씩 나오니까 중복되어서 하나는 없애는게 나을 것 같았습니다.
            // GameManager.Sound.PlayCountdownSound(countdownTime, false);
            if (countdownTime == 1)
            {
                //timer.text = ""; timerCanvas[idx].transform.GetChild(1).gameObject.SetActive(true);
            }
            else timer.text = (countdownTime - 1).ToString();
            //yield return new WaitForSecondsRealtime(0f); // TODO: 임시로... 240216 JMH
            await UniTask.Delay(TimeSpan.FromSeconds(0), ignoreTimeScale: true, cancellationToken: token);
            countdownTime--;
        }
        timerCanvas[idx].SetActive(false);
        // CMS: Count down ends

        CallContinueSetting(waveState);
        
        if (_waitAfterPlayingCTS != null)
        {
            _waitAfterPlayingCTS.Cancel();
            _waitAfterPlayingCTS.Dispose();
        }
        
        countdownTime = 4;
    }

    private void CallContinueSetting(WaveState waveState)
    {
        //[XMC]Debug.Log("[WAVE] Wave 일시중지 해제함");
        _isPause = false;
        Time.timeScale = 1;
        
        // 진행된 wave가 최종 wave 수와 같아지면 게임 종료, wave가 남았다면 Next Wave Start
        if (currenWaveNum > endWaveNum) EndGame();
        else if(currenWaveNum <= endWaveNum && waveState == WaveState.Init) NextWaveStart();
        else if(currenWaveNum <= endWaveNum && waveState == WaveState.Playing) NextWaveInit();
        else if(currenWaveNum <= endWaveNum && waveState == WaveState.Waiting) NextWaveStart();
    }
    private void NextWaveStart()
    {
        //Debug.Log("[WAVE] Wave Start");
        currentState = WaveState.Playing;
        waveTime = 0.0f;
        // 음악 시작!
        GameManager.Instance.Metronome.StartMusic();
        // 노드는 Time.timeScale == 1일 경우 자동으로 Update 됨.
    }

    private void NextWaveInit()
    {
        //[XMC]Debug.Log("[WAVE] Next Wave Init");
        //beforeState = WaveState.Waiting;
        currentState = WaveState.Init;
    }

    public void GameOver()
    {
        currentState = WaveState.End;
        GetWaveScoreUI().gameObject.SetActive(false);
        nodeInstantiator.FinishAllWaveNode();
    }
    private void EndGame()
    {
        // 모든 웨이브가 종료되었을 때 호출.
        currentState = WaveState.End;
        //Debug.Log("[WAVE] 게임 종료!");
        GetWaveScoreUI().gameObject.SetActive(false);
        nodeInstantiator.FinishAllWaveNode();
        GameManager.Instance.Save.SaveLoadData(stageID, (int)GameManager.Score.TotalScore);
        SetResultUI();
    }
    
    private void SetResultUI()
    {
        // 결과 저장
        // 결과 UI 호출
        SetWavePlayer(WaveType.Punching); // Player 주먹으로 변경
        
        UI_Results result = GameObject.FindWithTag("ResultUI").GetComponent<UI_Results>();
        result.SettingValues(GameManager.Score.TotalScore, GameManager.Player.playerLifeValue);
        result.ShowResults();
        //DontDestroyOnLoad(result);
    }

    public void SetIsPause(bool pause)
    {
        if (pause) {
            Time.timeScale = 0;
            PauseMusic_Popup(true);
            // 소리 끄기
        } else {
            // 소리 3초 후 틀기
            StartCoroutine(CountdownToStart());
            //[XMC]Debug.Log("Resume the game after 3 sec...");
        }
    }
    
    IEnumerator CountdownToStart()
    {
        //[XMC]Debug.Log("[Wave] : Countdown To Start");
        int idx = (int)currentWave;
        timerCanvas[idx].SetActive(true);
        timerCanvas[idx].transform.GetChild(0).gameObject.SetActive(true);
        timerCanvas[idx].transform.GetChild(1).gameObject.SetActive(false);

        timer = timerCanvas[idx].transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        while(countdownTime > 0)
        {   
            if (countdownTime == 1) { timerCanvas[idx].transform.GetChild(0).gameObject.SetActive(false); timerCanvas[idx].transform.GetChild(1).gameObject.SetActive(true);}
            else timer.text = (countdownTime - 1).ToString();
            yield return new WaitForSecondsRealtime(1f);
            countdownTime--;
        }
        timerCanvas[idx].SetActive(false);
        
        PauseMusic_Popup(false);
        Time.timeScale = 1;
        countdownTime = 4;
        currentState = WaveState.Playing;
    }
    
    public void PauseMusic_Popup(bool isPause = false)
    {
        this._isPause = isPause;
        GameManager.Sound.PauseMusic(waveMusicGUID, isPause);
    }
    
    // Update에서 반복, 비트가 남았을 경우 계속 진행(beatNum, beat값 수정), 모든 비트가 마무리된 경우 currentState -> Waiting으로 전환 
    public void UpdateBeat()
    {
        if (currentState == WaveState.Playing && CurMusicData.NodeCount + GameManager.Instance.Metronome.shootStandard <= GameManager.Instance.Metronome.currentBeat) // 조건 : 1beat 시간이 흘렀을 경우 한 번 호출
        { 
            //Debug.Log("[WAVE BEAT] " + _beatNum + "beat");
            /*if (87 == nodeInstantiator._musicDataIndex)
            {
                Debug.Log("Stop Coroutine!");
                nodeInstantiator.NotesExistButAreNLongerEnqueued();
            }*/

            // 존재 비트 모두 플레이 했을 때 State : Playing -> Waiting으로 전환
                currentState = WaveState.Waiting;
        }
    }
}
