using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

//使わないかも
public class QuickTimeEvent : MonoBehaviour
{
    Queue<float> _eventQueue = new();
    [SerializeField] TimelineAsset _timelineAsset;
    [SerializeField] PlayableDirector _playableDirector;
    [SerializeField, Header("ノーツのトラックの名前")] string _trackName;
    [SerializeField, Header("判定の猶予時間")] float _graceTime = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var track in _timelineAsset.GetOutputTracks())
        {
            foreach (var clip in track.GetClips())
            {
                Debug.Log(clip.displayName);
                if (_trackName == clip.displayName)
                {
                    _eventQueue.Enqueue((float)clip.start);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_eventQueue.Count > 0)
        {
            if (_playableDirector.time <= _eventQueue.Peek() - _graceTime)
            {

            }
            if (_playableDirector.time >= _eventQueue.Peek() + _graceTime)
            {
                _eventQueue.Dequeue();
            }
        }
    }
}
