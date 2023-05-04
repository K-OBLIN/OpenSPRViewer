using Microsoft.Win32;

namespace OpenSPRViewer {
    class RegManager {
        #region Constants
        /// <summary>
        /// 32Bit Registry Path
        /// </summary>
        public const string RegistryPath = @"SOFTWARE\OpenSPRViewer";
        #endregion

        #region Properties
        /// <summary>
        /// You can get or set the convert directory(path)
        /// </summary>
        public string ConvertDir { get; set; }

        /// <summary>
        /// You can get or set the theme value
        /// </summary>
        public Int32 Theme { get; set; }

        /// <summary>
        /// You can get or set the color style value
        /// </summary>
        public Int32 ColorStyle { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public RegManager() {
            ConvertDir = string.Empty;
            Theme = 0;
            ColorStyle = 0;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Create a registry
        /// </summary>
        public void CreateRegistry() {
            RegistryKey regKey;

            if (Environment.Is64BitOperatingSystem == true) {
                regKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
            } else {
                regKey = Registry.CurrentUser;
            }

            try {
                var subKey = regKey.CreateSubKey(RegistryPath, true);
                var convertDir = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "data", "convert");

                if (subKey != null) {
                    subKey.SetValue("ConvertDir", convertDir);
                    subKey.SetValue("Theme", 0);
                    subKey.SetValue("ColorStyle", 0);
                }
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } finally {
                regKey?.Dispose();
                regKey = null;
            }
        }

        /// <summary>
        /// Get the registry's value of key
        /// </summary>
        public void GetRegistryKeyValue() {
            RegistryKey regKey;

            if (Environment.Is64BitOperatingSystem == true) {
                regKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
            } else {
                regKey = Registry.CurrentUser;
            }

            try {
                var subKey = regKey.OpenSubKey(RegistryPath);
                if (subKey == null) {
                    var result = MessageBox.Show("The registry is not exist!\n\nWould you like to create a registry path?", "Wait!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (result == DialogResult.Yes) {
                        CreateRegistry();
                    }
                } else {
                    ConvertDir = subKey.GetValue("ConvertDir").ToString();
                    Theme = Convert.ToInt32(subKey.GetValue("Theme"));
                    ColorStyle = Convert.ToInt32(subKey.GetValue("ColorStyle"));
                }
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } finally {
                regKey?.Dispose();
                regKey = null;
            }
        }

        /// <summary>
        /// Set the registry's value of key
        /// </summary>
        public void SetRegistryKeyValue() {
            RegistryKey regKey;

            if (Environment.Is64BitOperatingSystem == true) {
                regKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
            } else {
                regKey = Registry.CurrentUser;
            }

            try {
                var subKey = regKey.OpenSubKey(RegistryPath, true);
                if (subKey == null) {
                    var result = MessageBox.Show("The registry is not exist!\n\nWould you like to create a registry path?", "Wait!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (result == DialogResult.Yes) {
                        CreateRegistry();
                    }
                } else {
                    subKey.SetValue("ConvertDir", ConvertDir);
                    subKey.SetValue("Theme", Theme);
                    subKey.SetValue("ColorStyle", ColorStyle);
                }
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } finally {
                regKey?.Dispose();
                regKey = null;
            }
        }
        #endregion
    }
}