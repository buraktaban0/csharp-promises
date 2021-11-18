using System;

namespace Promises
{
	public delegate void PromiseResultDelegate(object result);

	public delegate void PromiseResultDelegate<in T>(T result);

	public delegate void PromiseExceptionDelegate(Exception ex);

	public delegate void PromiseExceptionDelegate<in TEx>(TEx ex) where TEx : Exception;

	public delegate TNew PromiseConstructorDelegate<out TNew>() where TNew : IPromise;

	public delegate TNew PromiseConstructorDelegate<in TResult, out TNew>(TResult result) where TNew : IPromise;
}
