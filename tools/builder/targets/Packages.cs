using System.IO;
using System.Linq;
using System.Threading.Tasks;

[Target(
	BuildTarget.Packages,
	BuildTarget.Build, BuildTarget.Build32
)]
public static class Packages
{
	public static async Task OnExecute(BuildContext context)
	{
		context.BuildStep("Creating NuGet packages");

		// Clean up any existing packages to force re-packing
		var packageFiles = Directory.GetFiles(context.PackageOutputFolder, "*.nupkg");
		foreach (var packageFile in packageFiles)
			File.Delete(packageFile);

		// Enumerate the project folders to find what to pack
		var srcFolder = Path.Join(context.BaseFolder, "src");
		var projectFolders =
			Directory.GetFiles(srcFolder, "xunit.v3.*.csproj", SearchOption.AllDirectories)
				.Where(x => !x.EndsWith(".tests.csproj") && !x.EndsWith(".NonNullable.csproj") && !x.EndsWith(".tdnet.csproj"))
				.OrderBy(x => x)
				.Select(x => Path.GetDirectoryName(x).Substring(context.BaseFolder.Length + 1));

		foreach (var projectFolder in projectFolders)
		{
			var packArgs = $"pack --nologo --no-build --configuration {context.ConfigurationText} --output {context.PackageOutputFolder} --verbosity {context.Verbosity} {projectFolder}";
			var nuspecFiles =
				Directory.GetFiles(projectFolder, "*.nuspec")
					.OrderBy(x => x)
					.Select(x => Path.GetFileName(x));

			// Pack the .nuspec file(s)
			foreach (var nuspecFile in nuspecFiles)
				await context.Exec("dotnet", $"{packArgs} -p:NuspecFile={nuspecFile}");

			// Only pack the .csproj if there's not an exact matching .nuspec file
			if (!nuspecFiles.Any(f => File.Exists(Path.Combine(projectFolder, Path.GetFileNameWithoutExtension(f) + ".csproj"))))
				await context.Exec("dotnet", packArgs);
		}
	}
}
