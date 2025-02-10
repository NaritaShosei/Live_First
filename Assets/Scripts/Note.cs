using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    [SerializeField] float _duration;
    [SerializeField] RectTransform _rectTransform;
    public bool IsHit;
    [SerializeField] Image _mainImage;
    [SerializeField] Image _subImage;
    [SerializeField] Sprite _perfectSprite;
    [SerializeField] Sprite _goodSprite;
    [SerializeField] Sprite _missSprite;
    private void Start()
    {
        var a = 745 / _duration;//始点から判定ラインまで到達する速度
        var duration = 890 / a;//全体の所要時間
        _rectTransform.DOAnchorPosX(-445, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    public void ChangeImage(HitType type)
    {
        _subImage.enabled = false;
        Sprite sprite = type switch
        {
            HitType.perfect => _perfectSprite,
            HitType.good => _goodSprite,
            HitType.miss => _missSprite,
        };
        _mainImage.sprite = sprite;
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
