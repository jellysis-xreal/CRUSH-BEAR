using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

/// <summary>
/// Simple helper script that conditionally enables rendering of a controller if it is connected.
/// </summary>
[HelpURL("https://developer.oculus.com/documentation/unity/controller-animations/")]
public class OVRControllerHelperCustom : MonoBehaviour,
    OVRInputModule.InputSource
{
    /// <summary>
    /// The root GameObject that represents the Meta Quest Plus Controller model (Left).
    /// </summary>
    public GameObject m_modelMetaLeftController;

    /// <summary>
    /// The root GameObject that represents the Meta Quest Plus Controller model (Right).
    /// </summary>
    public GameObject m_modelMetaRightController;

    /// <summary>
    /// The controller that determines whether or not to enable rendering of the controller model.
    /// </summary>
    public OVRInput.Controller m_controller;

    /// <summary>
    /// Determines if the controller should be hidden based on held state.
    /// </summary>
    public OVRInput.InputDeviceShowState m_showState = OVRInput.InputDeviceShowState.ControllerInHandOrNoHand;

    /// <summary>
    /// If controller driven hand poses is on, and the mode is Natural, controllers will be hidden unless this is true.
    /// </summary>
    public bool showWhenHandsArePoweredByNaturalControllerPoses = false;

    /// <summary>
    /// The animator component that contains the controller animation controller for animating buttons and triggers.
    /// </summary>
    private Animator m_animator;

    /// <summary>
    /// An optional component for provind shell like ray functionality - highlighting where you're selecting in the UI and responding to pinches / button presses.
    /// </summary>
    public OVRRayHelper RayHelper;

    private GameObject m_activeController;

    private bool m_controllerModelsInitialized = false;

    private bool m_hasInputFocus = true;
    private bool m_hasInputFocusPrev = false;
    private bool m_isActive = false;
    
    private bool m_prevControllerConnected = false;
    private bool m_prevControllerConnectedCached = false;

    private OVRInput.ControllerInHandState m_prevControllerInHandState = OVRInput.ControllerInHandState.NoHand;

    void Start()
    {
        if (OVRManager.OVRManagerinitialized)
        {
            InitializeControllerModels();
        }
    }

    void OnEnable()
    {
        OVRInputModule.TrackInputSource(this);
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void OnDisable()
    {
        OVRInputModule.UntrackInputSource(this);
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    void OnSceneChanged(Scene unloading, Scene loading)
    {
        OVRInputModule.TrackInputSource(this);
    }

    void InitializeControllerModels()
    {
        if (m_controllerModelsInitialized)
            return;

        OVRPlugin.SystemHeadset headset = OVRPlugin.GetSystemHeadsetType();
        OVRPlugin.Hand controllerHand = m_controller == OVRInput.Controller.LTouch
            ? OVRPlugin.Hand.HandLeft
            : OVRPlugin.Hand.HandRight;
        OVRPlugin.InteractionProfile profile = OVRPlugin.GetCurrentInteractionProfile(controllerHand);
        // If multimodality is enabled, then overwrite the value if we find the controllers to be unheld
        if (OVRPlugin.IsMultimodalHandsControllersSupported())
        {
            OVRPlugin.InteractionProfile detachedProfile =
                OVRPlugin.GetCurrentDetachedInteractionProfile(controllerHand);
            if (detachedProfile != OVRPlugin.InteractionProfile.None)
            {
                profile = detachedProfile;
            }
        }
        
        // Hide all controller models until controller get connected
        m_modelMetaLeftController.SetActive(false);
        m_modelMetaRightController.SetActive(false);

        OVRManager.InputFocusAcquired += InputFocusAquired;
        OVRManager.InputFocusLost += InputFocusLost;

        m_controllerModelsInitialized = true;
    }

    void Update()
    {
        m_isActive = false;
        if (!m_controllerModelsInitialized)
        {
            if (OVRManager.OVRManagerinitialized)
            {
                InitializeControllerModels();
            }
            else
            {
                return;
            }
        }

        OVRInput.Hand handOfController = (m_controller == OVRInput.Controller.LTouch)
            ? OVRInput.Hand.HandLeft
            : OVRInput.Hand.HandRight;
        OVRInput.ControllerInHandState controllerInHandState = OVRInput.GetControllerIsInHandState(handOfController);

        bool controllerConnected = OVRInput.IsControllerConnected(m_controller);

        if ((controllerConnected != m_prevControllerConnected) || !m_prevControllerConnectedCached ||
            (controllerInHandState != m_prevControllerInHandState) ||
            (m_hasInputFocus != m_hasInputFocusPrev))
        {
            m_modelMetaLeftController.SetActive(controllerConnected &&
                                                         (m_controller == OVRInput.Controller.LTouch));
            m_modelMetaRightController.SetActive(controllerConnected &&
                                                          (m_controller == OVRInput.Controller.RTouch));

            m_animator = m_controller == OVRInput.Controller.LTouch
                ? m_modelMetaLeftController.GetComponent<Animator>()
                : m_modelMetaRightController.GetComponent<Animator>();
            m_activeController = m_controller == OVRInput.Controller.LTouch
                ? m_modelMetaLeftController
                : m_modelMetaRightController;


            m_prevControllerConnected = controllerConnected;
            m_prevControllerConnectedCached = true;
            m_prevControllerInHandState = controllerInHandState;
            m_hasInputFocusPrev = m_hasInputFocus;
        }

        bool shouldSetControllerActive = m_hasInputFocus && controllerConnected;
        switch (m_showState)
        {
            case OVRInput.InputDeviceShowState.Always:
                // intentionally blank
                break;
            case OVRInput.InputDeviceShowState.ControllerInHandOrNoHand:
                if (controllerInHandState == OVRInput.ControllerInHandState.ControllerNotInHand)
                {
                    shouldSetControllerActive = false;
                }

                break;
            case OVRInput.InputDeviceShowState.ControllerInHand:
                if (controllerInHandState != OVRInput.ControllerInHandState.ControllerInHand)
                {
                    shouldSetControllerActive = false;
                }

                break;
            case OVRInput.InputDeviceShowState.ControllerNotInHand:
                if (controllerInHandState != OVRInput.ControllerInHandState.ControllerNotInHand)
                {
                    shouldSetControllerActive = false;
                }

                break;
            case OVRInput.InputDeviceShowState.NoHand:
                if (controllerInHandState != OVRInput.ControllerInHandState.NoHand)
                {
                    shouldSetControllerActive = false;
                }

                break;
        }

        if (!showWhenHandsArePoweredByNaturalControllerPoses && OVRPlugin.IsControllerDrivenHandPosesEnabled() && OVRPlugin.AreControllerDrivenHandPosesNatural())
        {
            shouldSetControllerActive = false;
        }

        m_isActive = shouldSetControllerActive;

        if (m_activeController != null)
        {
            m_activeController.SetActive(shouldSetControllerActive);
        }

        if (RayHelper != null)
        {
            RayHelper.gameObject.SetActive(shouldSetControllerActive);
        }


        if (m_animator != null)
        {
            //m_animator.SetFloat("Button 1", OVRInput.Get(OVRInput.Button.One, m_controller) ? 1.0f : 0.0f);
            //m_animator.SetFloat("Button 2", OVRInput.Get(OVRInput.Button.Two, m_controller) ? 1.0f : 0.0f);
            //m_animator.SetFloat("Button 3", OVRInput.Get(OVRInput.Button.Start, m_controller) ? 1.0f : 0.0f);

            //m_animator.SetFloat("Joy X", OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_controller).x);
            //m_animator.SetFloat("Joy Y", OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_controller).y);

            m_animator.SetFloat("Trigger", OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller));
            m_animator.SetFloat("Grip", OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller));
        }
    }

    public void InputFocusAquired()
    {
        m_hasInputFocus = true;
    }

    public void InputFocusLost()
    {
        m_hasInputFocus = false;
    }

    public bool IsPressed()
    {
        return OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, m_controller);
    }

    public bool IsReleased()
    {
        return OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, m_controller);
    }

    public Transform GetPointerRayTransform()
    {
        return transform;
    }

    // This helps identify if the object has been destroyed.
    public bool IsValid()
    {
        return this != null;
    }

    public bool IsActive()
    {
        return m_isActive;
    }

    public OVRPlugin.Hand GetHand()
    {
        return m_controller == OVRInput.Controller.LTouch ? OVRPlugin.Hand.HandLeft : OVRPlugin.Hand.HandRight;
    }

    public void UpdatePointerRay(OVRInputRayData rayData)
    {
        if (RayHelper)
        {
            rayData.IsActive = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, m_controller);
            rayData.ActivationStrength = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller);
            RayHelper.UpdatePointerRay(rayData);
        }
    }
}
