using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptimisedToggle : Toggle
{
    protected override void Start()
    {
        base.Start();

        onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(bool _value)
    {
        targetGraphic.enabled = !_value;
        graphic.enabled = _value;
    }
}
