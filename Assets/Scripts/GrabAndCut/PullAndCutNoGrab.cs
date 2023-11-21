using System;
using System.Collections;
using System.Collections.Generic;
using Deform;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class PullAndCutNoGrab : MonoBehaviour // Pose -> Transform으로 바꾸기 손에 닿은 순간 Transform 할당
{
    [SerializeField] private float maxPullDistance;
    [SerializeField] private bool isSetPosition = false;
    [SerializeField] private bool activeCut = false;
    
    private GameObject MeshCutterPrefab;
    private GameObject MeshCutter;
    private XRGrabInteractable grabInteractable;
    private Pose primaryAttachPose, secondaryAttachPose;
    private GameObject deformable;
    private SquashAndStretchDeformer deformer;
    
    private Pose originPose;
    private Vector3 middlePoint;
    private Vector3 movementMiddle;
    [SerializeField] private float CurDistance;
    private float maxDefromation = 1.5f;

    // 추가한 코드
    public bool isPrimaryHandAttached, isSecondaryHandAttached = false;
    public Transform primaryAttachHandTransform ,secondaryAttachHandTransform;
    public Transform handTransform; // 위치 추적용
    public Vector3 leftHandPosition;
    // ========================================================================
    // Start is called before the first frame update
    void Start()
    {
        //MeshCutter = GameObject.FindWithTag("Cutter");
        MeshCutterPrefab = Resources.Load("Prefabs/Mesh Cutter") as GameObject;
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable == null)
        {
            grabInteractable = transform.root.GetComponent<XRGrabInteractable>();
        }
        // Deformable 특성이 있는 경우
        if (TryGetComponent(out Deformable deform))
        {
            deformable = transform.GetChild(0).gameObject;
            deformer = deformable.GetComponent<SquashAndStretchDeformer>();
        }
        
        maxPullDistance = 1.1f;
    }

    void Initiate()
    {
        // object를 잡은 두 Hand
        primaryAttachPose = grabInteractable.interactorsSelecting[0].GetAttachTransform(grabInteractable).GetWorldPose();
        secondaryAttachPose = grabInteractable.interactorsSelecting[1].GetAttachTransform(grabInteractable).GetWorldPose();
        
        if (MeshCutter == null)
            MeshCutter = Instantiate(MeshCutterPrefab, Vector3.zero, Quaternion.identity);
        MeshCutter.GetComponent<MeshCutter>().enabled = true;

        if (!isSetPosition)
        {
            // 두 손으로 Grab한 순간의 Sliceable object의 위치를 저장함
            originPose = this.gameObject.transform.GetWorldPose();
            isSetPosition = true;
            
            // 두 손으로 Grab한 위치의 중간 지점
            middlePoint = (primaryAttachPose.position + secondaryAttachPose.position) / 2;
            movementMiddle = middlePoint;

            // 오브젝트가 중간에 위치함
            grabInteractable.trackPosition = false;
        }
    }
    void InitiateNoGrab()
    {
        // object를 잡은 두 Hand
        // primaryAttachPose = grabInteractable.interactorsSelecting[0].GetAttachTransform(grabInteractable).GetWorldPose();
        // secondaryAttachPose = grabInteractable.interactorsSelecting[1].GetAttachTransform(grabInteractable).GetWorldPose();

        if (MeshCutter == null)
            MeshCutter = Instantiate(MeshCutterPrefab, Vector3.zero, Quaternion.identity);
        MeshCutter.GetComponent<MeshCutter>().enabled = true;

        if (!isSetPosition)
        {
            // 두 손으로 Grab한 순간의 Sliceable object의 위치를 저장함
            originPose = this.gameObject.transform.GetWorldPose();
            isSetPosition = true;
            
            // 두 손으로 Grab한 위치의 중간 지점
            middlePoint = (primaryAttachHandTransform.position + secondaryAttachHandTransform.position) / 2;
            movementMiddle = middlePoint;

            // 오브젝트가 중간에 위치함
            grabInteractable.trackPosition = false;
        }
    }
    
    void SetMeshCutter(Pose First, Pose Second)
    {
        //Debug.DrawRay(First.position, Second.position - First.position, Color.red, 0.5f, false);

        Vector3 handsVector = (Second.position - First.position).normalized;
        Quaternion rotationQuaternion = Quaternion.Euler(0, 90, 0);

        Vector3 handsUpVector = rotationQuaternion * handsVector;

        //Debug.DrawRay(middlePoint, handsUpVector.normalized, Color.blue, 0.5f, false);

        MeshCutter.transform.position = middlePoint + Vector3.up * 0.5f;
        MeshCutter.transform.rotation = Quaternion.LookRotation(handsUpVector);
    }

    void SetObjectMiddle()
    {
        middlePoint = (primaryAttachPose.position + secondaryAttachPose.position) / 2;
        
        // [FIX] 오브젝트가 중간에 위치함
        //Debug.Log(middlePoint - movementMiddle);
        this.gameObject.transform.position += (middlePoint - movementMiddle);
    }
    void SetObjectMiddleNoGrab()
    {
        middlePoint = (primaryAttachHandTransform.position + secondaryAttachHandTransform.position) / 2;
        
        // [FIX] 오브젝트가 중간에 위치함
        //Debug.Log(middlePoint - movementMiddle);
        this.gameObject.transform.position += (middlePoint - movementMiddle);
    }
    
    void SetSlicePoint(Pose First, Pose Second)
    {
        // [FIX] 오브젝트가 중간에 위치함
        this.gameObject.transform.position += movementMiddle - middlePoint;
        
        // [HAVE TO] Update cut position
        float negativeRatio, positiveRatio;
    }

    void sliceObjcts()
    {
        //Debug.Log("cut!");
        Vector3 targetPosition = new Vector3(originPose.position.x, 0.0f, originPose.position.z);
        Debug.DrawLine(MeshCutter.transform.position, targetPosition, Color.yellow);
        MeshCutter.transform.position =
            Vector3.MoveTowards(MeshCutter.transform.position, targetPosition, Time.deltaTime * 10.0f);
        // Cut이 완료된다면
        // if (MeshCutter.transform.position.y <= (middlePoint.y + Vector3.down.y * 0.5f))
        // {
        //     Debug.Log("여기가 먼저??");
        //     isSetPosition = false;
        //     activeCut = false;
        //     MeshCutter.SetActive(false);
        //     grabInteractable.trackPosition = true;
        // }
    }

    public void FinishSlice()
    {
        isSetPosition = false;
        activeCut = false;
        MeshCutter.GetComponent<MeshCutter>().enabled = false;
        MeshCutter.transform.position = new Vector3(0.0f, -5.0f, 0.0f);

        Destroy(MeshCutter);
    }

    // Update is called once per frame
    void Update()
    {
        if (primaryAttachHandTransform != null)
        {
            leftHandPosition = primaryAttachHandTransform.position;    
        }

        if (isPrimaryHandAttached || isSecondaryHandAttached)
        {
            
        }

        if ((grabInteractable.interactorsSelecting.Count == 2) || (isPrimaryHandAttached && isSecondaryHandAttached)) // 각 손이 접촉해있으면 실행
        {
            // Initiate();
            InitiateNoGrab();
            SetObjectMiddle();
            CurDistance = Vector3.Distance(primaryAttachPose.position, secondaryAttachPose.position);
            activeCut = CurDistance >= maxPullDistance;
            
            // Mesh Cutter가 Player의 위쪽으로 Set
            if (!activeCut)
            {
                //this.GetComponent<MeshRenderer>().enabled = false;
                if (!(deformable == null))
                {
                    Vector3 handsVector = (secondaryAttachPose.position - primaryAttachPose.position).normalized;
                    deformable.transform.rotation = Quaternion.LookRotation(handsVector);
                    float weight = Mathf.Clamp(CurDistance, 0, maxPullDistance) / maxPullDistance;
                    deformer.Factor = CurDistance * weight;
                }
                
                SetMeshCutter(primaryAttachPose, secondaryAttachPose);
                //SetSlicePoint(primaryAttachPose, secondaryAttachPose);
            }
            else
            {
                this.GetComponent<MeshRenderer>().enabled = true;
                sliceObjcts();
                //Debug.Log("Slice " + this.gameObject.name);
            }
            
            movementMiddle = middlePoint;
        }
        else
        {
            isSetPosition = false;
            if (MeshCutter != null)
            {
                MeshCutter.GetComponent<MeshCutter>().enabled = false;
                Destroy(MeshCutter);
            }
        }
    }
    // 처음 붙은 손에 대해서만
    public void AttachHand(Transform handTransform, Vector3 attachPosition)
    {
        if (!isPrimaryHandAttached && !isSecondaryHandAttached)
        {
            Debug.Log($"primary Attach Hand: {handTransform.name} ");
            primaryAttachHandTransform = handTransform;
            primaryAttachPose = handTransform.GetComponent<XRDirectInteractor>().GetAttachTransform(grabInteractable)
                .GetWorldPose();
            isPrimaryHandAttached = true;
            SetAttachTransform(attachPosition);
        }
        else if ((isPrimaryHandAttached && !isSecondaryHandAttached) || (!isPrimaryHandAttached && isSecondaryHandAttached))
        {
            Debug.Log($"secondary Attach Hand: {handTransform.name} ");
            secondaryAttachHandTransform = handTransform;
            secondaryAttachPose = handTransform.GetComponent<XRDirectInteractor>().GetAttachTransform(grabInteractable)
                .GetWorldPose();
            isSecondaryHandAttached = true;
        }
        
        /*if (handTransform.name == "Left Controller")
        {
            isPrimaryHandAttached = true;
            primaryAttachHandTransform = handTransform;
        }
        else if (handTransform.name == "Right Controller")
        {
            isSecondaryHandAttached = true;
            secondaryAttachHandTransform = handTransform;
        }*/
        
    }
    public void DetachHand(Transform handTransform)
    {
        isPrimaryHandAttached = false;
        primaryAttachHandTransform = null;
        isSecondaryHandAttached = false;
        secondaryAttachHandTransform = null;
        /*if (handTransform.name == "Left Controller")
        {
            isPrimaryHandAttached = false;
            primaryAttachHandTransform = null;
        }
        if (handTransform.name == "Right Controller")
        {
            isSecondaryHandAttached = false;
            secondaryAttachHandTransform = null;
        }*/
    }
    public void DetachHand()
    {
        // 첫 번째 붙은 손의 Transform의 자식으로 할당
        // 한 손에 붙어있는 상태일 경우 자식으로 할당은 하지 않는다.
    }

    private void SetAttachTransform(Vector3 attachWorldPosition)
    {
        // Grab한 물체를 실제로 잡은(적절한 위치를 Grab하는) 듯한 모양으로 추적하기 위함.
        // 일단 물체의 origin Transform을 추적하고 해당 코드 추가.
    }
    private void FollowPrimaryHand()
    {
        // 손을 쥔(Grab을 누르고 있는) 모양으로 첫 번째로 잡은 손의 위치를 추적해야 한다. 손의 Attach Transform은 물체의 origin이 아닌 Trigger된 순간(손에 착 달라붙은 순간)의 위치를 의미한다.
        // Trigger된 순간의 Trigger된 world 좌표를 물체의 localposition으로 변경해서 저장해놓고 손의 이동, 회전을 따라오게 해야 함.
        // 추적하기 위해 필요한 세팅
        // Trigger된 순간의 Trigger된 world 좌표, 그때의 물체의 localposition
        // 추적하는 코드
        transform.position = primaryAttachHandTransform.position;
    }
}
