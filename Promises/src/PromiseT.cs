namespace Promises
{
	public partial struct Promise<T> : IPromise<T>
	{
		public int Id { get; set; }
	}
}
