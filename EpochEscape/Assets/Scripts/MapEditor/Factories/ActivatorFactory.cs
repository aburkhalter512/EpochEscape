using System.Collections.Generic;
using System.Xml;

using UnityEngine;

using Utilities;

namespace MapEditor
{
	public class ActivatorFactory : IFactory<PlaceableActivator>
	{
		private DoorFactory _doorFactory;

		public ActivatorFactory(DoorFactory doorFactory)
		{
			_doorFactory = doorFactory;
		}

		public override PlaceableActivator create(XmlElement element)
		{
			if (element == null)
				return null;

			GameObject prefab;
			GameObject go;
			PlaceableActivator retVal = null;
			switch (element.GetAttribute("type"))
			{
				case "PressureSwitch":
					prefab = PlaceablePressureSwitch.getPrefab();
					go = GameObject.Instantiate(prefab);
					retVal = go.GetComponent<PlaceableActivator>();

					retVal.onStart = (obj) =>
					{
            			PlaceablePressureSwitch activator = obj as PlaceablePressureSwitch;
            			activator.setID(element.GetAttribute("id"));

        				foreach (XmlNode _child in element.ChildNodes)
        				{
        					XmlElement child = _child as XmlElement;

        					switch (child.Name)
        					{
								case "transform":
									Serialization.deserialize(activator.transform, element.FirstChild as XmlElement);
        							break;
        						case "activatable":
        							connect(activator, child.GetAttribute("id"));
        							break;
        					}
        				}

						if (!activator.place())
							GameObject.Destroy(activator.gameObject);
					};
					break;
				case "PressurePlate":
					prefab = PlaceablePressurePlate.getPrefab();
					go = GameObject.Instantiate(prefab);
					retVal = go.GetComponent<PlaceableActivator>();

					retVal.onStart = (obj) =>
					{
            			PlaceablePressurePlate activator = obj as PlaceablePressurePlate;
            			activator.setID(element.GetAttribute("id"));

        				foreach (XmlNode _child in element.ChildNodes)
        				{
        					XmlElement child = _child as XmlElement;

        					switch (child.Name)
        					{
								case "boxcollider2d":
									Serialization.deserialize(activator.transform, element.FirstChild as XmlElement);
        							break;
        						case "activatable":
        							connect(activator, child.GetAttribute("id"));
        							break;
        					}
        				}

						if (!activator.place())
							GameObject.Destroy(activator.gameObject);
					};
					break;
			}

			return retVal;
		}

		private void connect(PlaceableActivator activator, string targetID)
		{
			IConnectable connectable;
			if (_doorFactory._connectables.TryGetValue(targetID, out connectable))
			{
				activator.connect(connectable);
				return;
			}
		}
	}
}
