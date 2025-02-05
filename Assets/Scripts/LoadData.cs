using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class LoadData : MonoBehaviour
{
    [SerializeField] PlayableDirector _director;
    async void Start()
    {
        Debug.Assert(_director != null);
        _director.RebuildGraph();

        await LoadTimeLineAudio(_director);

        _director.Play();
    }

    async Task LoadTimeLineAudio(PlayableDirector director)
    {
        if (director.playableAsset is TimelineAsset timeLine)
        {
            List<AudioClip> audioList = new();

            foreach (var track in timeLine.GetOutputTracks())
            {
                if (track is AudioTrack audioTrack)
                {
                    foreach (var clip in audioTrack.GetClips())
                    {
                        if (clip.asset is AudioPlayableAsset audioPlayable)
                        {
                            AudioClip audioClip = audioPlayable.clip;

                            if (audioClip != null && !audioList.Contains(audioClip))
                            {
                                audioList.Add(audioClip);
                            }
                        }
                    }
                }
            }

            List<Task> loadTasks = new List<Task>();
            foreach (var clip in audioList)
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

        while (!clip.isReadyToPlay)
        {
            await Task.Yield();
        }
    }
    void Update()
    {

    }
}
