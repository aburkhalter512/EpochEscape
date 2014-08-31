using UnityEngine;
using System.Collections;
using System.IO;

public class LevelEditorUtilities
{
    public static string Tab(int n)
    {
        return new string('\t', n);
    }
    
    public static string Escape(string str)
    {
        return "\"" + str + "\"";
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
}
