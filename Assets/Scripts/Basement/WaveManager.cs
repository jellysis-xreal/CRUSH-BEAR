using System;
using UnityEngine;
using EnumTypes;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Collections;


public class WaveManager : MonoBehaviour
    {
        [Header("Wave Information")] 
        [SerializeField] private uint waveNum = 0;        // Wave number
        [SerializeField] private WaveType currentWave;    // 진행 중인 Wave Type
        [SerializeField] public float waveTime = 0.0f;
        [SerializeField] private bool IsPause = false;
        public GameObject node_forTest;
        [SerializeField] private GameObject[] GO_nodeLines;
        
        [Header("setting(auto)")] 
        [SerializeField] private GameObject RightInteraction;
        [SerializeField] private GameObject LeftInteraction;        
        [SerializeField] private TextMesh countText_mesh;
        private uint waveTypeNum = 3;   // Wave Type 갯수
        private int countdownTime = 3;
        public GameObject timerCanvas;
        
        public void Init()
        {
            // Wave Num
            waveNum = 0;
            waveTime = 0.0f;
                        
            // TODO: 임시로 노드 초기화좀 ...
            // node_forTest.gameObject.SetActive(true);

            foreach (GameObject nodeline in GO_nodeLines) {
                Debug.Log("[TEST] node set active false");
                nodeline.SetActive(false);
            }

            // RightInteraction = Utils.FindChildByRecursion(GameManager.Player.RightController.transform, "Interaction").gameObject;
            // LeftInteraction = Utils.FindChildByRecursion(GameManager.Player.LeftController.transform, "Interaction").gameObject;

        }
        
        public void Test()
        {
            currentWave = WaveType.Hitting;
            // SetThisWave();
            // SetThisWave 대신 countdown 한 번 해주기
            Time.timeScale = 0;
            CountDown(true);
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
            
            // Random으로 다음 wave를 지정
            currentWave = GetRandomWave();
            
            // Wave 세팅 (Player, )
            SetThisWave();
            waveNum++;
            
            // TODO: Wave 시작하기(240108)
            waveTime = 0.0f;
            // 음악 start
            // 노드 start
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

        private void SetThisWave()
        {
            int TypeNum = (int)currentWave;
            Debug.Log($"TypeNum : {TypeNum}");
            GO_nodeLines[TypeNum].SetActive(true);

            // RightInteraction.transform.GetChild(0).gameObject.SetActive(false);
            // RightInteraction.transform.GetChild(1).gameObject.SetActive(false);
            // RightInteraction.transform.GetChild(2).gameObject.SetActive(false);
            
            // LeftInteraction.transform.GetChild(0).gameObject.SetActive(false);
            // LeftInteraction.transform.GetChild(1).gameObject.SetActive(false);
            // LeftInteraction.transform.GetChild(2).gameObject.SetActive(false);

            // RightInteraction.transform.GetChild(TypeNum).gameObject.SetActive(true);
            // LeftInteraction.transform.GetChild(TypeNum).gameObject.SetActive(true);
        }

        private void Update()
        {
            waveTime += Time.deltaTime;
            
            if (Input.GetKeyDown(KeyCode.F))
            {
                //Wave 일시정지 활성화
                if (IsPause == false)
                {
                    SetIsPause(true);
                    // IsPause = true;
                    return;
                }
                
                //Wave 일시정지 비활성화
                if (IsPause == true)
                {
                    SetIsPause(false);
                    // IsPause = false;
                    return;
                }
            }
        }

        public void SetIsPause(bool _isPause)
        {
            if (_isPause) {
                Time.timeScale = 0;
                PauseAudio(true);
                // 소리 끄기
            } else {
                // 소리 3초 후 틀기
                CountDown(false);
                Debug.Log("Resume the game after 3 sec...");
            }
        }

        public void PauseAudio(bool _isPause = false)
        {
            IsPause = _isPause;
            GameManager.Sound.PauseAudio(_isPause);
        }

        public void CountDown(bool Init)
        {
            StartCoroutine(CountdownToStart(Init));
        }

        IEnumerator CountdownToStart(bool Init)
        {
            timerCanvas.SetActive(true);

            while(countdownTime > 0)
            {
                countText_mesh.text = countdownTime.ToString();
                yield return new WaitForSecondsRealtime(1f);
                countdownTime--;
            }
            Debug.Log("[TEST] Game Start");
            if (Init) SetThisWave(); //{ timerCanvas.SetActive(false); PauseAudio(false); SetThisWave();}
            // else { timerCanvas.SetActive(false);}

            timerCanvas.SetActive(false);
            PauseAudio(false);
            Time.timeScale = 1;
            countdownTime = 3;
        }
    }
