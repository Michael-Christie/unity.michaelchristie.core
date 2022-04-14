using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuBase : MonoBehaviour
{
    [SerializeField] protected GameObject menuPanel;

    protected Action onShowComplete;
    protected Action onHideComplete;

    protected bool isInitalized;

    //
    protected abstract void Initalize();

    protected abstract IEnumerator PlayShowAnimation();
    protected abstract IEnumerator PlayHideAnimation();

    public virtual void Show(Action _onShowComplete)
    {
        if (isInitalized)
        {
            Initalize();
        }

        onShowComplete += _onShowComplete;

        menuPanel.SetActive(true);
    }

    public virtual void Hide(Action _onHideComplete)
    {
        onHideComplete += _onHideComplete;

        menuPanel.SetActive(false);
    }

    public virtual void OnEscHit()
    {

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
}
