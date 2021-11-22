using System;
using System.Collections.Generic;
using System.Linq;

namespace Promises
{
	public static class PromiseExtensions
	{
#region Conversion

		public static Promise<TConverted> Convert<TConverted>(this Promise promise)
		{
			var convertedPromise = new Promise<TConverted>
			                       {
				                       Id = promise.Id
			                       };

			return convertedPromise;
		}

		public static Promise<TConverted> Convert<TConverted>(this IPromiseBase promise)
		{
			var convertedPromise = new Promise<TConverted>
			                       {
				                       Id = promise.Id
			                       };

			return convertedPromise;
		}

		public static Promise ConvertToTypeless<TPromise>(this TPromise promise) where TPromise : IPromiseBase
		{
			return new Promise
			       {
				       Id = promise.Id
			       };
		}

#endregion

#region Callbacks

		public static TPromise Then<TPromise>(this TPromise promise, Action callback) where TPromise : IPromiseBase
		{
			var prom = Promise<string>.Create();
			var prom2 = Promise.Create();
			var fprom = prom2.Convert<float>();
			var prom3 = prom.Convert<int>();


			Promise.AddResultCallback(promise, result => callback());
			return promise;
		}

		public static TPromise Then<TPromise>(this TPromise promise, PromiseResultDelegate callback)
			where TPromise : IPromise
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

		public static IPromise<TResult> Then<TResult>(this IPromise<TResult> promise,
		                                              PromiseResultDelegate<TResult> callback)
		{
			Promise<TResult>.AddResultCallback(promise, callback);
			return promise;
		}

#endregion

#region Constructors

		public static TNew Then<TPromise, TNew>(this TPromise promise, PromiseConstructorDelegate<TNew> constructor)
			where TPromise : IPromiseBase where TNew : struct, IPromiseBase
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

		public static TNew Then<TPromise, TNew>(this TPromise promise,
		                                        PromiseConstructorDelegate<object, TNew> constructor)
			where TPromise : IPromise where TNew : struct, IPromiseBase
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
		                                       PromiseConstructorDelegate<TResult, TNew> constructor)
			where TNew : struct, IPromiseBase
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

		public static TNew Then<TResult, TNew>(this IPromise<TResult> promise,
		                                       PromiseConstructorDelegate<TResult, TNew> constructor)
			where TNew : struct, IPromiseBase
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

#endregion

#region BulkOperations

		public static Promise<List<object>> ThenAll<TPromise>(this TPromise promise,
		                                                      Func<IEnumerable<IPromiseBase>> constructor)
			where TPromise : IPromiseBase
		{
			var placeholder = new Promise<List<object>>
			                  {
				                  Id = Promise.AllocateId(),
			                  };
			Promise.AddChain(promise, placeholder);
			Promise.AddResultCallback(promise, result =>
			{
				var generatedPromise = Promise.All(constructor());
				Promise.Bind(generatedPromise, placeholder);
			});
			// Promise.AddExceptionCallback(promise, (ex) => { placeholder.Reject(ex); });

			return placeholder;
		}

		public static Promise<List<object>> ThenAll<TPromise>(this TPromise promise,
		                                                      Func<object, IEnumerable<IPromiseBase>> constructor)
			where TPromise : IPromise
		{
			var placeholder = new Promise<List<object>>
			                  {
				                  Id = Promise.AllocateId(),
			                  };
			Promise.AddChain(promise, placeholder);
			Promise.AddResultCallback(promise, result =>
			{
				var generatedPromise = Promise.All(constructor(result));
				Promise.Bind(generatedPromise, placeholder);
			});
			// Promise.AddExceptionCallback(promise, (ex) => { placeholder.Reject(ex); });

			return placeholder;
		}

		public static Promise<List<object>> ThenAll<TResult>(this Promise<TResult> promise,
		                                                     Func<TResult, IEnumerable<IPromiseBase>> constructor)

		{
			var placeholder = new Promise<List<object>>
			                  {
				                  Id = Promise.AllocateId(),
			                  };
			Promise.AddChain(promise, placeholder);
			Promise<TResult>.AddResultCallback(promise, result =>
			{
				var generatedPromise = Promise.All(constructor(result));
				Promise.Bind(generatedPromise, placeholder);
			});
			// Promise.AddExceptionCallback(promise, (ex) => { placeholder.Reject(ex); });

			return placeholder;
		}

		public static Promise<List<object>> ThenAll<TResult>(this IPromise<TResult> promise,
		                                                     Func<TResult, IEnumerable<IPromiseBase>> constructor)

		{
			var placeholder = new Promise<List<object>>
			                  {
				                  Id = Promise.AllocateId(),
			                  };
			Promise.AddChain(promise, placeholder);
			Promise<TResult>.AddResultCallback(promise, result =>
			{
				var generatedPromise = Promise.All(constructor(result));
				Promise.Bind(generatedPromise, placeholder);
			});
			// Promise.AddExceptionCallback(promise, (ex) => { placeholder.Reject(ex); });

			return placeholder;
		}

#endregion

#region ExceptionCallbacks

		public static TPromise Catch<TPromise>(this TPromise promise, PromiseExceptionDelegate callback)
			where TPromise : IPromiseBase
		{
			Promise.AddExceptionCallback(promise, callback);
			return promise;
		}

		public static TPromise Catch<TPromise, TException>(this TPromise promise,
		                                                   PromiseExceptionDelegate<TException> callback)
			where TPromise : IPromiseBase where TException : Exception
		{
			Promise.AddExceptionCallback(promise, exception =>
			{
				if (exception is TException ex)
				{
					callback(ex);
				}
			});
			return promise;
		}

#endregion

#region FinallyCallbacks

		public static TPromise Finally<TPromise>(this TPromise promise, Action callback) where TPromise : IPromiseBase
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

		public static IPromise<TResult> Finally<TResult>(this IPromise<TResult> promise,
		                                                 Action callback)
		{
			Promise.AddFinallyCallback(promise, callback);
			return promise;
		}

#endregion

#region Resolution

		public static void Resolve<TPromise>(this TPromise promise, object result) where TPromise : IPromiseBase
		{
			Promise.Resolve(promise, result);
		}

		public static void Resolve<TResult>(this Promise<TResult> promise, TResult result)
		{
			Promise.Resolve(promise, result);
		}

		public static void Resolve<TResult>(this IPromise<TResult> promise, TResult result)
		{
			Promise.Resolve(promise, result);
		}

		public static void Reject<TPromise>(this TPromise promise, Exception error = null) where TPromise : IPromiseBase
		{
			error ??= new UnknownPromiseErrorException();
			Promise.Reject(promise, error);
		}

#endregion
	}
}
