using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows;
using System.Windows.Interop;

namespace TVRemoteGUI.Windows.Utils {
    /// <summary>
    /// A form that utilizes the vista glass API
    /// </summary>
    public class AeroWindow : Window {

        /// <summary>
        /// Gets or sets a value indicating whether it will handle mouse events
        /// </summary>
        /// <value>
        ///   <c>true</c> if [handle mouse movement]; otherwise, <c>false</c>.
        /// </value>
        public bool HandleMouseMovement { get; set; }

        private HwndSource _winSource;

        private readonly HwndSourceHook _hook;


        /// <summary>
        /// Initializes a new instance of the <see cref="AeroWindow"/> class.
        /// </summary>
        public AeroWindow () {
            HandleMouseMovement = true;
            _hook = WndProc;
        }

        #region Props

        private Margins _glassArea = new Margins( 0 );

        /// <summary>
        /// Gets or sets the glass area.
        /// </summary>
        /// <value>
        /// The glass area.
        /// </value>
        [Browsable( false )]
        public Margins GlassArea {
            get {
                return _glassArea;
            }
            set {
                _glassArea = value;


                UpdateGlass();
            }
        }

        #endregion

        #region Glass Methods


        /// <summary>
        /// Updates the glass.
        /// </summary>
        public void UpdateGlass () {
            if ( DesignerProperties.GetIsInDesignMode( this ) && !Natives.CanUseAero ) {
                Background = SystemColors.WindowBrush;
                return;
            }

            if(_winSource != null) {
               _winSource.RemoveHook(_hook);
            }

            Background = Brushes.Transparent;

            WindowInteropHelper helper = new WindowInteropHelper( this );
            _winSource = HwndSource.FromHwnd( helper.Handle );

            if ( _winSource != null ) {
                _winSource.AddHook(_hook);


                if (_winSource.CompositionTarget != null ) {
                    _winSource.CompositionTarget.BackgroundColor = Colors.Transparent;
                    Natives.ExtendGlass( _winSource.Handle, _glassArea );
                }
            }
        }

        [System.Diagnostics.DebuggerStepThrough]
        private IntPtr WndProc ( IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled ) {
            if ( msg == 0x031E ) {
                UpdateGlass();
                handled = true;
            }
            return IntPtr.Zero;
        }

        #endregion



    }
}