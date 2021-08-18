using System;
using System.IO;

namespace NugetEfficientTool.Utils
{
    public static class VideoCompressHelper
    {
        /// <summary>
        /// 压缩视频
        /// </summary>
        /// <param name="ffmpegExePath"></param>
        /// <param name="sourceVideoPath"></param>
        /// <param name="baudRate">波特率(k)
        /// 1.值越小，转换后的视频越小。baudRate小于等于0时，默认以500k设置
        /// 2.一般500k左右即可，人眼看不到明显的闪烁
        /// 比如大小217k的MP4文件，以参数600k波特率压缩后，大小为1075k，大致压缩一倍左右。
        /// </param>
        /// <returns></returns>
        public static string CompressVideo(string ffmpegExePath, string sourceVideoPath, int baudRate)
        {
            var compressedVideoPath = string.Empty;
            if (string.IsNullOrWhiteSpace(sourceVideoPath))
            {
                return compressedVideoPath;
            }
            //默认500k
            if (baudRate <= 0)
            {
                baudRate = 500;
            }
            var tempFolder = Path.GetTempPath();
            var videoExtension = Path.GetExtension(sourceVideoPath);
            var sourceTempVideoPath = Path.Combine(tempFolder, Guid.NewGuid() + videoExtension);
            File.Delete(sourceTempVideoPath);
            File.Copy(sourceVideoPath, sourceTempVideoPath);

            compressedVideoPath = Path.Combine(tempFolder, "Compressed" + Guid.NewGuid() + videoExtension);
            File.Delete(compressedVideoPath);

            var command = $"-i {sourceTempVideoPath} -b {baudRate}k {compressedVideoPath}";
            ProcessExecuteHelper.StartProcess(ffmpegExePath, command, false, true);
            return compressedVideoPath;
        }

        /// <summary>
        /// 压缩视频
        /// </summary>
        /// <param name="ffmpegExePath"></param>
        /// <param name="sourceVideoPath"></param>
        /// <param name="baudRate">波特率(k)，建议500k以上。值越小，转换后的视频越小</param>
        /// <param name="resolutionRatio">分辨率,比如1920x1080</param>
        /// <param name="compressedVideoPath"></param>
        /// <returns></returns>
        public static string CompressVideo(string ffmpegExePath, string sourceVideoPath, int baudRate, string resolutionRatio, out string compressedVideoPath)
        {
            compressedVideoPath = string.Empty;
            if (string.IsNullOrWhiteSpace(sourceVideoPath))
            {
                return compressedVideoPath;
            }
            //默认500k
            if (baudRate <= 0)
            {
                baudRate = 500;
            }

            var tempFolder = Path.GetTempPath();
            compressedVideoPath = Path.Combine(tempFolder, Path.GetFileName(sourceVideoPath));
            //ffmpeg执行路径 -i 源文件 -b 波特率/码率 -s 分辨率()
            var command = $"-i {sourceVideoPath} -b {baudRate}k -s {resolutionRatio} {compressedVideoPath}";
            ProcessExecuteHelper.StartProcess(ffmpegExePath, command);
            return compressedVideoPath;
        }
    }
}
