using UnityEngine;

/// <summary>
/// いろいろテストするコンポーネント
/// </summary>
public class Test : GameManager
{
    double _startMusicTime;
    double StartMusicTime { get { return _startMusicTime; } }
    void Start()
    {
        _startMusicTime = AudioSettings.dspTime;
    }

    public double GetStartMusicTimes()
    {
        return _startMusicTime;
    }

    public double GetMusicTimes()
    {
        return AudioSettings.dspTime - _startMusicTime;
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void OpenWeb()
    {
        Application.OpenURL("https://unity-chan.com/");
    }
}
