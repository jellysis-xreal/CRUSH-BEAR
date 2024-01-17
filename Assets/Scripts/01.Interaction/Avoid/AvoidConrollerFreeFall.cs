using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidConrollerFreeFall : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public Transform playerTransform;
    public float minDrag = 10f;
    public float maxDrag = 30f;
    public bool isHit;
    public GameObject warningSign;
    public GameObject generatedWarningSign;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        float _Drag = Random.Range(minDrag, maxDrag);
        isHit = false;
        _rigidbody.drag = _Drag;
        int layerMask = 1 << 7;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, layerMask))
        {
            Vector3 spawnPosition = hit.point + new Vector3(0f, 0.01f, 0f);
            generatedWarningSign = Instantiate(warningSign, spawnPosition, Quaternion.Euler(90f, 0f, 0f));
        }

    }

    void Update()
    {
    }

    private void OnTriggerEnter(Collider other){
        Crashed();
    }

    public void Crashed()
    {
        Debug.Log("[Test] Crashed!");
        isHit = true;
        BearManager.instance.DecreaseHearts();
        Destroy(generatedWarningSign);
        Destroy(gameObject);
    }
}
