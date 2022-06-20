using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Flags]
public enum ScrollDirection
{
    Up = 1, 
    Left = 2,
    Down = 4, 
    Right = 8
}

[RequireComponent(typeof(ScrollRect), typeof(UIFrustrumCulling))]
public class PooledScrollRect : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private PooledCardBase[] pooledCards;

    private RectTransform thisRect;

    [SerializeField] private Vector2Int elementSize;

    private Vector2 cardSize;

    [SerializeField] private ScrollDirection scrollDirection;

    //
    private void Start()
    {
        thisRect = (RectTransform)transform;

        scrollRect.onValueChanged.AddListener(OnScrollRectUpdate);

        cardSize = new Vector2(pooledCards[0].rectTransform.rect.width * (scrollDirection.HasFlag(ScrollDirection.Left) ? -1 : 1), 
                                pooledCards[0].rectTransform.rect.height * (scrollDirection.HasFlag(ScrollDirection.Down) ? -1 : 1));

        Initalize();
    }

    private void OnScrollRectUpdate(Vector2 _value)
    {
        CheckCards();
    }

    [ContextMenu("Initalize")]
    private void Initalize()
    {
        PooledCardBase _baseCard;

        for (int i = 0; i < elementSize.x; i++)
        {
            for (int j = 0; j < elementSize.y; j++)
            {
                _baseCard = GetNextAvalibleCard();

                if (_baseCard == null)
                {
                    continue;
                }

                Debug.Log(GetCardPosition(i, j));

                _baseCard.transform.localPosition = GetCardPosition(i, j); //This is good for going left to right but struggles with other directions which I need to flesh it out in.

                Debug.Log(_baseCard.transform.localPosition);

                _baseCard.gameObject.SetActive(true);
            }
        }
    }

    private Vector2 GetCardPosition(int _xPos, int _yPos)
    {
        ///TODO: This needs reformatting to be nicer to look at and read, as well as hopefuly getting ride of the second new Vector2.
        return new Vector2(_xPos * cardSize.x, _yPos * cardSize.y) 
            + new Vector2(cardSize.x * (!scrollDirection.HasFlag(ScrollDirection.Left) && !scrollDirection.HasFlag(ScrollDirection.Right) ? 0 : 0.5f), cardSize.y * (!scrollDirection.HasFlag(ScrollDirection.Up) && !scrollDirection.HasFlag(ScrollDirection.Down) ? 0 : 0.5f));
    }

    private void CheckCards()
    {

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

}
