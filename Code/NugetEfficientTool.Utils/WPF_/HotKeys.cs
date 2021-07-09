using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace NugetEfficientTool.Utils
{
    public class HotKeys
    {
        #region 注册快捷键

        /// <summary>
        /// 注册快捷键
        /// </summary>
        /// <param name="modifiers"></param>
        /// <param name="key"></param>
        public void Register(List<HotkeyModifiers> modifiers, Key key)
        {
            if (KeysTransferHelper.TryConvertToWinformKey(key, out var winformKey))
            {
                Register(IntPtr.Zero, modifiers, winformKey);
            }
        }

        /// <summary>
        /// 注册快捷键
        /// </summary>
        /// <param name="modifiers"></param>
        /// <param name="key"></param>
        public void Register(List<HotkeyModifiers> modifiers, Keys key)
        {
            Register(IntPtr.Zero, modifiers, key);
        }
        /// <summary>
        /// 注册快捷键
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="modifiers"></param>
        /// <param name="key"></param>
        /// <param name="callBack"></param>
        public void Register(IntPtr hWnd, List<HotkeyModifiers> modifiers, Keys key, HotKeyCallBackHanlder callBack = null)
        {
            int modifyKeys = 0;
            foreach (var hotkeyModifierse in modifiers)
            {
                modifyKeys += (int)hotkeyModifierse;
            }
            var registerRecord = _hotkeyRegisterRecords.FirstOrDefault(i => i.IntPtr == hWnd && i.Modifiers == modifyKeys && i.Key == key);
            if (registerRecord != null)
            {
                UnregisterHotKey(hWnd, registerRecord.Id);
                _hotkeyRegisterRecords.Remove(registerRecord);
            }
            int id = registerId++;
            if (!RegisterHotKey(hWnd, id, modifyKeys, key))
                throw new Exception("注册失败！");
            _hotkeyRegisterRecords.Add(new HotkeyRegisterRecord()
            {
                Id = id,
                IntPtr = hWnd,
                Modifiers = modifyKeys,
                Key = key,
                CallBack = callBack
            });
        }

        #endregion

        #region 注销快捷键

        /// <summary>
        /// 注销快捷键
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="modifiers"></param>
        /// <param name="key"></param>
        public void UnRegister(IntPtr hWnd, List<HotkeyModifiers> modifiers, Keys key)
        {
            int modifyKeys = 0;
            foreach (var hotkeyModifierse in modifiers)
            {
                modifyKeys += (int)hotkeyModifierse;
            }
            var registerRecord = _hotkeyRegisterRecords.FirstOrDefault(i => i.IntPtr == hWnd && i.Modifiers == modifyKeys && i.Key == key);
            if (registerRecord != null)
            {
                UnregisterHotKey(hWnd, registerRecord.Id);
                _hotkeyRegisterRecords.Remove(registerRecord);
            }
        }
        /// <summary>
        /// 注销快捷键
        /// </summary>
        /// <param name="modifiers"></param>
        /// <param name="key"></param>
        public void UnRegister(List<HotkeyModifiers> modifiers, Keys key)
        {
            int modifyKeys = 0;
            foreach (var hotkeyModifierse in modifiers)
            {
                modifyKeys += (int)hotkeyModifierse;
            }
            var registerRecord = _hotkeyRegisterRecords.FirstOrDefault(i => i.IntPtr == IntPtr.Zero && i.Modifiers == modifyKeys && i.Key == key);
            if (registerRecord != null)
            {
                UnregisterHotKey(IntPtr.Zero, registerRecord.Id);
                _hotkeyRegisterRecords.Remove(registerRecord);
            }
        }
        /// <summary>
        /// 注销快捷键
        /// </summary>
        /// <param name="hWnd"></param>
        public void UnRegister(IntPtr hWnd)
        {
            var registerRecords = _hotkeyRegisterRecords.Where(i => i.IntPtr == hWnd);
            //注销所有
            foreach (var registerRecord in registerRecords)
            {
                UnregisterHotKey(hWnd, registerRecord.Id);
                _hotkeyRegisterRecords.Remove(registerRecord);
            }
        }

        #endregion

        #region 快捷键消息处理

        // 快捷键消息处理
        public void ProcessHotKey(Message message)
        {
            ProcessHotKey(message.Msg, message.WParam);
        }

        /// <summary>
        /// 快捷键消息处理
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="wParam">消息Id</param>
        public void ProcessHotKey(int msg, IntPtr wParam)
        {
            if (msg == 0x312)
            {
                int id = wParam.ToInt32();
                var registerRecord = _hotkeyRegisterRecords.FirstOrDefault(i => i.Id == id);
                registerRecord?.CallBack?.Invoke();
            }
        }

        #endregion

        #region MyRegion

        //引入系统API
        [DllImport("user32.dll")]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, int modifiers, Keys vk);
        [DllImport("user32.dll")]
        static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        //标识-区分不同的快捷键
        int registerId = 10;
        //添加key值注册字典，后续调用时有回调处理函数
        private readonly List<HotkeyRegisterRecord> _hotkeyRegisterRecords = new List<HotkeyRegisterRecord>();
        public delegate void HotKeyCallBackHanlder();

        #endregion

    }

    public class HotkeyRegisterRecord
    {
        public IntPtr IntPtr { get; set; }
        public int Modifiers { get; set; }
        public Keys Key { get; set; }
        public int Id { get; set; }
        public HotKeys.HotKeyCallBackHanlder CallBack { get; set; }
    }
    //组合控制键
    public enum HotkeyModifiers
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }
}
