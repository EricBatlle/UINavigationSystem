using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UINavigation
{
	public class OpenViewTransition : ViewTransition
	{
		[SerializeField]
		protected float duration = 0.35f;
		[SerializeField]
		protected AnimationCurve easeIn = AnimationCurve.EaseInOut(0, 0, 1, 1);

		private CancellationTokenSource cts;

		public override async UniTask Animate(Transform target)
		{
			// Cancel animation in progress, if any
			cts?.Cancel();
			cts?.Dispose();
			cts = new CancellationTokenSource();
			var cancellationToken = cts.Token;

			target.localScale = Vector3.zero;

			var time = 0f;
			while (time < duration)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				var t = time / duration;
				target.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, easeIn.Evaluate(t));
				time += Time.deltaTime;
				await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
			}

			target.localScale = Vector3.one;
		}
	}
}