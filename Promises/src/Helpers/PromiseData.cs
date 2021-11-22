using System;

namespace Promises
{
	public struct PromiseData
	{
		public PromiseResultDelegate ResultCallback;
		public PromiseExceptionDelegate ExceptionCallback;
		public Action FinallyCallback;
		public PromiseState State;
		public object Result;
		public object Fallback;

		public static PromiseData Empty => new PromiseData
		                                   {
			                                   ResultCallback = delegate { },
			                                   ExceptionCallback = delegate { },
			                                   FinallyCallback = delegate { },
			                                   Result = null,
			                                   Fallback = null,
			                                   State = PromiseState.InProgress,
		                                   };
	}
}
