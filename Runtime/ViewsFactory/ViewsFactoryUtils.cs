using UnityEngine;

namespace UINavigation
{
	public static class ViewsFactoryUtils
	{
		public static void AdjustUI(GameObject viewPrefab, Transform parentTransform, GameObject instanceGameObject)
		{
			var instanceRectTransform = instanceGameObject.transform as RectTransform;
			var parentRectTransform   = parentTransform as RectTransform;
			var prefabRectTransform   = viewPrefab.transform as RectTransform;

			if (instanceRectTransform != null && parentRectTransform != null)
			{
				instanceRectTransform.SetParent(parentRectTransform, false);

				if (prefabRectTransform != null)
				{
					ApplyPrefabRectTransform(instanceRectTransform, prefabRectTransform);
				}
			}
			else
			{
				instanceGameObject.transform.SetParent(parentTransform, false);
			}
		}
		
		private static void ApplyPrefabRectTransform(RectTransform instance, RectTransform prefab)
		{
			instance.anchorMin = prefab.anchorMin;
			instance.anchorMax = prefab.anchorMax;
			instance.pivot     = prefab.pivot;
			instance.sizeDelta = prefab.sizeDelta;
			instance.anchoredPosition3D = prefab.anchoredPosition3D;
			instance.localRotation = prefab.localRotation;
			instance.localScale    = prefab.localScale;
		}
	}
}