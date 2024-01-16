using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NodeTimingMaker : MonoBehaviour
{
    private AudioSource _audioSource;
    public TextMeshProUGUI text;
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.Play();
    }

    private List<float> nodeTimes = new List<float>();
       
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveTime();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LogTime();
        }
    }

    private void SaveTime()
    {
        nodeTimes.Add(Time.time);
        text.text = nodeTimes.Count.ToString();
    }

    private void LogTime()
    {
        string timeData = "";
        for (int i = 0; i < nodeTimes.Count; i++)
        {
            timeData += nodeTimes[i].ToString() + " ";
        }

        Debug.Log(timeData);
        Debug.Log(nodeTimes.Count);
    }
}
