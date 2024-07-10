using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private int phaseIndex;
    
    public void Init()
    {
        Debug.Log("Initialize Tutorial Manager");
        phaseIndex = 0;

        StartCoroutine(TutorialRoutine());
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
    
    private IEnumerator Phase1()
    {
        Debug.Log("Phase 1 시작!");
        // Phase 1 동작을 구현합니다.
        // Dialogue: 토핑을 부수고 야수성을 키워서 멋진 곰이 되어보자!
        // TODO : 텍스트 시각화, 안내 음성 추가
        yield return new WaitForSeconds(10f); // 예시: 2초 대기
        Debug.Log("Phase 1 완료!");
    }

    private IEnumerator Phase2()
    {
        Debug.Log("Phase 2 시작!");
        // Phase 2 동작을 구현합니다.
        // Dialogue: 눈 앞의 쿠키를 부수면 시작할게
        // TODO : 눈 앞의 쿠키가 부숴지는지 감지하는 기능
        yield return StartCoroutine(GameManager.TutorialPunch.SpawnAndHandleCookie());
        Debug.Log("Phase 2 완료!");
    }

    private IEnumerator Phase3()
    {
        Debug.Log("Phase 3 시작!");
        // Phase 3 동작을 구현합니다.
        // Dialogue: 좋았어!
        // TODO : 텍스트 시각화
        yield return new WaitForSeconds(5f); 
        Debug.Log("Phase 3 완료!");
    }

    private IEnumerator Phase4()
    {
        Debug.Log("Phase 4 시작!");
        // Phase 4 동작을 구현합니다.
        // 예: 쿠키를 향해 펀치를 날리기
        
        while (true)
        {
            // 두 개의 쿠키를 날리기
            yield return StartCoroutine(GameManager.TutorialPunch.SpawnAndHandle2CookiesZap());

            // 두 개의 쿠키를 성공적으로 부셨는지 확인
            if (GameManager.TutorialPunch.CheckCookiesDestroyed())
            {
                Debug.Log("Phase 4 완료!");
                break; // 조건이 충족되면 반복을 종료하고 Phase4를 탈출
            }
            else
            {
                Debug.Log("Phase 4 조건 미충족 - 다시 시도");
                yield return StartCoroutine(Phase4_1());
            }
        }
    }

    private IEnumerator Phase4_1()
    {
        // TODO : 텍스트 시각화
        // Dialogue : 다시 한 번 해볼까? 
        yield return new WaitForSeconds(5f);
    }
    private IEnumerator Phase5()
    {
        Debug.Log("Phase 5 시작!");
        // Phase 5 동작을 구현합니다.
        // Dialogue : 다시 한 번 해볼까? 
        // TODO : 텍스트 시각화
        yield return new WaitForSeconds(5f); // 예시: 2초 대기
        Debug.Log("Phase 5 완료!");
    }

    private IEnumerator Phase6()
    {
        Debug.Log("Phase 6 시작!");
        // Phase 6 동작을 구현합니다.
        // Dialogue : 쿠키를 세게 칠 수록 좋은 점수를 받을 수 있어! 야수곰처럼 팔을 쫙 펴고 힘껏 펀치해보자!  
        
        yield return new WaitUntil((() => GameManager.TutorialPunch.GetPerfectScoreNumberOfCookie() == 3));
        
        Debug.Log("Phase 6 완료!");
    }

    private IEnumerator Phase7()
    {
        Debug.Log("Phase 7 시작!");
        // Phase 7 동작을 구현합니다.
        // Dialogue : 다시 한 번 해볼까? 
        // TODO : 텍스트 시각화
        yield return new WaitForSeconds(5f); // 예시: 2초 대기
        Debug.Log("Phase 7 완료!");
    }
    
    private IEnumerator Phase8()
    {
        Debug.Log("Phase 8 시작!");
        // Phase 8 동작을 구현합니다.
        // 예: 쿠키를 두 번 이상 perfect로 치기
        while (true)
        {
            yield return StartCoroutine(GameManager.TutorialPunch.ZapRoutine());

            if (GameManager.TutorialPunch.CheckCookiesDestroyedAndPerfect())
            {
                Debug.Log("Phase 8 완료!");
                break; // 조건이 충족되면 반복을 종료하고 Phase6를 탈출
            }
            else
            {
                Debug.Log("Phase 8 조건 미충족 - 다시 시도");
                yield return StartCoroutine(Phase4_1());
            }
        }
        Debug.Log("Phase 8 완료!");
    }
    private IEnumerator Phase9()
    {
        Debug.Log("Phase 9 시작!");
        // Phase 9 동작을 구현합니다.
        // Dialogue: 좋은데! 
        // TODO : 텍스트 시각화
        yield return new WaitForSeconds(5f); 
        Debug.Log("Phase 9 완료!");
    }
    private IEnumerator Phase10()
    {
        Debug.Log("Phase 10 시작!");
        // Phase 10 동작을 구현합니다.
        // Dialogue: 이번엔 몸을 틀어 냉장고 쪽을 바라봐줘
        yield return new WaitForSeconds(10f); 
        Debug.Log("Phase 10 완료!");
    }
    private IEnumerator Phase11()
    {
        Debug.Log("Phase 11 시작!");
        // Phase 11 동작을 구현합니다.
        // Dialogue: 좋은데! 
        // TODO : 텍스트 시각화
        yield return new WaitForSeconds(5f); // 예시: 2초 대기
        Debug.Log("Phase 11 완료!");
    }
    private IEnumerator Phase12()
    {
        Debug.Log("Phase 12 시작!");
        // Phase 12 동작을 구현합니다.
        // Dialouge : 날아오는 과일을 향해 색깔에 맞춰 잼나이프를 휘둘러보자!
        GameManager.TutorialTennis.InitializeTennis();
        while (true)
        {
            yield return StartCoroutine(GameManager.TutorialTennis.TennisTutorialRoutine());

            if (GameManager.TutorialTennis.CheckCookiesDestroyedAndPerfect())
            {
                Debug.Log("Phase 12 완료!");
                break; // 조건이 충족되면 반복을 종료하고 Phase6를 탈출
            }
            else
            {
                Debug.Log("Phase128 조건 미충족 - 다시 시도");
                yield return StartCoroutine(Phase4_1());
            }
        }
        Debug.Log("Phase 8 완료!");
    }
    private IEnumerator Phase13()
    {
        Debug.Log("Phase 13 시작!");
        // Phase 13 동작을 구현합니다.
        // 예: 잘했어!
        yield return new WaitForSeconds(2f); // 예시: 2초 대기
        Debug.Log("Phase 13 완료!");
    }
    private IEnumerator Phase14()
    {
        Debug.Log("Phase 14 시작!");
        // Phase 14 동작을 구현합니다.
        // 예:  perfect 이상의 가속도로 스윙 2회 이상 → 15로 이동
        // perfect 이상의 가속도로 스윙 2회 미만 → 14-1로 이동
        yield return new WaitForSeconds(2f); // 예시: 2초 대기
        Debug.Log("Phase 14 완료!");
    }
    private IEnumerator Phase15()
    {
        Debug.Log("Phase 15 시작!");
        // Phase 13 동작을 구현합니다.
        // 예: 좋아!!
        yield return new WaitForSeconds(2f); // 예시: 2초 대기
        Debug.Log("Phase 15 완료!");
    }
    private IEnumerator Phase16()
    {
        Debug.Log("Phase 16 시작!");
        // Phase 13 동작을 구현합니다.
        // 예: 이제 야수성을 키울 준비가 다 됐어! 열심히 훈련해서 멋진 곰이 되는 거야!
        // 로비로 이동
        yield return new WaitForSeconds(2f); // 예시: 2초 대기
        Debug.Log("Phase 16 완료!");
    }
}
