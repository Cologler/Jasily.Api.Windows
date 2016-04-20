using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;

namespace Jasily.Api.Windows
{
    public struct Window
    {
        public IntPtr Ptr { get; }

        private Window(IntPtr ptr)
        {
            this.Ptr = ptr;
        }

        #region get window text

        public string GetText()
        {
            if (this.Ptr == IntPtr.Zero) return null;
            var length = Win32.Window.GetWindowTextLength(this.Ptr);
            var sb = new StringBuilder(length + 1);
            Win32.Window.GetWindowText(this.Ptr, sb, sb.Capacity);
            return sb.ToString();
        }

        #endregion

        #region get child

        public Window? GetChild([CanBeNull] string className, [CanBeNull] string windowName)
        {
            if (this.Ptr == IntPtr.Zero) return null;
            var ptr = Win32.Window.FindWindowEx(this.Ptr, IntPtr.Zero, className, windowName);
            return WrapWindow(ptr);
        }

        public Window? GetChildAfter([CanBeNull] string className, [CanBeNull] string windowName, Window childAfter)
        {
            if (this.Ptr == IntPtr.Zero) return null;
            var ptr = Win32.Window.FindWindowEx(this.Ptr, childAfter.Ptr, className, windowName);
            return WrapWindow(ptr);
        }

        #endregion

        #region get childs

        public IEnumerable<Window> GetChilds()
            => this.Ptr == IntPtr.Zero ? Enumerable.Empty<Window>() : WrapWindow(GetChildWindows(this.Ptr));

        private static IEnumerable<IntPtr> GetChildWindows(IntPtr parent)
        {
            var result = new List<IntPtr>();
            var listHandle = GCHandle.Alloc(result);
            try
            {
                Win32.Window.EnumChildWindows(parent, EnumWindow, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated) listHandle.Free();
            }
            return result;
        }

        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            var gch = GCHandle.FromIntPtr(pointer);
            var list = gch.Target as List<IntPtr>;
            if (list == null) throw new InvalidCastException();
            list.Add(handle);
            return true;
        }

        #endregion

        private static IEnumerable<Window> WrapWindow(IEnumerable<IntPtr> ptrs)
            => from ptr in ptrs where ptr != IntPtr.Zero select new Window(ptr);

        private static Window? WrapWindow(IntPtr ptr) => ptr == IntPtr.Zero ? (Window?)null : new Window(ptr);

        public static Window? Find([CanBeNull] string className, [CanBeNull] string windowName)
            => WrapWindow(Win32.Window.FindWindow(className, windowName));

        public static Window? DeepFindByClassNames([NotNull] params string[] classNames)
        {
            if (classNames == null) throw new ArgumentNullException(nameof(classNames));
            if (classNames.Length == 0) return null;
            var ptr = IntPtr.Zero;
            for (var i = 0; i < classNames.Length; i++)
            {
                ptr = Win32.Window.FindWindowEx(ptr, IntPtr.Zero, classNames[i], null);
                if (ptr == IntPtr.Zero) return null;
            }
            return WrapWindow(ptr);
        }

        public static Window? DeepFindByQueryParameter([NotNull] params WindowQueryParameter[] query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (query.Length == 0) return null;
            var ptr = IntPtr.Zero;
            for (var i = 0; i < query.Length; i++)
            {
                Win32.Window.FindWindowEx(ptr, IntPtr.Zero, query[i].ClassName, query[i].WindowName);
                if (ptr == IntPtr.Zero) return null;
            }
            return WrapWindow(ptr);
        }

        public struct WindowQueryParameter
        {
            [CanBeNull]
            public string ClassName { get; set; }

            [CanBeNull]
            public string WindowName { get; set; }
        }
    }
}
