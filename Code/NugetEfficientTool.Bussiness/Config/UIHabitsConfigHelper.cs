using Newtonsoft.Json;
using NugetEfficientTool.Utils;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// UI使用习惯
    /// </summary>
    public class UiHabitsConfigHelper
    {
        private const string YeekitSettingSectionKey = "UiHabits";

        #region 窗口大小及位置

        /// <summary>
        /// 语言方向
        /// </summary>
        private const string WindowLocationKey = "WindowLocation";
        public static WindowLocationSizeMode GetWindowLocation()
        {
            var valueJson = IniFileHelper.IniReadValue(YeekitSettingSectionKey, WindowLocationKey);
            if (string.IsNullOrEmpty(valueJson))
            {
                return null;
            }
            var windowLocationSize = JsonConvert.DeserializeObject<WindowLocationSizeMode>(valueJson);
            return windowLocationSize;
        }

        public static void SaveWindowLocation(WindowLocationSizeMode locationSize)
        {
            var jsonData = JsonConvert.SerializeObject(locationSize);
            IniFileHelper.IniWriteValue(YeekitSettingSectionKey, WindowLocationKey, jsonData);
        }

        #endregion
    }
}
