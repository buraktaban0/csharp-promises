using System;
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

		public static void Release<T>(T promise) where T : IPromiseBase
		{
			s_PromiseDataMap.Remove(promise.Id);
		}


		public static bool TryGetResult<TPromise>(TPromise promise, out object result) where TPromise : IPromiseBase
		{
			if (!s_PromiseDataMap.TryGetValue(promise.Id, out var data))
			{
				result = null;
				return false;
			}

			result = data.Result;
			return result != null;
		}

		public static PromiseState GetState<TPromise>(TPromise promise) where TPromise : IPromiseBase
		{
			return s_PromiseDataMap[promise.Id].State;
		}

		public static void SetState<TPromise>(TPromise promise, PromiseState state) where TPromise : IPromiseBase
		{
			var data = s_PromiseDataMap[promise.Id];
			data.State = state;
			s_PromiseDataMap[promise.Id] = data;
		}

		public static bool IsDone<TPromise>(TPromise promise) where TPromise : IPromiseBase
		{
			return GetState(promise) > PromiseState.InProgress;
		}

		public static object GetResult<TPromise>(TPromise promise) where TPromise : IPromiseBase
		{
			return s_PromiseDataMap[promise.Id].Result;
		}

		public static void SetResult<TPromise>(TPromise promise, object result) where TPromise : IPromiseBase
		{
			var data = s_PromiseDataMap[promise.Id];
			data.Result = result;
			s_PromiseDataMap[promise.Id] = data;
		}

		public static bool HasResult<TPromise>(TPromise promise) where TPromise : IPromiseBase
		{
			return s_PromiseDataMap.TryGetValue(promise.Id, out var data) && data.Result != null;
		}

		public static void AddResultCallback<TPromise>(TPromise promise, PromiseResultDelegate callback)
			where TPromise : IPromiseBase
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

		public static PromiseResultDelegate GetResultCallback<TPromise>(TPromise promise) where TPromise : IPromiseBase
		{
			return s_PromiseDataMap[promise.Id].ResultCallback;
		}

		public static void AddExceptionCallback<TPromise>(TPromise promise, PromiseExceptionDelegate callback)
			where TPromise : IPromiseBase
		{
			var data = s_PromiseDataMap[promise.Id];
			data.ExceptionCallback += callback;
			s_PromiseDataMap[promise.Id] = data;
		}

		public static PromiseExceptionDelegate GetExceptionCallback<TPromise>(TPromise promise)
			where TPromise : IPromiseBase
		{
			return s_PromiseDataMap[promise.Id].ExceptionCallback;
		}


		public static void AddFinallyCallback<TPromise>(TPromise promise, Action callback)
			where TPromise : IPromiseBase
		{
			var data = s_PromiseDataMap[promise.Id];
			data.FinallyCallback += callback;
			s_PromiseDataMap[promise.Id] = data;
		}

		public static Action GetFinallyCallback<TPromise>(TPromise promise) where TPromise : IPromiseBase
		{
			return s_PromiseDataMap[promise.Id].FinallyCallback;
		}

		public static void AddChain<TPrev, TNext>(TPrev prev, TNext next) where TPrev : IPromiseBase where TNext : IPromiseBase
		{
			s_PromiseChainMap[prev.Id] = next.Id;
			s_PromiseChainMapReverse[next.Id] = prev.Id;
		}


		public static void Resolve<TPromise>(TPromise promise, object result) where TPromise : IPromiseBase
		{
			Exception exception = null;
			try
			{
				SetResult(promise, result);
				var resultCallback = GetResultCallback(promise);
				resultCallback(result);
			}
			catch (Exception ex)
			{
				exception = ex;
				Console.WriteLine(
					$"An exception inside promises was silently taken care of. Exception callback was invoked. Exception: {ex}");
				var exceptionCallback = GetExceptionCallback(promise);
				exceptionCallback(ex);
			}
			finally
			{
				var finallyCallback = GetFinallyCallback(promise);
				finallyCallback();
			}

			if (TryGetNextInChain(promise, out var next) && exception != null)
			{
				next.Reject(exception);
			}
		}

		public static void Reject<TPromise>(TPromise promise, Exception exception = null) where TPromise : IPromiseBase
		{
			exception ??= new UnknownPromiseErrorException();
			var exceptionCallback = GetExceptionCallback(promise);
			exceptionCallback(exception);

			var finallyCallback = GetFinallyCallback(promise);
			finallyCallback();

			if (TryGetNextInChain(promise, out var nextPromise))
			{
				nextPromise.Reject(exception);
			}
		}

		public static bool TryGetNextInChain<TPromise>(TPromise promise, out Promise next) where TPromise : IPromiseBase
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
			where TPromise1 : IPromiseBase where TPromise2 : IPromiseBase
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
