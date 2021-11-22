using System.Collections.Generic;
using System.Linq;

namespace Promises
{
	// Using partial struct with the same name for quality of life.
	public partial struct Promise
	{
		// public static Promise<List<object>> All<T1>(T1 promise1) where T1 : IPromise
		// {
		// 	return All(new IPromise[] { promise1 });
		// }

		public static Promise<List<object>> All<T1, T2>(T1 promise1, T2 promise2)
			where T1 : IPromise where T2 : IPromise
		{
			return All(new IPromise[] { promise1, promise2 });
		}

		public static Promise<List<object>> All<T1, T2, T3>(T1 promise1, T2 promise2, T3 promise3)
			where T1 : IPromise where T2 : IPromise where T3 : IPromise
		{
			return All(new IPromise[] { promise1, promise2, promise3 });
		}

		public static Promise<List<object>> All<T1, T2, T3, T4>(T1 promise1, T2 promise2, T3 promise3, T4 promise4)
			where T1 : IPromise where T2 : IPromise where T3 : IPromise where T4 : IPromise
		{
			return All(new IPromise[] { promise1, promise2, promise3, promise4 });
		}

		public static Promise<List<object>> All(params IPromiseBase[] promises)
		{
			return All(promises.AsEnumerable());
		}

		public static Promise<List<object>> All(IEnumerable<IPromiseBase> promises, List<object> results = null)
		{
			var allPromise = Promise<List<object>>.Create();

			IEnumerable<IPromiseBase> enumerable = promises as IPromiseBase[] ?? promises.ToArray();
			int promiseCount = enumerable.Count();
			int completedPromiseCount = 0;

			bool anyPromiseFailed = false;

			results ??= new List<object>(promiseCount);
			results.Clear();
			for (var i = 0; i < promiseCount; i++)
			{
				results.Add(null);
			}

			int index = 0;
			foreach (var promise in enumerable)
			{
				int k = index++;
				promise.Then(() =>
				{
					if (anyPromiseFailed)
						return;

					results[k] = Promise.GetResult(promise);

					if (++completedPromiseCount == promiseCount)
					{
						// All done
						allPromise.Resolve(results);
						return;
					}
				}).Catch(exception =>
				{
					anyPromiseFailed = true;
					allPromise.Reject(new CombinedPromiseFailedException());
				});
			}

			return allPromise;
		}

		public static Promise<List<T>> All<T>(IEnumerable<IPromise<T>> promises, List<T> results = null)
		{
			var allPromise = Promise<List<T>>.Create();

			IEnumerable<IPromise<T>> enumerable = promises as IPromise<T>[] ?? promises.ToArray();
			int promiseCount = enumerable.Count();
			int completedPromiseCount = 0;

			bool anyPromiseFailed = false;

			results ??= new List<T>(promiseCount);
			results.Clear();
			for (var i = 0; i < promiseCount; i++)
			{
				results.Add(default);
			}

			int index = 0;
			foreach (var promise in enumerable)
			{
				int k = index++;
				var _promise = promise.Then(result =>
				{
					if (anyPromiseFailed)
						return;

					results[k] = result;

					if (++completedPromiseCount == promiseCount)
					{
						// All done
						allPromise.Resolve(results);
						return;
					}
				});
				_promise.Catch(exception =>
				{
					anyPromiseFailed = true;
					allPromise.Reject(new CombinedPromiseFailedException());
				});
			}

			return allPromise;
		}
	}
}
