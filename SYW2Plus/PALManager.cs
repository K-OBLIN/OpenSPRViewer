using System.Drawing.Imaging;

namespace SYW2Plus {
    class PALManager {
        #region Properties
        /// <summary>
        /// You can get or set the file path
        /// </summary>
        public List<string> FilePath { get; set; }

        /// <summary>
        /// You can get or set the color palette
        /// </summary>
        /// <value></value>
        public List<ColorPalette> ColorPalette { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public PALManager() {
            FilePath = new List<string>();
            ColorPalette = new List<ColorPalette>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Load the pal file
        /// </summary>
        /// <param name="dirPath">Directory Path</param>
        /// <returns>Successful(true), Failed(false)</returns>
        public bool LoadPALFile(string dirPath) {
            if (string.IsNullOrEmpty(dirPath) == true) { return false; }
            if (Directory.Exists(dirPath) == false) { return false; }

            var files = Directory.GetFiles(dirPath, "*.pal");
            if (files.Length == 0) { return false; }

            for (var i = 0; i < files.Length; ++i) {
                if (File.Exists(files[i]) == false) { return false; }

                // Add file path
                FilePath.Add(files[i]);

                using (var fs = new FileStream(files[i], FileMode.Open, FileAccess.Read)) {
                    using (var br = new BinaryReader(fs)) {
                        var palette = new Bitmap(1, 1, PixelFormat.Format8bppIndexed).Palette;

                        for (var j = 0; j < palette.Entries.Length; ++j) {
                            palette.Entries[j] = Color.FromArgb(br.ReadByte(), br.ReadByte(), br.ReadByte());
                        }

                        // Add color palette
                        ColorPalette.Add(palette);
                    }
                }
            }

            return true;
        }
        #endregion
    }
}