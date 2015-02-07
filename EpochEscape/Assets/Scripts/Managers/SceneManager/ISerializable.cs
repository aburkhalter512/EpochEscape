using System.Xml;

public interface ISerializable
{
    XmlElement Serialize(XmlDocument document);
}
