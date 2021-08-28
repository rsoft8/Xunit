﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit.Internal;

namespace Xunit.Sdk
{
	/// <summary>
	/// Abstract attribute which represents a data source for a data theory.
	/// Data source providers derive from this attribute and implement GetData
	/// to return the data for the theory.
	/// </summary>
	[DataDiscoverer(typeof(DataDiscoverer))]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public abstract class DataAttribute : Attribute
	{
		static readonly MethodInfo? tupleIndexerGetter;
		static readonly MethodInfo? tupleLengthGetter;
		static readonly Type? tupleType;

		static DataAttribute()
		{
			tupleType = Type.GetType("System.Runtime.CompilerServices.ITuple");
			if (tupleType == null)
				return;

			tupleIndexerGetter = tupleType.GetProperty("Item")?.GetMethod;
			tupleLengthGetter = tupleType.GetProperty("Length")?.GetMethod;
		}

		/// <summary>
		/// Converts an item yielded by the data member to an object array, for return from <see cref="GetData"/>.
		/// Items yielded will typically be <see cref="T:object[]"/> or <see cref="ITheoryDataRow"/>, but this
		/// override will allow derived types to support additional data items. Also will return an empty
		/// theory data row when <paramref name="item"/> is <c>null</c>. If the data item cannot be converted,
		/// this method returns <c>null</c>.
		/// </summary>
		/// <param name="testMethod">The method that is being tested.</param>
		/// <param name="item">An item yielded from the data member.</param>
		/// <returns>An <see cref="ITheoryDataRow"/> suitable for return from <see cref="GetData"/>, or <c>null</c>
		/// if the data item is not compatible with <see cref="ITheoryDataRow"/>.</returns>
		protected virtual ITheoryDataRow? ConvertDataItem(
			MethodInfo testMethod,
			object? item)
		{
			Guard.ArgumentNotNull(nameof(testMethod), testMethod);

			if (item == null)
				return new TheoryDataRow();

			if (item is ITheoryDataRow dataRow)
				return dataRow;

			if (item is object?[] array)
				return new TheoryDataRow(array);

			if (tupleType != null && tupleIndexerGetter != null && tupleLengthGetter != null)
			{
				if (tupleType.IsAssignableFrom(item.GetType()))
				{
					var countObj = tupleLengthGetter.Invoke(item, null);
					if (countObj != null)
					{
						var count = (int)countObj;
						var data = new object?[count];
						for (var idx = 0; idx < count; ++idx)
							data[idx] = tupleIndexerGetter.Invoke(item, new object[] { idx });
						return new TheoryDataRow(data);
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Returns the data to be used to test the theory.
		/// </summary>
		/// <param name="testMethod">The method that is being tested</param>
		/// <returns>One or more rows of theory data. Each invocation of the test method
		/// is represented by a single instance of <see cref="ITheoryDataRow"/>.</returns>
		public abstract ValueTask<IReadOnlyCollection<ITheoryDataRow>?> GetData(MethodInfo testMethod);

		/// <summary>
		/// Marks all test cases generated by this data source as skipped.
		/// </summary>
		public virtual string? Skip { get; set; }
	}
}
