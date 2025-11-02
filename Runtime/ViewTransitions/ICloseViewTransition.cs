using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UINavigation
{
	public interface ICloseViewTransition
	{
		UniTask AnimateClose(Transform target);
	}
}