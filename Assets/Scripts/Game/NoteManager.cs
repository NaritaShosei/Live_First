using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    [SerializeField] NoteData _data;
    [SerializeField] GameManager _gameManager;
    [SerializeField] GameObject _notePrefab;
    [SerializeField] Canvas _canvas;
    [SerializeField] float _beatTime = 0.10715f;//ノーツの最短間隔(秒)
    [SerializeField, Header("Noteの_durationと同じ値")] float _spawnOffset = 2;
    [SerializeField]
    AudioClip _audio;
    [SerializeField]
    AudioSource _audioSource;
    int _spawnCount = 0;
    List<(float time, Note note)> _notes = new();
    void Start()
    {
        foreach (var note in _data.ScoreNum)
        {
            _notes.Add((note * _beatTime, null));
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
                    _notes[_spawnCount] = (_notes[_spawnCount].time, note.GetComponent<Note>());
                    _spawnCount++;
                }
            }
            //成功判定
            foreach (var note in _notes)
            {
                if (note.note != null)
                {
                    if (!note.note.IsHit && Mathf.Abs((float)musicTime - note.time) <= 0.25f)
                    {
                        if (_gameManager.InputButton())
                        {
                            _audioSource.PlayOneShot(_audio);
                            var type = _gameManager.CheckHit(note.time);
                            note.note.ChangeImage(type);
                            note.note.IsHit = true;
                            break;
                        }
                    }
                }
            }
            //おせなかったノーツの判定
            foreach (var note in _notes)
            {
                if (note.note != null)
                {
                    if (!note.note.IsHit && musicTime > note.time + 0.25f)
                    {
                        Debug.Log("Miss");
                        note.note.ChangeImage(HitType.miss);
                        _gameManager.MissHit();
                        note.note.IsHit = true;
                    }
                }
            }
        }
    }
}
