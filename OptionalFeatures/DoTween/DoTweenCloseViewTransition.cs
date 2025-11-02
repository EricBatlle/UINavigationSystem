using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace UINavigation.DoTween
{
	public class DoTweenCloseViewTransition : ViewTransition
	{
		[SerializeField]
		protected float duration = 0.35f;
		[SerializeField]
		protected Ease easeOut = Ease.InBack;

		private Tween closeTween;

		public override async UniTask Animate(Transform target)
		{
			closeTween?.Kill();
			closeTween = target.DOScale(Vector3.zero, duration)
				.SetEase(easeOut);
			await closeTween.AwaitCompletion();
		}
	}
}