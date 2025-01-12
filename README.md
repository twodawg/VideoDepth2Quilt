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
Parallel.ForEach(images, new ParallelOptions { MaxDegreeOfParallelism = 8 }, async image =>
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
Step 2: Generate Missing Quilt Frames
File: Gen Missing Quilt.linq

This script identifies and generates any missing quilt frames in the sequence.

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
            Arguments = $"--input \"{image}\" --output \"{outputFileName}\" --columns 5 --rows 9",
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
Step 3: Convert Quilt Images to Video
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
Step 4: Customizing Quilt Generation Parameters
You can adjust the quilt generation parameters in the RGBDToQuilt.exe command:

Columns & Rows: The grid dimensions for quilt generation.
Depthiness: Controls the intensity of depth rendering.
Example Modification:


RGBDToQuilt.exe --input "frame.png" --output "quilt.png" --columns 8 --rows 8 --depthiness 2.5
Step 5: Reference to NullEngine-Bridge
The RGBDToQuilt.exe tool is part of the NullEngine-Bridge project.

How to Use:
Download from GitHub:
bash
Copy code
git clone https://github.com/NullandKale/NullEngine-Bridge.git
Build:
bash
Copy code
dotnet publish -c Release
Reference the Executable: Ensure the path to RGBDToQuilt.exe is correctly set in the scripts.
Final Workflow Summary:
Convert Images to Quilt: Process depth video frames into quilt images.
Generate Missing Frames: Fill in missing quilt frames.
Convert to Video: Compile quilt frames into a video with audio using FFmpeg.
Customize Settings: Adjust columns, rows, and depthiness for optimal results.
✅ Troubleshooting Tips:
Ensure FFmpeg and RGBDToQuilt.exe paths are correct.
Verify proper image naming conventions (000001.png, 000002.png, ...).
Adjust the MaxDegreeOfParallelism for faster processing on powerful systems.
