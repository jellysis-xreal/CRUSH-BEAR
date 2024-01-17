using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;

public class BaseObject : MonoBehaviour
{
    public InteractionType InteractionType;
    public MoveType MoveType;

    private bool IsScored = false;

    public void SetScoreBool()
    {
        IsScored = true;
    }

    public bool IsItScored()
    {
        return IsScored;
    }
    
    public void UpdateMovement()
    {
        
    }
}
