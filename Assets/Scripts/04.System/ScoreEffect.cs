using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreEffect : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float duration = 2f;
    private Camera cameraToLookAt;
    public SpriteRenderer spriteRenderer;
    private float firstY;
    
    // Start is called before the first frame update
    void Start()
    {
        cameraToLookAt = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        firstY = this.transform.position.y;
        
        Destroy(this.gameObject, duration);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cameraToLookAt.transform.position);

        transform.DOMoveY(firstY + 1f, duration, false);
        spriteRenderer.DOFade(0.1f, duration).SetEase(Ease.OutSine);
        //Out(0.5f, duration).From().SetEase(Ease.OutSine);
        // text.material.DOFade(0.0f, duration).From().SetEase(Ease.OutSine);
    }
}
