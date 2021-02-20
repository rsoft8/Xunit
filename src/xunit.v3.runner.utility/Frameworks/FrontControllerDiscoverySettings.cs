﻿using Xunit.Internal;
using Xunit.Runner.Common;
using Xunit.v3;

namespace Xunit
{
	/// <summary>
	/// Contains the information used to run test discovery.
	/// </summary>
	// TODO: This is current ctor-based because everything else uses factories,
	// are we comfortable with that design?
	public class FrontControllerDiscoverySettings
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FrontControllerDiscoverySettings"/> class.
		/// </summary>
		/// <param name="options">The discovery options</param>
		/// <param name="filters">The optional filters (when not provided, finds all tests)</param>
		public FrontControllerDiscoverySettings(
			_ITestFrameworkDiscoveryOptions options,
			XunitFilters? filters = null)
		{
			Options = Guard.ArgumentNotNull(nameof(options), options);
			Filters = filters ?? new XunitFilters();
		}

		/// <summary>
		/// Get the test case filters used during discovery.
		/// </summary>
		public XunitFilters Filters { get; }

		/// <summary>
		/// The options used during discovery.
		/// </summary>
		public _ITestFrameworkDiscoveryOptions Options { get; }
	}
}
