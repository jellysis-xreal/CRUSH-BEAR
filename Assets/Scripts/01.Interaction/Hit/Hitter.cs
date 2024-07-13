using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

public class Hitter : MonoBehaviour
{
    //public string triggerObj;
    //public InteractionSide triggerColor;
    
    // public void OnTriggerEnter(Collider other)
    // {
    //     triggerObj = other.name;
    //
    //     if (other.TryGetComponent(out HittableMovement hit))
    //     {
    //         triggerColor = hit.sideType;
    //     }
    // }


    private void OnTriggerStay(Collider other)
    {
        // 두 Hitter가 충돌할 때
        if (other.name == this.name)
        {
            GameManager.Player.ActiveRightHaptic(0.4f, 0.1f);
            GameManager.Player.ActiveLeftHaptic(0.4f, 0.1f);
        }
    }
}
