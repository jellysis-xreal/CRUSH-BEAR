using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title, score, stage;
    [SerializeField] private Image background, curtain, rank, bearImage;
    [SerializeField] private SpriteData spriteData;

    public void SetID(int ID)
    {
        SaveData temp = GameManager.Instance.Save.data;
        StageData[] stageData = GameManager.Data.stageData;
        if (stageData.Length <= ID || !temp.isUnlocked[ID])
        {
            curtain.sprite = spriteData.lockedCurtain;
            score.gameObject.SetActive(false);
            rank.gameObject.SetActive(false);
            stage.gameObject.SetActive(false);
            title.gameObject.SetActive(false);
            background.color = Color.gray;
            bearImage.color = Color.gray;
        }
        else
        {
            curtain.sprite = spriteData.unlockedCurtain;
            score.gameObject.SetActive(true);
            rank.gameObject.SetActive(true);
            title.gameObject.SetActive(true);
            stage.gameObject.SetActive(true);
            background.color = Color.white;
            bearImage.color = Color.white;

            stage.text = $"Stage {ID + 1}";
            title.text = stageData[ID].stageName;
            score.text = string.Format("{0:D6}",temp.currentScore[ID]);
            // 현재 점수 비교해서 랭크 산정

            if (temp.currentScore[ID] == 0)
            {
                score.text = "New!";
                score.fontStyle = FontStyles.Bold;
                rank.gameObject.SetActive(false);
            }
            else
            {
                score.text = string.Format("{0:D6}", temp.currentScore[ID]);
                score.fontStyle = FontStyles.Normal;
                float rankScore = temp.currentScore[ID] / (float)stageData[ID].maxScore;
                if (rankScore > 0.9f)
                    rank.sprite = spriteData.rank[0];
                else if (rankScore > 0.8f)
                    rank.sprite = spriteData.rank[1];
                else if (rankScore > 0.6f)
                    rank.sprite = spriteData.rank[2];
            }
        }
    }
}
