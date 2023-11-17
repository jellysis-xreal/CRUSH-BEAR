using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombsGeneratorFreeFall : MonoBehaviour
{

    float ranInterval = 1.0f;
    float totalTime = 0;
    float nextGenTime = 0;
    public GameObject bombs;
    public Transform playerTransform;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;

        if (nextGenTime < totalTime){
            GenerateBombs();
            ranInterval = Random.Range(1.0f, 3.0f);
            nextGenTime += ranInterval;
        }
    }

    private void GenerateBombs()
    {
        playerTransform = GameObject.FindWithTag("MainCamera").transform;
        // float randomX = Random.Range(-3f, 3f);
        // float randomY = Random.Range(0.9f, 1.5f);
        // float randomZ = 5f;

        Vector3 randomPosition = new Vector3(playerTransform.position[0], 5f, playerTransform.position[2]);

        Instantiate(bombs, randomPosition, Quaternion.identity);
    }
}