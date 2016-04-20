using System;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;

namespace Jasily.Api.Windows.Win32
{
    public static class Window
    {
        #region GetWindowText

        /// <summary>
        /// also see: https://msdn.microsoft.com/en-us/library/windows/desktop/ms633521(v=vs.85).aspx
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        /// <summary>
        /// alse see: https://msdn.microsoft.com/en-us/library/windows/desktop/ms633520(v=vs.85).aspx
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpString"></param>
        /// <param name="nMaxCount"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        #endregion

        #region EnumWindows

        public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr window, EnumWindowsProc callback, IntPtr i);

        #endregion

        #region FindWindow

        /// <summary>
        /// Retrieves a handle to the top-level window whose class name and window name match the specified strings.
        /// see: https://msdn.microsoft.com/en-us/library/windows/desktop/ms633499(v=vs.85).aspx
        /// </summary>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow([CanBeNull] string lpClassName, [CanBeNull] string lpWindowName);

        /// <summary>
        /// see: https://msdn.microsoft.com/en-us/library/windows/desktop/ms633500(v=vs.85).aspx
        /// </summary>
        /// <param name="parentHandle"></param>
        /// <param name="childAfter"></param>
        /// <param name="className"></param>
        /// <param name="windowTitle"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, [CanBeNull] string className, [CanBeNull] string windowTitle);

        #endregion
    }
}