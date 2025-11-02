using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UINavigation
{
	public interface IOpenViewTransition
	{
		UniTask AnimateOpen(Transform target);
	}
}