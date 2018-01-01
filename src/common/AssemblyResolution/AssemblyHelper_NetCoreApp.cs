﻿#if NETCOREAPP1_0 || NETCOREAPP2_0

using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Internal.Microsoft.Extensions.DependencyModel;
using Xunit.Abstractions;

namespace Xunit
{
    /// <summary>
    /// This class provides assistance with assembly resolution for missing assemblies.
    /// </summary>
    class AssemblyHelper : AssemblyLoadContext, IDisposable
    {
        readonly DependencyContextAssemblyCache assemblyCache;
        readonly IMessageSink internalDiagnosticsMessageSink;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetCoreAssemblyDependencyResolver"/> class.
        /// </summary>
        /// <param name="assemblyFileName">The path to the assembly</param>
        /// <param name="internalDiagnosticsMessageSink">An optional message sink for use with internal diagnostics messages;
        /// may pass <c>null</c> for no internal diagnostics messages</param>
        public AssemblyHelper(string assemblyFileName, IMessageSink internalDiagnosticsMessageSink)
        {
            this.internalDiagnosticsMessageSink = internalDiagnosticsMessageSink;

            Guard.FileExists(nameof(assemblyFileName), assemblyFileName);

            var assembly = LoadFromAssemblyPath(assemblyFileName);
            Guard.ArgumentValid(nameof(assemblyFileName), $"Could not load assembly '{assemblyFileName}'", assembly != null);

            var dependencyContext = DependencyContext.Load(assembly);
            Guard.ArgumentValid(nameof(assemblyFileName), $"Could not create dependency context for assembly '{assemblyFileName}'", dependencyContext != null);

            var assemblyFolder = Path.GetDirectoryName(assemblyFileName);
            assemblyCache = new DependencyContextAssemblyCache(assemblyFolder, dependencyContext, internalDiagnosticsMessageSink);

            Default.Resolving += OnResolving;
        }

        /// <inheritdoc/>
        public void Dispose()
            => Default.Resolving -= OnResolving;

        /// <inheritdoc/>
        protected override Assembly Load(AssemblyName assemblyName)
            => Default.LoadFromAssemblyName(assemblyName);

        /// <inheritdoc/>
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var result = assemblyCache.LoadUnmanagedLibrary(unmanagedDllName, path => LoadUnmanagedDllFromPath(path));
            if (result == default)
                result = base.LoadUnmanagedDll(unmanagedDllName);

            return result;
        }

        Assembly OnResolving(AssemblyLoadContext context, AssemblyName name)
            => assemblyCache.LoadManagedDll(name.Name, path => LoadFromAssemblyPath(path));

        /// <summary>
        /// Subscribes to the appropriate assembly resolution event, to provide automatic assembly resolution for
        /// an assembly and any of its dependencies. Depending on the target platform, this may include the use
        /// of the .deps.json file generated during the build process.
        /// </summary>
        /// <returns>An object which, when disposed, un-subscribes.</returns>
        public static IDisposable SubscribeResolveForAssembly(string assemblyFileName, IMessageSink internalDiagnosticsMessageSink = null)
            => new AssemblyHelper(assemblyFileName, internalDiagnosticsMessageSink);

        /// <summary>
        /// Subscribes to the appropriate assembly resolution event, to provide automatic assembly resolution for
        /// an assembly and any of its dependencies. Depending on the target platform, this may include the use
        /// of the .deps.json file generated during the build process.
        /// </summary>
        /// <returns>An object which, when disposed, un-subscribes.</returns>
        public static IDisposable SubscribeResolveForAssembly(Type typeInAssembly, IMessageSink internalDiagnosticsMessageSink = null)
            => new AssemblyHelper(typeInAssembly.GetTypeInfo().Assembly.Location, internalDiagnosticsMessageSink);
    }
}

#endif
