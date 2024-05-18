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

    private float shakeThreshold = 0.05f; // ��Ʈ�ѷ��� ��ġ�� �� ����ŭ ��ȭ�� �� shakeCount�� ������ų �� �ֵ��� ����

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HandController"))
        {
            isAttached = true;
            //grabInteractable.enabled = false;
            controllerTransform = other.transform;
            lastPosition = controllerTransform.position;
            //Debug.Log("Ʈ������ �ν�");
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
            Debug.Log("�� �ν�");

            if (deltaX >= shakeThreshold || deltaY >= shakeThreshold)
            {
                shakeCount++;
                Debug.Log("ī��Ʈ ����" + shakeCount);

                if (shakeCount >= 5)
                {
                    Detach();
                    Debug.Log("Detach �Լ� ����");
                }
            }

            lastPosition = currentPosition;

        }

    }

    public void Detach()
    {
        isAttached = false;
        //grabInteractable.enabled = true; // isAttached�� false�� �� Grab �����ϵ��� ����
        controllerTransform = null;
        shakeCount = 0; // Detach ���� shakeCount�� �ʱ�ȭ
       
    }
}
