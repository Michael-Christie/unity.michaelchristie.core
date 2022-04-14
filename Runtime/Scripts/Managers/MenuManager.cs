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
            return menuStack.Peek() ?? null;
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
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CurrentMenu?.OnEscHit();
        }
    }

    /// <summary>
    /// Shows a selected menu
    /// </summary>
    /// <param name="_index"></param>
    public void ShowMenu(int _index)
    {
        if (menuStack.Count > 0)
        {
            menuStack.Peek()?.Hide();
        }

        MenuBase _newMenu = allMenus[_index];

        _newMenu.Show();

        menuStack.Push(_newMenu);
    }

    /// <summary>
    /// Hides the current showing menu
    /// </summary>
    public void HideMenu()
    {
        if (menuStack.Count > 0)
        {
            menuStack.Pop().Hide();
        }

        menuStack.Peek()?.Show();
    }

    /// <summary>
    /// Clears the current menu stack and shows a menu after clearing
    /// </summary>
    /// <param name="_index"></param>
    public void ClearStack(int _index)
    {
        if (menuStack.Count > 0)
        {
            menuStack.Pop()?.Hide();

            menuStack.Clear();
        }

        MenuBase _newMenu = allMenus[_index];
        _newMenu.Show();

        menuStack.Push(_newMenu);
    }
}
