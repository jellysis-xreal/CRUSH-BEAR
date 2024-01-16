using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class UI_Popup_Lobby : UI_Popup
{
    enum Buttons
    {
        NoButton,
        YesButton
    }

    enum Texts
    {
        TXT_no,
        TXT_yes,
        TXT_popup_exit,
    }
    
    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        
        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));
        GetButton((int)Buttons.NoButton).gameObject.AddUIEvent(OnButtonClicked_No, Define.UIEvent.Click);
        GetButton((int)Buttons.YesButton).gameObject.AddUIEvent(OnButtonClicked_Yes, Define.UIEvent.Click);
    }
    
    public void OnButtonClicked_No(PointerEventData data)
    {
        base.ClosePopupUI();
    }

    public void OnButtonClicked_Yes(PointerEventData data)
    {
        base.ClosePopupUI();
    }
}
