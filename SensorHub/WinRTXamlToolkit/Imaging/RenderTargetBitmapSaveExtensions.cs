﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace WinRTXamlToolkit.Imaging
{
    /// <summary>
    /// Extension methods used for saving a RenderTargetBitmap to a file.
    /// </summary>
    public static class RenderTargetBitmapSaveExtensions
    {
        /// <summary>
        /// Saves the RenderTargetBitmap to a png file with a unique file name.
        /// </summary>
        /// <param name="renderTargetBitmap">The writeable bitmap.</param>
        /// <returns>The file the bitmap was saved to.</returns>
        public static async Task<StorageFile> SaveToFile(
            this RenderTargetBitmap renderTargetBitmap)
        {
            return await renderTargetBitmap.SaveToFile(
                KnownFolders.PicturesLibrary,
                string.Format(
                    "{0}_{1}.png",
                    DateTime.Now.ToString("yyyyMMdd_HHmmss_fff"),
                    Guid.NewGuid()),
                CreationCollisionOption.GenerateUniqueName);
        }

        /// <summary>
        /// Saves the RenderTargetBitmap to a png file in the given folder with a unique file name.
        /// </summary>
        /// <param name="renderTargetBitmap">The writeable bitmap.</param>
        /// <param name="storageFolder">The storage folder.</param>
        /// <returns>The file the bitmap was saved to.</returns>
        public static async Task<StorageFile> SaveToFile(
            this RenderTargetBitmap renderTargetBitmap,
            StorageFolder storageFolder)
        {
            return await renderTargetBitmap.SaveToFile(
                storageFolder,
                string.Format(
                    "{0}_{1}.png",
                    DateTime.Now.ToString("yyyyMMdd_HHmmss_fff"),
                    Guid.NewGuid()),
                CreationCollisionOption.GenerateUniqueName);
        }

        /// <summary>
        /// Saves the RenderTargetBitmap to a file in the given folder with the given file name.
        /// </summary>
        /// <param name="renderTargetBitmap">The writeable bitmap.</param>
        /// <param name="storageFolder">The storage folder.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="options">
        /// The enum value that determines how responds if the fileName is the same
        /// as the name of an existing file in the current folder. Defaults to ReplaceExisting.
        /// </param>
        /// <returns></returns>
        public static async Task<StorageFile> SaveToFile(
            this RenderTargetBitmap renderTargetBitmap,
            StorageFolder storageFolder,
            string fileName,
            CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
        {
            StorageFile outputFile =
                await storageFolder.CreateFileAsync(
                    fileName,
                    options);

            Guid encoderId;

            var ext = Path.GetExtension(fileName);

            if (new[] { ".bmp", ".dib" }.Contains(ext))
            {
                encoderId = BitmapEncoder.BmpEncoderId;
            }
            else if (new[] { ".tiff", ".tif" }.Contains(ext))
            {
                encoderId = BitmapEncoder.TiffEncoderId;
            }
            else if (new[] { ".gif" }.Contains(ext))
            {
                encoderId = BitmapEncoder.GifEncoderId;
            }
            else if (new[] { ".jpg", ".jpeg", ".jpe", ".jfif", ".jif" }.Contains(ext))
            {
                encoderId = BitmapEncoder.JpegEncoderId;
            }
            else if (new[] { ".hdp", ".jxr", ".wdp" }.Contains(ext))
            {
                encoderId = BitmapEncoder.JpegXREncoderId;
            }
            else //if (new [] {".png"}.Contains(ext))
            {
                encoderId = BitmapEncoder.PngEncoderId;
            }

            await renderTargetBitmap.SaveToFile(outputFile, encoderId);

            return outputFile;
        }

        /// <summary>
        /// Saves the RenderTargetBitmap to the given file with the specified BitmapEncoder ID.
        /// </summary>
        /// <param name="renderTargetBitmap">The writeable bitmap.</param>
        /// <param name="outputFile">The output file.</param>
        /// <param name="encoderId">The encoder id.</param>
        /// <returns></returns>
        public static async Task SaveToFile(
            this RenderTargetBitmap renderTargetBitmap,
            StorageFile outputFile,
            Guid encoderId)
        {
            var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();
            Stream stream = pixelBuffer.AsStream();
            byte[] pixels = new byte[(uint)stream.Length];
            await stream.ReadAsync(pixels, 0, pixels.Length);

            using (var writeStream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(encoderId, writeStream);
                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied,
                    (uint)renderTargetBitmap.PixelWidth,
                    (uint)renderTargetBitmap.PixelHeight,
                    96,
                    96,
                    pixels);
                await encoder.FlushAsync();

                using (var outputStream = writeStream.GetOutputStreamAt(0))
                {
                    await outputStream.FlushAsync();
                }
            }
        }
    }
}