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
    }
    
    public void OnButtonClicked_Play(PointerEventData data)
    {
        GameManager.UI.SetRayOn(false);
        GameManager.Instance.LobbyToWave();
        //SceneManager.LoadScene("01.WaveScene");
    }

    public void OnButtonClicked_Exit(PointerEventData data)
    {
        //Debug.Log("quit");
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
