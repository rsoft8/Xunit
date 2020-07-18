﻿using System.Collections.Generic;
using Xunit.Abstractions;

namespace Xunit.Sdk
{
	/// <summary>
	/// An implementation of <see cref="IEqualityComparer{T}"/> for <see cref="ITestCollection"/>.
	/// Compares the IDs of the test collections.
	/// </summary>
	public class TestCollectionComparer : IEqualityComparer<ITestCollection>
	{
		/// <summary>
		/// The singleton instance of the comparer.
		/// </summary>
		public static readonly TestCollectionComparer Instance = new TestCollectionComparer();

		/// <inheritdoc/>
		public bool Equals(ITestCollection? x, ITestCollection? y)
		{
			if (x == null && y == null)
				return true;
			if (x == null || y == null)
				return false;

			return x.UniqueID == y.UniqueID;
		}

		/// <inheritdoc/>
		public int GetHashCode(ITestCollection obj) =>
			obj.UniqueID.GetHashCode();
	}
}
