<Query Kind="Statements">
  <NuGetReference>OpenCvSharp4</NuGetReference>
  <NuGetReference>OpenCvSharp4.runtime.win</NuGetReference>
  <NuGetReference Version="4.8.1">OpenTK</NuGetReference>
  <Namespace>OpenTK</Namespace>
  <Namespace>OpenTK.Windowing.Common</Namespace>
  <Namespace>OpenTK.Windowing.Desktop</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "LookingGlassSDK"

var sourcePath = @"c:\Projects\DepthAnyVideo\outputs\Mina6\";
var quiltExePath = @"C:\Projects\NullEngine-Bridge\RGBDToQuilt\publish\RGBDToQuilt.exe";

// Foreach image in sourcePath call quiltExePath with the --input argument as the image filename specify the arg --output to quilt, using a parallel foreach using 16 threads
var images = Directory.GetFiles(sourcePath, "*.png");

Parallel.ForEach(images, new ParallelOptions { MaxDegreeOfParallelism = 8 }, image =>
{
	var outputFileName = Path.Combine(sourcePath, "quilt", Path.GetFileName(image));
	var process = new Process
	{
		StartInfo = new ProcessStartInfo
		{
			FileName = quiltExePath,
			Arguments = $"--input \"{image}\" --output \"{outputFileName}\" --columns 5 --rows 9 --depthiness 1.9",
			RedirectStandardOutput = false,
			UseShellExecute = false,
			CreateNoWindow = true
		}
	};
	process.Start();
	process.WaitForExit();

});
