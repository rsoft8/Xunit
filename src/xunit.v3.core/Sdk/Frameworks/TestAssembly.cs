﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Internal;

namespace Xunit.Sdk
{
	/// <summary>
	/// The default implementation of <see cref="ITestAssembly"/>.
	/// </summary>
	[DebuggerDisplay(@"\{ assembly = {Assembly.AssemblyPath}, config = {ConfigFileName} \}")]
	public class TestAssembly : ITestAssembly
	{
		IAssemblyInfo? assembly;
		Version? version;

		/// <summary/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
		public TestAssembly()
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="TestAssembly"/> class.
		/// </summary>
		/// <param name="assembly">The test assembly.</param>
		/// <param name="configFileName">The optional configuration filename</param>
		/// <param name="version">The version number of the assembly (defaults to "0.0.0.0")</param>
		public TestAssembly(IAssemblyInfo assembly, string? configFileName = null, Version? version = null)
		{
			Guard.ArgumentNotNull(nameof(assembly), assembly);

			Assembly = assembly;
			ConfigFileName = configFileName;

			this.version =
				version
				?? (assembly as IReflectionAssemblyInfo)?.Assembly?.GetName()?.Version
				?? new Version(0, 0, 0, 0);
		}

		/// <inheritdoc/>
		public IAssemblyInfo Assembly
		{
			get => assembly ?? throw new InvalidOperationException($"Attempted to get Assembly on an uninitialized '{GetType().FullName}' object");
			set => assembly = Guard.ArgumentNotNull(nameof(Assembly), value);
		}

		/// <inheritdoc/>
		public string? ConfigFileName { get; set; }

		/// <summary>
		/// Gets the assembly version.
		/// </summary>
		public Version Version
		{
			get => version ?? throw new InvalidOperationException($"Attempted to get Version on an uninitialized '{GetType().FullName}' object");
		}

		/// <inheritdoc/>
		public void Serialize(IXunitSerializationInfo info)
		{
			Guard.ArgumentNotNull(nameof(info), info);

			info.AddValue("AssemblyPath", Assembly.AssemblyPath);
			info.AddValue("ConfigFileName", ConfigFileName);
			info.AddValue("Version", Version.ToString());
		}

		/// <inheritdoc/>
		public void Deserialize(IXunitSerializationInfo info)
		{
			Guard.ArgumentNotNull(nameof(info), info);

			version = new Version(info.GetValue<string>("Version"));
			ConfigFileName = info.GetValue<string>("ConfigFileName");

			var assemblyPath = info.GetValue<string>("AssemblyPath");
			var assembly = System.Reflection.Assembly.Load(new AssemblyName
			{
				Name = Path.GetFileNameWithoutExtension(assemblyPath),
				Version = Version
			});

			Assembly = Reflector.Wrap(assembly);
		}
	}
}
