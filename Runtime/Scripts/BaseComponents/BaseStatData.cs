using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class BaseValueData
{
    [XmlIgnoreAttribute] public virtual int ValueIndex { get; }

    [XmlIgnoreAttribute] public virtual string DisplayName { get; set; }

    public virtual float Value { get; set; }
}
