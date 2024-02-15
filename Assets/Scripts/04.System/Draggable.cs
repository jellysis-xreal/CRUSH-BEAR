using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Draggable : MonoBehaviour
{
    [SerializeField] private float minZ = 0.0f;
    [SerializeField] private float maxZ = 0.2f;

    [SerializeField] public DiaryAniController diaryController_;
    Vector3 transformOrigin;

    void Start() {
        transformOrigin = transform.position;
    }

    void Update() {
        transform.position = new Vector3(transformOrigin.x, transformOrigin.y, transform.position.z);

        if ((transformOrigin.z - transform.position.z) > 0.0f)
        {
            diaryController_.returned = true;
        }

        if ((transformOrigin.z - transform.position.z) < -maxZ)
        {
            if (diaryController_.returned)
            {
                diaryController_.returned = false;
                // Debug.Log("Global Position: " + transform.localPosition);
                Debug.Log("[TEST] bookPage: " + diaryController_.bookPage.ToString());
                transform.position = new Vector3(transformOrigin.x, transformOrigin.y, transformOrigin.z + maxZ);
                diaryController_.nextPage();
            }
        }
    }
}