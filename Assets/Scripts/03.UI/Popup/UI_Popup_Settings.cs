using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;


public class UI_Popup_Settings : UI_Popup
{
    enum Buttons
    {
        ResumeButton,
        LobbyButton,
    }

    enum Sliders
    {
        Slider_volume,
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
        Bind<Slider>(typeof(Sliders));
        Bind<TMP_Text>(typeof(Texts));
        GetButton((int)Buttons.ResumeButton).gameObject.AddUIEvent(OnButtonClicked_Resume, Define.UIEvent.Click);
        GetButton((int)Buttons.LobbyButton).gameObject.AddUIEvent(OnButtonClicked_Lobby, Define.UIEvent.Click);
        GetSlider((int)Sliders.Slider_volume).onValueChanged.AddListener(OnSliderValueChanged); //AddUIEvent(OnButtonClicked_Resume, Define.UIEvent.Click);
        GetSlider((int)Sliders.Slider_volume).value = GameManager.Sound.volume;
    }
    
    public void OnButtonClicked_Resume(PointerEventData data)
    {
        base.ClosePopupUI();
        GameManager.UI.SetRayOn(false);
        GameManager.Wave.SetIsPause(false);
    }

    public void OnButtonClicked_Lobby(PointerEventData data)
    {
        GameObject go = GameManager.UI.ShowPopupUI<UI_Popup>("PopupLobby").gameObject;
        GameManager.UI.SetCanvas(go, true);
    }

    private void OnSliderValueChanged(float value)
    {
        GameManager.Sound.SetVolume(value);
    }
}
