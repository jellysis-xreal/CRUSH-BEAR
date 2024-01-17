using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UI_Settings : UI_Base
{
    enum Buttons
    {
        SettingsButton
    }

    enum Texts
    {
        TXT_settings
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
        GetButton((int)Buttons.SettingsButton).gameObject.AddUIEvent(OnButtonClicked_, Define.UIEvent.Click);
    }
    
    public void OnButtonClicked_(PointerEventData data)
    {
        GameObject go = GameManager.UI.ShowPopupUI<UI_Popup>("PopupSettings").gameObject;
        GameManager.UI.SetCanvas(go, true);
    }
}
