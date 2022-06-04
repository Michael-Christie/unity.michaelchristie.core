using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledCardBase : MonoBehaviour
{
    public int Index { get; private set; }

    private RectTransform _rectTransform = null;
    public RectTransform rectTransform
    {
        get
        {
            if(_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            return _rectTransform;
        }
    }

    public bool IsActive
    {
        get
        {
            return gameObject.activeInHierarchy;
        }
    }


    public virtual void UpdateCard(int _index)
    {
        Index = _index;
    }

    public virtual void UpdateContent()
    {

    }
}
