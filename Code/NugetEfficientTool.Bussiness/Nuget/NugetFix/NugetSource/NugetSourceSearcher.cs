using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using System.Threading;
using System.Threading.Tasks;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// Nuget源搜索器
    /// </summary>
    public class NugetSourceSearcher
    {
        private readonly string _sourceUrl;
        private readonly SourceRepository _sourceRepository;

        public NugetSourceSearcher(string sourceUrl)
        {
            _sourceUrl = sourceUrl;
            // 创建NuGet源
            var source = new PackageSource(_sourceUrl);
            // 创建NuGet源管理器
            _sourceRepository = Repository.Factory.GetCoreV3(source.Source);
        }

        public async Task<string> GetLatestVersionAsync(string packageName)
        {
            var resource = await _sourceRepository.GetResourceAsync<MetadataResource>();

            // 获取获取最新版本
            var latestVersion = await resource.GetLatestVersion(packageName, true, false, new SourceCacheContext(),
                new NullLogger(), new CancellationToken());

            return latestVersion?.Version.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Nuget源
        /// </summary>
        public string NugetSourceUrl => _sourceUrl;
    }
}
