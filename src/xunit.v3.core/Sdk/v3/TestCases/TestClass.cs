﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using Xunit.Abstractions;
using Xunit.Internal;
using Xunit.Runner.v2;
using Xunit.Sdk;

namespace Xunit.v3
{
	/// <summary>
	/// The default implementation of <see cref="_ITestClass"/>.
	/// </summary>
	[DebuggerDisplay(@"\{ class = {Class.Name} \}")]
	public class TestClass : _ITestClass, IXunitSerializable
	{
		ITypeInfo? @class;
		_ITestCollection? testCollection;
		string? uniqueID;

		/// <summary/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
		public TestClass() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="TestClass"/> class.
		/// </summary>
		/// <param name="testCollection">The test collection the class belongs to</param>
		/// <param name="class">The test class</param>
		public TestClass(
			_ITestCollection testCollection,
			ITypeInfo @class)
		{
			this.@class = Guard.ArgumentNotNull(nameof(@class), @class);
			this.testCollection = Guard.ArgumentNotNull(nameof(testCollection), testCollection);

			uniqueID = UniqueIDGenerator.ForTestClass(TestCollection.UniqueID, Class.Name);
		}

		/// <inheritdoc/>
		public ITypeInfo Class
		{
			get => @class ?? throw new InvalidOperationException($"Attempted to get Class on an uninitialized '{GetType().FullName}' object");
			set => @class = Guard.ArgumentNotNull(nameof(Class), value);
		}

		/// <inheritdoc/>
		public _ITestCollection TestCollection
		{
			get => testCollection ?? throw new InvalidOperationException($"Attempted to get TestCollection on an uninitialized '{GetType().FullName}' object");
			set => testCollection = Guard.ArgumentNotNull(nameof(TestCollection), value);
		}

		/// <inheritdoc/>
		public string UniqueID
		{
			get => uniqueID ?? throw new InvalidOperationException($"Attempted to get {nameof(UniqueID)} on an uninitialized '{GetType().FullName}' object");
			set => uniqueID = Guard.ArgumentNotNull(nameof(UniqueID), value);
		}

		/// <inheritdoc/>
		public void Serialize(IXunitSerializationInfo info)
		{
			Guard.ArgumentNotNull(nameof(info), info);

			info.AddValue("TestCollection", TestCollection);
			info.AddValue("ClassAssemblyName", Class.Assembly.Name);
			info.AddValue("ClassTypeName", Class.Name);
			info.AddValue("UniqueID", UniqueID);
		}

		/// <inheritdoc/>
		public void Deserialize(IXunitSerializationInfo info)
		{
			Guard.ArgumentNotNull(nameof(info), info);

			TestCollection = info.GetValue<_ITestCollection>("TestCollection");
			UniqueID = info.GetValue<string>("UniqueID");

			var assemblyName = info.GetValue<string>("ClassAssemblyName");
			var typeName = info.GetValue<string>("ClassTypeName");

			var type = SerializationHelper.GetType(assemblyName, typeName);
			if (type == null)
				throw new InvalidOperationException($"Failed to deserialize type '{typeName}' in assembly '{assemblyName}'");

			Class = Reflector.Wrap(type);
		}
	}
}
