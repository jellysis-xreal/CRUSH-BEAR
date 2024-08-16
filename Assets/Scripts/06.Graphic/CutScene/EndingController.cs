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
    private GameObject playerObject;
    public void InitSetting()
    {
        playerObject = GameObject.FindWithTag("Player");
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
            .AppendCallback(ResetCameraPosition).
            AppendCallback(StartEnding);
    }

    private void ResetCameraPosition()
    {
        playerObject.transform.position = cameraPoints.GetChild(currentPosition).position;
        playerObject.transform.rotation = cameraPoints.GetChild(currentPosition++).rotation;
    }
    public void CutsceneStart()
    {
        playerObject = GameObject.FindWithTag("Player");
        fadeOutPanel.gameObject.SetActive(true);
        fadeOutPanel.color = new Color(0, 0, 0, 0);
        DOTween.Sequence().Append(fadeOutPanel.DOFade(1f, 1f))
            .AppendCallback(ResetCameraPosition).
             AppendCallback(director.Play).
             Join(fadeOutPanel.DOFade(0f, 1f));


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
