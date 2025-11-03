using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace UINavigation
{
	public class CloseViewTransition : ViewTransition
	{
		[SerializeField]
		protected float duration = 0.35f;
		[SerializeField]
		protected AnimationCurve easeOut = AnimationCurve.EaseInOut(0, 0, 1, 1);

		private CancellationTokenSource cts;

		public override async UniTask Animate(Transform target)
		{
			// Cancel animation in progress, if any
			cts?.Cancel();
			cts?.Dispose();
			cts = new CancellationTokenSource();
			var cancellationToken = cts.Token;

			var initialScale = target.localScale;
			var time = 0f;
			
			while (time < duration)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				var t = time / duration;
				target.localScale = Vector3.LerpUnclamped(initialScale, Vector3.zero, easeOut.Evaluate(t));
				time += Time.deltaTime;
				await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
			}

			target.localScale = Vector3.zero;
		}
	}
}