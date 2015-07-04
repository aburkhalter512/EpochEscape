using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Utilities
{
    class Serialization
    {
        public static string toString(Vector3 vec)
        {
            return string.Format("({0:0.00}, {1:0.00}, {2:0.00})", vec.x, vec.y, vec.z);
        }
        public static string toString(Vector2 vec)
        {
            return string.Format("({0:0.00}, {1:0.00})", vec.x, vec.y);
        }

        public static Vector2 toVector2(string data)
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
        public static Vector3 toVector3(string data)
        {
            if (data == "" || data.Length < 8)
            {
                Debug.Log("Could not convert [" + data + "] to a Vector3");
                return Vector3.zero;
            }

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

        public static string generateUUID(UnityEngine.Object o)
        {
            UnityEngine.Random.seed = Mathf.RoundToInt(Time.realtimeSinceStartup * 1000);
            string preHash = o.GetType().ToString() + UnityEngine.Random.value;

            StringBuilder hash = new StringBuilder();

            using (MD5 md5 = MD5.Create())
            {
                byte[] hashData = md5.ComputeHash(Encoding.UTF8.GetBytes(preHash));

                for (int i = 0; i < hashData.Length; i++)
                    hash.Append(hashData[i].ToString("x2"));
            }

            return hash.ToString();
        }

        public static T ParseEnum<T>(string toParse)
        {
            return (T)Enum.Parse(typeof(T), toParse);
        }
    }
}