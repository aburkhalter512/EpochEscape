
using System.Collections.Generic;
using System.Xml;

using UnityEngine;

using Utilities;

namespace MapEditor
{
	public class StaticWallFactory : IFactory<PlaceableStaticWall>
	{
		public StaticWallFactory()
		{ }

		public override PlaceableStaticWall create(XmlElement element)
		{
			if (element == null)
				return null;

			GameObject prefab;
			GameObject go;
			PlaceableStaticWall retVal = null;
			switch (element.Name)
			{
				case "staticwall":
					prefab = PlaceableStaticWall.getPrefab();
					go = GameObject.Instantiate(prefab);
					retVal = go.GetComponent<PlaceableStaticWall>();

					retVal.onStart = (obj) =>
					{
            			PlaceableStaticWall staticWall = obj as PlaceableStaticWall;
						staticWall.setID(element.GetAttribute("id"));

						Serialization.deserialize(staticWall.transform, element.FirstChild as XmlElement);

						if (!staticWall.place())
							GameObject.Destroy(staticWall.gameObject);
					};
					break;
				case "door":
					XmlElement doorTransformElement = element.FirstChild as XmlElement;
					Vector3 doorTransform = Serialization.toVector3(doorTransformElement.GetAttribute("position"));
					Vector3 doorRotation = Serialization.toVector3(doorTransformElement.GetAttribute("rotation"));
					bool isVertical = (doorRotation.z > 89.5 && doorRotation.z < 90.5) ||
						(doorRotation.z > 269.5 && doorRotation.z < 270.5);

					prefab = PlaceableStaticWall.getPrefab();
					go = GameObject.Instantiate(prefab);
					retVal = go.GetComponent<PlaceableStaticWall>();

					retVal.onStart = (obj) =>
					{
            			PlaceableStaticWall staticWall = obj as PlaceableStaticWall;
						staticWall.setID(element.GetAttribute("id"));

						if (isVertical)
							staticWall.transform.position = new Vector3(doorTransform.x, doorTransform.y + .2f, doorTransform.z);
						else
							staticWall.transform.position = new Vector3(doorTransform.x + .2f, doorTransform.y, doorTransform.z);

						if (!staticWall.place())
							GameObject.Destroy(staticWall.gameObject);
					};

					go = GameObject.Instantiate(prefab);
					retVal = go.GetComponent<PlaceableStaticWall>();

					retVal.onStart = (obj) =>
					{
            			PlaceableStaticWall staticWall = obj as PlaceableStaticWall;
						staticWall.setID(element.GetAttribute("id"));

						if (isVertical)
							staticWall.transform.position = new Vector3(doorTransform.x, doorTransform.y - .2f, doorTransform.z);
						else
							staticWall.transform.position = new Vector3(doorTransform.x - .2f, doorTransform.y, doorTransform.z);

						if (!staticWall.place())
							GameObject.Destroy(staticWall.gameObject);
					};
					break;
			}

			return retVal;
		}
	}
}
