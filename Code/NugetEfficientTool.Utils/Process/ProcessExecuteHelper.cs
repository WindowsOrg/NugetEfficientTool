using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetEfficientTool.Utils
{
    /// <summary>
    /// 进程执行辅助类
    /// </summary>
    public class ProcessExecuteHelper
    {
        private const string ExistStr = "&exit";
        /// <summary>
        /// 执行CMD命令
        /// </summary>
        /// <param name="cmdCommands"></param>
        /// <returns></returns>
        public static string ExecuteCmd(string cmdCommands)
        {
            //创建一个进程
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.CreateNoWindow = true;//不显示程序窗口
            process.StartInfo.UseShellExecute = false;//是否使用操作系统shell启动
            process.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            process.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            process.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            process.Start();//启动程序

            process.StandardInput.WriteLine(cmdCommands + ExistStr);
            process.StandardInput.AutoFlush = true;

            string strOuput = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();

            return strOuput;
        }
        /// <summary>
        /// 执行参数
        /// </summary>
        /// <param name="exefile"></param>
        /// <param name="args"></param>
        /// <param name="useShell"></param>
        /// <param name="creatNoWindow"></param>
        public static void StartProcess(string exefile, string args, bool useShell = false, bool creatNoWindow = true)
        {
            var processInfo = new ProcessStartInfo
            {
                CreateNoWindow = creatNoWindow,
                UseShellExecute = useShell,
                WindowStyle = ProcessWindowStyle.Minimized,
                FileName = exefile,
                Arguments = args,
            };
            var process = Process.Start(processInfo);
            process.WaitForExit();
            process.Close();
        }
        /// <summary>
        /// 执行参数
        /// </summary>
        /// <param name="exefile"></param>
        public static void StartProcess(string exefile)
        {
            var processInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Minimized,
                FileName = exefile,
                Arguments = string.Empty,
            };
            var process = Process.Start(processInfo);
            process?.Close();
        }
    }
}
