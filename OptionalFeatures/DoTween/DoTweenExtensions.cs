using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace UINavigation.DoTween
{
	public static class DoTweenExtensions
	{
		public static UniTask AwaitCompletion(this Tween tween)
		{
#if UNITASK_DOTWEEN_SUPPORT
			return tween.ToUniTask();
#else
		return tween.AsyncWaitForCompletion().AsUniTask();
#endif
		}
	}
}