using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestoryOnLoad : MonoBehaviour
{
    private static DoNotDestoryOnLoad _instance = null;

    private void Awake()
    {
        if (_instance)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        _instance = this;
        
        DontDestroyOnLoad(this.gameObject);
    }
}
