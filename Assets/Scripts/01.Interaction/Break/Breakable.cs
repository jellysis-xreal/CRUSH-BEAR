using System;
using UnityEngine.Events;

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

        private MotionChecker _motionChecker;
        private PunchaleMovement _punchaleMovement;
        private void Awake()
        {
            _motionChecker = GetComponent<MotionChecker>();
            _punchaleMovement = GetComponent<PunchaleMovement>();
        }

        // 다시 풀링에 넣을 때 변수 초기화, VFX 초기화 
        public void InitBreakable()
        {
            m_Destroyed = false;
            _motionChecker._isTriggered = false;
        }
        
        
        // Trigger는 MotionChecker에서만 진행, MotionChecker에서 DidCorrectMotion, Wrong 호출
        // 메서드 이름을 역할에 맞춰 이후에 수정하기
        [ContextMenu("BreakTest")]
        public void MotionSucceedTest() // Breakable.IsHit의 파라미터 전달하기 위함.
        {
            if (m_Destroyed)
                return;

            Debug.Log("Motion Succeed!");
            
            m_Destroyed = true;
            var brokenVersion = Instantiate(m_BrokenVersion, transform.position, transform.rotation);

            // m_OnBreak.Invoke(other.gameObject, brokenVersion); // 현재 구현된 이벤트 없음. 이벤트 수정해서 사용
            brokenVersion.GetComponent<BreakController>().IsHit(Vector3.one);
            // GameManager.Score.ScoringPunch(this.gameObject, true);

            // 이펙트 테스트 코드
            GameObject effect;
            effect = Resources.Load("Prefabs/Effects/Score_perfect") as GameObject;
            Instantiate(effect, transform.position + Vector3.forward, Quaternion.identity);
            
            _punchaleMovement.EndInteraction();
        }
        
        public void MotionSucceed(Transform handTransform) // Breakable.IsHit의 파라미터 전달하기 위함.
        {
            if (m_Destroyed)
                return;

            Debug.Log("Motion Succeed!");
            
            m_Destroyed = true;
            var brokenVersion = Instantiate(m_BrokenVersion, transform.position, transform.rotation);

            // m_OnBreak.Invoke(other.gameObject, brokenVersion); // 현재 구현된 이벤트 없음. 이벤트 수정해서 사용
            brokenVersion.GetComponent<BreakController>().IsHit(handTransform.forward);
            GameManager.Score.ScoringPunch(this.gameObject, true);

            _punchaleMovement.EndInteraction();
        }
        public void MotionFailed(Transform handTransform)
        {
            // 인터랙션했지만 MotionChecker.correctMotion과 일치하지 않을 때 Fail 처리
            if (m_Destroyed)
                return;
            
            Debug.Log("Motion Failed!");
            
            m_Destroyed = true;
            var brokenVersion = Instantiate(m_BrokenVersion, transform.position, transform.rotation);

            // m_OnBreak.Invoke(other.gameObject, brokenVersion);
            brokenVersion.GetComponent<BreakController>().IsHit(handTransform.forward);
            // GameManager.Player.MinusPlayerLifeValue();
            // GameManager.Score.ScoringPunch(this.gameObject, false);

            _punchaleMovement.EndInteraction();
        }

        public void MotionMissed()
        {
            // 못 치고 지나가면 Miss
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (m_Destroyed)
                return;
            // Motion Checker OnTriggerEnter와 연결해야 함.

            if (other.gameObject.tag.Equals(m_ColliderTag, System.StringComparison.InvariantCultureIgnoreCase))
            {
                Debug.Log($"correct Motion is {_motionChecker.correctMotion}");
                Debug.Log($"Hand Motion is : {other.gameObject.GetComponent<HookMotionDetector>().motion}");
                // Debug.Log($"correct Motion is {_motionChecker.correctMotion}");
                // Debug.Log($"Hand Motion is : {other.gameObject.GetComponent<HookMotionDetector>().motion}");
                HookMotionDetector detector = other.gameObject.GetComponent<HookMotionDetector>();

                if ((detector.motion != _motionChecker.correctMotion) || !detector.GetControllerActivateAction())
                {
                    Debug.Log("you did Wrong Motion;");
                    return;
                }
                m_Destroyed = true;
                var brokenVersion = Instantiate(m_BrokenVersion, transform.position, transform.rotation);
                m_OnBreak.Invoke(other.gameObject, brokenVersion);
                brokenVersion.GetComponent<BreakController>().IsHit();
                GameManager.Score.Scoring(this.gameObject);
                _punchaleMovement.EndInteraction();
                // Destroy(gameObject, 0.1f);
            }
        }
    }
}
