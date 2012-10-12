using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using TVRemote.Utils;

namespace TVRemote.Components {
    /// <summary>
    /// A form that utilizes the vista glass API
    /// </summary>
    public partial class AeroForm : Form {

        /// <summary>
        /// Gets or sets a value indicating whether it will handle mouse events
        /// </summary>
        /// <value>
        ///   <c>true</c> if [handle mouse movement]; otherwise, <c>false</c>.
        /// </value>
        public bool HandleMouseMovement { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AeroForm"/> class.
        /// </summary>
        public AeroForm () {
            ResizeRedraw = true;
            HandleMouseMovement = true;
        }

        #region Props

        private Margins _glassArea = new Margins ( 0 );

        /// <summary>
        /// Gets or sets the glass area.
        /// </summary>
        /// <value>
        /// The glass area.
        /// </value>
        [Browsable ( false )]
        public Margins GlassArea {
            get {
                return _glassArea;
            }
            set {
                _glassArea = value;
                if ( DesignMode )
                    return;
                PrepareGlass ();
                UpdateGlass ();
            }
        }

        #endregion

        #region Glass Methods

        /// <summary>
        /// Prepares the glass for rendering.
        /// </summary>
        public void PrepareGlass () {
            PrepareGlass ( _glassArea );
        }

        /// <summary>
        /// Updates the glass.
        /// </summary>
        public void UpdateGlass () {
            UpdateGlass ( _glassArea );
        }

        /// <summary>
        /// Prepares the glass for rendering.
        /// </summary>
        /// <param name="margs">The margins.</param>
        public void PrepareGlass ( Margins margs ) {
            PrepareGlass ( CreateGraphics (), margs );
        }

        /// <summary>
        /// Updates the glass.
        /// </summary>
        /// <param name="margs">The margins.</param>
        public void UpdateGlass ( Margins margs ) {
            UpdateGlass ( Handle, margs );
        }

        /// <summary>
        /// Updates the glass.
        /// </summary>
        /// <param name="drawingPointer">The drawing handle.</param>
        /// <param name="margs">The margins.</param>
        public void UpdateGlass ( IntPtr drawingPointer, Margins margs ) {
            if ( DesignMode )
                return;

            if ( !margs.IsEmpty && Natives.CanUseAero ) {
                Natives.ExtendGlass ( drawingPointer, margs );
            }
        }

        /// <summary>
        /// Prepares the glass for rendering.
        /// </summary>
        /// <param name="graphics">The graphics canvas.</param>
        /// <param name="margs">The margins.</param>
        public void PrepareGlass ( Graphics graphics, Margins margs ) {
            if ( DesignMode )
                return;

            if ( !margs.IsEmpty && Natives.CanUseAero ) {
                Natives.FillBlackRegion ( graphics, margs );
            }
        }

        #endregion

        [DebuggerStepThrough]
        protected override void WndProc ( ref Message m ) {
            base.WndProc ( ref m );

            if ( !HandleMouseMovement )
                return;

            if ( m.Msg == Natives.WM_NCHITTEST && m.Result.ToInt32 () == Natives.HTCLIENT ) {
                uint lparam = (uint) m.LParam.ToInt32 ();
                ushort x = (ushort) lparam;
                ushort y = (ushort) ( lparam >> 16 );

                var point = this.PointToClient ( new Point ( x, y ) );
                if ( !_glassArea.IsTouchingGlass ( point ) ) {
                    m.Result = (IntPtr) Natives.HTCAPTION;
                    return;
                }
            }
        }

        private void InitializeComponent () {
            this.SuspendLayout();
            // 
            // AeroForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Name = "AeroForm";
            this.Load += new System.EventHandler(this.AeroForm_Load);
            this.ResumeLayout(false);

        }

        private void AeroForm_Load ( object sender, EventArgs e ) {

        }
    }
}