using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHaptic : MonoBehaviour
{
    public GameObject rightController; // 오른쪽 컨트롤러 오브젝트
    public GameObject leftController; // 왼쪽 컨트롤러 오브젝트

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameManager.Player.DecreaseRightHaptic(1.0f, 3.0f);
            //GameManager.Player.DecreaseLeftHaptic(1.0f, 3.0f);

            //GameManager.Player.IncreaseRightHaptic(1.0f, 3.0f);
            //GameManager.Player.IncreaseLeftHaptic(1.0f, 3.0f);


            //GameManager.Player.ActiveRightHaptic(0.4f, 0.1f);
            //GameManager.Player.ActiveLeftHaptic(0.4f, 0.1f);
            //GameManager.Player.RepeatRightHaptic(0.4f, 0.1f, 5);
            //GameManager.Player.RepeatLeftHaptic(0.4f, 0.1f, 3);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            GameManager.Player.DecreaseLeftHaptic(1.0f, 3.0f);
        }


        if (Input.GetKeyDown(KeyCode.D))
        {
            GameManager.Player.IncreaseRightHaptic(1.0f, 3.0f);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            GameManager.Player.IncreaseLeftHaptic(1.0f, 3.0f);
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            //GameManager.Player.ActiveRightHaptic(0.4f, 0.1f);
            //GameManager.Player.ActiveLeftHaptic(0.4f, 0.1f);
            GameManager.Player.RepeatRightHaptic(0.4f, 0.1f, 3);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            //GameManager.Player.ActiveRightHaptic(0.4f, 0.1f);
            //GameManager.Player.ActiveLeftHaptic(0.4f, 0.1f);
            //GameManager.Player.RepeatRightHaptic(0.4f, 0.1f, 3);
            GameManager.Player.RepeatLeftHaptic(0.4f, 0.1f, 3);
        }
    }
}
