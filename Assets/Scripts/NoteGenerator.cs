using System.Collections.Generic;
using UnityEngine;

public class NoteGenerator : MonoBehaviour
{
    [SerializeField] NoteData _data;
    [SerializeField] GameManager _gameManager;
    [SerializeField] GameObject _notePrefab;
    [SerializeField] Canvas _canvas;
    [SerializeField] float _beatTime = 0.10715f;//�m�[�c�̍ŒZ�Ԋu(�b)
    [SerializeField, Header("NoteMove��_duration�Ɠ����l")] float _spawnOffset = 2;
    int _spawnCount = 0;
    void Start()
    {
    }

    void Update()
    {
        if (_gameManager.IsPlayed)
        {
            if (_data.ScoreNum.Length > _spawnCount)
            {
                if ((_data.ScoreNum[_spawnCount] * _beatTime) - _spawnOffset <= _gameManager.GetMusicTime())
                {
                    Debug.LogWarning("Spawn");
                    Instantiate(_notePrefab, _canvas.transform);
                    _spawnCount++;
                }
            }
        }
    }
}
