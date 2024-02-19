using System;
using System.Collections;
using System.Collections.Generic;
using Deform;
using DG.Tweening;
using UnityEngine;

public class RhythmController : MonoBehaviour
{
    public float musicBPM = 100.0f; //Beat per minute(1분당 1/4박자의 개수)
    public List<GameObject> scaleObjects;
    public List<GameObject> transformObjects;

    [SerializeField] private List<Vector3> originScales = new List<Vector3>();
    [SerializeField] private List<BendDeformer> originDeformers = new List<BendDeformer>();
    
    private double tickTime = 0.0d;
    private GameObject deformBending;
    
    private void OnEnable()
    {
        deformBending = Resources.Load("Prefabs/Effects/Bend") as GameObject;
    }

    void Start()
    {
        InitializeScale();
        InitializeTransform();
    }
    
    // Update is called once per frame
    void Update()
    {
        tickTime += Time.deltaTime;

        if (tickTime >= 60d / musicBPM)
        {
            StartCoroutine(ObjectsScaleRhythm(tickTime));
            StartCoroutine(ObjectTransfromRhythm(tickTime));
            
            tickTime -= 60d / musicBPM;
        }
    }

    private void InitializeScale()
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

    private void InitializeTransform()
    {
        for (int i = 0; i < transformObjects.Count; i++)
        {            
            GameObject defromTry = Instantiate(deformBending, transformObjects[i].transform); 
            BendDeformer deformer = defromTry.GetComponent<BendDeformer>();
            transformObjects[i].AddComponent<Deformable>();

            transformObjects[i].GetComponent<Deformable>().AddDeformer(deformer);
            originDeformers.Add(deformer);
        }
    }

    IEnumerator ObjectsScaleRhythm(double tiKTime)
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

    IEnumerator ObjectTransfromRhythm(double tiKTime)
    {
        for (int i = 0; i < transformObjects.Count; i++)
        {
            float currAngle = originDeformers[i].GetAngle();
            
            bool HaveToMinus = (currAngle >= 0.0f);
            if (HaveToMinus)
                yield return StartCoroutine(AngleCoroutine(i, currAngle, -25.0f, (float)tiKTime));
            else
                yield return StartCoroutine(AngleCoroutine(i, currAngle, +25.0f, (float)tiKTime));

        }
        
        yield return new WaitForSeconds((float)tiKTime);
    }

    IEnumerator AngleCoroutine(int i, float startValue, float endValue, double tiKTime)
    {
        float currentTime = 0f;

        while (currentTime <= (float)tiKTime)
        {
            float t = currentTime / (float)tiKTime;
            float newValue = Mathf.Lerp(startValue, endValue, t);
            
            // 값 변경에 대한 작업 수행
            originDeformers[i].SetAngle(newValue);
            yield return null;
            
            currentTime += Time.deltaTime;
        }
    }

}
