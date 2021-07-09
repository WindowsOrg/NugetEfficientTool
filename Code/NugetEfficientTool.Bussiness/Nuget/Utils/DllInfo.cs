using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace NugetEfficientTool.Business
{
    public class NugetDllInfo
    {
        /// <summary>
        /// 构造 Nuget Dll 信息
        /// </summary>
        /// <param name="dllPath">Dll 绝对路径</param>
        /// <param name="dllFullName">Dll 完整名称</param>
        public NugetDllInfo(string dllPath, string dllFullName)
        {
            DllPath = dllPath ?? throw new ArgumentNullException(nameof(dllPath));
            var fullName = string.Empty;
            try
            {
                if (File.Exists(DllPath))
                {
                    var dllFile = Assembly.LoadFile(DllPath);
                    fullName = $"{dllFile.FullName.Replace(", PublicKeyToken=null", string.Empty)}, processorArchitecture=MSIL";
                }
            }
            catch (Exception)
            {
                // ignored
            }
            DllFullName = string.IsNullOrEmpty(fullName) ? dllFullName ?? throw new ArgumentNullException(nameof(dllFullName)) : fullName;
        }

        /// <summary>
        /// Dll 绝对路径
        /// </summary>
        public string DllPath { get; }

        /// <summary>
        /// Dll 完整名称
        /// </summary>
        public string DllFullName { get; }
    }
}