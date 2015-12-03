using System.Collections.Generic;

using UnityEngine;

namespace ResourceManager
{
	public class ResourceManager
	{
		protected SortedList<string, Sprite> _sprites;
		protected SortedList<string, Font> _fonts;
		protected SortedList<string, GameObject> _prefabs;

		private static ResourceManager _instance;

		protected ResourceManager()
		{
			_sprites = new SortedList<string, Sprite>();
			_fonts = new SortedList<string, Font>();
			_prefabs = new SortedList<string, GameObject>();
		}

		public static ResourceManager Get()
		{
			if (_instance == null)
				_instance = new ResourceManager();

			return _instance;
		}

		public Sprite sprite(string resPath)
		{
			Sprite sprite;
			if (_sprites.TryGetValue(resPath, out sprite))
				return sprite;

			sprite = Resources.Load<Sprite>(resPath);

			_sprites.Add(resPath, sprite);

			return sprite;
		}

		public Font font(string resPath)
		{
			Font font;
			if (_fonts.TryGetValue(resPath, out font))
				return font;

			font = Resources.Load<Font>(resPath);

			_fonts.Add(resPath, font);

			return font;
		}

		public GameObject prefab(string resPath)
		{
			GameObject prefab;
			if (_prefabs.TryGetValue(resPath, out prefab))
				return prefab;

			prefab = Resources.Load<GameObject>(resPath);

			_prefabs.Add(resPath, prefab);

			return prefab;
		}
	}
}