﻿using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Internal;

namespace Xunit.Sdk
{
	/// <summary>
	/// The implementation of <see cref="ITraitDiscoverer"/> which returns the trait values
	/// for <see cref="TraitAttribute"/>.
	/// </summary>
	public class TraitDiscoverer : ITraitDiscoverer
	{
		/// <inheritdoc/>
		public virtual IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
		{
			Guard.ArgumentNotNull(nameof(traitAttribute), traitAttribute);

			var ctorArgs = traitAttribute.GetConstructorArguments().Cast<string>().ToList();

			yield return new KeyValuePair<string, string>(ctorArgs[0], ctorArgs[1]);
		}
	}
}
