using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Results : MonoBehaviour
{
    [Header("----+ UI +----")]
    public GameObject ScoreUI;
    public GameObject HeartUI;
    public GameObject WaveUI;
    
    [Space(10)]
    
    [Header("----+ setting +----")]
    public Sprite BlankHeart;

    public void SettingValues(float Score, int heart, uint Wave)
    {
        ScoreUI.GetComponent<TMPro.TextMeshProUGUI>().text = $"{Score}";
        WaveUI.GetComponent<TMPro.TextMeshProUGUI>().text = $"Wave : {Wave}";
        
        for (int i = 4; i > heart; i--)
        {
            HeartUI.transform.GetChild(i).GetComponent<UnityEngine.UI.Image>().sprite = BlankHeart;
        }
    }
    
    public void ShowResults()
    {
        this.GetComponent<Canvas>().enabled = true;
    }
    
}
