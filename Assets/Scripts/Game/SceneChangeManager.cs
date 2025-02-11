using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangeManager : MonoBehaviour
{
    [SerializeField]
    Image _fadePanel;
    public static void SceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void StartSceneChange(string sceneName)
    {
        _fadePanel.DOFade(1, 0.5f).OnComplete(() => SceneChange(sceneName));
    }
}
