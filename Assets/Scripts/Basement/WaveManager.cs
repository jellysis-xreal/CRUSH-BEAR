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
        [SerializeField] private bool IsPause = false;

        [Header("setting(auto)")] 
        [SerializeField] private GameObject RightInteraction;
        [SerializeField] private GameObject LeftInteraction;
        private uint waveTypeNum = 3;   // Wave Type 갯수

        public void Init()
        {
            // Wave Num
            waveNum = 0;
            waveTime = 0.0f;
            
            RightInteraction = Utils.FindChildByRecursion(GameManager.Player.RightController.transform, "Interaction").gameObject;
            LeftInteraction = Utils.FindChildByRecursion(GameManager.Player.LeftController.transform, "Interaction").gameObject;
        }
        
        public void Test()
        {
            currentWave = WaveType.Hitting;
            SetThisWave();
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
            RightInteraction.transform.GetChild(0).gameObject.SetActive(false);
            RightInteraction.transform.GetChild(1).gameObject.SetActive(false);
            RightInteraction.transform.GetChild(2).gameObject.SetActive(false);
            
            LeftInteraction.transform.GetChild(0).gameObject.SetActive(false);
            LeftInteraction.transform.GetChild(1).gameObject.SetActive(false);
            LeftInteraction.transform.GetChild(2).gameObject.SetActive(false);

            int TypeNum = (int)currentWave;
            RightInteraction.transform.GetChild(TypeNum).gameObject.SetActive(true);
            LeftInteraction.transform.GetChild(TypeNum).gameObject.SetActive(true);
        }

        private void Update()
        {
            waveTime += Time.deltaTime;
            
            if (Input.GetKeyDown(KeyCode.F))
            {
                //Wave 일시정지 활성화
                if (IsPause == false)
                {
                    Time.timeScale = 0;
                    IsPause = true;
                    return;
                }
                
                //Wave 일시정지 비활성화
                if (IsPause == true)
                {
                    Time.timeScale = 1;
                    IsPause = false;
                    return;
                }
            }
        }
    }
