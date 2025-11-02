using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UINavigation
{
	public sealed class ViewHandle
	{
		public GameObject GameObject { get; }
		public IView View { get; }

		private bool isShowing;
		
		public ViewHandle(GameObject go, IView view)
		{
			GameObject = go;
			View = view ?? throw new ArgumentNullException(nameof(view));
		}

		public bool TryGet<TCapability>(out TCapability cap) where TCapability : class
		{
			return GameObject.TryGetComponent(out cap);
		}

		public TCapability Require<TCapability>() where TCapability : class
		{
			if (TryGet<TCapability>(out var cap))
			{
				return cap;
			}

			throw new InvalidOperationException($"View '{GameObject.name}' does not implement {typeof(TCapability).Name}.");
		}

		public UniTask AwaitClose() => View.AwaitCloseComplete;
		
		public async UniTask<ViewHandle> Show()
		{
			if (isShowing)
			{
				return this;
			}

			if (GameObject.TryGetComponent<IOpenViewTransition>(out var open))
			{
				if (GameObject)
				{
					isShowing = true;
					await open.AnimateOpen(GameObject.transform);
				}
			}

			isShowing = false;
			return this;
		}
	}
}