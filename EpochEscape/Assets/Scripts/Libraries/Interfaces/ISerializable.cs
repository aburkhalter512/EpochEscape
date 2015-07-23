using System;
using System.Xml;
using System.Collections;

public interface ISerializable
{
    IEnumerator serialize(XmlDocument doc, Action<XmlElement> callback);
}
