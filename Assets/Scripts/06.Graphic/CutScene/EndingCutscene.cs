using Unity.VisualScripting;
using UnityEngine;
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


    private void Awake()
    {
        InitSetting();
    }
    public void InitSetting()
    {
        Camera cam = Camera.main;
        SetObjectPosition(bannerTransform, Vector3.zero, new Vector3(0, 180, 0));
        SetObjectPosition(cookie.transform, Vector3.zero, new Vector3(-90, 180, 0));
        Breakable breakable = cookie.GetComponent<Breakable>();
        breakable.onBreak.AddListener(StartCutScene);
        moveProvider.moveSpeed = 0f;
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
    private void StartCutScene(GameObject temp1, GameObject temp2)
    {
        bannerTransform.gameObject.SetActive(false);

    }
}
