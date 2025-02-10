using DG.Tweening;
using System;
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
    float _comboScale;
    [SerializeField]
    Image _image;
    [SerializeField]
    Text _startText;
    public int Score { get => _score; }
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
            _score += (int)(1000 * (1f + scale * comboCount));
            Debug.Log("Perfect!" + _score);
            return HitType.perfect;
        }
        else if (difference <= 0.15f)
        {
            _score += (int)(500 * (1f + scale * comboCount));
            Debug.Log("Good" + _score);
            return HitType.good;
        }
        else
        {
            Debug.Log("Miss Hit");
            return HitType.miss;
        }
    }
    public bool InputButton()
    {
        return Input.anyKeyDown;
    }

    public void LiveEnd()
    {
        Debug.Log("LiveEnd");
        _image.DOFade(1, 1);
    }
}
public enum HitType
{
    perfect,
    good,
    miss,
}