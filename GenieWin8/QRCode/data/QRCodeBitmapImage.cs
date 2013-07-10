//
//  QR Code libray. http://platform.twit88.com/  http://www.codeproject.com/Articles/20574/Open-Source-QRCode-Library
//

using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;
using System.Windows.Input;
using System.IO;
using Windows.UI;
using System.Diagnostics;


namespace ThoughtWorks.QRCode.Codec.Data
{
    public class QRCodeBitmapImage : IQRCodeImage
    {
        #region Declaration

        //WriteableBitmap image;    //Bitmap image;
        byte[] _imageByteArray = null;
        int _width;
        int _height;
        private decimal _tresholdFilter = 0m; // Treshold filter from 0 to 1


        #endregion

   
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ImageByteArray"></param>
        /// <param name="Widht"></param>
        /// <param name="Height"></param>
        public QRCodeBitmapImage(byte[] ImageByteArray, int Widht, int Height)
        {
            if (ImageByteArray == null)
            {
                this._imageByteArray = new byte[Widht * Height* 4];
            }
            else
            {
                this._imageByteArray = ImageByteArray;
            }
            this._width = Widht;
            this._height = Height;
        }

        virtual public int Width
        {
            get
            {
                return this._width;
            }
        }
        
        virtual public int Height
        {
            get
            {
                return this._height;
            }
        }

        virtual public decimal TresholdFilter
        {
            get { return _tresholdFilter; }
            set { _tresholdFilter = value; }
        }

        public int getPixel(int x, int y)
        {
            
            if (_imageByteArray == null)
            {
                //_imageByteArray = image.ToByteArray();
            }
            
            int offset = (y * Width + x) * 4;

            // B,G,R,A
            byte[] a = new byte[] 
                            { _imageByteArray[offset], 
                              _imageByteArray[offset+1], 
                              _imageByteArray[offset+2],
                              _imageByteArray[offset+3] 
                            };
            #region Apply a treshold filter

            if (_tresholdFilter != 0)
            {
                //if ((a[0] + a[1] + a[2] + a[3]) < ((255 * 4) * TresholdFilter))
                //{
                //    a[0] = 0;
                //    a[1] = 0;
                //    a[2] = 0;
                //    a[3] = 0;
                //}
                //else
                //{
                //    a[0] = 255;
                //    a[1] = 255;
                //    a[2] = 255;
                //    a[3] = 255;
                //}
            }
            #endregion

            
            int colorCodeWithAlpha = BitConverter.ToInt32(a, 0);

            return colorCodeWithAlpha;
        }

        public void FillRectangle(int x1, int y1, int x2, int y2, Color color)
        {
            int _pixelCounter = 1;
            for (int i = 0; i <= _imageByteArray.Length; i += 4)
            {

                int _currentYCoordinate = ((_pixelCounter-1) / _width)+1;
                int _currentXCoordinate = ((_pixelCounter-1) % _width)+1;

                if (_currentXCoordinate >= x1 && _currentXCoordinate <= x2 &&
                   _currentYCoordinate >= y1 && _currentYCoordinate <= y2)
                {
                    _imageByteArray[i] = color.B;
                    _imageByteArray[i + 1] = color.G;
                    _imageByteArray[i + 2] = color.R;
                    _imageByteArray[i + 3] = color.A;
                }


                _pixelCounter++;
            }

        }

        public byte[] ImageByteArray
        {
            get { return _imageByteArray; }
        }

    }
}
