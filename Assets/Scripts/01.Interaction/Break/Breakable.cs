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

        bool m_Destroyed = false;

        /// <summary>
        /// Events to fire when a matching object collides and break this object.
        /// The first parameter is the colliding object, the second parameter is the 'broken' version.
        /// </summary>
        public BreakEvent onBreak => m_OnBreak;

        private MotionChecker _motionChecker;

        private void Awake()
        {
            _motionChecker = GetComponent<MotionChecker>();
        }

        private void OnCollisionStay(Collision collisionInfo)
        {

        }

        /*void OnCollisionEnter(Collision collision)
        {
            if (m_Destroyed)
                return;
            
            // Motion Checker OnTriggerEnter와 연결해야 함.
            
            if (collision.gameObject.tag.Equals(m_ColliderTag, System.StringComparison.InvariantCultureIgnoreCase))
            {
                Debug.Log($"correct Motion is {_motionChecker.correctMotion}");
                Debug.Log($"Hand Motion is : {collision.gameObject.GetComponent<HookMotionDetector>().motion}");
                if (collision.gameObject.GetComponent<HookMotionDetector>().motion != _motionChecker.correctMotion)
                {
                    Debug.Log("you did Wrong Motion;");
                    return;
                }

                m_Destroyed = true;
                var brokenVersion = Instantiate(m_BrokenVersion, transform.position, transform.rotation);
                
                m_OnBreak.Invoke(collision.gameObject, brokenVersion);
                brokenVersion.GetComponent<BreakController>().IsHit();
                GameManager.Score.Scoring(this.gameObject);
                
                Destroy(gameObject, 0.1f);
            }
        }*/

        private void OnTriggerEnter(Collider other)
        {
            if (m_Destroyed)
                return;

            // Motion Checker OnTriggerEnter와 연결해야 함.

            if (other.gameObject.tag.Equals(m_ColliderTag, System.StringComparison.InvariantCultureIgnoreCase))
            {
                Debug.Log($"correct Motion is {_motionChecker.correctMotion}");
                Debug.Log($"Hand Motion is : {other.gameObject.GetComponent<HookMotionDetector>().motion}");
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

                Destroy(gameObject, 0.1f);
            }
        }
    }
}
