using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class CutsceneCookie : MonoBehaviour
{
    private EndingCutscene cutscene;
    private int tryBreak;
    private WaitForSeconds wait;
    private bool isWaiting;
    [SerializeField] private GameObject brokenCookiePrefab;

    public void InitBreakable(EndingCutscene endingCutscene)
    {

        isWaiting = false;
        cutscene = endingCutscene;
        tryBreak = 0;
        wait = new WaitForSeconds(0.1f);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (isWaiting)
            return;

        if (other.CompareTag("Destroyer"))
        {
            var brokenVersion = Instantiate(brokenCookiePrefab, transform.position, transform.rotation);
            brokenVersion.GetComponent<BreakController>().IsHit();
            StartCoroutine(StopBreak());
            tryBreak++;
        }

        if (tryBreak >= 15)
        {
            cutscene.StartEndingCredit();
        }
    }

    public IEnumerator StopBreak()
    {
        isWaiting = true;
        yield return wait;
        isWaiting = false;
    }
}
