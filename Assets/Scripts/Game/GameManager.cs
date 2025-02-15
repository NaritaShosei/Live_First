using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
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
    TextMeshProUGUI _comboCountText;
    [SerializeField]
    TextMeshProUGUI _scoreText;
    [SerializeField]
    TextMeshProUGUI _typeText;
    int _score;
    int _comboCount;

    async void Start()
    {
        await LoadTimeLineAudio(_director);
        await Awaitable.WaitForSecondsAsync(1);

        _startText.DOFade(1, 0.5f).OnComplete(() => _isLoaded = true);
    }

    async Awaitable LoadTimeLineAudio(PlayableDirector director)
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

            List<Awaitable> loadTasks = new List<Awaitable>();
            foreach (var clip in audioSet)
            {
                loadTasks.Add(LoadAudioClipsAsync(clip));
            }

            foreach (var task in loadTasks)
            {
                await task;
            }
        }
    }

    async Awaitable LoadAudioClipsAsync(AudioClip clip)
    {
        if (clip == null) return;
        clip.LoadAudioData();

        await Awaitable.NextFrameAsync();

        while (!clip.isReadyToPlay)
        {
            await Awaitable.WaitForSecondsAsync(0.05f);
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
            var newText = _score.ToString("000000");
            if (_scoreText.text != newText)
                _scoreText.text = newText;
        }
    }

    public double GetMusicTime()
    {
        return _director.time;
    }

    public HitType CheckHit(float noteTime)
    {
        double currentTime = GetMusicTime();
        double difference = Mathf.Abs((float)(currentTime - noteTime));
        float scale = _maxComboScale / _maxComboCount;
        var type = HitType.perfect;
        if (difference <= 0.05f)
        {
            _comboCount++;
            AddScore((int)(1000 * (1f + Mathf.Min(scale * _comboCount, _maxComboScale))));
            type = HitType.perfect;
        }
        else if (difference <= 0.15f)
        {
            _comboCount++;
            AddScore((int)(500 * (1f + Mathf.Min(scale * _comboCount, _maxComboScale))));
            type = HitType.good;
        }
        else
        {
            _comboCount = 0;
            type = HitType.miss;
        }
        DrawHitType(type);
        DrawComboCount();
        return type;
    }
    void DrawHitType(HitType type)
    {
        string typeText = type switch
        {
            HitType.perfect => "Perfect!!",
            HitType.good => "Good!",
            HitType.miss => "Miss..."
        };
        _typeText.transform.DOShakePosition(0.1f, 2, 10, 1, false, true);
        _typeText.text = typeText;
    }
    void AddScore(int score)
    {
        int current = _score;
        _scoreText.transform.DOShakePosition(0.1f, 5, 10, 1, false, true);
        DOTween.To(() => current, x => _score = x, current + score, 0.3f);
    }
    public bool InputButton()
    {
        return Input.anyKeyDown;
    }

    public void MissHit()
    {
        _comboCount = 0;
        DrawHitType(HitType.miss);
        DrawComboCount();
    }
    public void DrawComboCount()
    {
        if (_comboCount < 10)
        {
            _comboCountText.DOFade(0, 0.1f);
        }
        if (_comboCount >= 10)
        {
            _comboCountText.DOFade(1, 0.3f);
            _comboCountText.text = $"{_comboCount}<size=25>combo</size>";
        }
    }

    public void LiveEnd(string name)
    {
        _image.DOFade(1, 1).OnComplete(() =>
        {
            ResultManager.AddScore(_score, _comboCount);
            SceneChangeManager.SceneChange(name);
        });
    }
}

public enum HitType
{
    perfect,
    good,
    miss,
}