using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    public List<GameObject> VFXs = new List<GameObject>();
    int a = 0;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
		if (VFXs.Count > 0 && Input.GetKeyDown(KeyCode.Space))
		{
                for (int i = 0; i < VFXs.Count; i++)
                {
					VFXs[i].SetActive(false);
                }
				VFXs[a].SetActive(true);
            a++;
            if (a == VFXs.Count)
                a = 0;
        }
		
	}
}
