using System.Collections;
using UnityEngine;

public class CutsceneCookie : MonoBehaviour
{
    private EndingController cutscene;
    private int tryBreak;
    private WaitForSeconds wait;
    private bool isWaiting;
    private AudioSource audioSource;
    [SerializeField] private GameObject brokenCookiePrefab;

    public void InitBreakable(EndingController endingCutscene)
    {
        isWaiting = false;
        cutscene = endingCutscene;
        tryBreak = 0;
        audioSource = GetComponent<AudioSource>();
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
            audioSource.Play();
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
