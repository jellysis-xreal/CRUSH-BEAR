using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BearManager : MonoBehaviour
{
    public static BearManager instance;
    public int playerHearts = 3;
    public TMP_Text heartsText;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        UpdateHeartsText();   
    }

    
    public void DecreaseHearts()
    {
        playerHearts--;
        UpdateHeartsText();
    }

    private void UpdateHeartsText()
    {
        if (heartsText != null)
            heartsText.text = "Hearts: " + playerHearts;
    }
}