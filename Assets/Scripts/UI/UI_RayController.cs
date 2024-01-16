using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class UI_RayController : MonoBehaviour
{
    public InputActionProperty menuAction;
    public XRInteractorLineVisual ray;
    
    private bool GetControllerActivateAction()
    {
        // hand의 grab버튼 활성화 확인
        // ########### 수정 필요 #############
        return menuAction.action.IsInProgress();
    }

    // Start is called before the first frame update
    void Start()
    {
        menuAction.action.performed += OnMyActionTriggered;
    }

    private void OnMyActionTriggered(InputAction.CallbackContext context)
    {
        // InputAction이 trigger될 때 호출되는 콜백 메서드
        Debug.Log("Action Triggered!");

        if (!ray.enabled) {
            GameManager.Wave.SetIsPause(true);
            ray.enabled = true;
            GameObject go = GameManager.UI.ShowPopupUI<UI_Popup>("PopupSettings").gameObject;
            GameManager.UI.SetCanvas(go, true);
        }
        
        GameManager.UI.CalibrateCanvasLocation();
        // ray.enabled = ray.enabled ? false : true;
    }
}