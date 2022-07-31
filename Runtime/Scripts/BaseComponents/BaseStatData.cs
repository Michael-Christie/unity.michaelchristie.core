using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class BaseValueData
{
    public virtual int valueIndex { get; }

    [XmlIgnoreAttribute] public virtual string displayName { get; private set; }

    public virtual float value { get; set; }
}
