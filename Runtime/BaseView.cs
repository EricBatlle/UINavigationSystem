using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UINavigation
{
	public abstract class BaseView : MonoBehaviour, IView
	{
		public virtual UniTask OnBeforeClose() => UniTask.CompletedTask;
		public virtual UniTask OnOpenComplete() => UniTask.CompletedTask;
	}
}