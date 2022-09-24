using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoreExtentionMethods
{
    /// <summary>
    /// Is T Equal to all of the critera passed in
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_component"></param>
    /// <param name="_params"></param>
    /// <returns></returns>
    public static bool Equals<T>(this T _component, params T[] _criteria)
    {
        for (int i = 0; i < _criteria.Length; i++)
        {
            if (!_component.Equals(_criteria[i]))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Does T equal one of the criteria passed to it
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_component"></param>
    /// <param name="_params"></param>
    /// <returns></returns>
    public static bool Contains<T>(this T _component, params T[] _criteria)
    {
        for (int i = 0; i < _criteria.Length; i++)
        {
            if (_component.Equals(_criteria[i]))
            {
                return true;
            }
        }

        return false;
    }

    public static bool CompareTags(this Component _component, params string[] _criteria)
    {
        for(int i = 0; i < _criteria.Length; i++)
        {
            if (_component.CompareTag(_criteria[i]))
            {
                return true;
            }
        }

        return false;
    }

    public static float ConvertFloatToDecibels(this float _value)
    {
        if (_value == 0)
            return -144.0f;
        return 20 * Mathf.Log10(_value);
    }

    public static float ConvertDecibelsToFloat(this float _value)
    {
        return Mathf.Pow(10.0f, _value / 20.0f);
    }
}

