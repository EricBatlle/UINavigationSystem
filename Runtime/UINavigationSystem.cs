using System.Collections.Generic;
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
        private readonly Dictionary<IView, ViewHandle> viewsMap = new();

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

            var viewHandle = new ViewHandle(go, view);
            viewsMap[view] = viewHandle;
            return viewHandle;
        }

        public async UniTask Close(ViewHandle viewHandle, bool immediate = false)
        {
            await Close(viewHandle.View, immediate);
        }

        public async UniTask Close(IView view, bool immediate = false)
        {
            if (view is not Component component || component == null)
            {
                Debug.LogError($"{view} is not a Component, can't destroy its GameObject automatically");
                return;
            }

            await view.OnBeforeClose();

            if (viewsMap.TryGetValue(view, out var viewHandle))
            {
                if (!immediate && viewHandle.GameObject && viewHandle.GameObject.TryGetComponent<ICloseViewTransition>(out var close))
                {
                    await close.AnimateClose(viewHandle.GameObject.transform);
                }
                viewHandle.MarkViewAsClosed();
            }

            Object.Destroy(component.gameObject);
            viewsMap.Remove(view);
        }
    }
}
