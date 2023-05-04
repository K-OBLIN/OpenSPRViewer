using SYW2Plus;
using System.Drawing;
using System.Drawing.Imaging;
using AnimatedGif;

namespace OpenSPRViewer;

public partial class Form1 : MetroFramework.Forms.MetroForm
{
    #region Constants
    public const string PALDirPath = @"./data/color_palette/";
    #endregion

    #region Member Variables
    private RegManager m_RegManager;
    private SPRManager m_SPRManager;
    private PALManager m_PALManager;

    private Image m_Image = null;
    private List<Image> m_ImageList = null;
    private Int32 m_ImageListIndex = 0;

    private float m_CurrentScale = 1.0F;

    private bool m_ImageMovingFlag = false;
    #endregion

    #region Constructors
    /// <summary>
    /// Default Constructors
    /// </summary>
    public Form1()
    {
        InitializeComponent();

        // Initialize
        m_RegManager = new RegManager();
        m_SPRManager = new SPRManager();
        m_PALManager = new PALManager();

        if (m_PALManager.LoadPALFile(PALDirPath) == false) {
            MessageBox.Show("pal 파일을 불러오는 데 실패하였습니다.\n\ndata 디렉토리 내 color_palette 디렉토리와 그 내부에 pal 파일이 존재하는 지 확인해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.Close();
        } else {
            for (var i = 0; i < m_PALManager.ColorPalette.Count; ++i) {
                m_ColorPaletteComboBox.Items.Add(Path.GetFileName(m_PALManager.FilePath[i]));
            }
            m_ColorPaletteComboBox.SelectedIndex = 0;
        }

        m_ColorStyleComboBox.Items.AddRange(Enum.GetNames(typeof(MetroFramework.MetroColorStyle)));

        m_ImageList = new List<Image>();

        // Form
        this.Load += new EventHandler(FormLoadEvent);
        this.FormClosed += new FormClosedEventHandler(FormClosedEvent);
        this.DragEnter += new DragEventHandler(FormDragEnterEvent);
        this.DragDrop += new DragEventHandler(FormDragDropEvent);

        // Toggle
        m_ThemeToggle.CheckedChanged += new EventHandler(ToggleCheckedChangedEvent);

        // ComboBox
        m_ColorStyleComboBox.SelectedIndexChanged += new EventHandler(ComboBoxSelectedIndexChangedEvent);
        m_ImageDataComboBox.SelectedIndexChanged += new EventHandler(ComboBoxSelectedIndexChangedEvent);
        m_ColorPaletteComboBox.SelectedIndexChanged += new EventHandler(ComboBoxSelectedIndexChangedEvent);

        // PictureBox
        m_ImagePictureBox.MouseUp += new MouseEventHandler(PictureBoxMouseUpEvent);
        m_ImagePictureBox.MouseDown += new MouseEventHandler(PictureBoxMouseDownEvent);
        m_ImagePictureBox.MouseMove += new MouseEventHandler(PictureBoxMouseMoveEvent);
        m_ImagePictureBox.MouseWheel += new MouseEventHandler(PictureBoxMouseWheelEvent);

        // TrackBar
        m_StartFrameTrackBar.Scroll += new ScrollEventHandler(TrackBarScrollEvent);
        m_EndFrameTrackBar.Scroll += new ScrollEventHandler(TrackBarScrollEvent);

        // Timer
        m_AnimationTimer.Tick += new EventHandler(TimerTickEvent);

        // Context Menu
        m_ConvertSPR2BMPMenuItem.Click += new EventHandler(ContextMenuItemClickEvent);
        m_ConvertSPR2FrameBMPMenuItem.Click += new EventHandler(ContextMenuItemClickEvent);
        m_ConvertSPR2GIFMenuItem.Click += new EventHandler(ContextMenuItemClickEvent);
        m_AnimationPlayMenuItem.Click += new EventHandler(ContextMenuItemClickEvent);
        m_AnimationStopMenuItem.Click += new EventHandler(ContextMenuItemClickEvent);
        m_ImageDataClearMenuItem.Click += new EventHandler(ContextMenuItemClickEvent);
    }
    #endregion

    #region Event Handler
    
    #region Form Event
    private void FormLoadEvent(object sender, EventArgs e) {
        // Registry
        m_RegManager.CreateRegistry();
        m_RegManager.GetRegistryKeyValue();

        // Create Directory
        Directory.CreateDirectory(Path.Combine(m_RegManager.ConvertDir, "bmp"));
        Directory.CreateDirectory(Path.Combine(m_RegManager.ConvertDir, "spr"));
        Directory.CreateDirectory(Path.Combine(m_RegManager.ConvertDir, "gif"));

        // Set Theme and Color Style
        m_MetroStyleManager.Theme = (MetroFramework.MetroThemeStyle)m_RegManager.Theme;
        m_MetroStyleManager.Style = (MetroFramework.MetroColorStyle)m_RegManager.ColorStyle;
        m_ThemeToggle.Checked = (m_MetroStyleManager.Theme == MetroFramework.MetroThemeStyle.Dark) ? true : false;
        m_ColorStyleComboBox.SelectedIndex = (Int32)m_MetroStyleManager.Style;
        this.StyleManager = m_MetroStyleManager;

        this.Refresh();
        this.Invalidate();
    }

    private void FormClosedEvent(object sender, EventArgs e) {
        m_RegManager.SetRegistryKeyValue();
        m_RegManager = null;
        m_SPRManager.SPRData.Clear();
        m_SPRManager = null;
        m_PALManager.FilePath.Clear();
        m_PALManager.ColorPalette.Clear();
        m_PALManager = null;
        m_Image?.Dispose();
        m_Image = null;
        m_ImageList?.Clear();
        m_ImageList = null;
    }

    private void FormDragEnterEvent(object sender, DragEventArgs e) {
        if (e.Data.GetDataPresent(DataFormats.FileDrop) == true) {
            e.Effect = DragDropEffects.Copy;
        } else {
            e.Effect = DragDropEffects.None;
        }
    }

    private async void FormDragDropEvent(object sender, DragEventArgs e) {
        // Check Image Data
        if (m_SPRManager.SPRData.Count > 0) {
            var result = MessageBox.Show("이미지 데이터가 이미 존재합니다. 이어서 이미지 데이터를 불러올까요?", "잠시만요!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) {
                m_SPRManager.SPRData.Clear();
                m_ImageDataComboBox.Items.Clear();

                m_ImagePictureBox.Image = null;

                GC.Collect();
            }
        }

        // Drag and Drop
        if (e.Data.GetDataPresent(DataFormats.FileDrop) == true) {
            var files = e.Data.GetData(DataFormats.FileDrop, true) as string[];
            if (files == null || files.Length == 0) { return; }

            m_ProcessProgressBar.Value = 0;
            m_ProcessProgressBar.Minimum = 0;
            m_ProcessProgressBar.Maximum = files.Length;

            this.UseWaitCursor = true;

            var failedCount = 0;
            for (var i = 0; i < files.Length; ++i) {
                var fileExtension = Path.GetExtension(files[i]).ToLower();

                if (string.Equals(fileExtension, ".spr") == true) {
                    var result = await Task.Run(() => m_SPRManager.LoadSPRFile(files[i]));
                    if (result == false) { failedCount+= 1; }
                }

                m_ProcessProgressBar.Value += 1;
            }

            this.UseWaitCursor = false;

            MessageBox.Show($"{files.Length}개의 파일 중 {files.Length - failedCount}개를 불러오는 데 성공하였습니다.", "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);

            m_ProcessProgressBar.Value = 0;

            m_ImageDataComboBox.Items.Clear();
            for (var i = 0; i < m_SPRManager.SPRData.Count; ++i) {
                m_ImageDataComboBox.Items.Add(Path.GetFileName(m_SPRManager.SPRData[i].FilePath));
            }
        }
    }
    #endregion

    #region Toggle Event
    private void ToggleCheckedChangedEvent(object sender, EventArgs e) {
        m_MetroStyleManager.Theme = (m_ThemeToggle.Checked == true) ? MetroFramework.MetroThemeStyle.Dark : MetroFramework.MetroThemeStyle.Light;

        m_RegManager.Theme = (Int32)m_MetroStyleManager.Theme;

        this.StyleManager = m_MetroStyleManager;
        this.Refresh();
        this.Invalidate();
    }
    #endregion

    #region ComboBox Event
    private void ComboBoxSelectedIndexChangedEvent(object sender, EventArgs e) {
        var cbBox = sender as MetroFramework.Controls.MetroComboBox;
        if (cbBox == null) { return; }

        m_AnimationTimer.Stop();
        m_AnimationTimer.Enabled = false;
        
        switch (cbBox.Name.ToString()) {
            case "ColorStyle": {
                m_MetroStyleManager.Style = (MetroFramework.MetroColorStyle)cbBox.SelectedIndex;
                
                m_RegManager.ColorStyle = (Int32)m_MetroStyleManager.Style;

                this.StyleManager = m_MetroStyleManager;
                this.Refresh();
                this.Invalidate();
            } break;

            case "ImageData":
            case "ColorPalette": {
                if (cbBox.SelectedItem != null) {
                    if (m_SPRManager.SPRData.Count == 0) {
                        MessageBox.Show("이미지 데이터를 불러와주세요!", "잠시만요!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    var selectedIndex = m_ImageDataComboBox.SelectedIndex;

                    if (m_SPRManager.SPRData[selectedIndex].SpriteWidth == 0) {
                        MessageBox.Show("이런! 선택하신 이미지 데이터는 불러올 수 없습니다. 다른 이미지 데이터를 불러와주세요.", "잠시만요!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    if (m_ColorPaletteComboBox.SelectedItem == null) {
                        MessageBox.Show("색상 팔레트를 선택해주세요.", "잠시만요!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    // TrackBar
                    m_StartFrameTrackBar.Minimum = 0;
                    m_StartFrameTrackBar.Maximum = (Int32)m_SPRManager.SPRData[selectedIndex].NumberOfFrame;
                    m_EndFrameTrackBar.Minimum = 0;
                    m_EndFrameTrackBar.Maximum = (Int32)m_SPRManager.SPRData[selectedIndex].NumberOfFrame;
                    m_EndFrameTrackBar.Value = (Int32)m_SPRManager.SPRData[selectedIndex].NumberOfFrame;

                    var selectedIndex2 = m_ColorPaletteComboBox.SelectedIndex;

                    this.UseWaitCursor = true;

                    try {
                        m_Image = m_SPRManager.GetBitmapByIndex(selectedIndex, m_PALManager.ColorPalette[selectedIndex2]);

                        var newWidth = (Int32)(m_Image.Width * m_CurrentScale);
                        var newHeight = (Int32)(m_Image.Height * m_CurrentScale);

                        m_ImagePictureBox.Image = new Bitmap(m_Image, newWidth, newHeight);
                        m_ImagePictureBox.Refresh();
                    } catch {
                        m_Image = null;
                    }

                    this.UseWaitCursor = false;
                }                
            } break;

            default: {
                MessageBox.Show("알 수 없는 컨트롤을 사용 중입니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } break;
        }
    }
    #endregion

    #region PictureBox
    private void PictureBoxMouseUpEvent(object sender, MouseEventArgs e) {
        if (e.Button == MouseButtons.Left) {
            m_ImageMovingFlag = false;
        }
    }

    private void PictureBoxMouseDownEvent(object sender, MouseEventArgs e) {
        if (e.Button == MouseButtons.Left) {
            m_ImageMovingFlag = true;
        }
    }

    private void PictureBoxMouseMoveEvent(object sender, MouseEventArgs e) {
        if (m_ImageMovingFlag == false || m_Image == null) { return; }

        if (e.Button == MouseButtons.Left) {
            var bmp = new Bitmap(m_ImagePictureBox.Width, m_ImagePictureBox.Height);

            using (var gr = Graphics.FromImage(bmp)) {
                gr.Clear(Color.White);
                gr.DrawImageUnscaled(m_Image, e.X - (m_Image.Width >> 1), e.Y - (m_Image.Height >> 1));

                m_ImagePictureBox.Image = bmp;
            }
        }
    }

    private void PictureBoxMouseWheelEvent(object sender, MouseEventArgs e) {
        if (m_Image == null) { return; }

        m_ImagePictureBox.SizeMode = PictureBoxSizeMode.CenterImage;

        var zoomScale = 1.1F;

        if (e.Delta > 0) {
            m_CurrentScale *= zoomScale;
        } else if (e.Delta < 0) {
            m_CurrentScale /= zoomScale;
        }

        var newWidth = (Int32)(m_Image.Width * m_CurrentScale);
        var newHeight = (Int32)(m_Image.Height * m_CurrentScale);

        m_ImagePictureBox.Image = new Bitmap(m_Image, newWidth, newHeight);
        m_ImagePictureBox.Refresh();
    }
    #endregion

    #region TrackBar
    private void TrackBarScrollEvent(object sender, ScrollEventArgs e) {
        var trackBar = sender as MetroFramework.Controls.MetroTrackBar;
        if (trackBar == null) { return; }

        m_ToolTip.SetToolTip(trackBar, trackBar.Value.ToString());
    }
    #endregion

    #region Timer
    private void TimerTickEvent(object sender, EventArgs e) {
        if (m_ImageList.Count == 0 || m_ImageDataComboBox.Items.Count == 0) { return; }

        m_ImagePictureBox.Image = m_ImageList[m_ImageListIndex];
        m_ImageListIndex += 1;
        if (m_ImageListIndex >= m_EndFrameTrackBar.Value) { m_ImageListIndex = m_StartFrameTrackBar.Value; }
    }
    #endregion

    #region Context Menu
    private void ContextMenuItemClickEvent(object sender, EventArgs e) {
        var contextMenuItem = sender as ToolStripMenuItem;
        if (contextMenuItem == null || string.IsNullOrEmpty(contextMenuItem.Name) == true) { return; }

        switch (contextMenuItem.Name.ToString()) {
            case "SPR2BMP": {
                if (m_SPRManager.SPRData.Count == 0 || m_ImageDataComboBox.Items.Count == 0) {
                    MessageBox.Show("이미지 데이터를 불러와주세요!", "잠시만요!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (m_Image == null) {
                    MessageBox.Show("이미지 데이터를 선택해주세요!", "잠시만요!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                var index1 = m_ImageDataComboBox.SelectedIndex;
                var index2 = m_ColorPaletteComboBox.SelectedIndex;

                var result = MessageBox.Show("모든 파일을 변환할까요?", "질문", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) {
                    try {
                        var failedCount = 0;

                        for (var i = 0; i < m_SPRManager.SPRData.Count; ++i) {
                            var fileName = Path.GetFileNameWithoutExtension(m_SPRManager.SPRData[i].FilePath) + ".bmp";
                            var outputFilePath = Path.Combine(m_RegManager.ConvertDir, "bmp", fileName);

                            var v = new Bitmap(1, 1);
                            try {
                                v = m_SPRManager.GetBitmapByIndex(i, m_PALManager.ColorPalette[index2]);
                            } catch {
                                failedCount += 1;
                                continue;
                            }

                            v?.Save(outputFilePath);
                        }
                        
                        MessageBox.Show($"{m_SPRManager.SPRData.Count}개의 파일 중 {m_SPRManager.SPRData.Count - failedCount}개 파일을 변환했습니다. 결과물을 확인해주세요!", "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    } catch (Exception ex) {
                        MessageBox.Show(ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } else {
                    try {
                        var fileName = Path.GetFileNameWithoutExtension(m_SPRManager.SPRData[index1].FilePath) + ".bmp";
                        var outputFilePath = Path.Combine(m_RegManager.ConvertDir, "bmp", fileName);

                        m_Image?.Save(outputFilePath);
                        
                        MessageBox.Show("파일을 변환했습니다. 결과물을 확인해주세요!", "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    } catch (Exception ex) {
                        MessageBox.Show(ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            } break;

            case "SPR2FBMP": {
                if (m_SPRManager.SPRData.Count == 0 || m_ImageDataComboBox.Items.Count == 0) {
                    MessageBox.Show("이미지 데이터를 불러와주세요!", "잠시만요!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                var index1 = m_ImageDataComboBox.SelectedIndex;
                var index2 = m_ColorPaletteComboBox.SelectedIndex;

                var result = MessageBox.Show("모든 파일을 변환할까요?", "질문", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) {
                    try {
                        for (var i = 0; i < m_SPRManager.SPRData.Count; ++i) {
                            var imgList = m_SPRManager.GetImageListByIndex(i, m_PALManager.ColorPalette[index2]);

                            for (var j = 0; j < imgList.Count; ++j) {
                                var fileName = Path.GetFileNameWithoutExtension(m_SPRManager.SPRData[i].FilePath) + "_" + j.ToString() + ".bmp";
                                var outputFilePath = Path.Combine(m_RegManager.ConvertDir, "bmp", fileName);

                                imgList[j].Save(outputFilePath);
                            }
                        }

                        MessageBox.Show("파일을 변환했습니다. 결과물을 확인해주세요!", "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    } catch (Exception ex) {
                        MessageBox.Show(ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } else {
                    try {
                        var imgList = m_SPRManager.GetImageListByIndex(index1, m_PALManager.ColorPalette[index2]);

                        for (var j = 0; j < imgList.Count; ++j) {
                            var fileName = Path.GetFileNameWithoutExtension(m_SPRManager.SPRData[index1].FilePath) + "_" + j.ToString() + ".bmp";
                            var outputFilePath = Path.Combine(m_RegManager.ConvertDir, "bmp", fileName);

                            imgList[j].Save(outputFilePath);
                        }
                        
                        MessageBox.Show("파일을 변환했습니다. 결과물을 확인해주세요!", "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    } catch (Exception ex) {
                        MessageBox.Show(ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            } break;

            case "SPR2GIF": {
                if (m_ImageDataComboBox.SelectedItem == null) {
                    MessageBox.Show("이미지 데이터를 선택해주세요.", "잠시만요!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                var result = MessageBox.Show("현재 선택하신 이미지 데이터만 GIF 파일로 내보냅니다. 그래도 내보내시겠습니까?", "질문", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) {
                    try {
                        m_ImageList?.Clear();
                        m_ImageList = m_SPRManager.GetImageListByIndex(m_ImageDataComboBox.SelectedIndex, m_PALManager.ColorPalette[m_ColorPaletteComboBox.SelectedIndex]);
                        if (m_ImageList.Count == 0) {
                            MessageBox.Show("GIF 파일로 내보낼 수 없습니다!", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        var outputFilePath = Path.Combine(m_RegManager.ConvertDir, "gif", Path.GetFileNameWithoutExtension(m_ImageDataComboBox.SelectedItem.ToString()) + ".gif");

                        if (m_StartFrameTrackBar.Value > m_EndFrameTrackBar.Value) {
                            MessageBox.Show("시작 프레임 값이 종료 프레임 값보다 클 수 없습니다.", "잠시만요!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }

                        using (var gif = AnimatedGif.AnimatedGif.Create(outputFilePath, m_AnimationTimer.Interval)) {
                            for (var i = m_StartFrameTrackBar.Value; i < m_EndFrameTrackBar.Value; ++i) {
                                gif.AddFrame(m_ImageList[i], quality: GifQuality.Bit8);
                            }
                        }
                        
                        MessageBox.Show("파일을 변환했습니다. 결과물을 확인해주세요!", "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    } catch (Exception ex) {
                        MessageBox.Show(ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            } break;

            case "AnimePlay": {
                if (m_Image != null) {
                    m_AnimationTimer.Stop();
                    m_AnimationTimer.Enabled = false;

                    if (m_StartFrameTrackBar.Value > m_EndFrameTrackBar.Value) {
                        MessageBox.Show("시작 프레임 값이 종료 프레임 값보다 클 수 없습니다.", "잠시만요!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    if (m_ImageDataComboBox.SelectedItem == null) {
                        MessageBox.Show("이미지 데이터를 선택해주세요!", "잠시만요!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    m_ImageListIndex = m_StartFrameTrackBar.Value;

                    m_ImageList?.Clear();
                    m_ImageList = m_SPRManager.GetImageListByIndex(m_ImageDataComboBox.SelectedIndex, m_PALManager.ColorPalette[m_ColorPaletteComboBox.SelectedIndex]);

                    m_AnimationTimer.Enabled = true;
                    m_AnimationTimer.Start();
                }
            } break;

            case "AnimeStop": {
                m_AnimationTimer.Stop();
                m_AnimationTimer.Enabled = false;

                m_ImagePictureBox.Image = m_Image;
                m_ImagePictureBox.Refresh();
            } break;

            case "ClearImageData": {
                m_AnimationTimer.Stop();
                m_AnimationTimer.Enabled = false;

                m_ImageDataComboBox?.Items.Clear();
                m_SPRManager.SPRData?.Clear();
                m_Image?.Dispose();
                m_Image = null;
                m_ImageList?.Clear();

                m_ImagePictureBox.Image = null;
                this.Refresh();

                GC.Collect();

                MessageBox.Show("데이터 정리 완료!", "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } break;

            default: {

            } break;
        }
    }
    #endregion

    #endregion
}