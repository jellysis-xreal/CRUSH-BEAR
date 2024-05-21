using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerGUI : Editor
{
    public override void OnInspectorGUI()
    {
        GameManager gameManager = (GameManager)target;
        if (GUILayout.Button("Start Wave Scene"))
        {
            gameManager.LobbyToWave();
        }
        if (GUILayout.Button("Start Tutorial Scene"))
        {
            gameManager.LobbyToTutorial();
        }

        DrawDefaultInspector();
    }
}

