using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreEffect : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float duration = 0.8f;
    private Camera cameraToLookAt;

    private float firstY;
    
    // Start is called before the first frame update
    void Start()
    {
        cameraToLookAt = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        firstY = this.transform.position.y;
        
        Destroy(this.gameObject, 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cameraToLookAt.transform.position);

        transform.DOMoveY(firstY + 0.8f, duration, false);
        text.material.DOFade(0.0f, duration).From().SetEase(Ease.OutQuad);
    }
}
