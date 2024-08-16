using Cysharp.Threading.Tasks;
using DG.Tweening;
using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    // [SerializeField] 
    //private float minZ = 0.0f;
    // [SerializeField] 
    private float maxZ = 0.07f;

    [SerializeField] public DiaryAniController diaryController_;
    private GrabInteractable grab;
    private bool isWaiting;
    private Vector3 originalPos;

    void Start() {
        grab = GetComponent<GrabInteractable>();
        grab.WhenPointerEventRaised -= OnGrab;
        grab.WhenPointerEventRaised += OnGrab;
        originalPos = transform.position;   
        isWaiting = false;
    }

    async void OnGrab(PointerEvent pointerEvent)
    {
        if (!isWaiting && pointerEvent.Type == PointerEventType.Select)
        {
            transform.DOLocalMoveZ(maxZ, 0.5f);
            //transform.position = new Vector3(originalPos.x, originalPos.y, originalPos.z - 1);
            isWaiting = true;   
        }
        else if(isWaiting && pointerEvent.Type == PointerEventType.Unselect)
        {
            if(diaryController_.gameObject.activeSelf)
            {
                diaryController_.nextPage();
            }
            transform.DOLocalMoveZ(0, 1f);
            //transform.position = new Vector3(originalPos.x, originalPos.y, originalPos.z);
            await UniTask.WaitForSeconds(2);
            isWaiting = false;
        }
    }
}