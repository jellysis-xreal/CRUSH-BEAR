using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectArrivalArea : MonoBehaviour
{
    public int boxIndex;
    // 오브젝트에게 area 각각의 위치를 알려주기 위함.
    private void OnTriggerEnter(Collider other)
    {
        // 오브젝트와 arrival area가 트리거되면 후속 코드 진행 (방향을 플레이어를 향해서 날아옴 ex) 공격모션) 
    }

    private void OnDrawGizmosSelected()
    {
        /*Gizmos.color = Color.gray;
        Gizmos.DrawCube(transform.position, Vector3.one * 0.5f);*/
    }
}
