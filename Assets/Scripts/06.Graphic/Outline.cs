using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Outline : MonoBehaviour
{
    private List<Material> materials = new List<Material>();
    private Transform player;
    public float maxDistance = 30f;
    public float maxRimPower = 2f;
    public float rimIntensityMultiplier = 15f; // Rim Color의 Intensity를 조절할 변수

    private HittableMovement _hittable;

    void Start()
    {
        _hittable = GetComponent<HittableMovement>();

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            materials.AddRange(renderer.materials);
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private Color GetUpdateSide()
    {
        if (_hittable.sideType == InteractionSide.Red)
            return Color.red;
        else
            return Color.blue;
    }

    void Update()
    {
        if (_hittable == null) return;

        if (player == null || materials.Count == 0)
        {
            Debug.LogError("Player나 Material이 설정되지 않았습니다.");
            return;
        }

        // 플레이어와의 거리 계산
        float distance = Vector3.Distance(transform.position, player.position);

        // Rim Power 및 Rim Color의 Intensity 계산
        float normalizedDistance = Mathf.Clamp01(distance / maxDistance);
        float rimPower = Mathf.Lerp(0f, maxRimPower, 1f - normalizedDistance);
        float rimIntensity = Mathf.Lerp(1f, rimIntensityMultiplier, 1f - normalizedDistance);

        // Rim 컬러 및 베이스 맵 컬러 설정
        Color sideColor = GetUpdateSide();
        foreach (Material mat in materials)
        {
            mat.SetFloat("_RimPower", rimPower);
            mat.SetColor("_RimColor", sideColor * rimIntensity); // Rim Color의 Intensity 조절
        }
    }
}