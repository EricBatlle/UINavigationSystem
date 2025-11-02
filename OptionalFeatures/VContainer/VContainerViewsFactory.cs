using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UINavigation.VContainer
{
	public class VContainerViewsFactory : IViewsFactory
	{
		private readonly LifetimeScope sceneScope;
		private readonly IObjectResolver resolver;

		public VContainerViewsFactory(LifetimeScope sceneScope, IObjectResolver resolver)
		{
			this.sceneScope = sceneScope;
			this.resolver = resolver;
		}

		public GameObject Create(GameObject viewPrefab, Transform parentTransform)
		{
			if (viewPrefab.GetComponent<LifetimeScope>() != null)
			{
				return CreateWithScope(viewPrefab, parentTransform);
			}

			var instance = Object.Instantiate(viewPrefab, parentTransform, false);
			resolver.Inject(instance);
			return instance;
		}
		
		private GameObject CreateWithScope(GameObject viewPrefab, Transform parentTransform)
		{
			var instanceScope = sceneScope.CreateChildFromPrefab(viewPrefab.GetComponent<LifetimeScope>());
			var instanceGameObject = instanceScope.gameObject;

			ViewsFactoryUtils.AdjustUI(viewPrefab, parentTransform, instanceGameObject);

			return instanceGameObject;
		}
	}
}