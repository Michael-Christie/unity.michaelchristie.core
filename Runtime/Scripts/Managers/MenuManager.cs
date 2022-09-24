using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A controller that handles menu stuff
/// </summary>
public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    private Stack<MenuBase> menuStack = new Stack<MenuBase>();

    [SerializeField] private MenuBase[] allMenus;

    public MenuBase CurrentMenu
    {
        get
        {
            return menuStack?.Peek() ?? null;
        }
    }

    //
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(this);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)
            && menuStack.Count > 0)
        {
            CurrentMenu?.OnEscHit();
        }
    }

    /// <summary>
    /// Shows a selected menu
    /// </summary>
    /// <param name="_index"></param>
    public void ShowMenu(int _index, Action _onShowComplete = null, Action _onHideComplete = null)
    {
        if (menuStack.Count > 0)
        {
            menuStack.Peek()?.Hide(
                delegate
                {
                    _onHideComplete?.Invoke();

                    MenuBase _otherMenu = allMenus[_index];

                    _otherMenu.Show(_onShowComplete);

                    menuStack.Push(_otherMenu);
                });

            return;
        }

        _onHideComplete?.Invoke();

        MenuBase _newMenu = allMenus[_index];

        _newMenu.Show(_onShowComplete);

        menuStack.Push(_newMenu);
    }

    /// <summary>
    /// Hides the current showing menu
    /// </summary>
    public void HideMenu(Action _onHideComplete = null)
    {
        if (menuStack.Count > 0)
        {
            menuStack.Pop().Hide(
                delegate
                {
                    _onHideComplete?.Invoke();

                    menuStack.Peek()?.Show(null);
                });
            return;
        }

        menuStack.Peek()?.Show(null);
    }

    /// <summary>
    /// Clears the current menu stack and shows a menu after clearing
    /// </summary>
    /// <param name="_index"></param>
    public void ClearStack(int _index)
    {
        if (menuStack.Count > 0)
        {
            menuStack.Pop()?.Hide(null);

            menuStack.Clear();
        }

        MenuBase _newMenu = allMenus[_index];
        _newMenu.Show(null);

        menuStack.Push(_newMenu);
    }

    public MenuBase GetMenuAtIndex(int _index)
    {
        if (_index >= 0 && _index < allMenus.Length)
        {
            return allMenus[_index];
        }
        return null;
    }
}
