using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class UI_Popup_Settings : UI_Popup
{
    enum Buttons
    {
        ResumeButton,
        LobbyButton,
    }

    enum Texts
    {
        TXT_volume,
        TXT_mode,
        TXT_resume,
        TXT_lobby,
        TXT_popup_settings,
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
        GetButton((int)Buttons.ResumeButton).gameObject.AddUIEvent(OnButtonClicked_Resume, Define.UIEvent.Click);
        GetButton((int)Buttons.LobbyButton).gameObject.AddUIEvent(OnButtonClicked_Lobby, Define.UIEvent.Click);
    }
    
    public void OnButtonClicked_Resume(PointerEventData data)
    {
        base.ClosePopupUI();
    }

    public void OnButtonClicked_Lobby(PointerEventData data)
    {
        GameObject go = GameManager.UI.ShowPopupUI<UI_Popup>("PopupLobby").gameObject;
        GameManager.UI.SetCanvas(go, true);
    }
}
