using Cinemachine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class EndingCutscene : TimeLineController
{
    [Header("Debug")]
    [SerializeField]
    private RectTransform bannerTransform;
    [SerializeField]
    private float offset;
    [SerializeField]
    private ActionBasedContinuousMoveProvider moveProvider;
    [SerializeField]
    private GameObject cookie;
    [SerializeField]
    private Camera cutsceneCamera;
    private PlayableDirector director;

    private int shakeNumber;
    private bool isCutsceneStarted;
    private const int CUTSCENE_PASS_THRESHOLD = 4;
    private void Awake()
    {
        InitSetting();
    }
    public void InitSetting()
    {
        Debug.Log("½ÇÇàµÊ");
        Camera cam = Camera.main;
        SetObjectPosition(bannerTransform, Vector3.zero, new Vector3(0, 180, 0));
        SetObjectPosition(cookie.transform, Vector3.zero, new Vector3(-90, 180, 0));
        Breakable breakable = cookie.GetComponent<Breakable>();
        breakable.onBreak.AddListener(StartCutScene);
        moveProvider.moveSpeed = 0f;
        director = GetComponent<PlayableDirector>();
        isCutsceneStarted = false;
        cutsceneCamera.enabled = false; 
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
        isCutsceneStarted = true;
        bannerTransform.gameObject.SetActive(false);
        if(Camera.main != null)
            Camera.main.enabled = false;
        cutsceneCamera.enabled = true;
        director.Play();
    }

    public void StartShakeCutscene()
    {
        StartCoroutine(UpdateShakeInput());
    }

    public void CheckShakeCount()
    {
        if(shakeNumber >= CUTSCENE_PASS_THRESHOLD)
            shakeNumber = 0;
        else
            StartCoroutine (CheckShake());
    }

    private IEnumerator UpdateShakeInput()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                shakeNumber++;
            }
            yield return null;
        }
    }

    private IEnumerator CheckShake()
    {
        director.Pause();
        while (shakeNumber < CUTSCENE_PASS_THRESHOLD)
        {
            yield return null;
        }
        director.Play();
        shakeNumber = 0;
    }
}
