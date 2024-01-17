using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class UI_Lobby : UI_Base
{
    enum Buttons
    {
        PlayButton,
        ExitButton
    }

    enum Texts
    {
        TXT_Play,
        TXT_Exit
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
        GetButton((int)Buttons.PlayButton).gameObject.AddUIEvent(OnButtonClicked_Play, Define.UIEvent.Click);
        GetButton((int)Buttons.ExitButton).gameObject.AddUIEvent(OnButtonClicked_Exit, Define.UIEvent.Click);
    }
    
    public void OnButtonClicked_Play(PointerEventData data)
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void OnButtonClicked_Exit(PointerEventData data)
    {
        Debug.Log("quit");
        Application.Quit();
    }

    // public void OnApplicationQuit()
    // {
    //     Application.CancelQuit();
    //     #if!UNITY_EDITOR
    //         System.Diagnostics.Process.GetCurrentProcess().Kill();
    //     #endif
    // }
}
