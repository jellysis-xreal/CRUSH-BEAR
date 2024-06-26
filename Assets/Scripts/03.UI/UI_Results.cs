using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Results : MonoBehaviour
{
    [SerializeField] private Rank _rank;
    
    [Space(10)]
    
    [Header("----+ UI +----")]
    public Image RankImage;
    public Image NoteImage;
    public TextMeshProUGUI ScoreUI;
    public TextMeshProUGUI ComboUI;
    public TextMeshProUGUI PerfectUI;
    public GameObject HeartUI;
    
    [Space(10)]
    
    [Header("----+ setting +----")]
    public GameObject EndCookie;

    [Header("----+ sprites+----")]
    public Sprite BlankHeart;
    public List<Sprite> RankSprites;
    public List<Sprite> NoteSprites;
    
 
    
    private enum Rank
    {
        S,
        A,
        B
    }

    private void ResultRank()
    {
        float TotalScore = 1000; // TODO 임시. 정확한 숫자 확인해야함
        float score = GameManager.Score.TotalScore;
        
        // 전체 스코어의 90% 이상을 받으면 S, 80% 이상을 받으면 A, 그 외는 B
        if (score >= (TotalScore * 0.9f))
        {
            _rank = Rank.S;
        }
        else if (score >= (TotalScore * 0.8f))
        {
            _rank = Rank.A;
        }
        else
        {
            _rank = Rank.B;
        }
    }
    
    public void SettingValues(float Score, int heart)
    {
        ResultRank(); // Set _rank
        RankImage.sprite = RankSprites[(int)_rank];
        NoteImage.sprite = NoteSprites[(int)_rank];
        
        ScoreUI.text = $"{Score}";
        ComboUI.text = $"{GameManager.Combo.GetMaxCombo()}";
        PerfectUI.text = $"{GameManager.Score.GetPerfectNum()}";
        
        for (int i = 4; i >= heart; i--)
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
        float distance = 7.5f; // 이 값은 원하는 대로 조정할 수 있습니다.
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

        // GameObject cookie = Instantiate(EndCookie,
        //     playerPosition + playerDirection*0.3f,
        //     Quaternion.Euler(0, yRotation, 0)
        //     );
        
        GameObject cookie = Instantiate(EndCookie,
            playerPosition + playerDirection*0.3f,
            Quaternion.Euler(0,0,90)
        );
    }
    
}
