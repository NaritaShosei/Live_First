using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayableDirector _director;
    public bool IsPlayed { get => _isPlayed; }
    bool _isPlayed = false;
    bool _isLoaded = false;
    [SerializeField, Header("最大スコア倍率")]
    float _maxComboScale = 5;
    [SerializeField, Header("最大スコア倍率に到達するコンボ数")]
    int _maxComboCount = 50;
    [SerializeField]
    Image _image;
    [SerializeField]
    Text _startText;
    [SerializeField]
    Text _comboCountText;
    [SerializeField]
    Text _scoreText;
    int _score;

    async void Start()
    {
        await LoadTimeLineAudio(_director);
        await Task.Delay(1000);

        _startText.DOFade(1, 0.5f).OnComplete(() => _isLoaded = true);
    }

    async Task LoadTimeLineAudio(PlayableDirector director)
    {
        if (director.playableAsset is TimelineAsset timeLine)
        {
            HashSet<AudioClip> audioSet = new();

            foreach (var track in timeLine.GetOutputTracks())
            {
                if (track is AudioTrack audioTrack)
                {
                    foreach (var clip in audioTrack.GetClips())
                    {
                        if (clip.asset is AudioPlayableAsset audioPlayable)
                        {
                            AudioClip audioClip = audioPlayable.clip;

                            if (audioClip != null)
                            {
                                audioSet.Add(audioClip);
                            }
                        }
                    }
                }
            }

            List<Task> loadTasks = new List<Task>();
            foreach (var clip in audioSet)
            {
                loadTasks.Add(LoadAudioClipsAsync(clip));
            }

            await Task.WhenAll(loadTasks);
        }
    }

    async Task LoadAudioClipsAsync(AudioClip clip)
    {
        if (clip == null) return;
        clip.LoadAudioData();

        await Task.Yield();

        while (!clip.isReadyToPlay)
        {
            await Task.Delay(50);
        }
    }
    void Update()
    {
        if (!_isPlayed && _isLoaded)
        {
            if (InputButton())
            {
                DOTween.Sequence()
             .Join(_startText.DOFade(0, 0.5f))
             .Join(_image.DOFade(0, 1).OnComplete(() => _director.Play()));
                _isPlayed = true;
            }
        }
        else if (_isPlayed)
        {
            _scoreText.text = _score.ToString("000000");
        }
    }

    public double GetMusicTime()
    {
        return _director.time;
    }

    public HitType CheckHit(float noteTime, int comboCount)
    {
        double currentTime = GetMusicTime();
        double difference = Mathf.Abs((float)(currentTime - noteTime));
        float scale = _maxComboScale / _maxComboCount;
        if (difference <= 0.05f)
        {
            AddScore((int)(1000 * (1f + Mathf.Min(scale * comboCount, _maxComboScale))));
            Debug.Log("Perfect!" + _score);
            return HitType.perfect;
        }
        else if (difference <= 0.15f)
        {
            AddScore((int)(500 * (1f + Mathf.Min(scale * comboCount, _maxComboScale))));
            Debug.Log("Good" + _score);
            return HitType.good;
        }
        else
        {
            Debug.Log("Miss Hit");
            return HitType.miss;
        }
    }
    void AddScore(int score)
    {
        int current = _score;
        DOTween.To(() => current, x => _score = x, current + score, 0.3f);
    }
    public bool InputButton()
    {
        return Input.anyKeyDown;
    }

    public void DrawComboCount(int comboCount)
    {
        if (comboCount == 0)
        {
            _comboCountText.DOFade(0, 0.1f);
        }
        if (comboCount > 0)
        {
            _comboCountText.DOFade(1, 0.3f);
            _comboCountText.text = comboCount.ToString();
        }
    }

    public void LiveEnd(string name)
    {
        Debug.Log("LiveEnd");
        _image.DOFade(1, 1).OnComplete(() => SceneChangeManager.SceneChange(name));
    }
}

public enum HitType
{
    perfect,
    good,
    miss,
}