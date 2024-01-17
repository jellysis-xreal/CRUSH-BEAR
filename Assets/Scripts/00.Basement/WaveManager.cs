using System;
using UnityEngine;
using EnumTypes;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
    {
        [Header("Wave Information")] 
        [SerializeField] private uint waveNum = 0;        // Wave number
        [SerializeField] private WaveType currentWave;    // 진행 중인 Wave Type
        [SerializeField] public float waveTime = 0.0f;
        public bool IsPause = false;
        //public GameObject node_forTest;
        
        [Header("setting(auto)")] 
        [SerializeField] private GameObject RightInteraction;
        [SerializeField] private GameObject LeftInteraction;
        private uint waveTypeNum = 3;   // Wave Type 갯수
        [SerializeField] private NodeInstantiator_minha nodeInstantiator;

        private bool IsWait = false;
        private float timerDuration = 5f;
        private float waitTimer;
        
        public void Init()
        {
            Debug.Log("Initialize WaveManager");
            
            // Wave Num
            waveNum = 0;
            waveTime = 0.0f;
            
            RightInteraction = Utils.FindChildByRecursion(GameManager.Player.RightController.transform, "Interaction").gameObject;
            LeftInteraction = Utils.FindChildByRecursion(GameManager.Player.LeftController.transform, "Interaction").gameObject;
            
            // TODO: 임시로 노드 초기화좀 ...
            //node_forTest.gameObject.SetActive(true);
            
            // Player가 플레이할 Wave 진행함
            NextWave();
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
        /// Wave 전환 시 사용합니다.
        /// Random으로 다음 wave를 지정하고, 세팅합니다.
        /// </summary>
        public void NextWave()
        {
            // TODO: Wave를 지정하는 효과 ON(240108)
            
            // ~초 대기
            SetPauseWave();
            
            // Random으로 다음 wave를 지정
            //currentWave = GetRandomWave();
            // TODO: 임시 설정. For Test
            currentWave = WaveType.Hitting;
            
            // Wave 세팅
            SetWavePlayer();                     // Player의 Interact 세팅
            nodeInstantiator.Init(currentWave);  // Node Loader 세팅
            // TODO: Scene 내의 점수판 세팅
            // TODO: Scene 내의 조명 세팅
            waveNum++;
            
            // TODO: Wave 시작하기(240108)
            waveTime = 0.0f;
            // 음악 start
            // 노드는 Time.timeScale == 1일 경우 자동으로 Update 됨.
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
            waveTime += Time.deltaTime;

            // 기다리는 중이라면,
            if (IsPause && IsWait)
            {
                if (waitTimer > 0f)
                {
                    waitTimer -= Time.unscaledDeltaTime;
                    Debug.Log("Timer: " + waitTimer.ToString("F2"));
                }
                else
                {
                    Debug.Log("Timer finished!");
                    
                    // 타이머가 종료되었을 때 Wave Active
                    SetContinueWave();
                    IsWait = false;
                }
            }
        }

        public void SetPauseWave()
        {
            Debug.Log("[WAVE] Wave 일시중지");
            Time.timeScale = 0f;
            IsPause = true;
            
            if (IsWait == false)
            {
                IsWait = true;
                waitTimer = timerDuration;
                return;
            }
            
            if (IsWait == true)
            {
                IsWait = false;
                return;
            }
        }
        
        private void SetContinueWave()
        {
            Debug.Log("[WAVE] Wave 재개");
            Time.timeScale = 1f;
            IsPause = false;
        }
        
        public void SetIsPause(bool _isPause)
        {
            IsPause = _isPause;
            if (_isPause) {
                // 소리 끄기
            } else {
                // 소리 3초 후 틀기
                Debug.Log("Resume the game after 3 sec...");

            }
        }
    }
