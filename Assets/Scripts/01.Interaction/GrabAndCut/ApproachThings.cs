using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class ApproachThings : MonoBehaviour
{
    [Header("Moving")]
    private Animator animator;
    private GameObject target;
    private float curDistance;
    private float maxDistance;
    [SerializeField] private float moveSpeed = 1.3f;
    [SerializeField] private float rotAnglePerSecond = 360.0f;

    [Header("Talking")]
    public TMP_Text uiText;
    [SerializeField] private float delay = 0.125f;
    private string text;

    [Header("Mesh")] 
    public GameObject TotalModel;
    private List<SkinnedMeshRenderer> skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
    private List<MeshRenderer> objectMeshRenderers = new List<MeshRenderer>();

    private string[] Lines =
    {
        "Are you really going to tear me apart?",
        "Don't do that.",
        "I trust you.",
        "Oh My God..",
        "I want to live."
    };


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player");

        InitMoving();
        InitTalking();
    }

    void InitMoving()
    {
        float scaleValue = Random.Range(1.0f, 3.0f);
        //this.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        moveSpeed = Random.Range(0.8f, 1.5f);
        maxDistance = Vector3.Distance(target.transform.position, this.transform.position);
    }

    void InitTalking()
    {
        text = uiText.text.ToString();
        uiText.text = " ";

        StartCoroutine(UITextPrint(delay));
    }

    IEnumerator UITextPrint(float delay)
    {
        int count = 0;

        while (count != text.Length)
        {
            if (count < text.Length)
            {
                uiText.text += text[count].ToString();
                count++;
            }

            yield return new WaitForSeconds(delay);
        }
    }

    void UpdateAnimation()
    {
        float ratio = (maxDistance - curDistance) / maxDistance;
        animator.SetFloat("Blend", ratio * 1.2f);

        //Debug.Log("[ratio]: " + ratio);
    }

    void MoveToTarget()
    {
        Quaternion lookRotation = Quaternion.LookRotation(target.transform.position - transform.position);

        if (lookRotation != quaternion.identity)
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * rotAnglePerSecond);

        if (curDistance <= 0.5f)
        {
            //Debug.Log("VAR");
        }
        else
        {
            transform.position =
                Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * moveSpeed);

        }
    }

    // 처음 Grab되면 실행
    public void FirstSelected()
    {
        // Animated Object는 Cut이 안되므로, 정적인 물체로 exchange가 필요합니다
        SkinnedMeshRenderer[] SkinnedMesh = GetComponentsInChildren<SkinnedMeshRenderer>();
        
        int index = 1; // index0은 Armature
        foreach (var skinned in SkinnedMesh)
        {
            //Transform child = transform.GetChild(index);
            //MeshFilter staticMesh = child.GetComponent<MeshFilter>();
            //MeshRenderer staticMeshRenderer = child.GetComponent<MeshRenderer>();
            skinnedMeshRenderers.Add(skinned);
            //objectMeshRenderers.Add(staticMeshRenderer);

            //Mesh skinnedMesh = skinned.sharedMesh;
            //staticMesh.mesh = skinnedMesh;

            //Material[] skinnedMaterials = skinned.materials;
            //staticMeshRenderer.materials = skinnedMaterials;

            //staticMeshRenderer.enabled = true;
            TotalModel.SetActive(true);
            skinned.enabled = false;
            index++;
        }
    }

    // Grab이 해제되면 실행
    public void ExitSelected()
    {
        // foreach (var mesh in objectMeshRenderers)
        //     mesh.enabled = false;
        TotalModel.SetActive(false);

        //foreach (var skinned in skinnedMeshRenderers)
            //skinned.enabled = true;
    }
    
    // Update is called once per frame
    void Update()
    {
        curDistance = Vector3.Distance(target.transform.position, this.transform.position);

        //if (curDistance < maxDistance)
            UpdateAnimation();

        MoveToTarget();

    }
}
