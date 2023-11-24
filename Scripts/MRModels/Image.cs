using System;

using UnityEngine;

namespace MRModels
{
    [System.Serializable]
    public class Image
    {
        private byte[] _imgBytes;
        [SerializeField]
        private string _imgBytesBase64;
        [SerializeField]
        private int _width;
        [SerializeField]
        private int _height;

        public byte[] ImgBytes
        {
            get => this._imgBytes;
        }

        public string ImgBytesBase64
        {
            get => this._imgBytesBase64; // Convert from Base64 to byte array
        }

        public int Width
        {
            get => this._width;
        }

        public int Height
        {
            get => this._height;
        }

        public byte[] GetBytes()
        {
            return this._imgBytes ??= Convert.FromBase64String(this._imgBytesBase64);
        }

        private void SetBytes(byte[] imgBytes)
        {
            this._imgBytes = imgBytes;
            this._imgBytesBase64 = Convert.ToBase64String(imgBytes);
        }

        public Image(byte[] imgBytes, int width, int height)
        {
            this.SetBytes(imgBytes);
            this._width = width;
            this._height = height;
        }

        public override string ToString()
        {
            return $"size:\t({this._width}, {this._height})\n" +
                $"bytes:\t{this._imgBytesBase64}";
        }
    }
}
