using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UINavigation
{
	[RequireComponent(typeof(OpenViewTransition))]
	[RequireComponent(typeof(CloseViewTransition))]
	public abstract class BaseAnimatedView : BaseView, IOpenViewTransition, ICloseViewTransition
	{
		[Header("Open/Close Transitions")]
		[SerializeField]
		private OpenViewTransition openTransition;
		[SerializeField]
		private CloseViewTransition closeTransition;

		public UniTask AnimateOpen(Transform target)
		{
			if (openTransition == null)
			{
				Debug.LogWarning($"View transition not attached to {gameObject.name}");
				return UniTask.CompletedTask;
			}

			return openTransition.Animate(target);
		}

		public UniTask AnimateClose(Transform target)
		{
			if (closeTransition == null)
			{
				Debug.LogWarning($"View transition not attached to {gameObject.name}");
				return UniTask.CompletedTask;
			}

			return closeTransition.Animate(target);
		}

		public override async UniTask Close(bool immediate = false)
		{
			if (!immediate)
			{
				await AnimateClose(gameObject.transform);
			}
			
			await base.Close(immediate);
		}
	}
}