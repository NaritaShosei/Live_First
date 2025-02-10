using DG.Tweening.Core.Easing;
using System.Collections.Generic;
using UnityEngine;
using static NoteData;

public class NoteManager : MonoBehaviour
{
    [SerializeField] NoteData _data;
    [SerializeField] GameManager _gameManager;
    [SerializeField] GameObject _notePrefab;
    [SerializeField] Canvas _canvas;
    [SerializeField] float _beatTime = 0.10715f;//ノーツの最短間隔(秒)
    [SerializeField, Header("NoteMoveの_durationと同じ値")] float _spawnOffset = 2;
    int _spawnCount = 0;
    Dictionary<float, NoteMove> _notes = new();
    void Start()
    {
        foreach (var note in _data.ScoreNum)
        {
            _notes.Add(note * _beatTime, null);
        }
    }

    void Update()
    {
        if (_gameManager.IsPlayed)
        {
            var musicTime = _gameManager.GetMusicTime();
            if (_data.ScoreNum.Length > _spawnCount)
            {
                var targetTime = _data.ScoreNum[_spawnCount] * _beatTime;
                if (targetTime - _spawnOffset <= musicTime)
                {
                    Debug.LogWarning("Spawn");
                    var note = Instantiate(_notePrefab, _canvas.transform);
                    _notes[targetTime] = note.GetComponent<NoteMove>();
                    _spawnCount++;
                }
            }
            //成功判定
            foreach (var note in _notes)
            {
                if (note.Value != null)
                {
                    if (!note.Value.IsHit && Mathf.Abs((float)musicTime - note.Key) <= 0.25f)
                    {
                        if (_gameManager.InputButton())
                        {
                            _gameManager.CheckHit(note.Key);

                            note.Value.IsHit = true; 
                            break; 
                        }
                    }
                }
            }
            //おせなかったノーツの判定
            foreach (var note in _notes)
            {
                if (note.Value != null)
                {
                    if (!note.Value.IsHit && musicTime > note.Key + 0.25f)
                    {
                        Debug.Log("Miss");
                        note.Value.IsHit = true; 
                    }
                }
            }
        }
    }
}
