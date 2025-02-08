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
        var a = 745 / _duration;//始点から判定ラインまで到達する速度
        var duration = 890 / a;//全体の所要時間
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
