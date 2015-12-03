using UnityEngine;
using System.Xml;

public abstract class IFactory<T> where T : Object
{
    public abstract T create(XmlElement element);
}
