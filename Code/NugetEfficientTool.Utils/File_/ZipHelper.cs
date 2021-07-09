using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Overwrite = ICSharpCode.SharpZipLib.Zip.FastZip.Overwrite;

namespace NugetEfficientTool.Utils
{
    public static class ZipHelper
    {
        #region 压缩

        public static bool CreateZip(string zipFilePath, string sourceFolder)
        {
            try
            {
                //删除已有文件
                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
                }
                //确认文件夹存在
                var zipfolder = Path.GetDirectoryName(zipFilePath);
                if (zipfolder == null)
                {
                    throw new InvalidOperationException($"文件夹{zipfolder}不存在");
                }
                if (!Directory.Exists(zipfolder))
                {
                    Directory.CreateDirectory(zipfolder);
                }
                if (!Directory.Exists(sourceFolder))
                {
                    throw new InvalidOperationException($"文件夹{sourceFolder}不存在");
                }
                //添加压缩地址到文件夹内部的判断
                if (zipfolder == sourceFolder)
                {
                    throw new InvalidOperationException($"压缩文件{zipFilePath}不能保存在文件夹内{sourceFolder},会有冲突");
                }

                var fastZip = new FastZip();
                fastZip.CreateEmptyDirectories = true;
                fastZip.CreateZip(zipFilePath, sourceFolder, true, "");
                if (File.Exists(zipFilePath))
                {
                    return true;
                }
                fastZip = null;
            }
            catch (Exception)
            {

            }
            return false;
        }
        public static void CreateZip(string zipFilePath, List<string> filenames)
        {
            if (filenames == null || filenames.Count == 0)
            {
                return;
            }
            try
            {
                using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
                {

                    s.SetLevel(9); // 压缩级别 0-9
                    //s.Password = "123"; //Zip压缩文件密码
                    byte[] buffer = new byte[4096]; //缓冲区大小
                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);
                        using (FileStream fs = File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    s.Finish();
                    s.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during processing {0}", ex);
            }
        }
        #endregion

        #region 解压

        /// <summary>
        /// 尝试解压zip文件
        /// 如果失败则会返回解压过程异常
        /// </summary>
        /// <param name="zipFileName"></param>
        /// <param name="targetDirectory"></param>
        /// <param name="fileFilter">组成<see cref="NameFilter"/>的正则表达式</param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static bool TryExtractZip(string zipFileName, string targetDirectory,
            string fileFilter, out Exception exception)
        {
            return TryExtractZip(zipFileName, targetDirectory, Overwrite.Always,
                fileFilter, out exception);
        }

        /// <summary>
        /// 尝试解压zip文件
        /// 如果失败则会返回解压过程异常
        /// </summary>
        /// <param name="zipFileName"></param>
        /// <param name="targetDirectory"></param>
        /// <param name="overwrite"></param>
        /// <param name="fileFilter">组成<see cref="NameFilter"/>的正则表达式</param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static bool TryExtractZip(string zipFileName, string targetDirectory, Overwrite overwrite,
            string fileFilter, out Exception exception)
        {
            exception = null;
            try
            {
                ExtractZip(zipFileName, targetDirectory, overwrite, fileFilter);
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        /// <summary>
        /// 解压zip文件,默认覆盖
        /// 可能抛出<exception cref="AggregateException"></exception>
        /// </summary>
        /// <param name="zipFileName"></param>
        /// <param name="targetDirectory"></param>
        /// <param name="fileFilter">组成<see cref="NameFilter"/>的正则表达式</param>
        public static void ExtractZip(string zipFileName, string targetDirectory,
            string fileFilter)
        {
            ExtractZip(zipFileName, targetDirectory, Overwrite.Always, fileFilter);
        }

        /// <summary>
        /// 解压zip文件
        /// 可能抛出<exception cref="AggregateException"></exception>
        /// </summary>
        /// <param name="zipFileName"></param>
        /// <param name="targetDirectory"></param>
        /// <param name="overwrite"></param>
        /// <param name="fileFilter">组成<see cref="NameFilter"/>的正则表达式</param>
        public static void ExtractZip(string zipFileName, string targetDirectory, Overwrite overwrite,
            string fileFilter)
        {
            var exceptions = new List<Exception>();
            var zipEvent = new FastZipEvents();
            try
            {
                zipEvent.DirectoryFailure += ExtractFailure;
                zipEvent.FileFailure += ExtractFailure;
                var fastZip = new FastZip();
                //进行不强制覆盖的解压，否则可能因为文件被占用而解压失败
                fastZip.ExtractZip(zipFileName, targetDirectory, overwrite.ToOverWrite(), null, fileFilter, "", false);
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
            finally
            {
                zipEvent.DirectoryFailure -= ExtractFailure;
                zipEvent.FileFailure -= ExtractFailure;
            }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }


            void ExtractFailure(object sender, ScanFailureEventArgs e)
            {
                e.ContinueRunning = false;
                exceptions.Add(e.Exception);
            }
        }

        public static Overwrite ToOverWrite(this Overwrite value)
        {
            switch (value)
            {
                case Overwrite.Prompt:
                    return Overwrite.Prompt;
                default:
                case Overwrite.Always:
                    return Overwrite.Always;
                case Overwrite.Never:
                    return Overwrite.Never;
            }
        }
        private static void UnZipFile(string zipFilePath)
        {
            if (!File.Exists(zipFilePath))
            {
                Console.WriteLine("Cannot find file '{0}'", zipFilePath);
                return;
            }

            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
            {

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {

                    Console.WriteLine(theEntry.Name);

                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    // create directory
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(theEntry.Name))
                        {

                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion


    }
}
