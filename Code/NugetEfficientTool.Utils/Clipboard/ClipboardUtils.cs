using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NugetEfficientTool.Utils
{
    public static class ClipboardUtils
    {
        public static bool EmptyClipboard()
        {
            if (!Clipboard32Apis.OpenClipboard(IntPtr.Zero))
            {
                //SetText(text); 
                return false;
            }
            Clipboard32Apis.EmptyClipboard();
            Clipboard32Apis.CloseClipboard();
            return true;
        }
        public static bool SetText(string text)
        {
            if (!Clipboard32Apis.OpenClipboard(IntPtr.Zero))
            {
                //SetText(text); 
                return false;
            }
            Clipboard32Apis.EmptyClipboard();
            Clipboard32Apis.SetClipboardData((int)ClipboardFormat.CF_UNICODETEXT, Marshal.StringToHGlobalUni(text));
            Clipboard32Apis.CloseClipboard();
            return true;
        }

        public static string GetText()
        {
            string value = string.Empty;
            Clipboard32Apis.OpenClipboard(IntPtr.Zero);
            if (Clipboard32Apis.IsClipboardFormatAvailable((int)ClipboardFormat.CF_UNICODETEXT))
            {
                IntPtr ptr = Clipboard32Apis.GetClipboardData((int)ClipboardFormat.CF_UNICODETEXT);
                if (ptr != IntPtr.Zero)
                {
                    value = Marshal.PtrToStringUni(ptr);
                }
            }
            Clipboard32Apis.CloseClipboard();
            return value;
        }

        public static bool SetDataObject(object data)
        {
            if (!Clipboard32Apis.OpenClipboard(IntPtr.Zero))
            {
                return false;
            }
            Clipboard32Apis.EmptyClipboard();
            //Clipboard32Apis.SetClipboardData();
            Clipboard32Apis.CloseClipboard();
            return true;
        }

        public static string GetDataObject()
        {
            string value = string.Empty;
            ClipboardFormat format;
            Clipboard32Apis.OpenClipboard(IntPtr.Zero);

            foreach (ClipboardFormat clipboardFormat in Enum.GetValues(typeof(ClipboardFormat)))
            {
                if (Clipboard32Apis.IsClipboardFormatAvailable((int)clipboardFormat))
                {
                    IntPtr ptr = Clipboard32Apis.GetClipboardData((int)clipboardFormat);
                    if (ptr != IntPtr.Zero)
                    {
                        GCHandle handle2 = GCHandle.FromIntPtr(ptr);
                        var obj2 = handle2.Target;
                        handle2.Free();
                        format = clipboardFormat;
                        Console.WriteLine(obj2 + ","+ format);
                    }
                }
            }
            Clipboard32Apis.CloseClipboard();
            return value;
        }
    }
}
