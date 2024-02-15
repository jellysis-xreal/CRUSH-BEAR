using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Draggable : MonoBehaviour
{
    [SerializeField] private float minZ = 0.0f;
    [SerializeField] private float maxZ = 0.2f;

    [SerializeField] public DiaryAniController diaryController_;

    void Update() {
        if (transform.localPosition.z > maxZ)
        {
            if (!diaryController_.getIsPlaying()) {
                diaryController_.setIsPlaying(true);
                Debug.Log("Global Position: " + transform.localPosition);
                Debug.Log("[TEST] bookPage: " + diaryController_.bookPage.ToString());
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, maxZ);
                diaryController_.nextPage();
            }
        }
    }
}