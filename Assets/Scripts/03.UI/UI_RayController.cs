using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UI_RayController : MonoBehaviour
{
    [SerializeField] private InputActionAsset m_ActionAsset;
    public InputActionProperty menuAction;
    public GameObject ray_left;
    public GameObject ray_right;
    
    // Start is called before the first frame update
    void Start()
    {
        m_ActionAsset.Enable();
        menuAction.action.performed += OnMyActionTriggered;
    }

    void Update()
    {
        if (GameManager.Instance != null)
        {
            ray_left.SetActive(GameManager.UI.IsRayOn());
            ray_right.SetActive(GameManager.UI.IsRayOn());    
        }
    }

    private void OnMyActionTriggered(InputAction.CallbackContext context)
    {
        // InputAction이 trigger될 때 호출되는 콜백 메서드
        Debug.Log("Action Triggered!");
        
        GameManager.Sound.PlayEffect_UI_PopUp();
        
        if (SceneManager.GetActiveScene().name == "00.StartScene"){
            GameManager.UI.SetRayOn(!GameManager.UI.IsRayOn());
        }
        else {
            if (!GameManager.UI.IsRayOn())
            {
                GameManager.UI.SetRayOn(true);
                GameManager.Wave.SetIsPause(true);
                GameObject go = GameManager.UI.ShowPopupUI<UI_Popup>("PopupSettings").gameObject;
                GameManager.UI.SetCanvas(go, true);
            }
            GameManager.UI.CalibrateCanvasLocation();
        }
        // ray.enabled = ray.enabled ? false : true;
    }
}