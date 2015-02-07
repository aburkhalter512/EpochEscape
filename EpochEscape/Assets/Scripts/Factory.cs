using UnityEngine;
using System.Xml;

public abstract class Factory<T> where T : Object
{
    public abstract T create(XmlElement element);
}
