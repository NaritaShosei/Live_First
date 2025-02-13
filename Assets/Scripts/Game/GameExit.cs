using UnityEngine;

public class GameExit : MonoBehaviour
{
    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else   
        Application.Quit();
#endif
    }
}