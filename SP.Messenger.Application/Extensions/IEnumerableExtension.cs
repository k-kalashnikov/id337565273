﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Linq
{
	public static class IEnumerableExtension
	{
		public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
		{
			return items.GroupBy(property).Select(x => x.First());
		}

	}
}
