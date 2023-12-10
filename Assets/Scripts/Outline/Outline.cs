using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    private List<Material> materials = new List<Material>();
    private Transform player;  
    public float maxDistance = 10f;  
    public float maxRimPower = 1f;  

    void Start()
    {
        // ������Ʈ�� ����� ��� Renderer���� Material�� ��������
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            materials.AddRange(renderer.materials);
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;  
    }

    void Update()
    {
        if (player == null || materials.Count == 0)
        {
            Debug.LogError("Player �Ǵ� Material�� �������� �ʾҽ��ϴ�.");
            return;
        }

        // �÷��̾���� �Ÿ� ���
        float distance = Vector3.Distance(transform.position, player.position);

        // Rim Power ��� (�Ÿ��� ���� ����)
        float normalizedDistance = Mathf.Clamp01(distance / maxDistance);
        float rimPower = Mathf.Lerp(0f, maxRimPower, 1f - normalizedDistance);

        // ��� Material�� Rim Power ���� ����
        foreach (Material mat in materials)
        {
            mat.SetFloat("_RimPower", rimPower);
        }
    }
}