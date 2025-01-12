# Converting Depth Video Frames into a Video Using LINQPad

This tutorial guides you through converting depth video frames into a quilt video using LINQPad and external tools like **OpenCV**, **OpenTK**, and **FFmpeg**. The process involves three major steps: converting images to quilt format, generating missing quilt frames, and combining quilt frames into a final video.

---

## Prerequisites

Ensure the following tools are installed and accessible:

- [FFmpeg](https://ffmpeg.org/download.html) for video encoding.
- OpenCVSharp4 (NuGet Package)
- OpenTK (NuGet Package)
- [NullEngine-Bridge](https://github.com/NullandKale/NullEngine-Bridge) for quilt image generation.

---

## Step 1: Convert Images to Quilt
**File:** `Convert Images To Quilt First Pass.linq`

This LINQPad script processes a folder of depth images and converts them into quilt images using the `RGBDToQuilt.exe` tool from the **NullEngine-Bridge** repository.

```csharp
var sourcePath = @"c:\\Projects\\DepthAnyVideo\\outputs\\Mina6\\";
var quiltExePath = @"C:\\Projects\\NullEngine-Bridge\\RGBDToQuilt\\publish\\RGBDToQuilt.exe";

var images = Directory.GetFiles(sourcePath, "*.png");

// Parallel processing for faster conversion
Parallel.ForEach(images, new ParallelOptions { MaxDegreeOfParallelism = 8 }, image =>
{
    var outputFileName = Path.Combine(sourcePath, "quilt", Path.GetFileName(image));
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = quiltExePath,
            Arguments = $"--input \"{image}\" --output \"{outputFileName}\" --columns 5 --rows 9 --depthiness 1.9",
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };
    process.Start();
    process.WaitForExit();
});
```

Explanation:

Converts .png images to quilt format using the RGBDToQuilt.exe.
Parallel processing speeds up the conversion process by running multiple instances simultaneously.

---

##Step 2: Generate Missing Quilt Frames
File: Gen Missing Quilt.linq

This script identifies and generates any missing quilt frames in the sequence. I found that the first script would miss frames. Perhaps the conversion failed on a few.

```csharp
var sourcePath = @"c:\\Projects\\DepthAnyVideo\\outputs\\Mina6\\";
var quiltExePath = @"C:\\Projects\\NullEngine-Bridge\\RGBDToQuilt\\publish\\RGBDToQuilt.exe";

// Counting existing images
var numImages = Directory.GetFiles(sourcePath, "*.png").Length;

// Find missing frames by checking for filename patterns
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
    
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = quiltExePath,
            Arguments = $"--input \"{image}\" --output \"{outputFileName}\" --columns 5 --rows 9 --depthiness 1.9",
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };
    process.Start();
    process.WaitForExit();
}
```
Explanation:

Scans the source directory for missing quilt frames.
Re-generates missing frames using the same RGBDToQuilt.exe tool.

---

##Step 3: Convert Quilt Images to Video
File: Convert Quilt To Video.linq

This script uses FFmpeg to combine the quilt images into a video with an audio track.

```csharp
var sourcePath = @"c:\\Projects\\DepthAnyVideo\\outputs\\Mina6\\";
var audioPath = @"C:\\Projects\\DepthAnyVideo\\demos\\MinaBass6.mp3";
var ffmpegExePath = @"C:\\Program Files\\FFMPEG\\ffmpeg.exe";
var outputVideoPath = Path.Combine(sourcePath, "Mina6a.mp4");

// Run FFmpeg to generate video from quilt images
var ffmpegProcess = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = ffmpegExePath,
        Arguments = $"-framerate 30 -i \"{Path.Combine(sourcePath, "quilt", "%06d.png")}\" -i \"{audioPath}\" -c:v libx264 -pix_fmt yuv420p \"{outputVideoPath}\"",
        UseShellExecute = false,
        CreateNoWindow = false
    }
};
ffmpegProcess.Start();
ffmpegProcess.WaitForExit();
```

Explanation:

Uses FFmpeg to create a 30 FPS video using quilt images.
The audio track is included for synchronized playback.
