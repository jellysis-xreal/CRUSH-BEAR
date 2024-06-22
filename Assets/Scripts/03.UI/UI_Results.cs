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
    public GameObject EndCookie;

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
        // 플레이어의 위치와 방향을 가져옵니다.
        GameObject player = GameObject.FindWithTag("MainCamera");
        Vector3 playerPosition = player.transform.position;
        Vector3 playerDirection = player.transform.forward;

        // 플레이어의 정면 방향으로 일정 거리만큼 이동한 위치를 계산합니다.
        float distance = 6.0f; // 이 값은 원하는 대로 조정할 수 있습니다.
        Vector3 resultPosition = playerPosition + 
                                 (Vector3.up * 1.5f) + 
                                 (playerDirection * distance);

        // result를 그 위치에 설정합니다.
        transform.position = resultPosition;
        
        // UI가 플레이어를 바라보도록 회전을 설정합니다.
        Vector3 directionToPlayer = transform.position - player.transform.position;
        Quaternion rotationToPlayer = Quaternion.LookRotation(directionToPlayer);

        // y축 회전만을 추출합니다.
        float yRotation = rotationToPlayer.eulerAngles.y;

        // y축 회전만을 적용합니다.
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
        
        this.GetComponent<Canvas>().enabled = true;

        GameObject cookie = Instantiate(EndCookie,
            playerPosition + playerDirection*1.3f,
            Quaternion.Euler(0, yRotation, 0)
            );
    }
    
}
