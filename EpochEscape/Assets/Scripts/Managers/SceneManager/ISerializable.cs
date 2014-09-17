using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public interface ISerializable
{
    void Serialize(ref Dictionary<string, object> data);
    void Unserialize(ref Dictionary<string, object> data);
}
