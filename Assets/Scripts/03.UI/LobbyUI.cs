using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private StageUI stageUI;
    // 0번이 플레이 버튼, 1번이 왼쪽 버튼, 2번이 우측버튼
    [SerializeField] private BreakableButton[] buttons;
    [SerializeField] private GameObject[] tutorialObject;
    [SerializeField] private GameObject[] lobbyObject;
    [SerializeField] private Image playButton;
    private int currentIndex;
    public int CurrentIndex { get { return currentIndex; } set { currentIndex = value;  SetButtons(); } }
    private const int PLAY_INDEX = 0;
    private const int LEFT_INDEX = 1;
    private const int RIGHT_INDEX = 2;

    public void InitSettings()
    {
        foreach (var button in buttons)
            button.InitSettings();
        RefreshObjects();
        CurrentIndex = GameManager.Wave.stageID;
        buttons[PLAY_INDEX].AddEvent(StartGame);
        buttons[LEFT_INDEX].AddEvent(GoLeft);
        buttons[RIGHT_INDEX].AddEvent(GoRight);
        stageUI.SetID(CurrentIndex);
    }

    [ContextMenu("Test")]
    public void StartGame()
    {
        GameManager.Sound.SetMusic(GameManager.Data.stageData[CurrentIndex]);
        GameManager.Data.LoadInitialWaveData(CurrentIndex);
        GameManager.Instance.LobbyToWave();
    }

    public void GoLeft()
    {
        CurrentIndex = Mathf.Max(0, CurrentIndex - 1);
        stageUI.SetID(CurrentIndex);
    }

    public void GoRight()
    {
        //현재 2스테이지가 더미라 Length로 해놓음 추후 Length - 1로 변경
        CurrentIndex = Mathf.Min(CurrentIndex + 1, GameManager.Data.stageData.Length);
        stageUI.SetID(CurrentIndex);
    }

    public void RefreshObjects()
    {
        if (GameManager.Instance.Save.data.isFirst)
        {
            foreach (var gameObject in tutorialObject)
            {
                gameObject.SetActive(true);
            }
            foreach (var gameObject in lobbyObject)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (var gameObject in tutorialObject)
            {
                gameObject.SetActive(false);
            }
            foreach (var gameObject in lobbyObject)
            {
                gameObject.SetActive(true);
            }
        }
    }

    public void SetButtons()
    {
        buttons[LEFT_INDEX].transform.parent.gameObject.SetActive(true);
        buttons[RIGHT_INDEX].transform.parent.gameObject.SetActive(true);
        if (CurrentIndex == 0)
        {
            buttons[LEFT_INDEX].transform.parent.gameObject.SetActive(false);
        }
        else if (CurrentIndex == GameManager.Data.stageData.Length)
        {
            buttons[RIGHT_INDEX].transform.parent.gameObject.SetActive(false);
        }

        if (GameManager.Instance.Save.data.isUnlocked.Length <= currentIndex || !GameManager.Instance.Save.data.isUnlocked[currentIndex])
        {
            buttons[PLAY_INDEX].isBlocked = true;
            playButton.color = Color.gray;
        }
        else
        {
            buttons[PLAY_INDEX].isBlocked = false;
            playButton.color = Color.white;
        }
    }
}
