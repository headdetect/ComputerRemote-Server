using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace TVRemoteGUI.Windows.Utils {

    internal class NativesMethods {

        #region Consts

        private const int SOURCE_COPY = 0x00CC0020;
        private const int BI_RGB = 0;
        private const int DIB_RGB_COLORS = 0;

        private const int DTT_COMPOSITED = ( int ) ( 1UL << 13 );
        private const int DTT_GLOWSIZE = ( int ) ( 1UL << 11 );

        private const int DT_SINGLELINE = 0x00000020;
        private const int DT_CENTER = 0x00000001;
        private const int DT_VCENTER = 0x00000004;
        private const int DT_NOPREFIX = 0x00000800;

        public const int WM_NCHITTEST = 0x84;
        public const int WM_NCLBUTTONUP = 0x00A2;
        public const int WM_NCLBUTTONDOWN = 0x00A1;
        public const int WM_NCLBUTTONDBLCLK = 0x00A3;

        public const int HTCAPTION = 2;
        public const int HTCLIENT = 1;

        public const int S_OK = 0x0;

        public const int EP_EDITTEXT = 1;
        public const int ETS_DISABLED = 4;
        public const int ETS_NORMAL = 1;
        public const int ETS_READONLY = 6;

        public const int WM_THEMECHANGED = 0x031A;
        public const int WM_NCPAINT = 0x85;
        public const int WM_NCCALCSIZE = 0x83;

        public const int WS_EX_CLIENTEDGE = 0x200;
        public const int WVR_HREDRAW = 0x100;
        public const int WVR_VREDRAW = 0x200;
        public const int WVR_REDRAW = ( WVR_HREDRAW | WVR_VREDRAW );

        #endregion

        #region Enums/Structs

        public struct POINTAPI {
            public int x;
            public int y;
        };

        public struct DTTOPTS {
            public uint dwSize;
            public uint dwFlags;
            public uint crText;
            public uint crBorder;
            public uint crShadow;
            public int iTextShadowType;
            public POINTAPI ptShadowOffset;
            public int iBorderSize;
            public int iFontPropId;
            public int iColorPropId;
            public int iStateId;
            public int fApplyOverlay;
            public int iGlowSize;
            public IntPtr pfnDrawTextCallback;
            public int lParam;
        };

        public struct BITMAPINFOHEADER {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }

        public struct RGBQUAD {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        }

        public struct BITMAPINFO {
            public BITMAPINFOHEADER bmiHeader;
            public RGBQUAD bmiColors;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct DLLVersionInfo {
            public int cbSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformID;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct NCCALCSIZE_PARAMS {
            public RECT rgrc0, rgrc1, rgrc2;
            public IntPtr lppos;
        }

        #endregion

        #region P/Invoke API Calls

        [DllImport( "dwmapi.dll", PreserveSig = false )]
        private static extern bool DwmIsCompositionEnabled();

        [DllImport( "dwmapi.dll" )]
        private static extern void DwmExtendFrameIntoClientArea( IntPtr hWnd, ref Margins margin );

        [DllImport( "gdi32.dll", ExactSpelling = true, SetLastError = true )]
        private static extern int SaveDC( IntPtr hdc );

        [DllImport( "user32.dll", ExactSpelling = true, SetLastError = true )]
        private static extern int ReleaseDC( IntPtr hdc, int state );

        [DllImport( "user32.dll", ExactSpelling = true, SetLastError = true )]
        private static extern IntPtr GetDC( IntPtr hdc );

        [DllImport( "gdi32.dll", ExactSpelling = true, SetLastError = true )]
        private static extern IntPtr CreateCompatibleDC( IntPtr hDC );

        [DllImport( "gdi32.dll", ExactSpelling = true )]
        private static extern IntPtr SelectObject( IntPtr hDC, IntPtr hObject );

        [DllImport( "gdi32.dll", ExactSpelling = true, SetLastError = true )]
        private static extern bool DeleteObject( IntPtr hObject );

        [DllImport( "gdi32.dll", ExactSpelling = true, SetLastError = true )]
        private static extern bool DeleteDC( IntPtr hdc );

        [DllImport( "gdi32.dll" )]
        private static extern bool BitBlt( IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop );

        [DllImport( "gdi32.dll", ExactSpelling = true, SetLastError = true )]
        private static extern IntPtr CreateDIBSection( IntPtr hdc, ref BITMAPINFO pbmi, uint iUsage, int ppvBits, IntPtr hSection, uint dwOffset );

        [DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = false )]
        public static extern IntPtr SendMessage( IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam );

        [DllImport( "user32.dll", SetLastError = false )]
        public static extern bool PostMessage( IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam );

        [DllImport( "UxTheme.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode )]
        private static extern int DrawThemeTextEx( IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int iCharCount, int dwFlags, ref RECT pRect, ref DTTOPTS pOptions );

        [DllImport( "UxTheme.dll", CharSet = CharSet.Auto )]
        public static extern bool IsAppThemed();

        [DllImport( "UxTheme.dll", CharSet = CharSet.Auto )]
        public static extern bool IsThemeActive();

        [DllImport( "comctl32.dll", CharSet = CharSet.Auto )]
        public static extern int DllGetVersion( ref DLLVersionInfo version );

        [DllImport( "uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode )]
        public static extern IntPtr OpenThemeData( IntPtr hWnd, String classList );

        [DllImport( "uxtheme.dll", ExactSpelling = true )]
        public extern static Int32 CloseThemeData( IntPtr hTheme );

        [DllImport( "uxtheme", ExactSpelling = true )]
        public extern static Int32 DrawThemeBackground( IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref RECT pRect, IntPtr pClipRect );

        [DllImport( "uxtheme", ExactSpelling = true )]
        public extern static int IsThemeBackgroundPartiallyTransparent( IntPtr hTheme, int iPartId, int iStateId );

        [DllImport( "uxtheme", ExactSpelling = true )]
        public extern static Int32 GetThemeBackgroundContentRect( IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref RECT pBoundingRect, out RECT pContentRect );

        [DllImport( "uxtheme", ExactSpelling = true )]
        public extern static Int32 DrawThemeParentBackground( IntPtr hWnd, IntPtr hdc, ref RECT pRect );

        [DllImport( "uxtheme", ExactSpelling = true )]
        public extern static Int32 DrawThemeBackground( IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref RECT pRect, ref RECT pClipRect );

        [DllImport( "user32.dll" )]
        public static extern IntPtr GetWindowDC( IntPtr hWnd );

        [DllImport( "user32.dll" )]
        public static extern int ReleaseDC( IntPtr hWnd, IntPtr hDC );

        [DllImport( "user32.dll" )]
        public static extern bool GetWindowRect( IntPtr hWnd, out RECT lpRect );

        [DllImport( "gdi32.dll" )]
        public static extern int ExcludeClipRect( IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect );

        #endregion

        #region Drawing / Utils

        public static bool CanUseAero {
            get {
                try {
                    return Environment.OSVersion.Version.Major >= 6 && DwmIsCompositionEnabled();
                }
                catch {
                    return false;
                }
            }
        }

        public static void FillBlackRegion( Graphics gph, Rectangle rgn ) {

            RECT rc = new RECT() {
                Left = rgn.Left,
                Right = rgn.Right,
                Top = rgn.Top,
                Bottom = rgn.Bottom,
            };

            IntPtr destdc = gph.GetHdc();
            IntPtr Memdc = CreateCompatibleDC( destdc );
            IntPtr bitmapOld = IntPtr.Zero;

            BITMAPINFO dib = new BITMAPINFO() {
                bmiHeader = new BITMAPINFOHEADER() {
                    biHeight = -( rc.Bottom - rc.Top ),
                    biWidth = rc.Right - rc.Left,
                    biPlanes = 1,
                    biSize = Marshal.SizeOf( typeof( BITMAPINFOHEADER ) ),
                    biBitCount = 32,
                    biCompression = BI_RGB
                }
            };

            if ( SaveDC( Memdc ) != 0 ) {
                IntPtr bitmap = CreateDIBSection( Memdc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0 );

                try {

                    if ( bitmap != IntPtr.Zero ) {
                        bitmapOld = SelectObject( Memdc, bitmap );
                        BitBlt( destdc, rc.Left, rc.Top, rc.Right - rc.Left, rc.Bottom - rc.Top, Memdc, 0, 0, SOURCE_COPY );
                    }
                }
                finally {
                    SelectObject( Memdc, bitmapOld );
                    DeleteObject( bitmap );
                    ReleaseDC( Memdc, -1 );
                    DeleteDC( Memdc );
                }
            }
            gph.ReleaseHdc();
        }

        public static void DrawTextOnGlass( IntPtr hwnd, string text, Font font, Rectangle ctlrct, int iglowSize = 10 ) {
            if ( !CanUseAero ) {
                return;
            }

            RECT rc = new RECT() {
                Left = ctlrct.Left,
                Right = ctlrct.Right + 2 * iglowSize,
                Top = ctlrct.Top,
                Bottom = ctlrct.Bottom + 2 * iglowSize,
            };

            RECT rc2 = new RECT() {
                Left = 0,
                Top = 0,
                Right = rc.Right - rc.Left,
                Bottom = rc.Bottom - rc.Top
            };

            IntPtr destdc = GetDC( hwnd );
            IntPtr Memdc = CreateCompatibleDC( destdc );
            IntPtr bitmapOld = IntPtr.Zero;

            const int uFormat = DT_SINGLELINE | DT_CENTER | DT_VCENTER | DT_NOPREFIX;

            BITMAPINFO dib = new BITMAPINFO() {
                bmiHeader = new BITMAPINFOHEADER() {
                    biHeight = -( rc.Bottom - rc.Top ),
                    biWidth = rc.Right - rc.Left,
                    biPlanes = 1,
                    biSize = Marshal.SizeOf( typeof( BITMAPINFOHEADER ) ),
                    biBitCount = 32,
                    biCompression = BI_RGB
                }
            };

            if ( SaveDC( Memdc ) != 0 ) {
                IntPtr bitmap = CreateDIBSection( Memdc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0 );
                if ( bitmap != IntPtr.Zero ) {
                    bitmapOld = SelectObject( Memdc, bitmap );
                    IntPtr hFont = font.ToHfont();
                    IntPtr logfnotOld = SelectObject( Memdc, hFont );
                    try {

                        VisualStyleRenderer renderer = new VisualStyleRenderer( VisualStyleElement.Window.Caption.Active );

                        DTTOPTS dttOpts = new DTTOPTS() {
                            dwSize = ( uint ) Marshal.SizeOf( typeof( DTTOPTS ) ),
                            dwFlags = DTT_COMPOSITED | DTT_GLOWSIZE,
                            iGlowSize = iglowSize
                        };
                        DrawThemeTextEx( renderer.Handle, Memdc, 0, 0, text, -1, uFormat, ref rc2, ref dttOpts );
                        BitBlt( destdc, rc.Left, rc.Top, rc.Right - rc.Left, rc.Bottom - rc.Top, Memdc, 0, 0, SOURCE_COPY );
                    }
                    catch ( Exception e ) {
                        Logger.LogError( e );
                    }

                    finally {
                        try {
                            SelectObject( Memdc, bitmapOld );
                            SelectObject( Memdc, logfnotOld );
                            DeleteObject( bitmap );
                            DeleteObject( hFont );

                            ReleaseDC( Memdc, -1 );
                            DeleteDC( Memdc );
                        }
                        catch { }
                    }

                }
            }
        }

        public static void ExtendGlass( IntPtr handle, Rectangle bounds ) {

            if ( !CanUseAero )
                return;

            try {
                Margins m = bounds;
                DwmExtendFrameIntoClientArea( handle, ref m );
            }
            catch {
                Logger.Log( "Vista/Win7 Found, but cannot load glass frames", LogType.Debug );
            }
        }

        public static bool CanRender() {
            Type t = typeof(Application);
            System.Reflection.PropertyInfo pi = t.GetProperty("RenderWithVisualStyles");

            if (pi == null) {
                OperatingSystem os = System.Environment.OSVersion;
                if (os.Platform == PlatformID.Win32NT && (((os.Version.Major == 5) && (os.Version.Minor >= 1)) || (os.Version.Major > 5))) {
                    DLLVersionInfo version = new DLLVersionInfo { cbSize = Marshal.SizeOf ( typeof ( DLLVersionInfo ) ) };
                    if (DllGetVersion(ref version) == 0) {
                        return (version.dwMajorVersion > 5) && IsThemeActive() && IsAppThemed();
                    }
                }

                return false;
            }

            bool result = (bool)pi.GetValue(null, null);
            return result;
        }

        #endregion
    }
}
