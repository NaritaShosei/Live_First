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
    [SerializeField] Image _image;
    [SerializeField] Text _startText;

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
            if (Input.anyKeyDown)
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

    public void CheckHit(float noteTime)
    {
        double currentTime = GetMusicTime();
        double difference = Mathf.Abs((float)(currentTime - noteTime));

        if (difference <= 0.05f)
        {
            Debug.Log("Perfect!");
        }
        else if (difference <= 0.15f)
        {
            Debug.Log("Good");
        }
        else
        {
            Debug.Log("Miss Hit");
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
