using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : UI_Base
{
    public virtual void init()
    {
        GameManager.UI.SetCanvas(gameObject, true);
    }

    public virtual void ClosePopupUI()
    {
        // Debug.Log("[TEST] Close Popup UI");
        GameManager.UI.ClosePopupUI(this);
    }
}