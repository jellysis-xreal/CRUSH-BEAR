using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EnumTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private int phaseIndex;
    
    [SerializeField] private GameObject _tutorialItems;
    
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI dialogueText2;

    [SerializeField] private Animator aniPunch1;
    [SerializeField] private Animator aniPunch2;
    [SerializeField] private Animator aniSwing;
    [SerializeField] private Animator aniFighting;
    [SerializeField] private GameObject nodePunchingRail;
    [SerializeField] private GameObject nodeHittingRail;

    [SerializeField] private AudioSource _audioSource;
    public List<AudioClip> tutorialAudioSources = new List<AudioClip>();
    
    
    public void Init()
    {
        //Debug.Log("Initialize Tutorial Manager");
        phaseIndex = 0;

        // Sorry for hard coding...
        _tutorialItems = GameObject.FindWithTag("TutorialItems");
        _audioSource = GameObject.FindWithTag("TutorialAudio").GetComponent<AudioSource>();
            
        nodePunchingRail = _tutorialItems.transform.GetChild(0).GetChild(1).GetChild(0).gameObject;
        dialogueText = nodePunchingRail.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        Transform animations = nodePunchingRail.transform.GetChild(0).GetChild(2);
        aniPunch1 = animations.GetChild(0).GetComponent<Animator>(); //FindAnimator("Tutorial-Ani01-punch1");
        aniPunch2 = animations.GetChild(1).GetComponent<Animator>(); //FindAnimator("Tutorial-Ani02-punch2");
                
        nodeHittingRail = _tutorialItems.transform.GetChild(0).GetChild(2).gameObject;
        dialogueText2 = nodeHittingRail.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        animations = nodeHittingRail.transform.GetChild(0).GetChild(2);
        
        aniSwing = animations.GetChild(2).GetComponent<Animator>(); //FindAnimator("Tutorial-Ani03-swing");
        aniFighting = animations.GetChild(3).GetComponent<Animator>();//FindAnimator("Tutorial-Ani04-fighting");

        StartCoroutine(TutorialRoutine());
    }

    private Animator FindAnimator(string name)
    {
        Animator[] animators = Resources.FindObjectsOfTypeAll<Animator>();
        foreach (Animator anim in animators)
        {
            if (anim.name == name)
            {
                return anim;
            }
        }
        return null;
    }

    private GameObject FindInactiveObject(string name)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform obj in objs)
        {
            if (obj.name == name)
            {
                return obj.gameObject;
            }
        }
        return null;
    }

    IEnumerator TutorialRoutine()
    {
        // 각 단계를 순차적으로 실행합니다.
        yield return StartCoroutine(Phase1());
        yield return StartCoroutine(Phase2());
        yield return StartCoroutine(Phase3());
        yield return StartCoroutine(Phase4());
        yield return StartCoroutine(Phase5());
        yield return StartCoroutine(Phase6());
        yield return StartCoroutine(Phase7());
        yield return StartCoroutine(Phase8());
        yield return StartCoroutine(Phase9());
        yield return StartCoroutine(Phase10());
        yield return StartCoroutine(Phase11());
        yield return StartCoroutine(Phase12());
        yield return StartCoroutine(Phase13());
        yield return StartCoroutine(Phase14());
        yield return StartCoroutine(Phase15());
        yield return StartCoroutine(Phase16());
    }
    
    private void PlayAudioSource(int index)
    {
        _audioSource.clip = tutorialAudioSources[index];
        _audioSource.Play();
    }
    
    private IEnumerator Phase1()
    {
        //Debug.Log("Phase 1 시작!");
        // Phase 1 동작을 구현합니다.
        // Dialogue: 토핑을 부수고 야수성을 키워서 멋진 곰이 되어보자!
        ShowDialogue("Let’s become a savage bear \nby crushing the jelly topping!", 10f);
        PlayAudioSource(0);
        yield return new WaitForSeconds(10f); // 예시: 2초 대기
        //Debug.Log("Phase 1 완료!");
    }

    private IEnumerator Phase2()
    {
        //Debug.Log("Phase 2 시작!");
        // Phase 2 동작을 구현합니다.
        // Dialogue: 눈 앞의 쿠키를 부수면 시작할게
        ShowDialogue("Let's get started by smashing the cookie in front of us!", 5f);
        // TODO : 눈 앞의 쿠키가 부숴지는지 감지하는 기능
        PlayAudioSource(1);
        GameManager.TutorialPunch.Init();
        yield return StartCoroutine(GameManager.TutorialPunch.SpawnAndHandleCookie());
        //Debug.Log("Phase 2 완료!");
    }

    private IEnumerator Phase3()
    {
        //Debug.Log("Phase 3 시작!");
        // Phase 3 동작을 구현합니다.
        // Dialogue: 좋았어!
        ShowDialogue("Great job!", 5f);
        PlayAudioSource(2);
        // TODO : 텍스트 시각화
        yield return new WaitForSeconds(5f); 
        //Debug.Log("Phase 3 완료!");
    }

    private IEnumerator Phase4()
    {
        //Debug.Log("Phase 4 시작!");
        // Phase 4 동작을 구현합니다.
        // 예: 쿠키를 향해 펀치를 날리기
        PlayAudioSource(3);
        while (true)
        {
            // Dialogue: 눈 앞의 쿠키를 부수면 시작할게
            ShowDialogue("Let’s punch those cookies \nflying through the air", 10f);
            PlayAnimation(aniPunch1, 10f); // 애니메이션

            // 두 개의 쿠키를 날리기
            yield return StartCoroutine(GameManager.TutorialPunch.SpawnAndHandle2CookiesZap());

            // 두 개의 쿠키를 성공적으로 부셨는지 확인
            if (GameManager.TutorialPunch.CheckCookiesDestroyed())
            {
                //Debug.Log("Phase 4 완료!");
                break; // 조건이 충족되면 반복을 종료하고 Phase4를 탈출
            }
            else
            {
                //Debug.Log("Phase 4 조건 미충족 - 다시 시도");
                yield return StartCoroutine(Phase4_1());
            }
        }
    }

    private IEnumerator Phase4_1()
    {
        // TODO : 텍스트 시각화
        // Dialogue : 다시 한 번 해볼까? 
        PlayAudioSource(4);
        ShowDialogue("Let's try one more time", 5f);
        yield return new WaitForSeconds(5f);
    }
    private IEnumerator Phase5()
    {
        //Debug.Log("Phase 5 시작!");
        // Phase 5 동작을 구현합니다.
        // Dialogue : 잘했어!
        ShowDialogue("Well done!", 5f);
        PlayAudioSource(5);
        // TODO : 텍스트 시각화
        yield return new WaitForSeconds(5f); // 예시: 2초 대기
        //Debug.Log("Phase 5 완료!");
    }

    private IEnumerator Phase6()
    {
        //Debug.Log("Phase 6 시작!");
        // Phase 6 동작을 구현합니다.
        PlayAudioSource(6);
        while (true)
        {
            // Dialogue : 쿠키를 세게 칠 수록 좋은 점수를 받을 수 있어! 야수곰처럼 팔을 쫙 펴고 힘껏 펀치해보자!
            ShowDialogue("The harder you hit the cookie, \nthe higher your score will be! \nSwing your arms like a savage bear \nand punch with all your might!", 10f);
            PlayAnimation(aniPunch1, 10f); // 애니메이션

            // 두 개의 쿠키를 날리기
            yield return StartCoroutine(GameManager.TutorialPunch.ZapRoutine());

            // 두 개의 쿠키를 성공적으로 부셨는지 확인
            if (GameManager.TutorialPunch.GetPerfectScoreNumberOfCookie() >= 3) // 임시로 원래 3개
            {
                //Debug.Log("Phase 6 완료!");
                break; // 조건이 충족되면 반복을 종료하고 Phase4를 탈출
            }
            else
            {
                //Debug.Log("Phase 6 조건 미충족 - 다시 시도");
                yield return StartCoroutine(Phase4_1());
            }
        }
    }

    private IEnumerator Phase7()
    {
        PlayAudioSource(7);
        //Debug.Log("Phase 7 시작!");
        // Phase 7 동작을 구현합니다.
        ShowDialogue("Good!", 5f);

        // TODO : 텍스트 시각화
        yield return new WaitForSeconds(5f); // 예시: 2초 대기
        //Debug.Log("Phase 7 완료!");
    }
    
    private IEnumerator Phase8()
    {
        PlayAudioSource(8);
        //Debug.Log("Phase 8 시작!");
        // Phase 8 동작을 구현합니다.
        // 예: 쿠키를 두 번 이상 perfect로 치기
        while (true)
        {
            ShowDialogue("This time, hit the cookie \nin the direction of the arrow sign! \nFor cookies without the sign, \npunch straight forward!", 5f);
            PlayAnimation(aniPunch2, 10f); // 애니메이션

            yield return StartCoroutine(GameManager.TutorialPunch.Phase8Routine());

            if (GameManager.TutorialPunch.Check4CookiesInteractionSucceed()){
                //Debug.Log("Phase 8 완료!");
                break; // 조건이 충족되면 반복을 종료하고 Phase6를 탈출
            }else{
                //Debug.Log("Phase 8 조건 미충족 - 다시 시도");
                yield return StartCoroutine(Phase4_1());
            }
        }
        //Debug.Log("Phase 8 완료!");
    }
    private IEnumerator Phase9()
    {
        PlayAudioSource(9);
        //Debug.Log("Phase 9 시작!");
        // Phase 9 동작을 구현합니다.
        // Dialogue: 좋은데! 
        ShowDialogue("Nice!", 5f);

        // TODO : 텍스트 시각화
        yield return new WaitForSeconds(5f); 
        //Debug.Log("Phase 9 완료!");
    }
    private IEnumerator Phase10()
    {
        PlayAudioSource(10);
        //Debug.Log("Phase 10 시작!");
        // Phase 10 동작을 구현합니다.
        // Dialogue: 이번엔 몸을 틀어 냉장고 쪽을 바라봐줘
        ShowDialogue2("Now, turn right and face the refrigerator", 10f);

        // Node_Punching_Rail을 비활성화하고 Node_Hitting_Rail을 활성화
        if (nodePunchingRail != null)
        {
            // nodePunchingRail의 rotation을 (0,81,0)으로 변경
            nodePunchingRail.transform.parent.DORotate(new Vector3(0, 81f, 0), 2.0f)
                .OnComplete(() => nodePunchingRail.SetActive(false)); // 애니메이션이 완료되면 nodePunchingRail을 비활성화
        }
        if (nodeHittingRail != null)
        {
            nodeHittingRail.SetActive(true);
        }

        yield return new WaitForSeconds(10f); 
        //Debug.Log("Phase 10 완료!");
    }
    private IEnumerator Phase11()
    {
        PlayAudioSource(11);
        //Debug.Log("Phase 11 시작!");
        // Phase 11 동작을 구현합니다.
        // Dialogue: 좋은데! 
        ShowDialogue2("Great!", 2f);

        // TODO : 텍스트 시각화
        yield return new WaitForSeconds(5f); // 예시: 2초 대기
        //Debug.Log("Phase 11 완료!");
    }
    private IEnumerator Phase12()
    {
        PlayAudioSource(12);
        //Debug.Log("Phase 12 시작!");
        // Phase 12 동작을 구현합니다.
        // Dialouge : 날아오는 과일을 향해 색깔에 맞춰 잼나이프를 휘둘러보자!

        GameManager.TutorialTennis.InitializeTennis();
        while (true)
        {
            ShowDialogue2("Swing the correct colored jam knife to match the flying fruits", 10f);
            PlayAnimation(aniSwing, 10f); // 애니메이션

            yield return StartCoroutine(GameManager.TutorialTennis.TennisPhase12Routine());
            
            if (GameManager.TutorialTennis.CheckPhase12Criteria())
            {
                //Debug.Log("Phase 12 완료!");
                break; // 조건이 충족되면 반복을 종료하고 Phase12를 탈출
            }
            else
            {
                //Debug.Log("Phase12 조건 미충족 - 다시 시도");
                yield return StartCoroutine(Phase4_1());
            }
        }
        //Debug.Log("Phase 12 완료!");
    }
    
    private IEnumerator Phase13()
    {
        PlayAudioSource(13);
        //Debug.Log("Phase 13 시작!");
        // Phase 13 동작을 구현합니다.
        // 예: 잘했어!
        ShowDialogue2("Well done!", 5f);

        yield return new WaitForSeconds(5f); // 예시: 2초 대기
        //Debug.Log("Phase 13 완료!");
    }

    private IEnumerator Phase14()
    {
        PlayAudioSource(14);
        //Debug.Log("Phase 14 시작!");
        // Phase 14 동작을 구현합니다.
        // 예: perfect 이상의 가속도로 스윙 2회 이상 → 15로 이동
        // perfect 이상의 가속도로 스윙 2회 미만 → 14-1로 이동
        ShowDialogue2("The harder you hit the fruit, \nthe higher your score will be! \nSwing your arms like a savage bear \nand punch with all your might!", 8f);
        PlayAnimation(aniSwing, 8f); // 애니메이션
        yield return new WaitForSeconds(8f); // 예시: 2초 대기

        while (true)
        {
            yield return StartCoroutine(GameManager.TutorialTennis.Phase14Routine());

            if (GameManager.TutorialTennis.Check4FruitInteractionPerfect())
            {
                //Debug.Log("Phase 14 완료!");
                break;
            }
            else
            {
                //Debug.Log("Phase 14 조건 미충족 - 다시 시도");
                yield return StartCoroutine(Phase4_1());
            }
        }
    }
    private IEnumerator Phase15()
    {
        PlayAudioSource(15);
        //Debug.Log("Phase 15 시작!");
        // Phase 13 동작을 구현합니다.
        ShowDialogue("Good!", 5f);

        // 예: 좋아!!
        yield return new WaitForSeconds(2f); // 예시: 2초 대기
        //Debug.Log("Phase 15 완료!");
    }
    private IEnumerator Phase16()
    {
        PlayAudioSource(16);
        //Debug.Log("Phase 16 시작!");
        // Phase 13 동작을 구현합니다.
        // 예: 이제 야수성을 키울 준비가 다 됐어! 열심히 훈련해서 멋진 곰이 되는 거야!
        ShowDialogue2("Now you’re ready to be a savage bear! \nLet’s train to become the savagest bear \nin “Jellysis”", 10f);

        PlayAnimation(aniFighting, 10f); // 애니메이션

        yield return new WaitForSeconds(2f); // 예시: 2초 대기
        //Debug.Log("Phase 16 완료!");

        GameManager.Instance.Save.ClearTutorial();
        GameManager.Sound.PlayMusic_Lobby(false);
        // 로비 씬으로 이동
        GameManager.Instance.WaveToLobby();
    }

    private void ShowDialogue(string message, float duration)
    {
        //Debug.Log("ShowDialogue 호출");

        dialogueText.text = message;

        if (dialogueText != null)
        {
            // 텍스트를 즉시 활성화하고 메시지를 설정
            dialogueText.enabled = true;
            dialogueText.text = message;

            // 10초 후 텍스트를 비활성화
            StartCoroutine(HideDialogueAfterDelay(duration));
        }
        else
        {
            Debug.LogWarning("Dialogue Text component is not assigned.");
        }
    }

    private void ShowDialogue2(string message, float duration)
    {
        //Debug.Log("ShowDialogue2호출");

        dialogueText2.text = message;

        if (dialogueText2 != null)
        {
            // 텍스트를 즉시 활성화하고 메시지를 설정
            dialogueText2.enabled = true;
            dialogueText2.text = message;

            // 10초 후 텍스트를 비활성화
            StartCoroutine(HideDialogue2AfterDelay(duration));
        }
        else
        {
            Debug.LogWarning("Dialogue Text component is not assigned.");
        }
    }

    private IEnumerator HideDialogueAfterDelay(float delay)
    {
        //Debug.Log("HideDialogueAfterDelay 호출");

        yield return new WaitForSeconds(delay);
        if(dialogueText != null)
            dialogueText.enabled = false; // 텍스트 비활성화
    }
    private IEnumerator HideDialogue2AfterDelay(float delay)
    {
        //Debug.Log("HideDialogueAfterDelay 호출");

        yield return new WaitForSeconds(delay);
        if (dialogueText2 != null)
            dialogueText2.enabled = false; // 텍스트 비활성화
    }

    // 애니메이션 재생 메서드
    private void PlayAnimation(Animator animator, float duration)
    {
        //Debug.Log($"Play Animation");

        if (animator != null)
        {
            animator.gameObject.SetActive(true); // 애니메이션이 시작될 때 모델을 활성화
            animator.Play(0); // 기본 레이어의 첫 번째 애니메이션 재생
            StartCoroutine(StopAnimationAfterDelay(animator, duration));
        }
        else
        {
            //Debug.LogError("Animator component is not assigned.");
        }
    }

    private IEnumerator StopAnimationAfterDelay(Animator animator, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (animator != null)
            animator.gameObject.SetActive(false); // 일정 시간 후 모델을 비활성화
    }
}
