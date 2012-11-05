﻿using System;
using System.IO;
using System.Runtime.InteropServices;

namespace TVRemoteGUI.Windows.Interop {
    public static class NativeFileUtils {

        /// <summary>
        /// Gets the <see cref="System.DateTime"/> the file was created.
        /// </summary>
        /// <param name="filetime">The filetime.</param>
        /// <returns></returns>
        public static DateTime GetDateTime ( this System.Runtime.InteropServices.ComTypes.FILETIME filetime ) {
            long highBits = filetime.dwHighDateTime;
            highBits = highBits << 32;
            return DateTime.FromFileTimeUtc( highBits + filetime.dwLowDateTime );
        }

        [DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
        public static extern IntPtr FindFirstFileW ( string lpFileName, out WIN32_FIND_DATAW lpFindFileData );

        [DllImport( "kernel32.dll", CharSet = CharSet.Unicode )]
        public static extern bool FindNextFile ( IntPtr hFindFile, out WIN32_FIND_DATAW lpFindFileData );

        [DllImport( "kernel32.dll" )]
        public static extern bool FindClose ( IntPtr hFindFile );

        [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
        public struct WIN32_FIND_DATAW {
            public FileAttributes dwFileAttributes;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public int nFileSizeHigh;
            public int nFileSizeLow;
            public int dwReserved0;
            public int dwReserved1;
            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 260 )]
            public string cFileName;
            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 14 )]
            public string cAlternateFileName;
        }
    }


}
