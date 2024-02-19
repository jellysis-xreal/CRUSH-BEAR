using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UI_Score : UI_Base
{
    enum Texts
    {
        TXT_score
    }

    private void Start()
    {
        Init();
    }

    public void Update()
    {
        ScoreUpdate();
    }

    public override void Init()
    {
        base.Init();
        Bind<TextMesh>(typeof(Texts));
    }

    public void ScoreUpdate()
    {
        int score = (int)GameManager.Score.TotalScore;
        Get<TextMesh>((int)Texts.TXT_score).text = score.ToString("D4");
    }
}
