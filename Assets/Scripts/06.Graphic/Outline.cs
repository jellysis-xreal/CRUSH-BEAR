using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

public class Outline : MonoBehaviour
{
    private List<Material> materials = new List<Material>();
    private Transform player;  
    public float maxDistance = 10f;  
    public float maxRimPower = 1f;

    private HittableMovement _hittable;

    void Start()
    {
        // ������Ʈ�� ����� ��� Renderer���� Material�� ��������
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            materials.AddRange(renderer.materials);
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;

        //_hittable = transform.GetComponent<HittableMovement>();
        
    }

    private Color GetUpdateSide()
    {
        Color sideColor = Color.white;
        if (_hittable.sideType == InteractionSide.Red)
            sideColor = Color.red;
        else
            sideColor = Color.blue;

        return sideColor;
    }

    void Update()
    {
        if (player == null || materials.Count == 0)
        {
            Debug.LogError("Player �Ǵ� Material�� �������� �ʾҽ��ϴ�.");
            return;
        }

        // 플레이어 거리 계산
        float distance = Vector3.Distance(transform.position, player.position);

        // Rim Power 계산
        float normalizedDistance = Mathf.Clamp01(distance / maxDistance);
        float rimPower = Mathf.Lerp(0f, maxRimPower, 1f - normalizedDistance);

        foreach (Material mat in materials)
        {
            mat.SetFloat("_RimPower", rimPower);
            //mat.color = GetUpdateSide();
        }
    }
}