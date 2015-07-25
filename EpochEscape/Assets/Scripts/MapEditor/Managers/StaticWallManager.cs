using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Utilities;

namespace MapEditor
{
    public class StaticWallManager : ISerializable
    {
        #region Interface Variables
        #endregion

        #region Instance Variables
        SortedList<Vec2Int, PlaceableStaticWall> mStaticWalls;

        static StaticWallManager mInstance;
        static Map _map;
        #endregion

        private StaticWallManager()
        {
            mStaticWalls = new SortedList<Vec2Int, PlaceableStaticWall>(Vec2IntComparer.Get());

            _map = Map.Get();
        }

        #region Interface Methods
        public static StaticWallManager Get()
        {
            if (mInstance == null)
                mInstance = new StaticWallManager();

            return mInstance;
        }

        public void add(PlaceableStaticWall wall)
        {
            if (wall == null || wall.bottomLeft == null)
                return;

            Vec2Int key = wall.bottomLeft.position();

            PlaceableStaticWall searcher;
            if (!mStaticWalls.TryGetValue(key, out searcher))
                mStaticWalls.Add(key, wall);
        }

        public void remove(Utilities.Vec2Int key)
        {
            if (key == null)
                return;

            mStaticWalls.Remove(key);
        }

        public IEnumerator serialize(XmlDocument doc, Action<XmlElement> callback)
        {
            if (doc == null || callback == null)
                yield break;

            XmlElement staticWalls = doc.CreateElement("staticwalls");

            Vector2 tileSize = Map.tileSize;

            Utilities.Vec2Int prevPos = null;

            Vector2 colliderPosition = Vector2.zero;
            int colliderHeight = 0;

            XmlElement boxcollider2d;

            foreach (KeyValuePair<Vec2Int, PlaceableStaticWall> pair in mStaticWalls)
            {
                if (prevPos == null)
                {
                    colliderHeight = 1;
                    prevPos = pair.Key;

                    colliderPosition = new Vector2(pair.Key.x * tileSize.x,
                        pair.Key.y * tileSize.y);
                    colliderPosition += tileSize;
                }
                else if (prevPos.x != pair.Key.x || prevPos.y + 2 != pair.Key.y)
                {
                    boxcollider2d = doc.CreateElement("boxcollider2d");
                    boxcollider2d.SetAttribute("size", new Vector2(tileSize.x * 2, tileSize.y * 2 * colliderHeight).ToString());
                    boxcollider2d.SetAttribute("center", colliderPosition.ToString());

                    staticWalls.AppendChild(boxcollider2d);

                    colliderHeight = 1;
                    prevPos = pair.Key;

                    colliderPosition = new Vector2(pair.Key.x * tileSize.x,
                        pair.Key.y * tileSize.y);
                    colliderPosition += tileSize;
                }
                else
                {
                    colliderHeight++;

                    colliderPosition.y += tileSize.y;

                    prevPos = pair.Key;
                }
            }

            boxcollider2d = doc.CreateElement("boxcollider2d");
            boxcollider2d.SetAttribute("size", new Vector2(tileSize.x * 2, tileSize.y * 2 * colliderHeight).ToString());
            boxcollider2d.SetAttribute("center", colliderPosition.ToString());

            staticWalls.AppendChild(boxcollider2d);

            callback(staticWalls);
        }
        #endregion

        #region Instance Methods
        #endregion
    }
}
