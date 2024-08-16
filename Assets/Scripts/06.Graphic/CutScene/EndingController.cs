using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.XR.Content.Interaction;

public class EndingController : TimeLineController
{
    [Header("Debug")]
    [SerializeField]
    private float offset;
    [SerializeField]
    private GameObject startCookie, particle, creditRoom, endingCredit; // startCookie
    [SerializeField]
    private Image fadeOutPanel;
    [SerializeField]
    private PlayableDirector director;
    [SerializeField]
    private Transform cameraPoints;
    private int currentPosition;
    [SerializeField]
    private GameObject playerObject, endingCookie;
    public void InitSetting()
    {
        currentPosition = 0;
        particle.SetActive(true);
        SetObjectPosition(startCookie.transform, new Vector3(0, -0.1f, -0.4f), new Vector3(-60, 180, 0));
        CutsceneCookie breakable = startCookie.GetComponent<CutsceneCookie>();
        breakable.InitBreakable(this);
        fadeOutPanel.gameObject.SetActive(true);
        fadeOutPanel.color = new Color(0, 0, 0, 0);
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

    public void StartEndingCredit()
    {
        DOTween.Sequence().Append(fadeOutPanel.DOFade(1f, 1f))
            .AppendCallback(EndingCameraPosition).
            AppendCallback(StartEnding);
    }

    private void CutsceneCameraPosition()
    {
        playerObject.transform.position = new Vector3(0,-1,-1);
        playerObject.transform.rotation = Quaternion.identity;
    }
    private void EndingCameraPosition()
    {
        playerObject.transform.position = new Vector3(0.85f, -0.5f, -11.5f);
            playerObject.transform.rotation = Quaternion.identity;
    }
    public void CutsceneStart()
    {
        endingCookie.SetActive(false);
        GetComponent<Canvas>().enabled = false;
        DOTween.Sequence().Append(fadeOutPanel.DOFade(1f, 1f))
            .AppendCallback(CutsceneCameraPosition).
             AppendCallback(director.Play).
             Append(fadeOutPanel.DOFade(0f, 1f));

    }
    private void StartEnding()
    {
        //playerObject.GetComponent<ActionBasedContinuousMoveProvider>().enabled = false;
        fadeOutPanel.gameObject.SetActive(false);
        creditRoom.SetActive(true);
        endingCredit.SetActive(true);
        RectTransform steproll = endingCredit.transform.GetChild(0).GetComponent<RectTransform>();
        steproll.DOLocalMoveY(2.5f, 40);
    }
}
