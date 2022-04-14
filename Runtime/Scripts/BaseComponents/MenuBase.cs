using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBase : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;

    //
    public virtual void Show()
    {
        menuPanel.SetActive(true);
    }

    public virtual void Hide()
    {
        menuPanel.SetActive(false);
    }

    public virtual void OnEscHit()
    {

    }
}
