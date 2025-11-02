using Cysharp.Threading.Tasks;

namespace UINavigation
{
	public static class ViewHandleResultExtensions
	{
		// Returns (handle, result) without forcing closure (useful if you want to close later).
		public static async UniTask<(ViewHandle handle, TResult result)> AwaitResult<TResult>(this UniTask<ViewHandle> handleTask)
		{
			var handle = await handleTask;
			var result = await handle.Require<IViewWithResult<TResult>>().AwaitResult;
			return (handle, result);
		}

		// Wait until it has finished and then until the view closes completely.
		public static async UniTask<TResult> AwaitCloseResult<TResult>(this UniTask<ViewHandle> handleTask)
		{
			var handle = await handleTask;
			var result = await handle.Require<IViewWithResult<TResult>>().AwaitResult;
			await handle.AwaitClose();
			return result;
		}
	}
}