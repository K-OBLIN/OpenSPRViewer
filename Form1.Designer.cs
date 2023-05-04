namespace OpenSPRViewer;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Member Variables
    #region ToolTip
    private ToolTip m_ToolTip = null;
    #endregion

    #region MetroStyleManager
    private MetroFramework.Components.MetroStyleManager m_MetroStyleManager = null;
    #endregion

    #region Toggle
    private MetroFramework.Controls.MetroToggle m_ThemeToggle = null;
    #endregion

    #region ComboBox
    private MetroFramework.Controls.MetroComboBox m_ColorStyleComboBox = null;
    private MetroFramework.Controls.MetroComboBox m_ImageDataComboBox = null;
    private MetroFramework.Controls.MetroComboBox m_ColorPaletteComboBox = null;
    #endregion

    #region PictureBox
    private PictureBox m_ImagePictureBox = null;
    #endregion

    #region TrackBar
    private MetroFramework.Controls.MetroTrackBar m_StartFrameTrackBar = null;
    private MetroFramework.Controls.MetroTrackBar m_EndFrameTrackBar = null;
    #endregion

    #region ProgressBar
    private MetroFramework.Controls.MetroProgressBar m_ProcessProgressBar = null;
    #endregion

    #region Context Menu
    private MetroFramework.Controls.MetroContextMenu m_ContextMenu = null;

    #region Menu Item
    private ToolStripMenuItem m_ConvertMenuItem = null;

    #region SPR Menu Item
    private ToolStripMenuItem m_ConvertSPRMenuItem = null;
    private ToolStripMenuItem m_ConvertSPR2BMPMenuItem = null;
    private ToolStripMenuItem m_ConvertSPR2FrameBMPMenuItem = null;
    private ToolStripMenuItem m_ConvertSPR2GIFMenuItem = null;
    #endregion

    #region Animation Menu Item
    private ToolStripMenuItem m_AnimationMenuItem = null;
    private ToolStripMenuItem m_AnimationPlayMenuItem = null;
    private ToolStripMenuItem m_AnimationStopMenuItem = null;
    #endregion

    #region Image Data Clear Menu Item
    private ToolStripMenuItem m_ImageDataClearMenuItem = null;
    #endregion

    #endregion

    #endregion

    #region Timer
    private System.Windows.Forms.Timer m_AnimationTimer = null;
    #endregion
    #endregion

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();

        #region ToolTip
        m_ToolTip = new ToolTip(this.components) { };
        #endregion

        #region MetroStyleManager
        m_MetroStyleManager = new MetroFramework.Components.MetroStyleManager(this.components) {
            Owner = this,
        };
        #endregion

        #region Toggle
        m_ThemeToggle = new MetroFramework.Controls.MetroToggle() {
            Location = new Point(765, 63),
            Size = new Size(80, 29),
            TabStop = false,
            Text = "Light",
            UseSelectable = true,
            UseStyleColors = true,
        };
        m_ToolTip.SetToolTip(m_ThemeToggle, "프로그램의 테마를 변경합니다.");
        #endregion

        #region ComboBox
        // Color Style
        m_ColorStyleComboBox = new MetroFramework.Controls.MetroComboBox() {
            FormattingEnabled = true,
            ItemHeight = 23,
            Location = new Point(851, 63),
            Size = new Size(150, 29),
            TabStop = false,
            UseSelectable = true,
            UseStyleColors = true,
            Name = "ColorStyle",
        };
        m_ToolTip.SetToolTip(m_ColorStyleComboBox, "프로그램의 색상 스타일을 변경합니다.");

        // Image Data
        m_ImageDataComboBox = new MetroFramework.Controls.MetroComboBox() {
            FormattingEnabled = true,
            ItemHeight = 23,
            Location = new Point(23, 63),
            Size = new Size(300, 29),
            TabStop = false,
            UseSelectable = true,
            UseStyleColors = true,
            Name = "ImageData",
        };
        m_ToolTip.SetToolTip(m_ImageDataComboBox, "이미지 데이터 파일 목록입니다.");

        // Color Palette
        m_ColorPaletteComboBox = new MetroFramework.Controls.MetroComboBox() {
            FormattingEnabled = true,
            ItemHeight = 23,
            Location = new Point(329, 63),
            Size = new Size(300, 29),
            TabStop = false,
            UseSelectable = true,
            UseStyleColors = true,
            Name = "ColorPalette",
        };
        m_ToolTip.SetToolTip(m_ColorPaletteComboBox, "이미지의 색상표 목록입니다.");
        #endregion

        #region PictureBox
        m_ImagePictureBox = new PictureBox() {
            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
            BackgroundImageLayout = ImageLayout.None,
            Location = new Point(23, 98),
            Size = new Size(978, 650),
            SizeMode = PictureBoxSizeMode.CenterImage,
        };
        #endregion

        #region TrackBar
        m_StartFrameTrackBar = new MetroFramework.Controls.MetroTrackBar() {
            BackColor = Color.Transparent,
            LargeChange = 1,
            Location = new Point(23, 754),
            Size = new Size(300, 30),
            TabStop = false,
            UseCustomBackColor = true,
            Value = 0,
            Minimum = 0,
        };
        m_ToolTip.SetToolTip(m_StartFrameTrackBar, "시작 프레임");

        m_EndFrameTrackBar = new MetroFramework.Controls.MetroTrackBar() {
            BackColor = Color.Transparent,
            LargeChange = 1,
            Location = new Point(329, 754),
            Size = new Size(300, 30),
            TabStop = false,
            UseCustomBackColor = true,
            Value = 1,
            Minimum = 1,
        };
        m_ToolTip.SetToolTip(m_EndFrameTrackBar, "종료 프레임");
        #endregion

        #region ProgressBar
        m_ProcessProgressBar = new MetroFramework.Controls.MetroProgressBar() {
            Location = new Point(635, 754),
            Size = new Size(366, 29),
        };
        m_ToolTip.SetToolTip(m_ProcessProgressBar, "작업의 진행률을 나타내는 프로그레스 바입니다.");
        #endregion

        #region Context Menu Item
        // Convert
        m_ConvertMenuItem = new ToolStripMenuItem() {
            Text = "변환하기",
            ToolTipText = "이미지 데이터를 변환합니다.",
        };
        // SPR
        m_ConvertSPRMenuItem = new ToolStripMenuItem() {
            Text = "SPR 파일",
            ToolTipText = "SPR 파일을 변환합니다.",
        };
        m_ConvertSPR2BMPMenuItem = new ToolStripMenuItem() {
            Text = "BMP 파일",
            ToolTipText = "SPR 파일을 BMP 파일로 변환합니다.",
            Name = "SPR2BMP",
        };
        m_ConvertSPR2FrameBMPMenuItem = new ToolStripMenuItem() {
            Text = "BMP 프레임 파일",
            ToolTipText = "SPR 파일의 각 프레임을 BMP 파일로 변환합니다.",
            Name = "SPR2FBMP",
        };
        m_ConvertSPR2GIFMenuItem = new ToolStripMenuItem() {
            Text = "GIF 파일",
            ToolTipText = "SPR 파일을 GIF 파일로 변환합니다.",
            Name = "SPR2GIF",
        };
        m_ConvertSPRMenuItem.DropDownItems.Add(m_ConvertSPR2BMPMenuItem);
        m_ConvertSPRMenuItem.DropDownItems.Add(m_ConvertSPR2FrameBMPMenuItem);
        m_ConvertSPRMenuItem.DropDownItems.Add(m_ConvertSPR2GIFMenuItem);
        m_ConvertMenuItem.DropDownItems.Add(m_ConvertSPRMenuItem);

        // Animation
        m_AnimationMenuItem = new ToolStripMenuItem() {
            Text = "애니메이션",
            ToolTipText = "이미지 데이터의 각 프레임을 재생합니다.",
        };
        m_AnimationPlayMenuItem = new ToolStripMenuItem() {
            Text = "재생",
            ToolTipText = "이미지 데이터의 각 프레임을 재생합니다.",
            Name = "AnimePlay",
        };
        m_AnimationStopMenuItem = new ToolStripMenuItem() {
            Text = "정지",
            ToolTipText = "이미지 데이터의 각 프레임 재생을 정지합니다.",
            Name = "AnimeStop",
        };
        m_AnimationMenuItem.DropDownItems.Add(m_AnimationPlayMenuItem);
        m_AnimationMenuItem.DropDownItems.Add(m_AnimationStopMenuItem);

        // Clear
        m_ImageDataClearMenuItem = new ToolStripMenuItem() {
            Text = "이미지 데이터 제거",
            ToolTipText = "이미지 데이터를 제거하여 메모리 공간을 확보합니다.",
            Name = "ClearImageData",
        };
        #endregion

        #region Context Menu
        m_ContextMenu = new MetroFramework.Controls.MetroContextMenu(this.components);
        m_ContextMenu.ResumeLayout(false);
        m_ContextMenu.Items.Add(m_ConvertMenuItem);
        m_ContextMenu.Items.Add(m_AnimationMenuItem);
        m_ContextMenu.Items.Add(m_ImageDataClearMenuItem);
        #endregion

        #region Forms
        this.AllowDrop = true;
        this.AutoScaleDimensions = new SizeF(7F, 12F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1024, 800);
        this.Text = "Open SPR Viewer";
        this.MaximizeBox = false;
        this.Resizable = false;
        this.ShowIcon = false;
        this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
        this.ContextMenuStrip = m_ContextMenu;
        #endregion

        #region Timer
        m_AnimationTimer = new System.Windows.Forms.Timer(this.components) {
            Enabled = false,
            Interval = 100,
        };
        #endregion

        #region Add Controls
        this.Controls.Add(m_ThemeToggle);
        this.Controls.Add(m_ColorStyleComboBox);
        this.Controls.Add(m_ImageDataComboBox);
        this.Controls.Add(m_ColorPaletteComboBox);
        this.Controls.Add(m_ImagePictureBox);
        this.Controls.Add(m_StartFrameTrackBar);
        this.Controls.Add(m_EndFrameTrackBar);
        this.Controls.Add(m_ProcessProgressBar);
        #endregion

        this.ResumeLayout(false);
    }

    #endregion
}
