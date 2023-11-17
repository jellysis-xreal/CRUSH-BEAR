using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombsGenerator : MonoBehaviour
{

    float ranInterval = 1.0f;
    float totalTime = 0;
    float nextGenTime = 0;
    public GameObject bombs;

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
        float randomX = Random.Range(-3f, 3f);
        float randomY = Random.Range(0.9f, 1.5f);
        float randomZ = 3f;

        Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);

        Instantiate(bombs, randomPosition, Quaternion.identity);
    }
}