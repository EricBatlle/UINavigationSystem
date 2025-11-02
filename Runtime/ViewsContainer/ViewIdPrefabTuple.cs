using System;
using UnityEngine;

namespace UINavigation
{
	[Serializable]
	public class ViewIdPrefabTuple
	{
		[SerializeField]
		private ViewId id;
		[SerializeField]
		private GameObject prefab;

		public string Id => id;
		public GameObject Prefab => prefab;
	}
}