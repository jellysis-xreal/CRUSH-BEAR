using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

/* [ Game Manager ]
 * 게임 내에 정의될 모든 객체들을 Child로 갖는 하나의 관리자 클래스를 정의합니다
 * 해당 클래스는 Scene의 가장 루트 GameObject로, 단 하나만 미리 인스턴싱 해두도록 한다
 *
 * Scene 전환에 걸쳐서도 유지되어야 하는 데이터들을 관리함
*/

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }

            return instance;
        }
    }
    
    //+-------- Managers --------+//
    [SerializeField] private DataManager _data = new DataManager();
    public static DataManager Data { get { return Instance._data; } }
    
    [SerializeField] private UIManager _ui = new UIManager();
    public static UIManager UI { get { return Instance._ui; } }
    
    [SerializeField] private ComboManager _combo = new ComboManager();
    public static ComboManager Combo { get { return Instance._combo; } }

    [SerializeField] private ResourceManager _resource = new ResourceManager();
    public static ResourceManager Resource { get { return Instance._resource; } }

    [SerializeField] private ScoreManager _score;
    public static ScoreManager Score { get { return Instance._score; } }

    [SerializeField] private PlayerManager _player;
    public static PlayerManager Player { get { return Instance._player; }}

    [SerializeField] private WaveManager _wave;
    public static WaveManager Wave { get { return Instance._wave; }}
    
    [SerializeField] private SoundManager _sound = new SoundManager();
    public static SoundManager Sound { get { return Instance._sound; } }

    [SerializeField] private TutorialManager tutorialManager = new TutorialManager();
    public static TutorialManager TutorialManager {get {return Instance.tutorialManager;}} 

    [SerializeField] private TutorialPunchManager tutorialPunch = new TutorialPunchManager();
    public static TutorialPunchManager TutorialPunch { get { return Instance.tutorialPunch; } }
    
    [SerializeField] private TutorialTennisManager tutorialTennis = new TutorialTennisManager();
    public static TutorialTennisManager TutorialTennis { get { return Instance.tutorialTennis; } }
    
    
    
    [SerializeField] private Metronome _metronome;
    public Metronome Metronome { get { return _metronome; } }

    private SaveManager _save = new SaveManager();
    public SaveManager Save { get { return _save; } }
    //+------------------------//
    
    // GameState 이벤트를 정의
    public delegate void GameStateChangedHandler(GameState newGameState);
    public static event GameStateChangedHandler OnGameStateChanged;
    private bool LoadWave = false;
    private bool LoadTutorial = false;
    [SerializeField] public GameState currentGameState;
    
    void Start()
    {
        //Debug.Log("GameManager Start");                                     
        if (currentGameState == GameState.Lobby)
        {           
            SetGameState(GameState.Lobby);
            InitLobby();
        }
        // Test();
    }

    // 게임 상태를 변경하고 이벤트를 호출하는 함수
    public void SetGameState(GameState newGameState)
    {
        if (currentGameState != newGameState)
        {
            currentGameState = newGameState;
            OnGameStateChanged?.Invoke(currentGameState); // GameState가 변경될 때마다 이벤트 호출
        }
    }
    
    private void HandleGameStateChanged(GameState newGameState)
    {
        //Debug.Log("Game state changed to: " + newGameState);
        // 여기서 원하는 동작을 수행
        // 예를 들어, 게임 상태에 따라 UI 갱신, 게임 오브젝트 활성화/비활성화 등 수행 가능
        switch (newGameState)
        {
            case GameState.Lobby:
                OVRManager.SetSpaceWarp(true);
                SceneManager.LoadScene("00.StartScene");
                break;

            case GameState.Waving:
                OVRManager.SetSpaceWarp(false);
                GameObject lobbyPlayer = GameObject.FindWithTag("Player");
                lobbyPlayer.SetActive(false);
                
                LoadWave = true; //비동기로 Load하던 01 Scene Active!

                Sound.PlayMusic_Lobby(false); //Effect & Sound
                Invoke("InitPlay", 1.0f); //Wave play를 위한 Manager들 Init()
                break;
            
            case GameState.Ending:
                OVRManager.SetSpaceWarp(false);
                _player.InActivePlayer();
                Sound.PlayEffectMusic_GameWin();
                SceneManager.LoadScene("02.EndingCutScene");
                break;
            
            case GameState.Tutorial:
                Debug.Log("State Changed to Tutorial");
                OVRManager.SetSpaceWarp(false);
                InitTutorial();
                // StartCoroutine(LoadWaveScene());

                break;
        }
    }
    
    private void OnEnable()
    {
        OnGameStateChanged += HandleGameStateChanged; // 이벤트 핸들러 등록
    }
    
    private void OnDisable()
    {
        OnGameStateChanged -= HandleGameStateChanged; // 이벤트 핸들러 해제
    }
    
    //TODO: For Test, 이후 제거하기
    [ContextMenu("DEBUG/Play()")]
    public void LobbyToWave()
    {
        SetGameState(GameState.Waving);
    }

    public void WaveToEnding()
    {
        SetGameState(GameState.Ending);
    }
    [ContextMenu("DEBUG/Lobby")]
    public void WaveToLobby()
    {
        tutorialManager.StopAllCoroutines();
        SetGameState(GameState.Lobby);
        Sound.PlayMusic_Lobby(true);
    }

    [ContextMenu("DEBUG/Tutorial")]
    public void LobbyToTutorial()
    {
        SetGameState(GameState.Tutorial);
    }
    private void InitLobby()
    {
        if (instance == null)
        {
            GameObject obj = GameObject.Find("Game Manager");

            if (obj == null)
            {
                obj = Resources.Load<GameObject>("Prefabs/Game Manager");
                obj.name = "Game Manager";
                Instantiate(obj);
            }

            DontDestroyOnLoad(obj);
            instance = obj.GetComponent<GameManager>();
            
            //+-------- GameManager Init +--------//
            currentGameState = GameState.Lobby;

            //+-------- Managers Init() +--------//

            //_score.Init();
            //_player.Init();


            _data.Init();
            Save.LoadSaveData();
            //_wave.Init();
            _sound.Init();
            _ui.Init(currentGameState);
            _resource.Init();
            // Effect & Sound
            Sound.PlayMusic_Lobby(true);
        }
        else
        {
            //Debug.LogWarning("GameManager instance isn't null, Destroy GameManager");
            _ui.Init(currentGameState);
            
            Destroy(this.gameObject);
        }
        
        if (_save.data.isFirst)
            StartCoroutine(LoadTutorialScene());
        else
            StartCoroutine(LoadWaveScene());
        
    }

    public void InitTutorial()
    {
        if (instance != null)
        {
            //Debug.Log("Init GameManager Tutorial Scene");
            //+-------- Managers Init() +--------//
            SceneManager.sceneLoaded += OnTutorialSceneLoaded;
            LoadTutorial = true;
        }
        else
        {
            //Debug.Log("GameManager instance is null");
        }
    }
    public void InitPlay()
    {
        if (instance != null)
        {
            //Debug.Log("Init GameManager Wave Scene");

            //+-------- Managers Init() +--------//
            //_data.Init();
            _player.Init();
            _wave.Init();
            _score.Init();
            _combo.Init();
            //_player.PlaySceneUIInit();
            //_sound.Init();
            _ui.Init(currentGameState);
            //_resource.Init();
        }
        else
        {
            //Debug.Log("GameManager instance is null");
        }
    }
    
    private void Test()
    {
        //Debug.Log("<<-------TEST------->>");
        
        // 이 밑으로 진행할 Test 코드를 입력한 후, Start 함수에 가서 Test의 주석 처리를 해제하면 됩니다.
        // Toast 치기 개발으로 잠시 테스트 - 240108 minha
        // _wave.Test();
        
    }

    IEnumerator LoadWaveScene()
    {
        yield return null;

        AsyncOperation Loading = SceneManager.LoadSceneAsync("01.WaveScene", LoadSceneMode.Single);
        Loading.allowSceneActivation = false;

        while (!Loading.isDone)
        {
             // [DEBUG]
             //Debug.Log("Async progress :" + (Loading.progress) + "%"
             //+ "\n async.allowSceneActivation = " + Loading.allowSceneActivation);

            if (LoadWave && Loading.progress >= 0.9f)
            {
                Loading.allowSceneActivation = true;
            }

            yield return null;
        }
    }
    
    IEnumerator LoadTutorialScene()
    {
        yield return null;

        AsyncOperation Loading = SceneManager.LoadSceneAsync("03.TutorialScene", LoadSceneMode.Single);
        Loading.allowSceneActivation = false;

        while (!Loading.isDone)
        {
            // [DEBUG]
            //Debug.Log("Async progress :" + (Loading.progress) + "%"
            //+ "\n async.allowSceneActivation = " + Loading.allowSceneActivation);

            if (LoadTutorial && Loading.progress >= 0.9f)
            {
                Loading.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    void OnTutorialSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log(scene.buildIndex);
        if (currentGameState == GameState.Tutorial)
        {
            /*// Tennis
            Wave.SetWaveType(WaveType.Hitting);
            Wave.SetWaveTutorial();
            Player.Init();
            tutorialTennis.Init();
            _score.Init();*/
            // Punch

            //Debug.Log("Tutorial Scene Loaded");
            _player.Init();
            
            _wave.Init();
            _wave.SetWaveType(WaveType.Punching);
            _wave.SetWaveTutorial();
            _score.Init();
            // tutorialPunch.Init();
            tutorialManager.Init();
        }

        SceneManager.sceneLoaded -= OnTutorialSceneLoaded;
    }
}
