namespace ConcernsCaseWork.Service.Base
{
	public class PageSearch
	{
		private int _page = 1;
		public int Page { get { return _page; } }
		
		public int PageIncrement()
		{
			return Interlocked.Increment(ref _page);
		}
	}
}