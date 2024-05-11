using System;
using UnityEngine.Events;
using EnumTypes;
using UnityEngine.SceneManagement;
using Motion = EnumTypes.Motion;

namespace UnityEngine.XR.Content.Interaction
{
    /// <summary>
    /// Detects a collision with a tagged collider, replacing this object with a 'broken' version
    /// </summary>
    public class Breakable : MonoBehaviour
    {
        [Serializable]
        public class BreakEvent : UnityEvent<GameObject, GameObject>
        {
        }

        [SerializeField] [Tooltip("The 'broken' version of this object.")]
        GameObject m_BrokenVersion;

        [SerializeField] [Tooltip("The tag a collider must have to cause this object to break.")]
        string m_ColliderTag = "Destroyer";

        [SerializeField]
        [Tooltip("Events to fire when a matching object collides and break this object. " +
                 "The first parameter is the colliding object, the second parameter is the 'broken' version.")]
        BreakEvent m_OnBreak = new BreakEvent();

        public bool m_Destroyed = false;

        /// <summary>
        /// Events to fire when a matching object collides and break this object.
        /// The first parameter is the colliding object, the second parameter is the 'broken' version.
        /// </summary>
        public BreakEvent onBreak => m_OnBreak;
        
        private IPunchableMovement _punchableMovement;
        
        private ChildTriggerChecker _childTriggerChecker;
        public EnumTypes.Motion correctMotion = EnumTypes.Motion.None;
        
        // 다시 풀링에 넣을 때 변수 초기화, VFX 초기화 
        public void InitBreakable()
        {
            if(_punchableMovement == null)
                _punchableMovement = GetComponent<IPunchableMovement>();
            
            if (_childTriggerChecker == null)
            {
                _childTriggerChecker = GetComponentInChildren<ChildTriggerChecker>();
                correctMotion = _childTriggerChecker.handMotion;
            }
            
            m_Destroyed = false;
        }
        
        public void MotionSucceed(EnumTypes.Motion motion) // Breakable.IsHit의 파라미터 전달하기 위함.
        {
            if (m_Destroyed)
                return;

            //Debug.Log("Motion Succeed!");
            
            m_Destroyed = true;
            var brokenVersion = Instantiate(m_BrokenVersion, transform.position, transform.rotation);

            // m_OnBreak.Invoke(other.gameObject, brokenVersion); // 현재 구현된 이벤트 없음. 이벤트 수정해서 사용
            brokenVersion.GetComponent<BreakController>().IsHit(motion);
            if (GameManager.Instance.currentGameState == GameState.Waving)
            {
                GameManager.Score.ScoringPunch(this.gameObject, true);
                _punchableMovement.EndInteraction();
            }
            else if (GameManager.Instance.currentGameState == GameState.Tutorial)
            {
                //Debug.Log("[Tutorial Punch] Succeed");
                GameManager.TutorialPunch.processedNumber++; 
                GameManager.TutorialPunch.succeedNumber++;
                // GameManager.Instnace.Tutorial.processedNumber++
                // GameManager.Score.ScoringPunch(this.gameObject, true);
                _punchableMovement.EndInteraction();
            }
            

        }
        public void MotionFailed()
        {
            // 인터랙션했지만 MotionChecker.correctMotion과 일치하지 않을 때 Fail 처리
            if (m_Destroyed)
                return;
            
            
            m_Destroyed = true;
            var brokenVersion = Instantiate(m_BrokenVersion, transform.position, transform.rotation);

            // m_OnBreak.Invoke(other.gameObject, brokenVersion);
            
            // Trigger 했지만 모션 fail
            brokenVersion.GetComponent<BreakController>().IsHit();

            if (GameManager.Instance.currentGameState == GameState.Waving)
            {
                GameManager.Score.ScoringPunch(this.gameObject, false);
                // GameManager.Player.MinusPlayerLifeValue();
                _punchableMovement.EndInteraction();
            }
            else if (GameManager.Instance.currentGameState == GameState.Tutorial)
            {
                //Debug.Log("[Tutorial Punch] Fail");
                GameManager.TutorialPunch.processedNumber++;
                GameManager.Score.ScoringPunch(this.gameObject, true);
                _punchableMovement.EndInteraction();
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
#if UNITY_EDITOR
            if (GameManager.Instance != null)
            {
                if(GameManager.Instance.currentGameState == GameState.Waving) return;
            }
#endif
            if (m_Destroyed)
                return;
            // Motion Checker OnTriggerEnter와 연결해야 함.

            if (other.CompareTag("Destroyer"))
            {
                if (_childTriggerChecker.isTriggered)
                {
                    MotionSucceed(correctMotion);
                    //Debug.Log("Motion succeed! (child.isTriggered True!)");
                }
                else if(CheckAdditionalCondition())
                {
                    MotionSucceed(correctMotion);
                    //Debug.Log("Motion succeed! (child.isTriggered True!, Additional Condition True)");
                }
                else
                {
                    MotionFailed();
                }
            }
        }
        
        private bool CheckAdditionalCondition()
        {
            // 추가 조건 검사. 프레임 사이에 콜라이더를 지나 자식의 콜라이더에 트리거되지 않았을 경우를 대비한 메서드
            bool isRightPosition = false;

            return isRightPosition;
        }
    }
}
