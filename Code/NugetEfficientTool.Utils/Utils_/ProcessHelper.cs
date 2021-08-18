using System;
using System.Diagnostics;
using System.Linq;

namespace NugetEfficientTool.Utils
{
    public static class ProcessHelper
    {
        /// <summary>
        /// 关闭进程
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="killCurrentProcess"></param>
        public static void KillProcess(string processName, bool killCurrentProcess = false)
        {
            try
            {
                var processList = Process.GetProcesses().Where(process => process.ProcessName.ToUpper() == processName.ToUpper());
                //删除所有同名进程
                Process currentProcess = Process.GetCurrentProcess();
                foreach (Process thisproc in processList)
                {
                    if (!killCurrentProcess && thisproc.Id == currentProcess.Id)
                    {
                        continue;
                    }
                    thisproc.Kill();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static bool ExistProcess(string processName)
        {
            var processList = Process.GetProcesses();
            foreach (Process process in processList)
            {
                if (process.ProcessName.ToUpper() == processName.ToUpper())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
