using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class CutsceneBreakable : Breakable
{
    public override void OnTriggerEnter(Collider other)
    {
        if (m_Destroyed)
            return;

        if (other.CompareTag("Destroyer"))
        {
            if (m_Destroyed)
                return;
            m_Destroyed = true;
            var brokenVersion = Instantiate(m_BrokenVersion, transform.position, transform.rotation);
            brokenVersion.GetComponent<BreakController>().IsHit();
            onBreak?.Invoke(null, null);
        }
    }
}
