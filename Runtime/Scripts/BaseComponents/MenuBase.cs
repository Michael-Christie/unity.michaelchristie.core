using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuBase : MonoBehaviour
{
    [SerializeField] protected GameObject menuPanel;

    private Action onShowComplete;
    private Action onHideComplete;

    protected bool isInitalized;

    //
    protected abstract void Initalize();

    public virtual void Show(Action _onShowComplete)
    {
        if (!isInitalized)
        {
            Initalize();
            isInitalized = true;
        }

        onShowComplete += _onShowComplete;
    }

    public virtual void Hide(Action _onHideComplete)
    {
        onHideComplete += _onHideComplete;
    }

    public virtual void OnEscHit()
    {
        //Left blank so that a. you don't have to implement it and b. you can override it in some menus but not others.
    }

    protected void OnShowComplete()
    {
        onShowComplete?.Invoke();
        onShowComplete = null;
    }

    protected void OnHideComplete()
    {
        onHideComplete?.Invoke();
        onHideComplete = null;
    }

    protected virtual IEnumerator PlayShowAnimation()
    {
        throw new NotImplementedException();
    }

    protected virtual IEnumerator PlayHideAnimation()
    {
        throw new NotImplementedException();
    }
}
