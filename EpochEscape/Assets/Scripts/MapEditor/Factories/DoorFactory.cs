using System.Collections.Generic;
using System.Xml;

using UnityEngine;

using Utilities;

namespace MapEditor
{
	public class DoorFactory : IFactory<PlaceableDoor>
	{
		public SortedDictionary<string, IConnectable> _connectables;
		private SortedDictionary<string, PlaceableTeleporterDoor> _teleporterDoors;

		public DoorFactory()
		{
			_connectables = new SortedDictionary<string, IConnectable>(StringComparer.Get());
			_teleporterDoors = new SortedDictionary<string, PlaceableTeleporterDoor>(StringComparer.Get());
		}

		public override PlaceableDoor create(XmlElement element)
		{
			if (element == null)
				return null;

			GameObject prefab;
			GameObject go;
			PlaceableDoor retVal = null;
			switch (element.GetAttribute("type"))
			{
				case "StandardDoorFrame":
					prefab = PlaceableStandardDoor.getPrefab();
					go = GameObject.Instantiate(prefab);
					retVal = go.GetComponent<PlaceableDoor>();

					retVal.onStart = (obj) =>
					{
            			PlaceableStandardDoor door = obj as PlaceableStandardDoor;
            			door.setID(element.GetAttribute("id"));

						switch (element.GetAttribute("initialState"))
						{
							case "ACTIVE":
								door.activate();
								break;
							case "INACTIVE":
								door.deactivate();
								break;
						}
						Serialization.deserialize(door.transform, element.FirstChild as XmlElement);
						SIDE_4 orientation = toOrientation(door.transform.localEulerAngles.z);
						door.rotateTo(orientation);

						if (!door.place())
							GameObject.Destroy(door.gameObject);
						else
							_connectables.Add(door.getID(), door);
					};
					break;
				case "DirectionalDoorFrame":
					prefab = PlaceableDirectionalDoor.getPrefab();
					go = GameObject.Instantiate(prefab);
					retVal = go.GetComponent<PlaceableDoor>();

					retVal.onStart = (obj) =>
					{
						PlaceableDirectionalDoor door = obj as PlaceableDirectionalDoor;
            			door.setID(element.GetAttribute("id"));

						switch (element.GetAttribute("initialState"))
						{
							case "ACTIVE":
								door.activate();
								break;
							case "INACTIVE":
								door.deactivate();
								break;
						}
						Serialization.deserialize(door.transform, element.FirstChild as XmlElement);
						SIDE_4 orientation = toOrientation(door.transform.localEulerAngles.z);
						door.rotateTo(orientation);

						if (!door.place())
							GameObject.Destroy(door.gameObject);
						else
							_connectables.Add(door.getID(), door);
					};
					break;
				case "EntranceDoorFrame":
					prefab = PlaceableEntranceDoor.getPrefab();
					go = GameObject.Instantiate(prefab);
					retVal = go.GetComponent<PlaceableDoor>();

					retVal.onStart = (obj) =>
					{
						PlaceableEntranceDoor door = obj as PlaceableEntranceDoor;
            			door.setID(element.GetAttribute("id"));

						Serialization.deserialize(door.transform, element.FirstChild as XmlElement);
						SIDE_4 orientation = toOrientation(door.transform.localEulerAngles.z);
						door.rotateTo(orientation);

						if (!door.place())
							GameObject.Destroy(door.gameObject);
					};
	            	break;
				case "CheckpointDoorFrame":
					prefab = PlaceableCheckpointDoor.getPrefab();
					go = GameObject.Instantiate(prefab);
					retVal = go.GetComponent<PlaceableDoor>();

					retVal.onStart = (obj) =>
					{
						PlaceableCheckpointDoor door = obj as PlaceableCheckpointDoor;
            			door.setID(element.GetAttribute("id"));

						Serialization.deserialize(door.transform, element.FirstChild as XmlElement);
						SIDE_4 orientation = toOrientation(door.transform.localEulerAngles.z);
						door.rotateTo(orientation);

						if (!door.place())
							GameObject.Destroy(door.gameObject);
						else
							_connectables.Add(door.getID(), door);
					};
	            	break;
				case "TeleporterDoorFrame":
					prefab = PlaceableTeleporterDoor.getPrefab();
					go = GameObject.Instantiate(prefab);
					retVal = go.GetComponent<PlaceableDoor>();

					retVal.onStart = (obj) =>
					{
						PlaceableTeleporterDoor door = obj as PlaceableTeleporterDoor;
            			door.setID(element.GetAttribute("id"));

						switch (element.GetAttribute("initialState"))
						{
							case "ACTIVE":
								door.activate();
								break;
							case "INACTIVE":
								door.deactivate();
								break;
						}

						Serialization.deserialize(door.transform, element.FirstChild as XmlElement);
						SIDE_4 orientation = toOrientation(door.transform.localEulerAngles.z);
						door.rotateTo(orientation);

						if (!door.place())
							GameObject.Destroy(door.gameObject);
						else
							_connectables.Add(door.getID(), door);

						connect(door, element.GetAttribute("targetID"));
					};
					break;
			}

			return retVal;
		}

		private void connect(PlaceableTeleporterDoor door, string targetID)
		{
			foreach (KeyValuePair<string, PlaceableTeleporterDoor> pair in _teleporterDoors)
			{
				if (pair.Key == door.getID())
					pair.Value.connect(door);

				if (targetID == pair.Value.getID())
					door.connect(pair.Value);
			}

			_teleporterDoors.Add(targetID, door);
		}

		private static SIDE_4 toOrientation(float angle)
		{
			if (315 <= angle || angle < 45)
				return SIDE_4.RIGHT;
			else if (45 <= angle && angle < 135)
				return SIDE_4.TOP;
			else if (135 <= angle && angle < 225)
				return SIDE_4.LEFT;
			else
				return SIDE_4.BOTTOM;
			
		}
	}
}
