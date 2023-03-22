using Kybs0.Log;

namespace NugetEfficientTool.Business
{
    public static class CustomText
    {
        public static UserPath Path { get; } = new UserPath();
        public static Logger Log { get; set; }
        public static INotificationProvider Notification { get; set; }=new NotificationProvider();

        public const string ProjectName = "NugetEfficientTool";
        public const string PackagesConfigSearchPattern = "packages.config";
        public const string CsProjSearchPattern = "*.csproj";

        public const string FixSuccessKey = "对 {0} 执行了以下修复操作：";
        public const string FixErrorKey = "修复失败";
    }
}
