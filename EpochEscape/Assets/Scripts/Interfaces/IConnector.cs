using UnityEngine;

public interface IConnector
{
    void connect(IConnectable obj);
    void disconnect(string id);
}
