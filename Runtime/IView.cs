using Cysharp.Threading.Tasks;

namespace UINavigation
{
	public interface IView
	{
		UniTask OnBeforeClose();
		UniTask OnOpenComplete();
	}
}