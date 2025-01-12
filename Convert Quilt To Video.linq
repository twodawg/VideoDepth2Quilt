<Query Kind="Statements" />

var sourcePath = @"c:\Projects\DepthAnyVideo\outputs\Mina6\";
var audioPath = @"C:\Projects\DepthAnyVideo\demos\MinaBass6.mp3";

// Use ffmpeg to create a new video file using the images located at sourcePath in the quilt directory
var quiltImages = Directory.GetFiles(Path.Combine(sourcePath, "quilt"), "*.png");
var ffmpegExePath = @"C:\Program Files\FFMPEG\ffmpeg.exe";
var outputVideoPath = Path.Combine(sourcePath, "Mina6a.mp4");

var ffmpegProcess = new Process
{
	StartInfo = new ProcessStartInfo
	{
		FileName = ffmpegExePath,
		Arguments = $"-framerate 30 -i \"{Path.Combine(sourcePath, "quilt", "%06d.png")}\" -i \"{ audioPath }\" -c:v libx264 -pix_fmt yuv420p \"{outputVideoPath}\"",
		//RedirectStandardOutput = true,
		//RedirectStandardError = true,
		UseShellExecute = false,
		CreateNoWindow = false
	}
};
//ffmpegProcess.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
//ffmpegProcess.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);
ffmpegProcess.Start();
//ffmpegProcess.BeginOutputReadLine();
//ffmpegProcess.BeginErrorReadLine();
ffmpegProcess.WaitForExit();