using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UINavigation
{
	public abstract class BaseView : MonoBehaviour, IView
	{
		public UniTask AwaitCloseComplete => closeCompleteTcs.Task;
        
		private readonly UniTaskCompletionSource closeCompleteTcs = new UniTaskCompletionSource();

		public virtual UniTask Close(bool immediate = false)
		{
			closeCompleteTcs?.TrySetResult();
			return UniTask.CompletedTask;
		}
	}
}