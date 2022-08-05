using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseValueTracker : MonoBehaviour
{
    protected virtual BaseValueData[] data { get; }

    //
    public void AddValueToStat(int _statID, float _value)
    {
        if (data.Length - 1 > _statID)
        {
            data[_statID].Value += _value;
            return;
        }

        Debug.Log($"Could not find Stat {_statID} to add value {_value}");
    }

    public void SetValueToStat(int _statID, float _value)
    {
        if (data.Length - 1 > _statID)
        {
            data[_statID].Value = _value;
            return;
        }

        Debug.Log($"Could not find Stat {_statID} to set value {_value}");
    }

    public void SetMaxValueToStat(int _statID, float _value)
    {
        if (data.Length - 1 > _statID)
        {
            data[_statID].Value = Mathf.Max(data[_statID].Value, _value);
            return;
        }

        Debug.Log($"Could not find Stat {_statID} to set max value {_value}");
    }

    /// <summary>
    /// Gets the value of a stat
    /// </summary>
    /// <param name="_stat"></param>
    /// <returns>Returns stat value or -1 if stat not found.</returns>
    public float GetValue(int _statID)
    {
        if (data.Length - 1 > _statID)
        {
            return data[_statID].Value;
        }

        return -1;
    }
}
