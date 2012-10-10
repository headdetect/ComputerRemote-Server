namespace TVRemote {
    partial class VideoForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose ( bool disposing ) {
            if ( disposing && ( components != null ) ) {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent () {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( VideoForm ) );
            this.mPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            this.btnFullScreen = new System.Windows.Forms.Button();
            ( (System.ComponentModel.ISupportInitialize) ( this.mPlayer ) ).BeginInit();
            this.SuspendLayout();
            // 
            // mPlayer
            // 
            this.mPlayer.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.mPlayer.Enabled = true;
            this.mPlayer.Location = new System.Drawing.Point( 12, 12 );
            this.mPlayer.Name = "mPlayer";
            this.mPlayer.OcxState = ( (System.Windows.Forms.AxHost.State) ( resources.GetObject( "mPlayer.OcxState" ) ) );
            this.mPlayer.Size = new System.Drawing.Size( 534, 394 );
            this.mPlayer.TabIndex = 0;
            // 
            // btnFullScreen
            // 
            this.btnFullScreen.Location = new System.Drawing.Point( 470, 431 );
            this.btnFullScreen.Name = "btnFullScreen";
            this.btnFullScreen.Size = new System.Drawing.Size( 75, 23 );
            this.btnFullScreen.TabIndex = 1;
            this.btnFullScreen.Text = "Full Screen";
            this.btnFullScreen.UseVisualStyleBackColor = true;
            this.btnFullScreen.Click += new System.EventHandler( this.btnFullScreen_Click );
            // 
            // VideoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 558, 466 );
            this.Controls.Add( this.btnFullScreen );
            this.Controls.Add( this.mPlayer );
            this.Name = "VideoForm";
            this.Text = "VideoForm";
            this.Load += new System.EventHandler( this.VideoForm_Load );
            ( (System.ComponentModel.ISupportInitialize) ( this.mPlayer ) ).EndInit();
            this.ResumeLayout( false );

        }

        #endregion

        private AxWMPLib.AxWindowsMediaPlayer mPlayer;
        private System.Windows.Forms.Button btnFullScreen;
    }
}