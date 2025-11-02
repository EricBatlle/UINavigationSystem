using Cysharp.Threading.Tasks;

namespace UINavigation
{
	public static class ViewHandleExtensions
	{
		public static async UniTask AwaitClose(this UniTask<ViewHandle> handleTask)
		{
			var handle = await handleTask;
			await handle.AwaitClose();
		}
	}
}