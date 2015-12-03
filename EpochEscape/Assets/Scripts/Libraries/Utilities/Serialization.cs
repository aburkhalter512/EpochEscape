using UnityEngine;
using UnityEditor;
using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Utilities
{
    public class Serialization
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
            if (vectorComponents.Length != 2)
                return vector;

            // Trim any whitespace.
            vectorComponents[0] = vectorComponents[0].Trim();
            vectorComponents[1] = vectorComponents[1].Trim();

            // Parse the results.
            try
            {
                vector.x = float.Parse(vectorComponents[0]);
                vector.y = float.Parse(vectorComponents[1]);
            }
            catch (FormatException e)
            {
                Debug.LogError(e.Message);

                return Vector2.zero;
            }

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
            if (vectorComponents.Length != 3)
                return vector;

            // Trim any whitespace.
            vectorComponents[0] = vectorComponents[0].Trim();
            vectorComponents[1] = vectorComponents[1].Trim();
            vectorComponents[2] = vectorComponents[2].Trim();

            // Parse the results.
            try
            {
                vector.x = float.Parse(vectorComponents[0]);
                vector.y = float.Parse(vectorComponents[1]);
                vector.z = float.Parse(vectorComponents[2]);
            }
            catch (FormatException e)
            {
                Debug.LogError(e.Message);

                return Vector3.zero;
            }

            return vector;
        }

        public static string toString(Color color)
        {
            return string.Format("({0:0.00}, {1:0.00}, {2:0.00}, {3:0.00})", color.r, color.g, color.b, color.a);
        }

        public static Color toColor(string data)
        {
            string[] components = null;
            Color color = Color.black;

            data = data.Substring(1, data.Length - 2); // Trim paranthesis.

            components = data.Split(','); // Get each component.

            // If the number of components is less than 2, then the string is corrupt.
            if (components.Length != 4)
                return color;

            // Trim any whitespace.
            components[0] = components[0].Trim();
            components[1] = components[1].Trim();
            components[2] = components[2].Trim();
            components[3] = components[3].Trim();

            // Parse the results.
            try
            {
                color.r = float.Parse(components[0]);
                color.g = float.Parse(components[1]);
                color.b = float.Parse(components[2]);
                color.a = float.Parse(components[3]);
            }
            catch (FormatException e)
            {
                Debug.LogError(e.Message);

                return Color.black;
            }

            return color;
        }

        public static string generateUUID(UnityEngine.Object o)
        {
            UnityEngine.Random.seed = Mathf.RoundToInt(Time.realtimeSinceStartup * 1000);
            string preHash = o.GetType().ToString() + UnityEngine.Random.value.ToString();

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

        public static XmlElement toXML(Transform transform, XmlDocument document)
        {
            XmlElement element = document.CreateElement("transform");
            element.SetAttribute("position", transform.position.ToString());
            element.SetAttribute("rotation", transform.eulerAngles.ToString());
            element.SetAttribute("localscale", transform.localScale.ToString());

            return element;
        }
        public static void deserialize(Transform transform, XmlElement element)
        {
            if (element.Name != "transform" || transform == null)
                return;

            transform.position = toVector3(element.GetAttribute("position"));
            transform.eulerAngles = toVector3(element.GetAttribute("rotation"));
            transform.localScale = toVector3(element.GetAttribute("localscale"));
        }

        public static XmlElement toXML(SpriteRenderer sr, XmlDocument document)
        {
            XmlElement element = document.CreateElement("spriterenderer");
            element.SetAttribute("sprite", AssetDatabase.GetAssetPath(sr.sprite).Remove(0, 17)); //Removes Asset/Resources/
            element.SetAttribute("sortingOrder", sr.sortingOrder.ToString());

            return element;
        }
        public static void deserialize(SpriteRenderer sr, XmlElement element)
        {
            if (element.Name != "spriterenderer" || sr == null)
                return;

            sr.sprite = Resources.Load<Sprite>(element.GetAttribute("sprite"));
            sr.sortingOrder = Convert.ToInt32(element.GetAttribute("sortingOrder"));
        }

        public static XmlElement toXML(BoxCollider2D collider, XmlDocument document)
        {
            XmlElement element = document.CreateElement("boxcollider2d");
            element.SetAttribute("size", collider.size.ToString());
            element.SetAttribute("center", collider.offset.ToString());

            return element;
        }
        public static void deserialize(BoxCollider2D collider, XmlElement element)
        {
            if (element.Name != "boxcollider2d" || collider == null)
                return;

            collider.size = toVector2(element.GetAttribute("size"));
            collider.offset = toVector2(element.GetAttribute("center"));
        }
    }
}