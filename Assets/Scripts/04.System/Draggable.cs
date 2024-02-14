using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Draggable : MonoBehaviour
{
    [SerializeField] private float minZ = 0.0f;
    [SerializeField] private float maxZ = 0.2f;

    void Update() {
        if (transform.localPosition.z > maxZ)
        {
            Debug.Log("Global Position: " + transform.localPosition);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, maxZ);
        }
    }
}