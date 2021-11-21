namespace Promises
{
	public partial struct Promise : IPromise
	{
		public int     Id { get; set; }
		
		public bool    MoveNext()
		{
			return !Promise.IsDone(this);
		}

		public void    Reset()
		{
			
		}

		public object Current { get; }
	}
}
