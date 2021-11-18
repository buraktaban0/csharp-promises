using System;
using System.Collections;
using System.Collections.Generic;

namespace Promises
{
	public partial struct Promise
	{
		private static int s_AvailableId = 0;
		private static Dictionary<int, PromiseData> s_PromiseDataMap = new Dictionary<int, PromiseData>();
		private static Dictionary<int, int> s_PromiseChainMap = new Dictionary<int, int>();
		private static Dictionary<int, int> s_PromiseChainMapReverse = new Dictionary<int, int>();

		public static int GetAndIncrementAvailableId()
		{
			return unchecked(s_AvailableId++);
		}

		public static int AllocateId()
		{
			var id = GetAndIncrementAvailableId();
			s_PromiseDataMap[id] = PromiseData.Empty;
			return id;
		}

		public static Promise Create()
		{
			var promise = new Promise
			              {
				              Id = AllocateId()
			              };

			return promise;
		}

		public static void Release<T>(T promise) where T : IPromise
		{
			s_PromiseDataMap.Remove(promise.Id);
		}


		public static bool TryGetResult<TPromise>(TPromise promise, out object result) where TPromise : IPromise
		{
			if (!s_PromiseDataMap.TryGetValue(promise.Id, out var data))
			{
				result = null;
				return false;
			}

			result = data.Result;
			return result != null;
		}

		public static object GetResult<TPromise>(TPromise promise) where TPromise : IPromise
		{
			return s_PromiseDataMap[promise.Id].Result;
		}

		public static void SetResult<TPromise>(TPromise promise, object result) where TPromise : IPromise
		{
			var data = s_PromiseDataMap[promise.Id];
			data.Result = result;
			s_PromiseDataMap[promise.Id] = data;
		}

		public static bool HasResult<TPromise>(TPromise promise) where TPromise : IPromise
		{
			return s_PromiseDataMap.TryGetValue(promise.Id, out var data) && data.Result != null;
		}

		public static void AddResultCallback<TPromise>(TPromise promise, PromiseResultDelegate callback)
			where TPromise : IPromise
		{
			var data = s_PromiseDataMap[promise.Id];
			data.ResultCallback += callback;
			s_PromiseDataMap[promise.Id] = data;

			if (HasResult(promise))
			{
				// Already invoked earlier, invoke this single callback so it does not miss the result
				var result = GetResult(promise);
				callback(result);
			}
		}

		public static PromiseResultDelegate GetResultCallback<TPromise>(TPromise promise) where TPromise : IPromise
		{
			return s_PromiseDataMap[promise.Id].ResultCallback;
		}

		public static void AddExceptionCallback<TPromise>(TPromise promise, PromiseExceptionDelegate callback)
			where TPromise : IPromise
		{
			var data = s_PromiseDataMap[promise.Id];
			data.ExceptionCallback += callback;
			s_PromiseDataMap[promise.Id] = data;
		}

		public static PromiseExceptionDelegate GetExceptionCallback<TPromise>(TPromise promise)
			where TPromise : IPromise
		{
			return s_PromiseDataMap[promise.Id].ExceptionCallback;
		}


		public static void AddFinallyCallback<TPromise>(TPromise promise, Action callback)
			where TPromise : IPromise
		{
			var data = s_PromiseDataMap[promise.Id];
			data.FinallyCallback += callback;
			s_PromiseDataMap[promise.Id] = data;
		}

		public static Action GetFinallyCallback<TPromise>(TPromise promise) where TPromise : IPromise
		{
			return s_PromiseDataMap[promise.Id].FinallyCallback;
		}

		public static void AddChain<TPrev, TNext>(TPrev prev, TNext next) where TPrev : IPromise where TNext : IPromise
		{
			s_PromiseChainMap[prev.Id] = next.Id;
			s_PromiseChainMapReverse[next.Id] = prev.Id;
		}


		public static void Resolve<TPromise>(TPromise promise, object result) where TPromise : IPromise
		{
			try
			{
				SetResult(promise, result);
				var resultCallback = GetResultCallback(promise);
				resultCallback(result);
			}
			catch (Exception e)
			{
				Throw(promise, e);
			}
			finally
			{
				var finallyCallback = GetFinallyCallback(promise);
				finallyCallback();
			}
		}

		public static void Reject<TPromise>(TPromise promise, Exception exception = null) where TPromise : IPromise
		{
			Throw(promise, exception);

			var finallyCallback = GetFinallyCallback(promise);
			finallyCallback();

			if (TryGetNextInChain(promise, out var nextPromise))
			{
				nextPromise.Reject(exception);
			}
		}

		public static void Throw<TPromise>(TPromise promise, Exception exception) where TPromise : IPromise
		{
			exception ??= new UnknownPromiseErrorException();
			var exceptionCallback = GetExceptionCallback(promise);
			exceptionCallback(exception);
		}

		public static bool TryGetNextInChain<TPromise>(TPromise promise, out Promise next) where TPromise : IPromise
		{
			if (s_PromiseChainMap.TryGetValue(promise.Id, out int nextId))
			{
				next = nextId;
				return true;
			}

			next = default;
			return false;
		}

		public static void Bind<TPromise1, TPromise2>(TPromise1 promise1, TPromise2 promise2)
			where TPromise1 : IPromise where TPromise2 : IPromise
		{
			AddResultCallback(promise1, result => promise2.Resolve(result));
			AddExceptionCallback(promise1, error => promise2.Reject(error));
		}

		public static implicit operator Promise(int id)
		{
			return new Promise
			       {
				       Id = id
			       };
		}
	}
}
