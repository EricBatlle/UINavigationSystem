using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UINavigation.DoTween
{
	[RequireComponent(typeof(DoTweenOpenViewTransition))]
	[RequireComponent(typeof(DoTweenCloseViewTransition))]
	public abstract class DoTweenBaseAnimatedView : BaseView, IOpenViewTransition, ICloseViewTransition
	{
		[Header("Open/Close Transitions")]
		[SerializeField]
		private DoTweenOpenViewTransition openTransition;
		[SerializeField]
		private DoTweenCloseViewTransition closeTransition;

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
	}
}