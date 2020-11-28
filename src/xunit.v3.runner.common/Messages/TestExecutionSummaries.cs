using System;
using System.Collections.Generic;
using Xunit.v3;

namespace Xunit.Runner.Common
{
	/// <summary>
	/// Reports multi-assembly test execution summary information.
	/// </summary>
	public class TestExecutionSummaries : _MessageSinkMessage
	{
		/// <summary>
		/// Gets the clock time elapsed when running the tests. This may different significantly
		/// from the sum of the times reported in the summaries, if the runner chose to run
		/// the test assemblies in parallel.
		/// </summary>
		public TimeSpan ElapsedClockTime { get; set; }

		/// <summary>
		/// Gets the summaries of all the tests run. The key is the unique ID of the test
		/// assembly; the value is the summary of test execution for that assembly.
		/// </summary>
		public List<(string AssemblyUniqueID, ExecutionSummary Summary)> SummariesByAssemblyUniqueID { get; } = new List<(string AssemblyUniqueID, ExecutionSummary Summary)>();

		/// <summary>
		/// Add assembly summary information.
		/// </summary>
		/// <param name="assemblyUniqueID">The unique ID of the assembly</param>
		/// <param name="summary">The execution summary</param>
		public void Add(
			string assemblyUniqueID,
			ExecutionSummary summary) =>
				SummariesByAssemblyUniqueID.Add((assemblyUniqueID, summary));
	}
}
