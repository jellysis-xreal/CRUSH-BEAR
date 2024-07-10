using UnityEngine;

[CreateAssetMenu(fileName = "SpriteData", menuName = "ScriptableObjects/SpriteData", order = 1)]
public class SpriteData : ScriptableObject
{
    [SerializeField] public Sprite[] numberSprites;
    [SerializeField] public Sprite comma;
    [SerializeField] public Sprite dot;
    [SerializeField] public Sprite x;
    [SerializeField] public Sprite[] rank;
    [SerializeField] public Sprite lockedCurtain;
    [SerializeField] public Sprite unlockedCurtain;
}
