using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace UINavigation.DoTween
{
	public class DoTweenOpenViewTransition : ViewTransition
	{
		[SerializeField]
		protected float duration = 0.35f;
		[SerializeField]
		protected Ease easeIn = Ease.OutBack;

		private Tween openTween;

		public override async UniTask Animate(Transform target)
		{
			target.localScale = Vector3.zero;
			openTween?.Kill();
			openTween = target.DOScale(Vector3.one, duration)
				.SetEase(easeIn);
			await openTween.AwaitCompletion();
		}
	}
}