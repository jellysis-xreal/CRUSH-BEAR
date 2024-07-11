using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject floatingUIPrefab;
    
    // Score Floating Text UI Pool
    private List<GameObject> _scoreFloatingTextPool = new List<GameObject>(5);
    
    int _order = 10;

    private bool ray = false;

    private Stack<UI_Popup> _popupStack;
    UI_Scene _sceneUI;
    GameObject player;

    public void Init()
    {
    }

    public void Start()
    {
        //InitializeFloatingTextPool();
        player = GameObject.FindWithTag("MainCamera");
        _popupStack = new Stack<UI_Popup>();
    }
    
    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }

    // private void InitializeFloatingTextPool()
    // {
    //     for (int i = 0; i < 5; i++)
    //     {
    //         GameObject floatingUI = Instantiate(floatingUIPrefab);
    //         floatingUI.SetActive(false);
    //         _scoreFloatingTextPool.Add(floatingUI);
    //     }
    // }
    
    public void RequestFloatingUI(float value)
    {
        Transform setTransform = GameManager.Wave.GetWaveScoreUI();
        
        if (_scoreFloatingTextPool.Count < 7)
        {
            GameObject UI = Instantiate(floatingUIPrefab);
            UI.SetActive(false);
            _scoreFloatingTextPool.Add(UI);
        }
        
        foreach (GameObject floatingUI in _scoreFloatingTextPool)
        {
            if (!floatingUI.activeInHierarchy)
            {
                Vector3 randomXYZ = new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, 0.0f);
                floatingUI.transform.position = setTransform.position + randomXYZ;

                if (player != null)
                {
                    Vector3 directionToPlayer =  floatingUI.transform.position - player.transform.position;
                    Quaternion rotationToPlayer = Quaternion.LookRotation(directionToPlayer);
                    floatingUI.transform.rotation = rotationToPlayer;
                }
                else
                {
                    player = GameObject.FindWithTag("MainCamera");
                }

                floatingUI.GetComponent<TextMesh>().text= value.ToString();
                floatingUI.SetActive(true);
                return;
            }
        }
    }
    
    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Utils.GetOrAddComponent<Canvas>(go);
        //Debug.Log("[TEST] Canvas");
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = GameManager.Resource.Instantiate($"UI/Scene/{name}");

        T SceneUI = Utils.GetOrAddComponent<T>(go);
        _sceneUI = SceneUI;

        go.transform.SetParent(Root.transform);

        return SceneUI;
    }

    public T ShowPopupUI<T> (string name = null) where T : UI_Popup
    {
        // Debug.Log("[TEST] calibraet first");
        // CalibrateCanvasLocation();
        if (string.IsNullOrEmpty (name))
            name = typeof (T).Name;

        GameObject go = GameManager.Resource.Instantiate($"UI/Popup/{name}");
        // print("[test]: " +go);

        T popup = Utils.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        go.transform.SetParent(Root.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;

        return popup;
    }

    public void ClosePopupUI(UI_Popup popup)
    {
        if(_popupStack.Count == 0)
            return;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("[ERROR] Close Popup Failed");
            return;
        }

        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        GameManager.Resource.Destroy(popup.gameObject);
        popup = null;

        _order--;
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

    public void CalibrateCanvasLocation()
    {
        Vector3 cameraPosition = player.transform.position;
        Vector3 cameraDirection = player.transform.forward;

        // 카메라에서 떨어진 거리 조절
        float distance = 1.0f;
        Vector3 newPosition = cameraPosition + cameraDirection * distance;

        // GameObject의 위치 이동
        Root.transform.position = newPosition;

        // GameObject의 회전 설정
        Root.transform.rotation = player.transform.rotation;
    }

    public bool IsRayOn()
    {
        return ray;
    }
    
    public bool SetRayOn(bool _ray)
    {
        ray = _ray;
        return ray;
    }
}