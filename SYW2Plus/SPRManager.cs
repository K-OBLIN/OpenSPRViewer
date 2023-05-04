using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SYW2Plus {
    class SPRManager {
        #region Constants
        /// <summary>
        /// Array Size 1
        /// </summary>
        public const Int32 SIZE = 300;

        /// <summary>
        /// Array Size 2
        /// </summary>
        public const Int32 SIZE2 = 8;
        #endregion

        #region Structures
        /// <summary>
        /// SPR data structure infomation
        /// </summary>
        public struct SPRInfo {
            #region Properties
            /// <summary>
            /// You can get or set the file path
            /// </summary>
            public string FilePath { get; set; }

            /// <summary>
            /// You can get or set the signature
            /// </summary>
            public UInt32 Signature { get; set; }

            /// <summary>
            /// You can get or set the frame width
            /// </summary>
            public UInt32 FrameWidth { get; set; }

            /// <summary>
            /// You can get or set the frame height
            /// </summary>
            public UInt32 FrameHeight { get; set; }

            /// <summary>
            /// You can get or set the number of frame
            /// </summary>
            public UInt32 NumberOfFrame { get; set; }

            /// <summary>
            /// You can get or set the dummy data
            /// </summary>
            public UInt32[] DummyData { get; set; }

            /// <summary>
            /// You can get or set the offset
            /// </summary>
            public UInt32[] Offsets { get; set; }

            /// <summary>
            /// You can get or set the last offset
            /// </summary>
            public UInt32 LastOffset { get; set; }

            /// <summary>
            /// You can get or set the compression size
            /// </summary>
            public UInt16[] CompressionSizes { get; set; }

            /// <summary>
            /// You can get or set the sprite width
            /// </summary>
            public UInt32 SpriteWidth { get; set; }

            /// <summary>
            /// You can get or set the sprite height
            /// </summary>
            public UInt32 SpriteHeight { get; set; }

            /// <summary>
            /// You can get or set the dummy data 2
            /// </summary>
            public UInt32[] DummyData2 { get; set; }

            /// <summary>
            /// You can get or set the pixel
            /// </summary>
            public byte[] Pixels { get; set; }
            #endregion
        }
        #endregion

        #region Properties
        /// <summary>
        /// You can get or set the SPR data
        /// </summary>
        public List<SPRInfo> SPRData { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public SPRManager() {
            SPRData = new List<SPRInfo>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Load the spr file
        /// </summary>
        /// <param name="filePath">The path to the spr file</param>
        /// <returns>Successful(true), Failed(false)</returns>
        public bool LoadSPRFile(string filePath) {
            if (string.IsNullOrEmpty(filePath) == true) { return false; }
            if (Path.GetExtension(filePath) == null) { return false; }
            if (File.Exists(filePath) == false) { return false; }

            try {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
                    using (var br = new BinaryReader(fs)) {
                        var spr = new SPRInfo();

                        // File Path
                        spr.FilePath = filePath;

                        // Signature
                        spr.Signature = br.ReadUInt32();
                        if (spr.Signature != 0x09) { return false; }

                        // Frame Width, Height
                        spr.FrameWidth = br.ReadUInt32();
                        spr.FrameHeight = br.ReadUInt32();

                        // Number Of Frame
                        spr.NumberOfFrame = br.ReadUInt32();

                        // Dummy Data
                        spr.DummyData = new UInt32[SIZE];
                        for (var i = 0; i < spr.DummyData.Length; ++i) {
                            spr.DummyData[i] = br.ReadUInt32();
                        }

                        // Offsets
                        spr.Offsets = new UInt32[SIZE];
                        for (var i = 0; i < spr.Offsets.Length; ++i) {
                            spr.Offsets[i] = br.ReadUInt32();
                        }

                        // Compression Sizes
                        spr.CompressionSizes = new UInt16[SIZE];
                        for (var i = 0; i < spr.CompressionSizes.Length; ++i) {
                            spr.CompressionSizes[i] = br.ReadUInt16();
                        }

                        // Last Offset
                        spr.LastOffset = br.ReadUInt32();

                        // Sprite Width, Height
                        spr.SpriteWidth = br.ReadUInt32();
                        spr.SpriteHeight = br.ReadUInt32();

                        // Dummy Data 2
                        spr.DummyData2 = new UInt32[SIZE2];
                        for (var i = 0; i < spr.DummyData2.Length; ++i) {
                            spr.DummyData2[i] = br.ReadUInt32();
                        }

                        // Pixels
                        spr.Pixels = new byte[spr.FrameWidth * spr.FrameHeight * spr.NumberOfFrame];
                        for (var i = 0; i < spr.Pixels.Length;) {
                            var pixel = br.ReadByte();

                            if (pixel == 0xFE) {
                                var numberOfRepeat = br.ReadByte();

                                for (var j = 0; j < numberOfRepeat; ++j) { spr.Pixels[i + j] = pixel; }

                                i += numberOfRepeat;
                            } else {
                                spr.Pixels[i] = pixel;
                                ++i;
                            }
                        }

                        // Add
                        SPRData.Add(spr);
                    }
                }
            } catch {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Save the spr file
        /// </summary>
        /// <param name="filePath">The path to the spr file</param>
        /// <param name="spr">SPR data</param>
        /// <returns>Successful(true), Failed(false)</returns>
        public bool SaveSPRFile(string filePath, SPRInfo spr) {
            if (string.IsNullOrEmpty(filePath) == true) { return false; }
            if (Path.GetExtension(filePath) == null) { return false; }

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write)) {
                using (var bw = new BinaryWriter(fs)) {
                    // Signature
                    bw.Write((UInt32)0x09);

                    // Frame Width, Height
                    bw.Write(spr.FrameWidth);
                    bw.Write(spr.FrameHeight);

                    // Number Of Frame
                    bw.Write(spr.NumberOfFrame);

                    // Dummy Data
                    for (var i = 0; i < spr.DummyData.Length; ++i) {
                        bw.Write(spr.DummyData[i]);
                    }

                    // Offsets, Compression Sizes, Pixels
                    var Offset = 0U;
                    var Size = 0U;
                    var Result = new List<byte>();
                    var Temp = new byte[spr.FrameWidth];

                    var frameSize = spr.FrameWidth * spr.FrameHeight;
                    for (var i = 0; i < spr.Pixels.Length; ++i) {
                        // Index
                        var idx = ((i + 1) / frameSize) - 1;

                        // Offsets
                        if ((i + 1) % frameSize == 0) {
                            bw.BaseStream.Position = 0x4C0 + (idx << 2);
                            bw.Write(Offset);
                        }

                        // Pixel
                        Temp[i % spr.FrameWidth] = spr.Pixels[i];

                        if ((i + 1) % spr.FrameWidth == 0) {
                            for (var j = 0; j < Temp.Length; ++j) {
                                var item = Temp[j];

                                if (item == 0xFE) {
                                    var NumberOfRepeat = 1;
                                    var NextItemIndex = j + 1;

                                    while (NextItemIndex < Temp.Length && Temp[NextItemIndex] == 0xFE) {
                                        NumberOfRepeat += 1;
                                        NextItemIndex += 1;
                                    }

                                    Result.Add(item);
                                    Result.Add((byte)(NumberOfRepeat /* & 0xFF */));

                                    item = Temp[NextItemIndex - 1];
                                    j = NextItemIndex - 1;
                                } else {
                                    Result.Add(item);
                                    Size += 1U;
                                }
                            }

                            if ((i + 1) % frameSize == 0) {
                                Offset += Size;

                                bw.BaseStream.Position = 0x970 + (idx << 1);
                                bw.Write((UInt16)(Size & 0xFF));

                                Size = 0U;
                            }
                        }
                    }

                    // Last Offset
                    bw.BaseStream.Position = 0xBC8;
                    bw.Write((UInt32)Result.Count);

                    // Sprite Width, Height
                    bw.Write(spr.SpriteWidth);
                    bw.Write(spr.SpriteHeight);

                    // Dummy Data 2
                    for (var i = 0; i < spr.DummyData2.Length; ++i) {
                        bw.Write(spr.DummyData2[i]);
                    }

                    // Pixels
                    for (var i = 0; i < Result.Count; ++i) {
                        bw.Write(Result[i]);
                    }

                    // Clear
                    Result.Clear();
                    Result = null;
                    Temp = null;
                }
            }

            return true;
        }

        /// <summary>
        /// Get the bitmap
        /// </summary>
        /// <param name="index">Index of SPR data</param>
        /// <param name="palette">Color Palette</param>
        /// <returns>Successful(Bitmap), Failed(null)</returns>
        public Bitmap GetBitmapByIndex(Int32 index, ColorPalette palette) {
            if (index < 0 || index >= SPRData.Count) { return null; }
            if (palette == null) { return null; }

            var bitmap = new Bitmap((Int32)SPRData[index].SpriteWidth, (Int32)SPRData[index].SpriteHeight, PixelFormat.Format8bppIndexed);
            bitmap.Palette = palette;

            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            
            var bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) >> 3;
            var stride = bitmapData.Stride;
            var ptr = bitmapData.Scan0;

            for (var j = 0; j < SPRData[index].SpriteHeight / SPRData[index].FrameHeight; ++j) {
                for (var y = 0; y < SPRData[index].FrameHeight; ++y) {
                    for (var i = 0; i < SPRData[index].SpriteWidth / SPRData[index].FrameWidth; ++i) {
                        for (var x = 0; x < SPRData[index].FrameWidth; ++x) {
                            var index1 = x + (y * stride) + (i * (Int32)SPRData[index].FrameWidth) + (j * (Int32)SPRData[index].FrameWidth * (Int32)SPRData[index].FrameHeight * ((Int32)SPRData[index].SpriteWidth / (Int32)SPRData[index].FrameWidth));
                            var index2 = x + (y * (Int32)SPRData[index].FrameWidth) + (i * (Int32)SPRData[index].FrameWidth * (Int32)SPRData[index].FrameHeight) + (j * (Int32)SPRData[index].FrameWidth * (Int32)SPRData[index].FrameHeight * ((Int32)SPRData[index].SpriteWidth / (Int32)SPRData[index].FrameWidth));
                            
                            Marshal.WriteByte(ptr, index1 * bytesPerPixel, SPRData[index].Pixels[index2]);
                        }
                    }
                }
            }

            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

        /// <summary>
        /// Get the bitmap
        /// </summary>
        /// <param name="spr">SPR Data</param>
        /// <param name="palette">Color Palette</param>
        /// <returns>Successful(Bitmap), Failed(null)</returns>
        public Bitmap GetBitmapBySPR(ref SPRInfo spr, ColorPalette palette) {
            if (palette == null) { return null; }

            var bitmap = new Bitmap((Int32)spr.SpriteWidth, (Int32)spr.SpriteHeight, PixelFormat.Format8bppIndexed);
            bitmap.Palette = palette;

            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            var bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) >> 3;
            var stride = bitmapData.Stride;
            var ptr = bitmapData.Scan0;

            for (var j = 0; j < spr.SpriteHeight / spr.FrameHeight; ++j) {
                for (var y = 0; y < spr.FrameHeight; ++y) {
                    for (var i = 0; i < spr.SpriteWidth / spr.FrameWidth; ++i) {
                        for (var x = 0; x < spr.FrameWidth; ++x) {
                            var index1 = x + (y * stride) + (i * (Int32)spr.FrameWidth) + (j * (Int32)spr.FrameWidth * (Int32)spr.FrameHeight * ((Int32)spr.SpriteWidth / (Int32)spr.FrameWidth));
                            var index2 = x + (y * (Int32)spr.FrameWidth) + (i * (Int32)spr.FrameWidth * (Int32)spr.FrameHeight) + (j * (Int32)spr.FrameWidth * (Int32)spr.FrameHeight * ((Int32)spr.SpriteWidth / (Int32)spr.FrameWidth));
                            
                            Marshal.WriteByte(ptr, index1 * bytesPerPixel, spr.Pixels[index2]);
                        }
                    }
                }
            }

            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

        /// <summary>
        /// Convert BMP to SPR
        /// </summary>
        /// <param name="filePath">The path to the spr file</param>
        /// <param name="bmp">Bitmap</param>
        /// <param name="frameWidth">Frame Width</param>
        /// <param name="frameHeight">Frame Height</param>
        /// <param name="numberOfFrame">Number Of Frame</param>
        /// <param name="palette">Color Palette</param>
        /// <returns>Successful(true), Failed(false)</returns>
        public bool BMP2SPR(string filePath, ref Bitmap bmp, UInt32 frameWidth, UInt32 frameHeight, UInt32 numberOfFrame, ColorPalette palette) {
            if (string.IsNullOrEmpty(filePath) == true) { return false; }
            if (Path.GetExtension(filePath) == null) { return false; }
            if (bmp.Width < frameWidth || (bmp.Width % frameWidth != 0)) { return false; }
            if (bmp.Height < frameHeight || (bmp.Height % frameHeight != 0)) { return false; }
            if ((bmp.Width * bmp.Height) != (frameWidth * frameHeight * numberOfFrame)) { return false; }
            if (palette == null) { return false; }

            var spr = new SPRInfo() {
                Signature = 0x09,
                FrameWidth = frameWidth,
                FrameHeight = frameHeight,
                NumberOfFrame = numberOfFrame,
                SpriteWidth = (UInt32)bmp.Width,
                SpriteHeight = (UInt32)bmp.Height,
                DummyData = new UInt32[SIZE],
                DummyData2 = new UInt32[SIZE2],
                Offsets = new UInt32[SIZE],
                CompressionSizes = new UInt16[SIZE],
                Pixels = new byte[frameWidth * frameHeight * numberOfFrame]
            };
            bmp.Palette = palette;

            var bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            var ptr = bitmapData.Scan0;
            var stride = bitmapData.Stride;

            for (var j = 0; j < spr.SpriteHeight / spr.FrameHeight; ++j) {
                for (var y = 0; y < spr.FrameHeight; ++y) {
                    for (var i = 0; i < spr.SpriteWidth / spr.FrameWidth; ++i) {
                        for (var x = 0; x < spr.FrameWidth; ++x) {
                            var pixelIndex = x + (y * (Int32)spr.FrameWidth) + (i * (Int32)spr.FrameWidth * (Int32)spr.FrameHeight) + (j * (Int32)spr.SpriteWidth);
                            var sourceIndex = x + (y * stride) + (i * (Int32)spr.FrameWidth) + (j * (Int32)spr.SpriteWidth * (Int32)spr.FrameHeight);
                            spr.Pixels[pixelIndex] = Marshal.ReadByte(ptr, sourceIndex);
                        }
                    }
                }
            }

            bmp.UnlockBits(bitmapData);

            return SaveSPRFile(filePath, spr);
        }

        /// <summary>
        /// Get image list by index
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="palette">Color Palette</param>
        /// <returns>Successful(List<Image>), Failed(null)</returns>
        public List<Image> GetImageListByIndex(Int32 index, ColorPalette palette) {
            if (index < 0 || index >= SPRData.Count) { return null; }
            if (SPRData.Count == 0) { return null; }
            if (palette == null) { return null; }

            var imgList = new List<Image>();

            for (var i = 0; i < SPRData[index].NumberOfFrame; ++i) {
                var bitmap = new Bitmap((Int32)SPRData[index].FrameWidth, (Int32)SPRData[index].FrameHeight, PixelFormat.Format8bppIndexed);
                bitmap.Palette = palette;

                var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

                var pixels = SPRData[index].Pixels;
                var lineSize = SPRData[index].FrameWidth;

                for (var y = 0; y < bitmap.Height; ++y) {
                    var idx = y * lineSize + i * (lineSize * bitmap.Height);
                    Marshal.Copy(pixels, (Int32)idx, bitmapData.Scan0 + y * bitmapData.Stride, (Int32)lineSize);
                }

                bitmap.UnlockBits(bitmapData);

                imgList.Add((Image)bitmap);
            }

            return imgList;
        }

        /// <summary>
        /// Get image list by spr
        /// </summary>
        /// <param name="spr">SPR Data</param>
        /// <param name="palette">Color Palette</param>
        /// <returns>Successful(List<Image>), Failed(null)</returns>
        public List<Image> getImageListBySPR(SPRInfo spr, ColorPalette palette) {
            if (palette == null) { return null; }
            
            var imgList = new List<Image>();

            for (var i = 0; i < spr.NumberOfFrame; ++i) {
                var bitmap = new Bitmap((Int32)spr.FrameWidth, (Int32)spr.FrameHeight, PixelFormat.Format8bppIndexed);
                bitmap.Palette = palette;

                var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

                var pixels = spr.Pixels;
                var lineSize = spr.FrameWidth;

                for (var y = 0; y < bitmap.Height; ++y) {
                    var idx = y * lineSize + i * (lineSize * bitmap.Height);
                    Marshal.Copy(pixels, (Int32)idx, bitmapData.Scan0 + y * bitmapData.Stride, (Int32)lineSize);
                }

                bitmap.UnlockBits(bitmapData);

                imgList.Add((Image)bitmap);
            }

            return imgList;
        }

        /// <summary>
        /// Get image list by bmp
        /// </summary>
        /// <param name="bmp">Bitmap</param>
        /// <param name="frameWidth">Frame Width</param>
        /// <param name="frameHeight">Frame Height</param>
        /// <param name="numberOfFrame">Number Of Frame</param>
        /// <param name="palette">Color Palette</param>
        /// <returns>Successful(List<Image>), Failed(null)</returns>
        public List<Image> GetImageListByBMP(ref Bitmap bmp, UInt32 frameWidth, UInt32 frameHeight, UInt32 numberOfFrame, ColorPalette palette) {
            if (bmp.Width < frameWidth || (bmp.Width % frameWidth != 0)) { return null; }
            if (bmp.Height < frameHeight || (bmp.Height % frameHeight != 0)) { return null; }
            if ((bmp.Width * bmp.Height) != (frameWidth * frameHeight * numberOfFrame)) { return null; }
            if (palette == null) { return null; }

            var imgList = new List<Image>();

            // Get Pixels
            var pixels = new byte[bmp.Width * bmp.Height];
            Marshal.Copy(bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat).Scan0, pixels, 0, (bmp.Width * bmp.Height));

            for (var i = 0; i < numberOfFrame; ++i) {
                var bitmap = new Bitmap((Int32)frameWidth, (Int32)frameHeight, PixelFormat.Format8bppIndexed);
                bitmap.Palette = palette;

                var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
                
                for (var y = 0; y < frameHeight; ++y) {
                    var idx = y * frameWidth + i * (frameWidth * frameHeight);
                    Marshal.Copy(pixels, (Int32)idx, bitmapData.Scan0 + y * bitmapData.Stride, (Int32)frameWidth);
                }

                bitmap.UnlockBits(bitmapData);

                imgList.Add((Image)bitmap);
            }

            return imgList;
        }
        #endregion
    }
}