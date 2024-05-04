using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float enableTime = 2.0f;
    private Vector3 offset = new Vector3(0, 2.0f, 0);
    
    private Animator anim;
    private TextMesh textMesh;
    
    public bool isEnable = false;
    
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
            enableTime -= Time.deltaTime;
            textMesh.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

            if (enableTime <= 0)
            {
                Initialize();
            }
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
    }
}
