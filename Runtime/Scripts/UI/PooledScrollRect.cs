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
    private ScrollRect scrollRect;

    private RectTransform thisRect;

    [SerializeField] private Vector2Int elementSize;

    private Vector2 cardSize;
    private Vector2 lastKnowPos;

    [SerializeField] private ScrollDirection scrollDirection;
    private ScrollDirection userInput;

    private UIFrustrumCulling culler;

    private Vector2Int minIndex = new Vector2Int(-1, -1);
    private Vector2Int maxIndex = new Vector2Int(-1, -1);

    private PooledCardBase cachedBaseCard;

    //
    private void Awake()
    {
        culler = GetComponent<UIFrustrumCulling>();
        scrollRect = GetComponent<ScrollRect>();
    }

    private void Start()
    {
        thisRect = (RectTransform)transform;

        scrollRect.onValueChanged.AddListener(OnScrollRectUpdate);

        cardSize = new Vector2(culler.PooledCards[0].rectTransform.rect.width * (scrollDirection.HasFlag(ScrollDirection.Left) ? -1 : 1),
                                culler.PooledCards[0].rectTransform.rect.height * (scrollDirection.HasFlag(ScrollDirection.Down) ? -1 : 1));

        culler.OnCardRemoved += OnCardRemove;

        Initalize();
    }

    private void OnScrollRectUpdate(Vector2 _value)
    {
        if ((_value - lastKnowPos).magnitude != 0)
        {
            userInput = 0;

            if (lastKnowPos.x - _value.x > 0)
            {
                userInput |= ScrollDirection.Left;
            }
            else if(lastKnowPos.x - _value.x < 0)
            {
                userInput |= ScrollDirection.Right;
            }

            if (lastKnowPos.y - _value.y > 0)
            {
                userInput |= ScrollDirection.Down;
            }
            else if (lastKnowPos.y - _value.y < 0)
            {
                userInput |= ScrollDirection.Up;
            }

            CheckCards();
            lastKnowPos = _value;
        }
    }

    private void OnCardRemove(int _index)
    {
        Vector2Int _cardPos = IndexToCardPos(_index);

        Debug.Log($"Removed Card {_index} at pos {_cardPos}");

        //Update the min/max
        if(minIndex.x == _cardPos.x && userInput.HasFlag(ScrollDirection.Right))
        {
            minIndex.x++;
        }
        else if(maxIndex.x == _cardPos.x && userInput.HasFlag(ScrollDirection.Left))
        {
            maxIndex.x--;
        }

        if (minIndex.y == _cardPos.y && userInput.HasFlag(ScrollDirection.Down))
        {
            minIndex.y++;
        }
        else if (maxIndex.y == _cardPos.y && userInput.HasFlag(ScrollDirection.Up))
        {
            maxIndex.y--;
        }
    }

    [ContextMenu("Initalize")]
    private void Initalize()
    {
        lastKnowPos = scrollRect.normalizedPosition;

        for (int i = 0; i < elementSize.x; i++)
        {
            for (int j = 0; j < elementSize.y; j++)
            {
                AddCard(i, j);
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
        //based upon the moveDirection
        //try and spawn a card in that direction...
        if (userInput.HasFlag(ScrollDirection.Down))
        {
            for(int i = 0; i < elementSize.x; i++)
            {
                AddCard(maxIndex.x + i, maxIndex.y + 1);
            }
        }
        else if (userInput.HasFlag(ScrollDirection.Up))
        {
            for (int i = 0; i < elementSize.x; i++)
            {
                AddCard(minIndex.x + i, minIndex.y - 1);
            }
        }

        if (userInput.HasFlag(ScrollDirection.Right))
        {
            for (int i = 0; i < elementSize.y; i++)
            {
                AddCard(maxIndex.x + 1, maxIndex.y + i);
            }
        }
        else if (userInput.HasFlag(ScrollDirection.Left))
        {
            for (int i = 0; i < elementSize.y; i++)
            {
                AddCard(minIndex.x - 1, minIndex.y + i);
            }
        }
    }

    private void AddCard(int _xPos, int _yPos)
    {
        cachedBaseCard = GetNextAvalibleCard();

        if (cachedBaseCard == null)
        {
            return;
        }

        cachedBaseCard.transform.localPosition = GetCardPosition(_xPos, _yPos); //This is good for going left to right but struggles with other directions which I need to flesh it out in.

        if (!culler.IsCardVisible(cachedBaseCard.rectTransform))
        {
            return;
        }

        CheckIndexIntoMinMax(_xPos, _yPos);

        cachedBaseCard.UpdateCard(CardPosToIndex(_xPos, _yPos));

        cachedBaseCard.gameObject.SetActive(true);
    }

    private void CheckIndexIntoMinMax(int _xPos, int _yPos)
    {
        if(minIndex.x == -1 || minIndex.x > _xPos)
        {
            minIndex.x = _xPos;
        }

        if(minIndex.y == -1 || minIndex.y > _yPos)
        {
            minIndex.y = _yPos;
        }

        if (maxIndex.x == -1 || maxIndex.x < _xPos)
        {
            maxIndex.x = _xPos;
        }

        if (maxIndex.y == -1 || maxIndex.y < _yPos)
        {
            maxIndex.y = _yPos;
        }
    }

    private PooledCardBase GetNextAvalibleCard()
    {
        for (int i = 0; i < culler.PooledCards.Length; i++)
        {
            Debug.Log(culler.PooledCards[i].IsActive);

            if (!culler.PooledCards[i].IsActive)
            {
                return culler.PooledCards[i];
            }
        }

        Debug.Log("Not Enough cards were found.");

        return null;
    }

    private Vector2Int IndexToCardPos(int _index)
    {
        return new Vector2Int(_index % elementSize.x, _index / elementSize.x);
    }

    private int CardPosToIndex(Vector2Int _pos)
    {
        return CardPosToIndex(_pos.x, _pos.y);
    }

    private int CardPosToIndex(int _xPos, int _yPos)
    {
        return _xPos * elementSize.y + _yPos;
    }
}
