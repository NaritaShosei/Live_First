using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class NoteManager : MonoBehaviour
{
    [SerializeField] float _duration;
    RectTransform _rectTransform;
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        var a = 745 / _duration;//�n�_���画�胉�C���܂œ��B���鑬�x
        var duration = 890 / a;//�S�̂̏��v����
        _rectTransform.DOAnchorPosX(-445, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
