using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableButton : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private GameObject brokenCookiePrefab;
    public bool isBlocked;
    public event Action onBreak;

    public void InitSettings()
    {
        audioSource = GetComponent<AudioSource>();
        isBlocked = false;  
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Controller") && !isBlocked)
        {
            var brokenVersion = Instantiate(brokenCookiePrefab, transform.position, transform.rotation);
            brokenVersion.GetComponent<BreakController>().IsHit();
            audioSource.Play();
            onBreak?.Invoke();
        }
    }

    public void AddEvent(Action onBreakDelegate)
    {
       onBreak -= onBreakDelegate;
       onBreak += onBreakDelegate;
    }
}
