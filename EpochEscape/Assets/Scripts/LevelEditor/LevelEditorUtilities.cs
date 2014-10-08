using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Security.Cryptography;

public class LevelEditorUtilities
{
    public static string Tab(int n)
    {
        return new string('\t', n);
    }
    
    public static string Escape<T>(T obj)
    {
        return "\"" + obj.ToString() +"\"";
    }

    public static void PrintVector(StreamWriter sw, Vector2 vector, int t)
    {
        if(vector == null) return;
        
        sw.WriteLine(Tab(t) + Escape("x") + ":" + vector.x + ",");
        sw.WriteLine(Tab(t) + Escape("y") + ":" + vector.y);
    }
    
    public static void PrintVector(StreamWriter sw, Vector3 vector, int t)
    {
        if(vector == null) return;
        
        sw.WriteLine(Tab(t) + Escape("x") + ":" + vector.x + ",");
        sw.WriteLine(Tab(t) + Escape("y") + ":" + vector.y + ",");
        sw.WriteLine(Tab(t) + Escape("z") + ":" + vector.z);
    }

    public static Vector2 StringToVector2(string data)
    {
        string[] vectorComponents = null;
        Vector2 vector = Vector2.zero;

        data = data.Substring(1, data.Length - 2); // Trim paranthesis.

        vectorComponents = data.Split(','); // Get each component.

        // If the number of components is less than 2, then the string is corrupt.
        if (vectorComponents.Length < 2)
            return vector;

        // Trim any whitespace.
        vectorComponents[0] = vectorComponents[0].Trim();
        vectorComponents[1] = vectorComponents[1].Trim();

        // Parse the results.
        vector.x = float.Parse(vectorComponents[0]);
        vector.y = float.Parse(vectorComponents[1]);

        return vector;
    }

    public static Vector3 StringToVector3(string data)
    {
        string[] vectorComponents = null;
        Vector3 vector = Vector3.zero;

        data = data.Substring(1, data.Length - 2); // Trim paranthesis.

        vectorComponents = data.Split(','); // Get each component.

        // If the number of components is less than 3, then the string is corrupt.
        if (vectorComponents.Length < 3)
            return vector;
        
        // Trim any whitespace.
        vectorComponents[0] = vectorComponents[0].Trim();
        vectorComponents[1] = vectorComponents[1].Trim();
        vectorComponents[2] = vectorComponents[2].Trim();

        // Parse the results.
        vector.x = float.Parse(vectorComponents[0]);
        vector.y = float.Parse(vectorComponents[1]);
        vector.z = float.Parse(vectorComponents[2]);

        return vector;
    }

    public static string GenerateObjectHash(string name, Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x * 10);
        int y = Mathf.FloorToInt(position.y * 10);

        string preHash = name + x.ToString() + y.ToString();

        StringBuilder hash = new StringBuilder();

        using (MD5 md5 = MD5.Create())
        {
            byte[] hashData = md5.ComputeHash(Encoding.UTF8.GetBytes(preHash));

            for (int i = 0; i < hashData.Length; i++)
                hash.Append(hashData[i].ToString("x2"));
        }

        return hash.ToString();
    }
}
