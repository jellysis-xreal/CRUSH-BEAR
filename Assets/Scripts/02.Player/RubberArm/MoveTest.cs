using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Move());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void mo()
    {
        transform.DOMoveX(2f, 2f).SetLoops(1);
    }
    IEnumerator Move()
    {
        transform.DOMoveY(2f, 2f).SetLoops(2, LoopType.Yoyo);
        yield return new WaitForSeconds(2f);
    }
}
