using System;

namespace Promises
{
	public partial struct Promise<T>
	{
		public static Promise<T> Create()
		{
			return Promise.Create().Convert<T>();
		}

		public static implicit operator Promise(Promise<T> promise)
		{
			return new Promise { Id = promise.Id };
		}

		public static implicit operator Promise<T>(Promise promise)
		{
			return new Promise<T> { Id = promise.Id };
		}

		public static void AddResultCallback(Promise<T> promise, PromiseResultDelegate<T> callback)
		{
			Promise.AddResultCallback(promise, result =>
			{
				if (!(result is T resultT))
				{
					throw new Exception(
						$"Could not invoke PromiseResultDelegate<{typeof(T).FullName}> result delegate since the result is of type {result.GetType().FullName}");
				}

				callback(resultT);
			});
		}

	}
}
