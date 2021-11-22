using System;

namespace Promises
{
	public delegate void PromiseResultDelegate(object result);

	public delegate void PromiseResultDelegate<in T>(T result);

	public delegate void PromiseExceptionDelegate(Exception ex);

	public delegate void PromiseExceptionDelegate<in TException>(TException ex) where TException : Exception;

	public delegate TNew PromiseConstructorDelegate<out TNew>() where TNew : IPromiseBase;

	public delegate TNew PromiseConstructorDelegate<in TResult, out TNew>(TResult result) where TNew : IPromiseBase;
}
