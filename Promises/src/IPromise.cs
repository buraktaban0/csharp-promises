using System.Collections;

namespace Promises
{
	public interface IPromise : IEnumerator
	{
		public int Id { get; set; }
	}
}
