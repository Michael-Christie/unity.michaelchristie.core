using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class PooledScrollRect : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private PooledCardBase[] pooledCards;

    private RectTransform thisRect;

    [SerializeField] private Vector2Int elementSize;

    private Vector2 cardSize;

    [SerializeField] private bool isJustShowHide;

    //
    private void Start()
    {
        thisRect = (RectTransform)transform;

        scrollRect.onValueChanged.AddListener(OnScrollRectUpdate);

        cardSize = new Vector2(pooledCards[0].rectTransform.rect.width, pooledCards[0].rectTransform.rect.height);
    }

    private void OnScrollRectUpdate(Vector2 _value)
    {
        //Go through each card and see if its still visible
        for (int i = 0; i < pooledCards.Length; i++)
        {
            if (pooledCards[i].IsActive || isJustShowHide)
            {
                pooledCards[i].gameObject.SetActive(IsCardVisible(pooledCards[i].rectTransform));
            }
        }

        if (isJustShowHide)
        {
            return;
        }

        //See if we should spawn a new card

    }

    [ContextMenu("Initalize")]
    private void Initalize()
    {
        if (isJustShowHide)
        {
            return;
        }

        PooledCardBase _baseCard;

        for (int i = 0; i < elementSize.x; i++)
        {
            for (int j = 0; j < elementSize.y; j++)
            {
                _baseCard = GetNextAvalibleCard();

                _baseCard.transform.localPosition = new Vector2(i * cardSize.x, j * cardSize.y) + (cardSize * 0.5f); //This is good for going left to right but struggles with other directions which I need to flesh it out in.

                _baseCard.gameObject.SetActive(true);
            }
        }

        //I think I need to like work out positions based on the rect size and not based on world space?
        //so maybe look back at gameDevGuides to making UI elements
    }

    private PooledCardBase GetNextAvalibleCard()
    {
        for (int i = 0; i < pooledCards.Length; i++)
        {
            if (!pooledCards[i].IsActive)
            {
                return pooledCards[i];
            }
        }

        Debug.Log("Not Enough cards were found.");

        return null;
    }

    /// <summary>
    /// Returns if the card is currently in the viewport.
    /// </summary>
    /// <param name="_card"></param>
    /// <returns></returns>
    private bool IsCardVisible(RectTransform _card)
    {
        Rect _cardRect = _card.RectRelativeTo(scrollRect.viewport);

        return scrollRect.viewport.rect.Overlaps(_cardRect);
    }
}

public static class ScrollRectExtentions
{
    public static Matrix4x4 TransformTo(this Transform _from, Transform _to)
    {
        return _to.worldToLocalMatrix * _from.localToWorldMatrix;
    }

    public static Rect RectRelativeTo(this RectTransform _transform, Transform _to)
    {
        Matrix4x4 _matrix = _transform.TransformTo(_to);

        Rect _rect = _transform.rect;

        Vector3 _p1 = new Vector2(_rect.xMin, _rect.yMin);
        Vector3 _p2 = new Vector2(_rect.xMax, _rect.yMax);

        _p1 = _matrix.MultiplyPoint(_p1);
        _p2 = _matrix.MultiplyPoint(_p2);

        _rect.xMin = _p1.x;
        _rect.yMin = _p1.y;
        _rect.xMax = _p2.x;
        _rect.yMax = _p2.y;

        return _rect;
    }
}
