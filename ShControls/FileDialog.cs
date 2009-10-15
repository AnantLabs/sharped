using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace ShControls
{
    public class FileDialog
    {
        public bool AddExtention;
        private char[] bufferMem;
        public bool CheckFileExists;
        public bool CheckPathExists;
        public string DefaultExt;
        public bool DereferenceLinks = true;
        public string FileName;
        public string[] FileNames;
        public string Filter;
        public int FilterIndex;
        public string InitialDirectory;
        private GCHandle memHandle;
        public bool MultiSelect;
        public bool ReadOnlyChecked;
        public bool RestoreDirectory;
        public bool ShowReadOnly;
        public object Tag;
        public string Title;
        public bool ValidateNames = true;

        protected NativeMethods.OpenFileName ToOfn(Window owner)
        {
            var ofn = new NativeMethods.OpenFileName();
            ofn.structSize = Marshal.SizeOf(ofn);
            ofn.dlgOwner = ((HwndSource) PresentationSource.FromVisual(owner)).Handle;
            if (!string.IsNullOrEmpty(Filter))
            {
                var sb = new StringBuilder();
                string[] parts = Filter.Split('|');
                for (int i = 1; i < parts.Length; i += 2)
                {
                    sb.Append(parts[i - 1]);
                    sb.Append('\0');
                    sb.Append(parts[i]);
                    sb.Append('\0');
                }
                sb.Append('\0');
                sb.Append('\0');
                ofn.filter = sb.ToString();
            }
            ofn.filterIndex = FilterIndex;
            bufferMem = new char[64001];
            memHandle = GCHandle.Alloc(bufferMem, GCHandleType.Pinned);
            ofn.file = memHandle.AddrOfPinnedObject();
            ofn.maxFile = 64000;
            ofn.title = Title;
            ofn.flags =
                (int) NativeMethods.OpenFileFlags.OFN_EXPLORER |
                (CheckFileExists ? (int) NativeMethods.OpenFileFlags.OFN_FILEMUSTEXIST : 0) |
                (CheckPathExists ? (int) NativeMethods.OpenFileFlags.OFN_PATHMUSTEXIST : 0) |
                (DereferenceLinks ? 0 : (int) NativeMethods.OpenFileFlags.OFN_NODEREFERENCELINKS) |
                (MultiSelect ? (int) NativeMethods.OpenFileFlags.OFN_ALLOWMULTISELECT : 0) |
                (ReadOnlyChecked ? (int) NativeMethods.OpenFileFlags.OFN_READONLY : 0) |
                (RestoreDirectory ? (int) NativeMethods.OpenFileFlags.OFN_NOCHANGEDIR : 0) |
                (ShowReadOnly ? 0 : (int) NativeMethods.OpenFileFlags.OFN_HIDEREADONLY) |
                (ValidateNames ? 0 : (int) NativeMethods.OpenFileFlags.OFN_NOVALIDATE);
            ofn.defExt = DefaultExt;
            return ofn;
        }

        protected void FromOfn(NativeMethods.OpenFileName ofn)
        {
            ReadOnlyChecked = (ofn.flags & (int) NativeMethods.OpenFileFlags.OFN_READONLY) != 0;
            FilterIndex = ofn.filterIndex;
            if (ofn.fileOffset > 0 && bufferMem[ofn.fileOffset - 1] == '\0')
            {
                var result = new List<string>();
                int l = 0;
                for (; l < bufferMem.Length && bufferMem[l] != '\0'; ++l)
                {
                }
                var path = new string(bufferMem, 0, l);
                while (true)
                {
                    ++l;
                    int s = l;
                    for (; l < bufferMem.Length && bufferMem[l] != '\0'; ++l)
                    {
                    }
                    if (l < s + 2) break;
                    var name = new string(bufferMem, s, l - s);
                    result.Add(Path.Combine(path, name));
                }
                FileNames = result.ToArray();
                FileName = FileNames[0];
            }
            else
            {
                int l = 0;
                for (; l < bufferMem.Length && bufferMem[l] != '\0'; ++l)
                {
                }
                FileName = new string(bufferMem, 0, l);
                FileNames = new[] {FileName};
            }
            FreeOfn(ofn);
        }

        protected void FreeOfn(NativeMethods.OpenFileName ofn)
        {
            memHandle.Free();
            bufferMem = null;
        }

        #region Nested type: NativeMethods

        protected sealed class NativeMethods
        {
            #region OpenFileFlags enum

            public enum OpenFileFlags
            {
                OFN_READONLY = 0x1,
                OFN_OVERWRITEPROMPT = 0x2,
                OFN_HIDEREADONLY = 0x4,
                OFN_NOCHANGEDIR = 0x8,
                OFN_SHOWHELP = 0x10,
                OFN_ENABLEHOOK = 0x20,
                OFN_ENABLETEMPLATE = 0x40,
                OFN_ENABLETEMPLATEHANDLE = 0x80,
                OFN_NOVALIDATE = 0x100,
                OFN_ALLOWMULTISELECT = 0x200,
                OFN_EXTENSIONDIFFERENT = 0x400,
                OFN_PATHMUSTEXIST = 0x800,
                OFN_FILEMUSTEXIST = 0x1000,
                OFN_CREATEPROMPT = 0x2000,
                OFN_SHAREAWARE = 0x4000,
                OFN_NOREADONLYRETURN = 0x8000,
                OFN_NOTESTFILECREATE = 0x10000,
                OFN_NONETWORKBUTTON = 0x20000,
                OFN_NOLONGNAMES = 0x40000,
                OFN_EXPLORER = 0x80000,
                OFN_NODEREFERENCELINKS = 0x100000,
                OFN_LONGNAMES = 0x200000
            }

            #endregion

            [DllImport("Comdlg32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetOpenFileNameW", ExactSpelling = true)
            ]
            public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

            [DllImport("Comdlg32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetSaveFileNameW", ExactSpelling = true)
            ]
            public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);

            #region Nested type: OpenFileName

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public class OpenFileName
            {
                public int structSize;
                public IntPtr dlgOwner = IntPtr.Zero;
                public IntPtr instance = IntPtr.Zero;

                public String filter;
                public String customFilter;
                public int maxCustFilter;
                public int filterIndex;

                public IntPtr file;
                public int maxFile;

                public String fileTitle;
                public int maxFileTitle;

                public String initialDir;

                public String title;

                public int flags;
                public short fileOffset;
                public short fileExtension;

                public String defExt;

                public IntPtr custData = IntPtr.Zero;
                public IntPtr hook = IntPtr.Zero;

                public String templateName;

                public IntPtr reservedPtr = IntPtr.Zero;
                public int reservedInt;
                public int flagsEx;
            }

            #endregion
        }

        #endregion
    }
}