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
    [Header("----+ Wave Setting +----")] 
    public uint endWaveNum = 0; // 진행할 웨이브 전체 숫자.
    public WaveType firstWaveType = WaveType.Punching;
    
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

    // For Tutorial 
    private bool _isTutorialPlayed = false; // 튜토리얼 플레이 완료 여부
    public bool isTutorial = false; // 현재 튜토리얼 게임중 여부
    public uint tutorialMusicGUID; // 튜토리얼 웨이브에 대한 음악 데이터의 GUID
    private bool tutorialWaveCompleted = false; // 튜토리얼의 한 웨이브가 완료되었는지(튜토리얼 웨이브의 노드가 다 생성 되었는지)
    private int tutorialRound = 1;// 튜토리얼의 라운드 (1,2 존재)
    private int tutorialScore = 0;
    ////


    private float _oneBeat;
    private float _beat;
   
    private Coroutine _waitBeforePlayingCoroutine;
    private Coroutine _waitAfterPlayingCoroutine;
    
    [Header("----+ Music Information +----")] 
    public uint waveMusicGUID; // 현재 세팅된 Music의 GUID
    public DataManager.MusicData CurMusicData; // 현재 세팅된 Music data

    [Header("----+ setting +----")] 
    //[SerializeField] private GameObject RightInteraction;
    //[SerializeField] private GameObject LeftInteraction;
    [SerializeField] private NodeInstantiator_minha nodeInstantiator;
    [SerializeField] private GameObject nodeArrivalArea;
    [SerializeField] private GameObject nodeArrivalUI;
    [SerializeField] public GameObject[] timerCanvas;
    private int countdownTime = 4;
    private TextMesh timer;

    // 기획에 따른 변수
    private int waveTypeNum = 3; // Wave Type 종류의 갯수
    [SerializeField] private List<GameObject> _toppingArea = new List<GameObject>();

    // wave 일시정지 기능을 구현하기 위한 변수
    // 일단 사용하지 않아서 주석처리
    /*private bool _isWait = false;
    private float _timerDuration = 2f;
    private float _waitTimer;
    private bool _hasSet = false;*/
    private bool _isPause = false;
    private bool _IsManagerInit = false;

    // wave 전환을 위한 변수
    public enum WaveState
    {
        Init,       //새로운 Wave로 Enter 하는 중
        TutorialWaiting, // 튜토리얼 Wave 대기 중
        TutorialPlaying, // 튜토리얼 Wave 진행 중
        Waiting,    //잠시 대기 중
        Playing,     //Wave 진행 중
        Pause,
        End
    }


    public void Init()
    {
        Debug.Log("Initialize WaveManager");

        // Wave Num
        waveTime = 0.0f;

        // Player Manager로 변수 이동
        // RightInteraction = Utils.FindChildByRecursion(GameManager.Player.RightController.transform, "Interaction")
        //     .gameObject;
        // LeftInteraction = Utils.FindChildByRecursion(GameManager.Player.LeftController.transform, "Interaction")
        //     .gameObject;


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

        _IsManagerInit = true;
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
    }

    public Vector3 GetArrivalPosition(int index)
    {
        //if (index < 0 || index > 3)
        //[XMC]Debug.Log("[ERROR] " + index);
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
        GameManager.Player.RightInteraction.transform.GetChild(0).gameObject.SetActive(false);
        GameManager.Player.RightInteraction.transform.GetChild(1).gameObject.SetActive(false);
        GameManager.Player.RightInteraction.transform.GetChild(2).gameObject.SetActive(false);

        GameManager.Player.LeftInteraction.transform.GetChild(0).gameObject.SetActive(false);
        GameManager.Player.LeftInteraction.transform.GetChild(1).gameObject.SetActive(false);
        GameManager.Player.LeftInteraction.transform.GetChild(2).gameObject.SetActive(false);

        int TypeNum = (int)currentWave;
        //[XMC]Debug.Log($"TypeNum : {TypeNum}");
        GameManager.Player.RightInteraction.transform.GetChild(TypeNum).gameObject.SetActive(true);
        GameManager.Player.LeftInteraction.transform.GetChild(TypeNum).gameObject.SetActive(true);
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
        /*nodeArrivalUI.transform.GetChild(0).gameObject.SetActive(false);
        nodeArrivalUI.transform.GetChild(1).gameObject.SetActive(false);
        nodeArrivalUI.transform.GetChild(2).gameObject.SetActive(false);
        nodeArrivalUI.transform.GetChild(TypeNum).gameObject.SetActive(true);*/
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
        /*nodeArrivalUI.transform.GetChild(0).gameObject.SetActive(false);
        nodeArrivalUI.transform.GetChild(1).gameObject.SetActive(false);
        nodeArrivalUI.transform.GetChild(2).gameObject.SetActive(false);*/
    }

    private void Update()
    {
        // Debug.Log("Time scale" +Time.timeScale);
        // 현재 Wave manager가 작동하는 상황이라면, Wave State를 업데이트 합니다.
        if (GameManager.Instance.currentGameState == GameState.Waving && _IsManagerInit)
        {
            //if (isTutorial) // 이거 튜토리얼 업데이트 하는 거 없어도 되게 수정중!
            //{
            //    UpdateTutorialState(); // 튜토리얼 상태 업데이트
            //}
            //else
            //{
            UpdateWaveState(); // 일반 웨이브 상태 업데이트
            //}
            UpdateBeat();
        }
    }

    private void UpdateWaveState()
    {
        // Init -> Waiting -> Playing(노래(wave) 재생 중..) -> Waiting(노래(wave) 종료) -> Init -> 반복하다 비트 끝나면 End
        // tutorial 추가 시 : (Init -> TutorialWaiting -> TutorialPlaying) * 2 (2회 반복) -> init -> Waiting -> ... (TutorialPlaying 이후는 위와 동일)
        switch (currentState)
        {
            case WaveState.Init:
                //Debug.Log("===WaveState : Init");
                //_isTutorialPlayed = true; // 튜토리얼 제외한 코드 테스트용
                // tutorial wave를 위해 새로 작성한 코드
                if (!_isTutorialPlayed)
                {
                    //Debug.Log("튜토리얼 미완료 상태 : _isTutorialPlayed = F");
                    tutorialWaveCompleted = false;
                    GameManager.Score.ResetTutorialScore();
                    //Debug.Log("튜토리얼 tutorialScore : " + tutorialScore);

                    NextTutorialWaveSetting();
                    //beforeState = WaveState.Init;
                    currentState = WaveState.TutorialWaiting;
                }
                else
                {
                    //Debug.Log("튜토리얼 완료한 상태 : _isTutorialPlayed = T");
                    NextWaveSetting();
                    beforeState = WaveState.Init;
                    currentState = WaveState.Waiting;
                }
                //[XMC]Debug.Log("[WAVE] Wave Initialize(next wave setting), 잠시 대기하는 중입니다.");

                // else if (beforeState == WaveState.Waiting)
                // {
                //     beforeState = WaveState.Init;
                //     currentState = WaveState.Playing;
                // }
                break;

            case WaveState.TutorialWaiting:
                Debug.Log("===WaveState : TutorialWaiting");
                SetPauseWave();
                
                //튜토리얼 설명 UI 추가 예정

                ContinueTutorialWave(beforeState);
                currentState = WaveState.TutorialPlaying; 
                break;

            case WaveState.TutorialPlaying: // 튜토리얼 Wave를 실행 중
                Debug.Log("===WaveState : TutorialPlaying");
                isTutorial = true;
                waveTime += Time.deltaTime;

                switch (tutorialRound)
                {
                    case 1:
                        Debug.Log("[tutorialRound] = 1");
                        Debug.Log("튜토리얼 tutorialScore : " + tutorialScore);

                        if (IsTutorialScoreComplete()) // 튜토리얼 완료 조건
                        {
                            Debug.Log("IsTutorialScoreComplete 달성 완료");

                            // 튜토리얼 1라운드만 완료 처리
                            //_isTutorialPlayed = true;
                            isTutorial = false;
                            tutorialRound++;
                            currentState = WaveState.Init; // 다음 상태로 전환
                        }
                        else
                        {
                            if (tutorialWaveCompleted) // 노드는 다 나왔는데, 점수 도달 못했다면 1라운드 반복
                            {
                                Debug.Log("XX IsTutorialScoreComplete XX");

                                isTutorial = false;
                                currentState = WaveState.Init; // 그냥 다음 상태로 전환
                            }
                        }
                        break;
                    case 2:
                        Debug.Log("[tutorialRound] = 2");
                        Debug.Log("튜토리얼 tutorialScore : " + tutorialScore);

                        if (IsTutorialScoreComplete()) // 튜토리얼 완료 조건
                        {
                            Debug.Log("IsTutorialScoreComplete 달성 완료");

                            // 튜토리얼 완전 완료 처리
                            _isTutorialPlayed = true;
                            isTutorial = false;
                            currentState = WaveState.Init; // 다음 상태로 전환
                        }
                        else
                        {
                            if (tutorialWaveCompleted)
                            {
                                Debug.Log("XX IsTutorialScoreComplete XX");

                                isTutorial = false;
                                currentState = WaveState.Init; // 그냥 다음 상태로 전환
                            }
                        }
                        break;
                }
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
    /// 

    private bool IsTutorialScoreComplete()
    {
        bool ret_value = false;
        int TnodeNum = 1; // 칠 수 있는 노드 1개라고 가정 
        tutorialScore = GameManager.Score.GetTutorialScore();

        if (tutorialScore >= TnodeNum)
        {
            ret_value = true;
        }
        return ret_value;
    }


    public void NextWaveSetting()
    {
        // Random으로 다음 wave를 지정
        //currentWave = GetRandomWave();
        // TODO: 임시설정. For Test
        waveTime = 0;
        currentBeatNum = 0;
        currenWaveNum++;

        if (currenWaveNum > endWaveNum)
            return;
        waveMusicGUID++;
        // 음악 세팅
        // waveMusicGUID = 1; // TODO: 임시로 GUID 1번으로 세팅
        CurMusicData = GameManager.Data.GetMusicData(waveMusicGUID); //받아올 Music Data 세팅
        Debug.Log($"[Wave] : received Music Data. Music GUID {CurMusicData.GUID}");
        _oneBeat = 60.0f / CurMusicData.BPM;
        _beat = _oneBeat;

        currentWave = (WaveType)CurMusicData.WaveType;
        
        // switch (firstWaveType)
        // {
        //     case WaveType.Punching:
        //         if (currenWaveNum % 2 == 1) currentWave = WaveType.Punching;
        //         else currentWave = WaveType.Hitting;
        //         break;
        //
        //     case WaveType.Hitting:
        //         if (currenWaveNum % 2 == 0) currentWave = WaveType.Punching;
        //         else currentWave = WaveType.Hitting;
        //         break;
        // }

        // Wave 세팅
        SetWavePlayer(); // Player의 Interact 세팅

        // TODO: Scene 내의 점수판, 조명, 노드 도착지점 세팅
        SetWavePlay();
        nodeInstantiator.InitToppingPool(currentWave); //Topping Pool 세팅
    }

    public void NextTutorialWaveSetting()
    {
        Debug.Log("============ NextTutorialWaveSetting 함수 내부 ==============");
        Debug.Log("[다음 튜토리얼 세팅] NextTutorialWaveSetting()");

        // 다음 튜토리얼 wave를 지정
        waveTime = 0;
        currentBeatNum = 0;

        Debug.Log("[tutorialRound] : " + tutorialRound);

        Debug.Log("[tutorialMusicGUID] : " + tutorialMusicGUID);


        if (tutorialRound == 1) // 튜토리얼 1라운드
        {
            tutorialMusicGUID = 11;
        }
        if (tutorialRound == 2) // 튜토리얼 2라운드
        {
            tutorialMusicGUID = 12;
        }

        CurMusicData = GameManager.Data.GetTutorialMusicData(tutorialMusicGUID); //받아올 Music Data 세팅
        Debug.Log("==NextTutorialWaveSetting==");
        Debug.Log($"[Tutorial Wave] : received Tutorial Music Data. Music GUID {CurMusicData.GUID}");
        _oneBeat = 60.0f / CurMusicData.BPM;
        _beat = _oneBeat;

        currentWave = (WaveType)CurMusicData.WaveType;

        // Wave 세팅
        SetWavePlayer(); // Player의 Interact 세팅
        SetWavePlay();
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
            // _isPause = true;
            // Time.timeScale = 0;
        }
        
    }

    public void ContinueTutorialWave(WaveState beforeState) //
    {
        Debug.Log("----[TutorialWaiting] ContinueTutorialWave");
        if (_isPause)
        {
            // Debug.Log("[WAVE] Wave Continue");
            // __초 뒤에 Wave 일시정지를 해제합니다.

            if (beforeState == WaveState.Init && _waitBeforePlayingCoroutine == null)
                _waitBeforePlayingCoroutine = StartCoroutine(WaitBeforePlaying(5, WaveState.TutorialPlaying));

            else if (beforeState == WaveState.TutorialPlaying && _waitAfterPlayingCoroutine == null)
            {
                countdownTime = 2;
                _waitAfterPlayingCoroutine = StartCoroutine(WaitAfterPlaying(3, WaveState.TutorialPlaying));
            }

            // currentState = WaveState.TutorialPlaying; 
            // TutorialWaiting -> TutorialPlaying state 관리 
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
                _waitBeforePlayingCoroutine = StartCoroutine(WaitBeforePlaying(5, waveState));

            else if (beforeState == WaveState.Playing && _waitAfterPlayingCoroutine == null)
            {
                countdownTime = 2;
                _waitAfterPlayingCoroutine = StartCoroutine(WaitAfterPlaying(3, waveState));
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
    IEnumerator WaitBeforePlaying(int sec, WaveState waveState)
    {
        //[XMC]Debug.Log($"[Wave] State : Waiting -> Playing Wait {sec}s. (이제 Wave 시작한다? 세팅 후에 게임 시작 전 대기 시간을 가짐. 플레이어 준비 시간.) ");

                // CMS: Count down starts
        // CMS TODO: 이거 WaitBefore After 합쳐도 되면 중복이라 합치고 싶은데 확인 부탁드려여2
        int idx = (int)currentWave;
        countdownTime = sec;

        timerCanvas[idx].SetActive(true);
        timerCanvas[idx].transform.GetChild(1).gameObject.SetActive(false);

        timer = timerCanvas[idx].transform.GetChild(0).GetComponent<TextMesh>();

        while(countdownTime > 0)
        {
            GameManager.Sound.PlayEffect_Countdown(countdownTime, true);
            if (countdownTime == 1)
            {
                timer.text = ""; timerCanvas[idx].transform.GetChild(1).gameObject.SetActive(true);
                
            }
            else
            {
                timer.text = (countdownTime - 1).ToString();
            }
            yield return new WaitForSecondsRealtime(1f);
            countdownTime--;
        }
        timerCanvas[idx].SetActive(false);
        // CMS: Count down ends

        CallContinueSetting(waveState);
        _waitBeforePlayingCoroutine = null;
        countdownTime = 4;
    }
    // Waiting -> Init -> Playing 
    IEnumerator WaitAfterPlaying(int sec, WaveState waveState)
    {
        //[XMC]Debug.Log($"[Wave] State : Playing -> Waiting Wait {sec}s. (이제 Wave 끝났다? 다음 Wave 시작 전 혹은 게임 종료 전 대기 시간) ");

        // CMS: Count down starts
        // CMS TODO: 이거 WaitBefore After 합쳐도 되면 중복이라 합치고 싶은데 확인 부탁드려여2
        int idx = (int)currentWave;
        countdownTime = sec;

        timerCanvas[idx].SetActive(true);
        timerCanvas[idx].transform.GetChild(1).gameObject.SetActive(false);

        timer = timerCanvas[idx].transform.GetChild(0).GetComponent<TextMesh>();

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
            yield return new WaitForSecondsRealtime(0f); // TODO: 임시로... 240216 JMH
            countdownTime--;
        }
        timerCanvas[idx].SetActive(false);
        // CMS: Count down ends


        CallContinueSetting(waveState);
        _waitAfterPlayingCoroutine = null;
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
        Debug.Log("[WAVE] Wave Start");
        currentState = WaveState.Playing;
        waveTime = 0.0f;
        GameManager.Sound.PlayWaveMusic(waveMusicGUID); //음악 start
        // 노드는 Time.timeScale == 1일 경우 자동으로 Update 됨.
    }

    private void NextWaveInit()
    {
        //[XMC]Debug.Log("[WAVE] Next Wave Init");
        //beforeState = WaveState.Waiting;
        currentState = WaveState.Init;
    }

    private void EndGame()
    {
        // 모든 웨이브가 종료되었을 때 호출.
        currentState = WaveState.End;
        Debug.Log("[WAVE] 게임 종료!");

        GameManager.Instance.WaveToEnding();
        nodeInstantiator.FinishAllWaveNode();
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

    public void SetPause(bool _isPause)
    {
        if (_isPause) {
            Time.timeScale = 0;
            PauseMusic(true);
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
        timerCanvas[idx].transform.GetChild(1).gameObject.SetActive(false);

        timer = timerCanvas[idx].transform.GetChild(0).GetComponent<TextMesh>();

        while(countdownTime > 0)
        {   
            if (countdownTime == 1) {timer.text = ""; timerCanvas[idx].transform.GetChild(1).gameObject.SetActive(true);}
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

    public void PauseMusic(bool _isPause = false)
    {
        this._isPause = _isPause;
        //GameManager.Sound.PauseMusic(waveMusicGUID, _isPause);
        GameManager.Sound.RestartMusic(waveMusicGUID, _isPause);
    }

    public void PauseMusic_Popup(bool isPause = false)
    {
        this._isPause = isPause;
        GameManager.Sound.PauseMusic(waveMusicGUID, isPause);
    }
    
    // Update에서 반복, 비트가 남았을 경우 계속 진행(beatNum, beat값 수정), 모든 비트가 마무리된 경우 currentState -> Waiting으로 전환 
    public void UpdateBeat()
    {
        if (waveTime > _beat && currentState == WaveState.Playing) // 조건 : 1beat 시간이 흘렀을 경우 한 번 호출
        { 
            //Debug.Log("[WAVE BEAT] " + _beatNum + "beat");
            /*if (87 == nodeInstantiator._musicDataIndex)
            {
                Debug.Log("Stop Coroutine!");
                nodeInstantiator.NotesExistButAreNLongerEnqueued();
            }*/

            // 존재 비트 모두 플레이 했을 때 State : Playing -> Waiting으로 전환
            if (CurMusicData.BeatNum == currentBeatNum)
            {
                //[XMC]Debug.Log($"[Wave] : Detected All Beat is Done");
                currentState = WaveState.Waiting;
            }
            
            currentBeatNum++;
            _beat += _oneBeat;
        }
        // 튜토리얼용) -> 나중에 합치기
        if (waveTime > _beat && currentState == WaveState.TutorialPlaying)
        {
            if (currentBeatNum == CurMusicData.BeatNum) // 튜토리얼 웨이브의 비트가 모두 재생되었는지 확인
            {
                tutorialWaveCompleted = true; // 튜토리얼 한 웨이브 완료
            }
            currentBeatNum++;
            _beat += _oneBeat;
        }
    }

    [ContextMenu("TimeScale 1")]
    private void ddd()
    {
        Time.timeScale = 1;
    }
    
    
    private void FixedUpdate()
    {
        
    }
}
