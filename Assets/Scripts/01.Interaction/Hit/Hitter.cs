using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

public class Hitter : MonoBehaviour
{
    public HandData handData;


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
