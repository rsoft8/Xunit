﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Runner.v2;

namespace Xunit.Sdk
{
	/// <summary>
	/// A helper class that gets the list of test collection definitions for a given assembly.
	/// Reports any misconfigurations of the test assembly via the diagnostic message sink.
	/// </summary>
	public static class TestCollectionFactoryHelper
	{
		/// <summary>
		/// Gets the test collection definitions for the given assembly.
		/// </summary>
		/// <param name="assemblyInfo">The assembly.</param>
		/// <param name="diagnosticMessageSink">The message sink which receives <see cref="IDiagnosticMessage"/> messages.</param>
		/// <returns>A list of mappings from test collection name to test collection definitions (as <see cref="ITypeInfo"/></returns>
		public static Dictionary<string, ITypeInfo> GetTestCollectionDefinitions(
			IAssemblyInfo assemblyInfo,
			IMessageSink diagnosticMessageSink)
		{
			var attributeTypesByName =
				assemblyInfo
					.GetTypes(false)
					.Select(type => new { Type = type, Attribute = type.GetCustomAttributes(typeof(CollectionDefinitionAttribute).AssemblyQualifiedName!).FirstOrDefault() })
					.Where(list => list.Attribute != null)
					.GroupBy(
						list => list.Attribute.GetConstructorArguments().Cast<string>().Single(),
						list => list.Type,
						StringComparer.OrdinalIgnoreCase
					);

			var result = new Dictionary<string, ITypeInfo>();

			foreach (var grouping in attributeTypesByName)
			{
				var types = grouping.ToList();
				result[grouping.Key] = types[0];

				if (types.Count > 1)
					diagnosticMessageSink.OnMessage(new DiagnosticMessage($"Multiple test collections declared with name '{grouping.Key}': {string.Join(", ", types.Select(type => type.Name))}"));
			}

			return result;
		}
	}
}
