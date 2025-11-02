namespace UINavigation
{
	public static class ViewHandleDataExtensions
	{
		public static ViewHandle WithData<TViewData>(this ViewHandle screen, TViewData data)
		{
			screen.Require<IViewWithData<TViewData>>().SetData(data);
			return screen;
		}
	}
}