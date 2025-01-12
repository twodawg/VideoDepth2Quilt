<Query Kind="Statements" />

var sourcePath = @"c:\Projects\DepthAnyVideo\outputs\Mina6\";
var quiltExePath = @"C:\Projects\NullEngine-Bridge\RGBDToQuilt\publish\RGBDToQuilt.exe";

// For the sourcePath get the number of files
var numImages = Directory.GetFiles(sourcePath, "*.png").Length;

// For the sourcePath find the missing filenames given that they should all be in the style 000000.pg through 000423.png
var existingFiles = Directory.GetFiles($"{sourcePath}quilt", "*.png")
														 .Select(Path.GetFileNameWithoutExtension)
														 .Select(f => int.Parse(f))
														 .ToHashSet();

var missingFiles = Enumerable.Range(0, numImages)
														 .Where(i => !existingFiles.Contains(i))
														 .Select(i => i.ToString("D6") + ".png");

foreach (var missingImage in missingFiles)
{
	var outputFileName = Path.Combine(sourcePath, "quilt", Path.GetFileName(missingImage));
	var image = $"{sourcePath}{missingImage}";
	Console.WriteLine(image);
	
	var process = new Process
	{
		StartInfo = new ProcessStartInfo
		{
			FileName = quiltExePath,
			Arguments = $"--input \"{image}\" --output \"{outputFileName}\" --columns 5 --rows 9 --depthiness 1.9",
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true
		}
	};
	process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
	process.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);
	process.Start();
	process.BeginOutputReadLine();
	process.BeginErrorReadLine();
	process.WaitForExit();
}