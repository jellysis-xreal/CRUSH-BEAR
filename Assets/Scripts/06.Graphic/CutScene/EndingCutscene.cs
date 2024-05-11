using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class EndingCutscene : TimeLineController
{
    [Header("Debug")]
    [SerializeField]
    private RectTransform bannerTransform; //
    [SerializeField]
    private float offset;
    [SerializeField]
    private GameObject playerObject, startCookie, particle, smoke, cutsceneCookie, shatteredCutsceneCookie, shakeInfo, endingCredit; // startCookie
    [SerializeField]
    private Renderer fadeOutPanel;
    [SerializeField]
    private Transform cameraPoints, leftControllerTransform, rightControllerTransform;
    private PlayableDirector director;

    private bool isCutsceneStarted;
    private int currentPosition;
    private float shakeAmount;


    private const float CAMERA_OFFSET = 1.1176f;
    private const int CUTSCENE_PASS_THRESHOLD = 5;
    private void Awake()
    {
        InitSetting();
    }
    public void InitSetting()
    {
        //Debug.Log("½ÇÇàµÊ");
        SetObjectPosition(bannerTransform, new Vector3(0, 0.4f, 0), new Vector3(-30, 180, 0));
        SetObjectPosition(startCookie.transform, new Vector3(0, -0.1f, -0.4f), new Vector3(-60, 180, 0));
        Breakable breakable = startCookie.GetComponent<Breakable>();
        breakable.onBreak.AddListener(StartCutScene);
        director = GetComponent<PlayableDirector>();
        isCutsceneStarted = false;
        fadeOutPanel.material.color = new Color(0, 0, 0, 0);
        shakeInfo.SetActive(false);
    }

    private void SetObjectPosition(Transform transform, Vector3 positionOffset, Vector3 rotationOffset)
    {
        Camera cam = Camera.main;
        transform.position = cam.transform.position + cam.transform.forward * offset;
        transform.position += positionOffset;
        transform.LookAt(cam.transform);
        Vector3 rotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(rotation.x + rotationOffset.x,
                                              rotation.y + rotationOffset.y,
                                              rotation.z + rotationOffset.z);
    }

    // Temporary parameters to subscribe event OnBreak
    public void StartCutScene(GameObject temp1, GameObject temp2)
    {
        if (isCutsceneStarted)
            return;
        startCookie.SetActive(false);
        isCutsceneStarted = true;
        playerObject.GetComponent<ActionBasedContinuousMoveProvider>().moveSpeed = 0;
        bannerTransform.gameObject.SetActive(false);
        particle.SetActive(false);
        director.Play();
    }

    public void StartShakeCutscene()
    {
        StartCoroutine(UpdateShakeInput());
    }

    public void CheckShakeCount()
    {
        if (shakeAmount >= CUTSCENE_PASS_THRESHOLD)
            shakeAmount = 0f;
        else
            StartCoroutine(UpdateShake());
    }
    public void InstantiateSmokeParticle()
    {
        Instantiate(smoke, cutsceneCookie.transform.position, Quaternion.identity);

    }
    public void StartEndingCredit()
    {
        DOTween.Sequence().Append(fadeOutPanel.material.DOFade(1f, 1f))
            .AppendCallback(ResetCameraPosition).
            OnComplete(StartEnding);
    }

    public void SetCamera()
    {
        director.Pause();
        DOTween.Sequence().
            Append(fadeOutPanel.material.DOFade(1f, 0.4f)).
            AppendCallback(() => ResetCameraPosition()).
            Append(fadeOutPanel.material.DOFade(0f, 0.4f)).
            OnComplete(() => director.Play());
    }

    public void ShakeInfo()
    {
        shakeInfo.SetActive(true);
        Image image = shakeInfo.transform.GetChild(0).GetComponent<Image>();
        DOTween.Sequence().
            Append(shakeInfo.transform.DOShakePosition(2, 0.1f)).
            Append(image.DOFade(0, 1f)).
            Join(image.transform.GetChild(0).GetComponent<TextMeshProUGUI>().DOFade(0, 1f));
    }
    private void ResetCameraPosition()
    {
        playerObject.transform.position = cameraPoints.GetChild(currentPosition).position - new Vector3(0, CAMERA_OFFSET, 0);
        playerObject.transform.rotation = cameraPoints.GetChild(currentPosition++).rotation;
    }
    private IEnumerator UpdateShakeInput()
    {
        float previousLeftY = leftControllerTransform.position.y;
        float previousRightY = rightControllerTransform.position.y;

        while (true)
        {
            CheckShake(ref previousLeftY, ref previousRightY);
            yield return null;
        }
    }

    private IEnumerator UpdateShake()
    {
        director.Pause();
        float previousLeftY = leftControllerTransform.position.y;
        float previousRightY = rightControllerTransform.position.y;

        while (shakeAmount < CUTSCENE_PASS_THRESHOLD)
        {
            CheckShake(ref previousLeftY, ref previousRightY);
            yield return null;
        }
        shakeAmount = 0;
        director.Play();
        Instantiate(shatteredCutsceneCookie, cutsceneCookie.transform);
    }

    private void CheckShake(ref float previousLeftY, ref float previousRightY)
    {
        shakeAmount += Mathf.Abs(leftControllerTransform.position.y - previousLeftY);
        shakeAmount += Mathf.Abs(rightControllerTransform.position.y - previousRightY);
        previousLeftY = leftControllerTransform.position.y;
        previousRightY = rightControllerTransform.position.y;
    }

    private void StartEnding()
    {
        fadeOutPanel.gameObject.SetActive(false);
        endingCredit.SetActive(true);
        RectTransform steproll = endingCredit.transform.GetChild(0).GetComponent<RectTransform>();
        steproll.DOLocalMoveY(2.5f, 40);
    }
}
