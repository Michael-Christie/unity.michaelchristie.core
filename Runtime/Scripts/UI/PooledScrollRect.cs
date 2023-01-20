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

    [SerializeField] private MC.Core.GameUtilities.Sides margin;
    [SerializeField] private MC.Core.GameUtilities.Sides padding;

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

    /// <summary>
    /// The scroll rect callback
    /// </summary>
    /// <param name="_value"></param>
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

    /// <summary>
    /// On Card Remove callback from UIFrustrumCulling. Indicates when a card has been removed
    /// </summary>
    /// <param name="_index"></param>
    private void OnCardRemove(int _index)
    {
        Vector2Int _cardPos = IndexToCardPos(_index);

        //Update the min/max values so we know what index to change
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

    /// <summary>
    /// Initalize the scroll rect with the cards for its current position
    /// </summary>
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

    /// <summary>
    /// Get the position of the card
    /// </summary>
    /// <param name="_xPos"></param>
    /// <param name="_yPos"></param>
    /// <returns>UI space position</returns>
    private Vector2 GetCardPosition(int _xPos, int _yPos)
    { 
        return new Vector2((margin.horizontal * (scrollDirection.Equals(ScrollDirection.Left) ? 1 : -1)) + _xPos * cardSize.x + cardSize.x * (!scrollDirection.HasFlag(ScrollDirection.Left) && !scrollDirection.HasFlag(ScrollDirection.Right) ? 0 : 0.5f) + (padding.horizontal * (scrollDirection.Equals(ScrollDirection.Left) ? -1 : 1) * _xPos),
                            (margin.vertical * (scrollDirection.Equals(ScrollDirection.Left) ? 1 : -1)) + _yPos * cardSize.y + cardSize.y * (!scrollDirection.HasFlag(ScrollDirection.Up) && !scrollDirection.HasFlag(ScrollDirection.Down) ? 0 : 0.5f) + (padding.vertical * (scrollDirection.Equals(ScrollDirection.Down) ? -1 : 1) * _yPos));
    }

    /// <summary>
    /// Check the cards to see if a new card can be spawned
    /// </summary>
    private void CheckCards()
    {
        //based upon the moveDirection
        //try and spawn a card in that direction...
        if (userInput.HasFlag(ScrollDirection.Down) && maxIndex.y + 1 < elementSize.y)
        {
            for(int i = 0; i < elementSize.x; i++)
            {
                AddCard(maxIndex.x + i, maxIndex.y + 1);
            }
        }
        else if (userInput.HasFlag(ScrollDirection.Up) && minIndex.y - 1 >= 0)
        {
            for (int i = 0; i < elementSize.x; i++)
            {
                AddCard(minIndex.x + i, minIndex.y - 1);
            }
        }

        if (userInput.HasFlag(ScrollDirection.Right) && maxIndex.x + 1 < elementSize.x)
        {
            for (int i = 0; i < elementSize.y; i++)
            {
                AddCard(maxIndex.x + 1, maxIndex.y + i);
            }
        }
        else if (userInput.HasFlag(ScrollDirection.Left) && minIndex.x - 1 >= 0)
        {
            for (int i = 0; i < elementSize.y; i++)
            {
                AddCard(minIndex.x - 1, minIndex.y + i);
            }
        }
    }

    /// <summary>
    /// Add a card in its X and Y pos
    /// </summary>
    /// <param name="_xPos"></param>
    /// <param name="_yPos"></param>
    private void AddCard(int _xPos, int _yPos)
    {
        cachedBaseCard = culler.GetNextAvalibleCard();

        if (cachedBaseCard == null)
        {
            return;
        }

        cachedBaseCard.transform.localPosition = GetCardPosition(_xPos, _yPos);

        if (!culler.IsCardVisible(cachedBaseCard.rectTransform))
        {
            return;
        }

        CheckIndexIntoMinMax(_xPos, _yPos);

        cachedBaseCard.UpdateCard(CardPosToIndex(_xPos, _yPos));

        cachedBaseCard.gameObject.SetActive(true);
    }

    /// <summary>
    /// Check to see if the min/max values should change based on this card pos
    /// </summary>
    /// <param name="_xPos"></param>
    /// <param name="_yPos"></param>
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

    /// <summary>
    /// Convert 1D index to 2D
    /// </summary>
    /// <param name="_index"></param>
    /// <returns></returns>
    private Vector2Int IndexToCardPos(int _index)
    {
        return new Vector2Int(_index % elementSize.x, _index / elementSize.x);
    }

    /// <summary>
    /// Convert 2D index to 1D
    /// </summary>
    /// <param name="_pos"></param>
    /// <returns></returns>
    private int CardPosToIndex(Vector2Int _pos)
    {
        return CardPosToIndex(_pos.x, _pos.y);
    }

    /// <summary>
    /// Convert 2D index to 1D
    /// </summary>
    /// <param name="_xPos"></param>
    /// <param name="_yPos"></param>
    /// <returns></returns>
    private int CardPosToIndex(int _xPos, int _yPos)
    {
        return _xPos * elementSize.y + _yPos;
    }
}
