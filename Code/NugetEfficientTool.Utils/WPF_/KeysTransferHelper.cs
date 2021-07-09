using System;
using System.Windows.Forms;
using System.Windows.Input;

namespace NugetEfficientTool.Utils
{
    public static class KeysTransferHelper
    {
        public static bool TryConvertToWinformKey(Key wpfKey, out Keys winformKey)
        {
            winformKey = Keys.None;
            if (wpfKey == Key.None)
            {
                return false;
            }
            var virtualKey = KeyInterop.VirtualKeyFromKey(wpfKey);
            return Enum.TryParse(virtualKey.ToString(), out winformKey);
        }
        public static bool TryConvertToWpfKey(Keys winformKey, out Key wpfKey)
        {
            wpfKey = Key.None;
            if (winformKey == Keys.None)
            {
                return false;
            }
            wpfKey = KeyInterop.KeyFromVirtualKey((int)winformKey);
            return wpfKey != Key.None;
        }

        public static bool TryConvertToWinformKey(string wpfKeyString, out Keys winformKey)
        {
            winformKey = Keys.None;
            if (Enum.TryParse<Key>(wpfKeyString, out var wpfKey) && wpfKey != Key.None &&
                TryConvertToWinformKey(wpfKey, out winformKey))
            {
                return true;
            }
            return false;
        }
        public static bool TryConvertToWpfKey(string winformKeyString, out Key wpfKey)
        {
            wpfKey = Key.None;
            if (Enum.TryParse<Keys>(winformKeyString, out var winformKey) && winformKey != Keys.None &&
                TryConvertToWpfKey(winformKey, out wpfKey))
            {
                return true;
            }
            return false;
        }
    }
}
