using Cysharp.Threading.Tasks;

namespace UINavigation
{
	public interface IViewWithResult<TResult> : IView
	{
		UniTask<TResult> AwaitResult  { get; }
	}
}