using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITransform : MonoBehaviour
{
    public GameObject Target;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(player.transform.position);

        if (Target != null)
        {
            bool IsActive = !Target.activeSelf;
            this.gameObject.SetActive(IsActive);
        }
    }
}
