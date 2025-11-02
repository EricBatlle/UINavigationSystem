namespace UINavigation
{
	public interface IViewWithData<TViewData> : IView
	{
		public void SetData(TViewData viewData);
	}
}