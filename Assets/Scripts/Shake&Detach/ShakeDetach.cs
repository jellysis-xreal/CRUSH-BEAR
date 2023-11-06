using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShakeDetach : MonoBehaviour
{
    public bool isAttached = false;
    public Transform controllerTransform;
    public XRGrabInteractable grabInteractable;

    public int shakeCount; 
    private Vector3 lastPosition;
    //public bool isAttached = false;

    private float shakeThreshold = 0.05f; // 컨트롤러의 위치가 이 값만큼 변화할 때 shakeCount를 증가시킬 수 있도록 설정

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HandController"))
        {
            isAttached = true;
            //grabInteractable.enabled = false;
            controllerTransform = other.transform;
            lastPosition = controllerTransform.position;
            Debug.Log("트랜스폼 인식");
        }
    }

    private void Update()
    {
        if (isAttached && controllerTransform != null)
        {
            transform.position = controllerTransform.position;
            transform.rotation = controllerTransform.rotation;

            Vector3 currentPosition = controllerTransform.position;
            float deltaX = Mathf.Abs(currentPosition.x - lastPosition.x);
            float deltaY = Mathf.Abs(currentPosition.y - lastPosition.y);
            Debug.Log("값 인식");

            if (deltaX >= shakeThreshold || deltaY >= shakeThreshold)
            {
                shakeCount++;
                Debug.Log("카운트 증가" + shakeCount);

                if (shakeCount >= 5)
                {
                    Detach();
                    Debug.Log("Detach 함수 실행");
                }
            }

            lastPosition = currentPosition;

        }

    }

    public void Detach()
    {
        isAttached = false;
        //grabInteractable.enabled = true; // isAttached가 false일 때 Grab 가능하도록 설정
        controllerTransform = null;
        shakeCount = 0; // Detach 이후 shakeCount를 초기화
       
    }
}
