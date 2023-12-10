using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RhythmController : MonoBehaviour
{
    public float musicBPM = 100.0f; //Beat per minute(1분당 1/4박자의 개수)
    public List<GameObject> scaleObjects;
    public List<GameObject> transformObjects;

    private List<Vector3> originScales = new List<Vector3>();
    
    [SerializeField] private double tickTime = 0.0d;

    void Start()
    {
        Initialize();
    }
    
    // Update is called once per frame
    void Update()
    {
        tickTime += Time.deltaTime;

        if (tickTime >= 60d / musicBPM)
        {
            StartCoroutine(ObjectsRhythm(tickTime));
            tickTime -= 60d / musicBPM;
        }
    }

    void Initialize()
    {
        for (int i = 0; i < scaleObjects.Count; i++)
        {
            Vector3 origin = scaleObjects[i].transform.localScale;
            originScales.Add(origin);

            if (i % 2 == 0)
            {
                float ratio = origin.magnitude * 0.2f;
                scaleObjects[i].transform.localScale = origin - new Vector3(ratio, ratio, ratio);
            }
        }
    }
    
    IEnumerator ObjectsRhythm(double tiKTime)
    {
        for(int i  = 0; i< scaleObjects.Count; i++)
        {
            Transform transform = scaleObjects[i].transform;
            Vector3 originScale = originScales[i];

            float ratio = originScales[i].magnitude * 0.2f;

            if (transform.transform.localScale.magnitude >= originScale.magnitude)
                transform.DOScale(originScale - new Vector3(ratio, ratio, ratio), (float)tiKTime).SetEase(Ease.InQuad);
            else
                transform.DOScale(originScale + new Vector3(ratio, ratio, ratio), (float)tiKTime).SetEase(Ease.InQuad);;
        }

        yield return new WaitForSeconds((float)tiKTime);
    }
}
