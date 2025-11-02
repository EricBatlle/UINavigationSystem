using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UINavigation
{
    public class UINavigationSystem
    {
        private readonly ViewsContainer viewsContainer;
        private readonly IViewsFactory viewsFactory;
        private readonly Transform rootCanvas;

        public UINavigationSystem(ViewsContainer viewsContainer, IViewsFactory viewsFactory, Transform rootCanvas)
        {
            this.viewsContainer = viewsContainer;
            this.viewsFactory = viewsFactory;
            this.rootCanvas = rootCanvas;
        }
        
        public ViewHandle CreateView(string viewId)
        {
            if (string.IsNullOrEmpty(viewId))
            {
                Debug.LogError($"ViewId {viewId} does not exist.");
                return null;
            }

            var viewPrefab = viewsContainer.GetViewPrefab(viewId);
            if (viewPrefab == null)
            {
                Debug.LogError($"ViewId {viewId} does not have any associated view prefab.");
                return null;
            }

            var go = viewsFactory.Create(viewPrefab, rootCanvas);
            var view = go.GetComponent<IView>();
            if (view == null)
            {
                Debug.LogWarning($"Prefab {go.name} does not implement {nameof(IView)}, adding temporary default {nameof(DefaultView)} component");
                view = go.AddComponent<DefaultView>();
            }

            return new ViewHandle(go, view);
        }

        public async UniTask Close(IView view, bool immediate = false)
        {
            await view.Close(immediate);
            await view.AwaitCloseComplete;
            if (view is Component component && component != null)
            {
                Object.Destroy(component.gameObject);
            }
            else
            {
                Debug.LogError($"{view} is not a Component, can't destroy its GameObject automatically");
            }
        }
    }
}
