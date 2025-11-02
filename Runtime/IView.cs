using Cysharp.Threading.Tasks;

namespace UINavigation
{
	public interface IView
	{
		UniTask AwaitCloseComplete { get; }
		UniTask Close(bool immediate = false);
	}
}