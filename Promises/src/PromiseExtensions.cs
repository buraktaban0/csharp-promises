using System;

namespace Promises
{
	public static class PromiseExtensions
	{
		public static Promise<TConverted> Convert<TConverted>(this Promise promise)
		{
			var convertedPromise = new Promise<TConverted>
			                       {
				                       Id = promise.Id
			                       };

			return convertedPromise;
		}

		public static TNew Then<TNew>(this Promise promise, PromiseConstructorDelegate<TNew> constructor)
			where TNew : struct, IPromise
		{
			var placeholder = new TNew
			                  {
				                  Id = Promise.AllocateId(),
			                  };
			Promise.AddChain(promise, placeholder);
			Promise.AddResultCallback(promise, result =>
			{
				var generatedPromise = constructor();
				Promise.Bind(generatedPromise, placeholder);
			});
			// Promise.AddExceptionCallback(promise, (ex) => { placeholder.Reject(ex); });

			return placeholder;
		}

		public static TNew Then<TNew>(this Promise promise, PromiseConstructorDelegate<object, TNew> constructor)
			where TNew : struct, IPromise
		{
			var placeholder = new TNew
			                  {
				                  Id = Promise.AllocateId(),
			                  };
			Promise.AddChain(promise, placeholder);
			Promise.AddResultCallback(promise, result =>
			{
				var generatedPromise = constructor(result);
				Promise.Bind(generatedPromise, placeholder);
			});
			// Promise.AddExceptionCallback(promise, (ex) => { placeholder.Reject(ex); });

			return placeholder;
		}

		public static TNew Then<TResult, TNew>(this Promise<TResult> promise,
		                                       PromiseConstructorDelegate<TNew> constructor)
			where TNew : struct, IPromise
		{
			var placeholder = new TNew
			                  {
				                  Id = Promise.AllocateId(),
			                  };
			Promise.AddChain(promise, placeholder);
			Promise<TResult>.AddResultCallback(promise, result =>
			{
				var generatedPromise = constructor();
				Promise.Bind(generatedPromise, placeholder);
			});
			// Promise.AddExceptionCallback(promise, (ex) => { placeholder.Reject(ex); });

			return placeholder;
		}

		public static TNew Then<TResult, TNew>(this Promise<TResult> promise,
		                                       PromiseConstructorDelegate<TResult, TNew> constructor)
			where TNew : struct, IPromise
		{
			var placeholder = new TNew
			                  {
				                  Id = Promise.AllocateId(),
			                  };
			Promise.AddChain(promise, placeholder);
			Promise<TResult>.AddResultCallback(promise, result =>
			{
				var generatedPromise = constructor(result);
				Promise.Bind(generatedPromise, placeholder);
			});
			// Promise.AddExceptionCallback(promise, (ex) => { placeholder.Reject(ex); });

			return placeholder;
		}

		public static T Then<T>(this T promise, Action callback) where T : IPromise
		{
			Promise.AddResultCallback(promise, result => callback());
			return promise;
		}

		public static Promise Then(this Promise promise, PromiseResultDelegate callback)
		{
			Promise.AddResultCallback(promise, callback);
			return promise;
		}

		public static Promise<TResult> Then<TResult>(this Promise<TResult> promise,
		                                             PromiseResultDelegate<TResult> callback)
		{
			Promise<TResult>.AddResultCallback(promise, callback);
			return promise;
		}

		public static Promise<TResult> Catch<TResult>(this Promise<TResult> promise,
		                                              PromiseExceptionDelegate callback)
		{
			Promise.AddExceptionCallback(promise, callback);
			return promise;
		}

		public static Promise<TResult> Catch<TResult, TEx>(this Promise<TResult> promise,
		                                                   PromiseExceptionDelegate<TEx> callback) where TEx : Exception
		{
			Promise.AddExceptionCallback(promise, exception =>
			{
				if (exception is TEx ex)
				{
					callback(ex);
				}
			});
			return promise;
		}

		public static T Catch<T>(this T promise, PromiseExceptionDelegate callback) where T : IPromise
		{
			Promise.AddExceptionCallback(promise, callback);
			return promise;
		}

		public static T Catch<T, TEx>(this T promise, PromiseExceptionDelegate<TEx> callback)
			where T : IPromise where TEx : Exception
		{
			Promise.AddExceptionCallback(promise, exception =>
			{
				if (exception is TEx ex)
				{
					callback(ex);
				}
			});
			return promise;
		}

		public static Promise Catch(this Promise promise, PromiseExceptionDelegate callback)
		{
			Promise.AddExceptionCallback(promise, callback);
			return promise;
		}

		public static Promise Catch<TEx>(this Promise promise, PromiseExceptionDelegate<TEx> callback)
			where TEx : Exception
		{
			Promise.AddExceptionCallback(promise, exception =>
			{
				if (exception is TEx ex)
				{
					callback(ex);
				}
			});
			return promise;
		}


		public static T Finally<T>(this T promise, Action callback) where T : IPromise
		{
			Promise.AddFinallyCallback(promise, callback);
			return promise;
		}


		public static Promise<TResult> Finally<TResult>(this Promise<TResult> promise,
		                                                Action callback)
		{
			Promise.AddFinallyCallback(promise, callback);
			return promise;
		}

		public static void Resolve<TPromise>(this TPromise promise, object result) where TPromise : IPromise
		{
			Promise.Resolve(promise, result);
		}

		public static void Resolve<TResult>(this Promise<TResult> promise, TResult result) =>
			Resolve<Promise>(promise, result);

		public static void Reject<TPromise>(this TPromise promise, Exception error = null) where TPromise : IPromise
		{
			error ??= new UnknownPromiseErrorException();
			Promise.Reject(promise, error);
		}
	}
}
