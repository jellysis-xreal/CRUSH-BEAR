using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

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
    
    //+-------- Managers +--------//
    [SerializeField] private UIManager _ui;
    public static UIManager UI { get { return Instance._ui; } }

    [SerializeField] private ResourceManager _resource;
    public static ResourceManager Resource { get { return Instance._resource; } }

    [SerializeField] private ScoreManager _score;
    public static ScoreManager Score { get { return Instance._score; } }

    [SerializeField] private PlayerManager _player;
    public static PlayerManager Player { get { return Instance._player; }}

    [SerializeField] private WaveManager _wave;
    public static WaveManager Wave { get { return Instance._wave; }}
    
    [SerializeField] private SoundManager _sound;
    public static SoundManager Sound { get { return Instance._sound; } }
    
    void Start()
    { 
        Init();
        Test();
    }

    void Init()
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

            //+-------- Managers Init() +--------//
            _score.Init();
            _player.Init();
            _wave.Init();
            _sound.Init();
            // _ui.Init();
            // _resource.Init();
        }
        else
        {
            Debug.LogWarning("GameManager instance isn't null, Destroy GameManager");

            Destroy(this.gameObject);
        }
    }
    
    private void Test()
    {
        Debug.Log("<<-------TEST------->>");
        
        // 이 밑으로 진행할 Test 코드를 입력한 후, Start 함수에 가서 Test의 주석 처리를 해제하면 됩니다.
        // Toast 치기 개발으로 잠시 테스트 - 240108 minha
        _wave.Test();
    }

}
