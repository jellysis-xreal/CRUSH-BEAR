using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreEffect : MonoBehaviour
{
    public EnumTypes.scoreType scoreType;
    public TextMeshProUGUI text;
    public float duration = 2f;
   
    public SpriteRenderer spriteRenderer;
    private float firstY;
     private Camera cameraToLookAt;
    private bool _isActive = false;
    
    // Start is called before the first frame update
    void Start()
    {
        cameraToLookAt = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void EffectGoUp()
    {
        transform.LookAt(cameraToLookAt.transform.position);

        transform.DOMoveY(firstY + 1f, duration, false);
        spriteRenderer.DOFade(0.1f, duration).SetEase(Ease.OutSine);
        //Out(0.5f, duration).From().SetEase(Ease.OutSine);
        // text.material.DOFade(0.0f, duration).From().SetEase(Ease.OutSine);
    }

    private void OnEnable()
    {
        if (cameraToLookAt == null)
            cameraToLookAt = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        firstY = transform.position.y;
        EffectGoUp();
    }

    private void OnDisable()
    {
        Color color = spriteRenderer.color;
        color.a = 1.0f;
        spriteRenderer.color = color;
    }
}
