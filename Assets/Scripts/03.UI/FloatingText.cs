using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EnumTypes;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float enableTime = 2.0f;
    private Vector3 offset = new Vector3(0, 1.3f, 0);
    
    private Animator anim;
    private TextMesh textMesh;
    
    public bool isEnable = false;
    private float elapsedTime = 0.0f;
    
    private Color textColor = Color.gray;
    
    // private Color[] bluePalette = new Color[] {
    //     new Color(0.0f, 0.0f, 1.0f), // Pure Blue
    //     new Color(0.0f, 0.0f, 0.5f), // Dark Blue
    //     new Color(0.0f, 0.0f, 0.75f), // Medium Blue
    //     new Color(0.5f, 0.5f, 1.0f), // Light Blue
    //     new Color(0.0f, 0.5f, 1.0f), // Sky Blue
    //     new Color(0.0f, 0.25f, 0.5f), // Navy Blue
    //     new Color(0.0f, 0.0f, 0.25f), // Deep Blue
    //     // 추가로 원하는 파란색 계열의 색상을 여기에 추가하세요.
    // };
    
    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        textMesh = this.GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnable)
        {
            //elapsedTime += Time.deltaTime;
            // if (elapsedTime >= 0.5f)
            // {
            //     textMesh.color = GetRandomColorFromPalette();
            //     elapsedTime = 0.0f;
            // }
            enableTime -= Time.deltaTime;

            if (enableTime <= 0)
            {
                Initialize();
            }
        }
    }
    
    // private Color GetRandomColorFromPalette()
    // {
    //     int randomIndex = UnityEngine.Random.Range(0, bluePalette.Length);
    //     return bluePalette[randomIndex];
    // }

    public void SetTextColor(scoreType type)
    {
        switch (type)
        {
            case scoreType.Perfect:
                textColor = new Color(0.2f, 0.5f, 1.0f); // Sky Blue
                break;
            case scoreType.Good:
                textColor = new Color(0.4f, 1.0f, 0.5f);
                break;
            case scoreType.Weak:
                textColor = new Color(0.9f, 1.0f, 0.5f);
                break;
            case scoreType.Bad:
                textColor = new Color(0.9f, 0.4f, 0.0f);
                break;
            case scoreType.Miss:
                textColor = Color.gray;
                break;
        }
    }

    private void Initialize()
    {
        gameObject.SetActive(false);
        isEnable = false;
        enableTime = 2.0f;
        transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }
    
    private void OnEnable()
    {
        transform.localPosition += offset;
        //anim.Rebind();
        isEnable = true;
        this.transform.DOMoveY(this.transform.position.y + 1.2f, 1.0f);

    }
}
