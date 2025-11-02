using UnityEngine;

namespace UINavigation
{
	public interface IViewsFactory
	{
		GameObject Create(GameObject viewPrefab, Transform parentTransform);
	}
}