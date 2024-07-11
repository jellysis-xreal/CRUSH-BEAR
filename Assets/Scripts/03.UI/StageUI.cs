using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title, score;
    [SerializeField] private Image background, curtain, rank, bearImage;
    [SerializeField] private SpriteData spriteData;
    [SerializeField] private Button stageStart;

    public void InitSettings(StageData[] stageData, int ID)
    {
        SaveData temp = GameManager.Instance.Save.data;
        if (stageData.Length <= ID || !temp.isUnlocked[ID])
        {
            curtain.sprite = spriteData.lockedCurtain;
            score.gameObject.SetActive(false);
            rank.gameObject.SetActive(false);
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
            background.color = Color.white;
            bearImage.color = Color.white;
            title.text = stageData[ID].stageName;
            // 현재 버튼으로 되어 있으나, 쿠키로 변경 예정 
            stageStart.onClick.RemoveAllListeners();
            stageStart.onClick.AddListener(() => StartStage(stageData[ID]));
            score.text = temp.currentScore.ToString();
            // 현재 점수 비교해서 랭크 산정
            float rankScore = temp.currentScore[ID] / (float)stageData[ID].maxScore;

            if (rankScore > 0.9f)
                rank.sprite = spriteData.rank[0];
            else if (rankScore > 0.8f)
                rank.sprite = spriteData.rank[1];
            else
                rank.sprite = spriteData.rank[2];
        }
    }

    public void StartStage(StageData stageName)
    {
        GameManager.Sound.SetMusic(stageName);
        GameManager.Instance.LobbyToWave();
    }
}
