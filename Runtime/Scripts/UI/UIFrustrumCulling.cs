using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class UIFrustrumCulling : MonoBehaviour
{
    private ScrollRect scrollRect;

    [SerializeField] private PooledCardBase[] pooledCards;

    public PooledCardBase[] PooledCards { get { return pooledCards; } }

    private RectTransform thisRect;

    [SerializeField] private bool canShow;

    public Action<int> OnCardRemoved;

    [SerializeField] private bool canSpawnCards;

    [SerializeField] private PooledCardBase templateCard;

    //
    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();

        if(pooledCards.Length == 0 && templateCard)
        {
            pooledCards = new PooledCardBase[10];

            for(int i = 0; i < 10; i++)
            {
                pooledCards[i] = Instantiate(templateCard, templateCard.transform.parent);
            }
        }
    }

    private void Start()
    {
        thisRect = (RectTransform)transform;

        scrollRect.onValueChanged.AddListener(OnScrollRectUpdate);

        OnScrollRectUpdate(Vector2.zero);
    }

    private void OnScrollRectUpdate(Vector2 _value)
    {
        //Go through each card and see if its still visible
        for (int i = 0; i < pooledCards.Length; i++)
        {
            if (pooledCards[i].IsActive || canShow)
            {
                if (!IsCardVisible(pooledCards[i].rectTransform))
                {
                    OnCardRemoved?.Invoke(pooledCards[i].Index);
                }
                pooledCards[i].gameObject.SetActive(IsCardVisible(pooledCards[i].rectTransform));
            }
        }
    }

    /// <summary>
    /// Returns if the card is currently in the viewport.
    /// </summary>
    /// <param name="_card"></param>
    /// <returns></returns>
    public bool IsCardVisible(RectTransform _card)
    {
        Rect _cardRect = _card.RectRelativeTo(scrollRect.viewport);

        return scrollRect.viewport.rect.Overlaps(_cardRect);
    }

    /// <summary>
    /// Return the next avaliable pooled card
    /// </summary>
    /// <returns></returns>
    public PooledCardBase GetNextAvalibleCard()
    {
        for (int i = 0; i < pooledCards.Length; i++)
        {
            if (!pooledCards[i].IsActive)
            {
                return pooledCards[i];
            }
        }

        if (canSpawnCards)
        {
            PooledCardBase _newCard = Instantiate(templateCard, templateCard.transform.parent);

            Array.Resize(ref pooledCards, pooledCards.Length + 1);
            pooledCards[pooledCards.Length - 1] = _newCard;

            return _newCard;
        }
        else
        {
            Debug.Log("Not Enough cards were found.");

            return null;
        }
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