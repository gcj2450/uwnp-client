using System.Collections;
using System.Collections.Generic;

namespace SuperSocket.ClientEngine
{
	public interface IPosList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		int Position { get; set; }
	}
}
