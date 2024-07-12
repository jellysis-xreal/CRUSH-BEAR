using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "ScriptableObjects/StageData", order = 2)]
public class StageData : ScriptableObject
{
    [SerializeField] public string stageName;
    [SerializeField] public List<AudioClip> musicClips;
    [SerializeField] public int maxScore;
    [SerializeField] public int unlockID;
}
