using System;

#if XUNIT_FRAMEWORK
namespace Xunit.v3
#else
namespace Xunit.Runner.v3
#endif
{
	/// <summary>
	/// This message is sent during execution to indicate that the After method of a
	/// <see cref="T:Xunit.Sdk.BeforeAfterTestAttribute"/> has completed executing.
	/// </summary>
	public class _AfterTestFinished : _TestMessage
	{
		string? attributeName;

		/// <summary>
		/// Gets or sets the fully qualified type name of the <see cref="T:Xunit.Sdk.BeforeAfterTestAttribute"/>.
		/// </summary>
		public string AttributeName
		{
			get => attributeName ?? throw new InvalidOperationException($"Attempted to get {nameof(AttributeName)} on an uninitialized '{GetType().FullName}' object");
			set => attributeName = Guard.ArgumentNotNull(nameof(AttributeName), value);
		}
	}
}
