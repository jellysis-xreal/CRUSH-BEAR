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
        // 오브젝트에 적용된 모든 Renderer에서 Material을 가져오기
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
            Debug.LogError("Player 또는 Material이 지정되지 않았습니다.");
            return;
        }

        // 플레이어와의 거리 계산
        float distance = Vector3.Distance(transform.position, player.position);

        // Rim Power 계산 (거리에 따라 감소)
        float normalizedDistance = Mathf.Clamp01(distance / maxDistance);
        float rimPower = Mathf.Lerp(0f, maxRimPower, 1f - normalizedDistance);

        // 모든 Material에 Rim Power 값을 설정
        foreach (Material mat in materials)
        {
            mat.SetFloat("_RimPower", rimPower);
        }
    }
}