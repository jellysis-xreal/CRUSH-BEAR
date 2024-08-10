using System.Collections;
using UnityEngine;
using EnumTypes;
using UnityEngine.UI;
using Oculus.Interaction;

[System.Serializable] // 반드시 필요
public class HeartsArray // 행에 해당되는 이름
{
    //public GameObject[] hearts = new GameObject[5];
    public Image[] hearts = new Image[5];
}

public class PlayerManager : MonoBehaviour
{
    [Header("setting(auto)")]
    [SerializeField] public Player player;
    [SerializeField] public GameObject mainCamera;
    //[SerializeField] public GameObject IK_player;
    public GameObject RightController;
    public GameObject LeftController;
    private OVRInput.Controller rightController;
    private OVRInput.Controller leftController;
    public GameObject RightInteraction;
    public GameObject LeftInteraction;
    public HandData R_HandData;
    public HandData L_HandData;
    
    
    [Header("player Life")] public int playerLifeValue = 0;
    public GameObject[] parentUI = new GameObject[3];
    public GameObject[] score_G = new GameObject[3];
    public ParticleSystem minusPrefab;
    [SerializeField] private GameObject gameOverUIPrefab;

    [Header("New Heart Sprite")]
    public Sprite defaultHeartSprite;
    public Sprite newHeartSprite; // 목숨 감소 하트 이미지


    [Header("Hearts (auto)")] public HeartsArray[] HeartGameObjects = new HeartsArray[3];

    public void Init()
    {
        //Debug.Log("Initialize PlayerManager");

        // Game object setting
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        mainCamera = GameObject.FindWithTag("MainCamera");
        //IK_player = GameObject.FindWithTag("IKPlayer");
        
        RightController = player.R_Controller;
        LeftController = player.L_Controller;
        
        // TODO
        R_HandData = player.R_HandData;
        L_HandData = player.L_HandData;
        //rightController = RightController.GetComponent<XRBaseController>();
        //leftController = LeftController.GetComponent<XRBaseController>();

        RightInteraction = player.R_Interaction;
        LeftInteraction = player.L_Interaction;
        // Player Life
        playerLifeValue = 5;

        // Initialize GameObject_Hearts
        HeartsInit();
    }

    public void HeartsInit()
    {
        int WaveTypeCount = System.Enum.GetValues(typeof(WaveType)).Length;
        //Debug.Log($"WaveTypeCount : {WaveTypeCount}");
        for (int i = 0; i < WaveTypeCount; i++)
        {
            /*HeartGameObjects[i].hearts[0] = score_G[i].transform.GetChild(0).GetComponent<Image>();
            HeartGameObjects[i].hearts[1] = score_G[i].transform.GetChild(1).GetComponent<Image>();
            HeartGameObjects[i].hearts[2] = score_G[i].transform.GetChild(2).GetComponent<Image>();
            HeartGameObjects[i].hearts[3] = score_G[i].transform.GetChild(3).GetComponent<Image>();
            HeartGameObjects[i].hearts[4] = score_G[i].transform.GetChild(4).GetComponent<Image>();*/
            for(int j = 0; j < 5; ++j)
            {
                // HeartGameObjects[i].hearts[j] = score_G[i].transform.GetChild(j).GetComponent<Image>();
                HeartGameObjects[i].hearts[j].sprite = defaultHeartSprite;
            }
            //Debug.Log("[TEST] hearts init " + i.ToString());
        }
    }

    public void PlaySceneUIInit(int idx)
    {
        for (int i = 0; i < parentUI.Length; i++)
        {
            parentUI[i].gameObject.SetActive(false);            
        }
        // Debug.Log($"TypeNum : {(int)GameManager.Wave.currentWave}");
        parentUI[idx].SetActive(true);
    }

    public void FinishSceneUI()
    {
        for (int i = 0; i < parentUI.Length; i++)
        {
            parentUI[i].gameObject.SetActive(false);            
        }
    }
    
    public void SetHearts(int playerLifeValue)
    {
        // TODO: XMC용 테스트 코드
        if (playerLifeValue == 0)
        {
#if UNITY_EDITOR
            Debug.Log("리듬감이 없으시군요!");
            return;
#else
            Instantiate(gameOverUIPrefab).transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 2.33f, player.transform.position.z + 10.48f);
            GameManager.Instance.Metronome.SetGameEnd();
            GameManager.Wave.GameOver();
            GameManager.Sound.PlayEffectMusic_GameOver();
            StartCoroutine(GameOver());
            return;
#endif
        }

        int WaveTypeCount = System.Enum.GetValues(typeof(WaveType)).Length;
        for (int i = 0; i < WaveTypeCount; i++)
        {
            // 목숨 감소 하트 이미지로 변경
            HeartGameObjects[i].hearts[playerLifeValue - 1].sprite = newHeartSprite;

            // 파티클 효과 위치 설정 및 재생
            ParticleSystem ps = Instantiate(minusPrefab);
            //Debug.Log($"ps exist? {ps != null} {(int)GameManager.Wave.currentWave}, {playerLifeValue - 1}");
            ps.transform.position = HeartGameObjects[(int)GameManager.Wave.currentWave].hearts[playerLifeValue - 1].transform.position;
            ps.Play();
            /*
            HeartGameObjects[i].hearts[playerLifeValue - 1].GetComponent<MeshRenderer>().material.color = Color.black;
            minusPrefab.transform.position = HeartGameObjects[(int)GameManager.Wave.currentWave].hearts[playerLifeValue - 1].transform.position;
            minusPrefab.Play();
            */
        }
    }

    // 처음에 3
    public void MinusPlayerLifeValue()
    {
        ////  || GameManager.Wave.currenWaveNum <= 2 <- XMC용 튜토리얼
        //if (playerLifeValue == 0) return;
        //// if (playerLifeValue == 0 || GameManager.Wave.currenWaveNum <= 2) return;

        //SetHearts(playerLifeValue);

        // [팝업스토어] SYJ 항상 목숨 1 이상 유지
        /*if (playerLifeValue <= 1)
        {
            return;
        }*/
        if (GameManager.Instance.currentGameState == GameState.Tutorial) return;
        
        SetHearts(playerLifeValue);
        playerLifeValue = Mathf.Max(0, playerLifeValue - 1);
        //Debug.Log($"After: playerLifeValue = {playerLifeValue}");

        //playerLifeValue--;
        //Debug.Log("Attack Success player HP -1");
    }

    // Ending Scene에서 main play 캐릭터 필요 없음
    public void InActivePlayer()
    {
        player.gameObject.SetActive(false);
        //IK_player.SetActive(false);
    }
    // ===================================
    // =========== Haptic 진동 =========== 
    // ===================================
    // 기본 진동
    public void ActiveRightHaptic(float amplitude, float duration)
    {
        OVRInput.SetControllerVibration(amplitude, amplitude, rightController);
        StartCoroutine(StopHapticAfterDuration(rightController, duration));
    }
    
    public void ActiveLeftHaptic(float amplitude, float duration)
    {
        OVRInput.SetControllerVibration(amplitude, amplitude, leftController);
        StartCoroutine(StopHapticAfterDuration(leftController, duration));
    }
    
    private IEnumerator StopHapticAfterDuration(OVRInput.Controller controller, float duration)
    {
        yield return new WaitForSeconds(duration);
        OVRInput.SetControllerVibration(0, 0, controller);
    }

    // 반복 진동
    public void RepeatRightHaptic(float amplitude, float duration, int n)
    {
        StartCoroutine(RepeatHapticCoroutine(rightController, amplitude, duration, n));
    }

    public void RepeatLeftHaptic(float amplitude, float duration, int n)
    {
        StartCoroutine(RepeatHapticCoroutine(leftController, amplitude, duration, n));
    }

    private IEnumerator RepeatHapticCoroutine(OVRInput.Controller controller, float amplitude, float duration, int n)
    {
        for (int i = 0; i < n; i++)
        {
            OVRInput.SetControllerVibration(amplitude, amplitude, controller);
            yield return new WaitForSeconds(duration);
            OVRInput.SetControllerVibration(0, 0, controller);
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(5f);
        GameManager.Instance.WaveToLobby();
    }

    // 점차 약해지는 진동
    public void DecreaseRightHaptic(float startAmplitude, float duration)
    {
        StartCoroutine(DecreaseHapticCoroutine(rightController, startAmplitude, duration));
    }

    public void DecreaseLeftHaptic(float startAmplitude, float duration)
    {
        StartCoroutine(DecreaseHapticCoroutine(leftController, startAmplitude, duration));
    }

    private IEnumerator DecreaseHapticCoroutine(OVRInput.Controller controller, float startAmplitude, float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            float elapsedTime = Time.time - startTime;
            float t = elapsedTime / duration;
            float currentAmplitude = Mathf.Lerp(startAmplitude, 0f, t);

            OVRInput.SetControllerVibration(currentAmplitude, currentAmplitude, controller);

            yield return null;
        }
        OVRInput.SetControllerVibration(0, 0, controller);
    }

    // 점차 강해지는 진동
    public void IncreaseRightHaptic(float startAmplitude, float duration)
    {
        StartCoroutine(IncreaseHapticCoroutine(rightController, startAmplitude, duration));
    }

    public void IncreaseLeftHaptic(float startAmplitude, float duration)
    {
        StartCoroutine(IncreaseHapticCoroutine(leftController, startAmplitude, duration));
    }

    private IEnumerator IncreaseHapticCoroutine(OVRInput.Controller controller, float startAmplitude, float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            float elapsedTime = Time.time - startTime;
            float t = elapsedTime / duration;
            float currentAmplitude = Mathf.Lerp(0f, startAmplitude, t);

            OVRInput.SetControllerVibration(currentAmplitude, currentAmplitude, controller);

            yield return null;
        }
        OVRInput.SetControllerVibration(0, 0, controller);
    }

}
