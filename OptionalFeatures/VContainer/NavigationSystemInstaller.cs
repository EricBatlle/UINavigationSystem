using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UINavigation.VContainer
{
	public class NavigationSystemInstaller : IInstaller
	{
		private readonly Transform rootCanvas;
		private readonly ViewsContainer viewsContainer;

		public NavigationSystemInstaller(Transform rootCanvas, ViewsContainer viewsContainer)
		{
			this.rootCanvas = rootCanvas;
			this.viewsContainer = viewsContainer;
		}

		public void Install(IContainerBuilder builder)
		{
			builder.Register<UINavigationSystem>(Lifetime.Singleton).WithParameter(rootCanvas);
			builder.Register<VContainerViewsFactory>(Lifetime.Singleton).WithParameter(this).As<IViewsFactory>();
			builder.RegisterInstance(viewsContainer);
		}
	}
}