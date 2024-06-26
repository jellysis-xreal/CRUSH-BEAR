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
    private GameObject playerObject, startCookie, particle, smoke, cutsceneCookie, shatteredCutsceneCookie, creditRoom, endingCredit; // startCookie
    [SerializeField]
    private Renderer fadeOutPanel;
    [SerializeField]
    private Transform cameraPoints, leftControllerTransform, rightControllerTransform;
    private PlayableDirector director;

    private bool isCutsceneStarted;
    private int currentPosition;
    private float totalShakeAmount;
    private float shakeAmount;
    private double startTime;

    private const float CAMERA_OFFSET = 1.1176f;
    private const float PUNCH_PASS_THRESHOLD = 0.5f;
    private const int CUTSCENE_PASS_THRESHOLD = 7;
    private void Awake()
    {
        InitSetting();
    }
    public void InitSetting()
    {
        //Debug.Log("½ÇÇàµÊ");
        SetObjectPosition(bannerTransform, new Vector3(0, 0.6f, 0), new Vector3(-30, 180, 0));
        SetObjectPosition(startCookie.transform, new Vector3(0, -0.1f, -0.4f), new Vector3(-60, 180, 0));
        CutsceneCookie breakable = startCookie.GetComponent<CutsceneCookie>();
        breakable.InitBreakable(this);
        director = GetComponent<PlayableDirector>();
        isCutsceneStarted = false;
        fadeOutPanel.material.color = new Color(0, 0, 0, 0);
        creditRoom.SetActive(false);
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

    private void ResetCameraPosition()
    {
        playerObject.transform.position = cameraPoints.GetChild(currentPosition).position - new Vector3(0, CAMERA_OFFSET, 0);
        playerObject.transform.rotation = cameraPoints.GetChild(currentPosition++).rotation;
    }

    private void StartEnding()
    {
        fadeOutPanel.gameObject.SetActive(false);
        creditRoom.SetActive(true);
        endingCredit.SetActive(true);
        RectTransform steproll = endingCredit.transform.GetChild(0).GetComponent<RectTransform>();
        steproll.DOLocalMoveY(2.5f, 40);
    }
}
