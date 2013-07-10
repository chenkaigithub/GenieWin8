using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;


namespace ThoughtWorks.QRCode
{
    public static class Utilities
    {

        private static async Task<bool> SaveWriteableBitmapToDisk(WriteableBitmap bmp, string filename)
        {
            StorageFile file;
            IRandomAccessStream stream = null;
            //FileSavePicker picker = new FileSavePicker();
            //picker.FileTypeChoices.Add("JPG File", new List<string>() { ".jpg" });
            try
            {
                file =
                    await Windows.Storage.KnownFolders.PicturesLibrary.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.ReplaceExisting);

                stream = await file.OpenAsync(FileAccessMode.ReadWrite);
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);

                //Stream pixelStream = bmp.ToByteArray();
                //byte[] pixels = bmp.ToByteArray();
                byte[] pixels = bmp.PixelBuffer.ToArray();
                //await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                

                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)bmp.PixelWidth, (uint)bmp.PixelHeight, 96.0, 96.0, pixels);
                await encoder.FlushAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (stream != null) stream.Dispose();
                file = null;
            }

        }

        public static void WriteTimespanToDebug(string title, Stopwatch sw)
        {
            Debug.WriteLine(title + (double)TimeSpan.FromTicks(sw.ElapsedMilliseconds).Milliseconds / 1000.00000 + " sec");
        }

        public static async void WriteBitmapToDrive(int[][] pixelData)
        {
          
            WriteableBitmap wb = new WriteableBitmap(pixelData.Length, pixelData[0].Length);

            byte[] bytePixelData = GetByteArrayFromInt2DArray(pixelData);
           
            WriteableBitmapFromArray(wb, bytePixelData);
            
            await SaveWriteableBitmapToDisk(wb, "FromPixelInfo.jpg");

        }

        public static async void WriteBitmapToDrive(bool[][] pixelData)
        {

            WriteableBitmap wb = new WriteableBitmap(pixelData.Length, pixelData[0].Length);

            byte[] bytePixelData = GetByteArrayFromBool2DArray(pixelData);

            WriteableBitmapFromArray(wb, bytePixelData);

            await SaveWriteableBitmapToDisk(wb, "AfterMedianfilter.jpg");

        }

        private static byte[] GetByteArrayFromInt2DArray(int[][] intArray)
        {
            int Width = intArray.Length;        
            int Height = intArray[0].Length;

    
            // Create the target data array
            byte[] data = new byte[Width * Height * 4];


            for (int y = 0; y < Height; y++)             // Height
            {
                for (int x = 0; x < Width; x++)          // Width
                {
                    int destinationIndex = (y * Width + x) * 4;

                    Array.Copy(BitConverter.GetBytes(intArray[x][y]), 0, data, destinationIndex, 4);
                }
            }

            return data;

        }

        private static byte[] GetByteArrayFromBool2DArray(bool[][] boolArray)
        {
            int Width = boolArray.Length;
            int Height = boolArray[0].Length;


            // Create the target data array
            byte[] data = new byte[Width * Height * 4];


            for (int y = 0; y < Height; y++)             // Height
            {
                for (int x = 0; x < Width; x++)          // Width
                {
                    int destinationIndex = (y * Width + x) * 4;

                    if (boolArray[x][y] == false) Array.Copy(new byte[] { 255, 255, 255, 255 }, 0, data, destinationIndex, 4);

                    if (boolArray[x][y] == true) Array.Copy(new byte[] { 0, 0, 0, 0 }, 0, data, destinationIndex, 4);

                }
            }

            return data;

        }

        public static void WriteableBitmapFromArray(WriteableBitmap wb, byte[] source)
        {

            Stream PixelBufferStream = wb.PixelBuffer.AsStream();
            byte[] dstArray = wb.PixelBuffer.ToArray();
            PixelBufferStream.Seek(0, 0);

            for (int cnt = 0; cnt < dstArray.Length; cnt++)
            {
                dstArray[cnt] = source[cnt];
            }


            PixelBufferStream.Write(dstArray, 0, dstArray.Length);

            wb.Invalidate();


        } 

    }
}
