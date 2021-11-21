using System;
using System.Threading.Tasks;

namespace Promises
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			var promise = Promise.Create();
			promise.Then(() => Console.WriteLine("Promise resolved"))
			       .Then(result => Console.WriteLine($"Result was: {result}"))
			       .Then(GetStringAsync)
			       .Then(result =>
			       {
				       Console.WriteLine("asd " + result);
				       // throw new ArgumentException("asd");
			       })
			       .Catch((ArgumentException exception) => { Console.WriteLine($"Exception received: {exception}"); })
			       .Finally(() => Console.WriteLine("Executing finally 1"))
			       .Then(() => Console.WriteLine("Empty callback"))
			       .Then(GetLongAsync)
			       .Then(result => Console.WriteLine("Long: " + result))
			       .Finally(() => Console.WriteLine("Executing finally 2"));

			Task.Run(() =>
			{
				Task.Delay(1000).Wait();
				promise.Resolve(5.21f);
			});

			Console.ReadKey();
		}

		static Promise<string> GetStringAsync(object result)
		{
			var promise = Promise<string>.Create();

			Task.Run(() =>
			{
				Task.Delay(1000).Wait();
				promise.Resolve("Burak: " + result);
			});

			return promise;
		}

		static Promise<long> GetLongAsync(string result)
		{
			var promise = Promise<long>.Create();

			Task.Run(() =>
			{
				Task.Delay(1000).Wait();
				promise.Resolve(1234u);
			});

			return promise;
		}
	}
}
