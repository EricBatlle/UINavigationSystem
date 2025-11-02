using System;
using UnityEngine;

namespace UINavigation
{
	public sealed class DefaultViewsFactory : IViewsFactory
	{
		private readonly Action<GameObject> onCreated;

		/// <param name="onCreated">
		/// Optional action that runs after creating the view (useful for manually injecting services, logging events, etc.)
		/// </param>
		public DefaultViewsFactory(Action<GameObject> onCreated = null)
		{
			this.onCreated = onCreated;
		}

		public GameObject Create(GameObject viewPrefab, Transform parentTransform)
		{
			if (viewPrefab == null)
			{
				Debug.LogError("DefaultViewsFactory.Create: viewPrefab es null");
				return null;
			}

			var instanceGameObject = UnityEngine.Object.Instantiate(viewPrefab);

			ViewsFactoryUtils.AdjustUI(viewPrefab, parentTransform, instanceGameObject);

			onCreated?.Invoke(instanceGameObject);

			return instanceGameObject;
		}
	}
}