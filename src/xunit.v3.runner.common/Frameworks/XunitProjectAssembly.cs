﻿using System;
using System.IO;
using System.Reflection;
using Xunit.Internal;

namespace Xunit.Runner.Common
{
	/// <summary>
	/// Represents an assembly in an <see cref="XunitProject"/>.
	/// </summary>
	public class XunitProjectAssembly
	{
		string? targetFramework;

		/// <summary>
		/// Initializes a new instance of the <see cref="XunitProjectAssembly"/> class.
		/// </summary>
		/// <param name="project">The project this assembly belongs to.</param>
		public XunitProjectAssembly(XunitProject project)
		{
			Project = Guard.ArgumentNotNull(project);
		}

		/// <summary>
		/// Gets or sets the assembly under test. May be <c>null</c> when the test assembly is not
		/// loaded into the current app domain.
		/// </summary>
		// TODO: Nobody is consuming this. Who should? Or should we delete it?
		public Assembly? Assembly { get; set; }

		/// <summary>
		/// Gets the assembly display name. Will return the value "&lt;dynamic&gt;" if the
		/// assembly does not have a file name.
		/// </summary>
		public string AssemblyDisplayName
		{
			get
			{
				if (AssemblyFileName != null)
					return Path.GetFileNameWithoutExtension(AssemblyFileName);

				return Assembly?.GetName()?.Name ?? "<unnamed dynamic assembly>";
			}
		}

		/// <summary>
		/// Gets or sets the assembly file name.
		/// </summary>
		public string? AssemblyFileName { get; set; }

		/// <summary>
		/// Gets or sets the config file name.
		/// </summary>
		public string? ConfigFileName { get; set; }

		/// <summary>
		/// Gets the configuration values for the test assembly.
		/// </summary>
		public TestAssemblyConfiguration Configuration { get; } = new();

		/// <summary>
		/// Gets an identifier for the current assembly. This is guaranteed to be unique, but not necessarily repeatable
		/// across runs (because it relies on <see cref="Assembly.GetHashCode"/>).
		/// </summary>
		public string Identifier
		{
			get
			{
				if (AssemblyFileName != null)
					return AssemblyFileName;

				if (Assembly == null)
					throw new InvalidOperationException($"Cannot get the UniqueID of a {GetType().FullName} instance when both {nameof(Assembly)} and {nameof(AssemblyFileName)} are null");

				return $"{Assembly.FullName ?? "<unnamed dynamic assembly>"}::{Assembly.GetHashCode()}";
			}
		}

		/// <summary>
		/// Gets the project that this project assembly belongs to.
		/// </summary>
		public XunitProject Project { get; }

		/// <summary>
		/// Gets the target framework that the test assembly was compiled against. If the value was not
		/// set, returns <see cref="AssemblyExtensions.UnknownTargetFramework"/>.
		/// </summary>
		public string TargetFramework
		{
			get => targetFramework ?? AssemblyExtensions.UnknownTargetFramework;
			set => targetFramework = Guard.ArgumentNotNull(value, nameof(TargetFramework));
		}
	}
}
