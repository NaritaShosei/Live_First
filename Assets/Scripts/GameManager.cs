using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayableDirector _director;
    bool _isLoaded = false;
    [SerializeField] Image _image;
    [SerializeField] Text _startText;
    double _startMusicTime;
    double StartMusicTime { get { return _startMusicTime; } }

    async void Start()
    {
        Debug.Assert(_director != null);

        await LoadTimeLineAudio(_director);
        await Task.Delay(1000);

        _startText.DOFade(1, 1).OnComplete(() => _isLoaded = true);
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
        if (_isLoaded)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DOTween.Sequence().
                Append(_startText.DOFade(0, 0.5f))
                .Join(_image.DOFade(0, 1).OnComplete(() => _director.Play()));
                _isLoaded = false;
                _startMusicTime = AudioSettings.dspTime;
            }
        }
    }

    public double GetMusicTime()
    {
        return AudioSettings.dspTime - _startMusicTime;
    }
    public void LiveEnd()
    {
        Debug.Log("LiveEnd");
        _image.DOFade(1, 1);
    }
}
