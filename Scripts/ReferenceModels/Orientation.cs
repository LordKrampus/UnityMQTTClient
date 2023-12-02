using System;
using System.Linq;

using UnityEngine;

namespace ReferenceModels
{
    [System.Serializable]
    public class Orientation
    {
        [SerializeField]
        private float[] _position;
        [SerializeField]
        private float[] _rotation;
        
        public Vector3 Position
        {
            get => new Vector3(this._position[0], this._position[1], this._position[2]);
            set => this._position = new float[] { value.x, value.y, value.z };
        }

        public Quaternion Rotation
        {
            get => new Quaternion(this._rotation[0], this._rotation[1], this._rotation[2], this._rotation[3]);
            set => this._rotation = new float[] { value.x, value.y, value.z, value.w };
        }

        public Orientation(Vector3 position, Quaternion rotation)
        {
            this.Position = position;
            this.Rotation = rotation;
        }

        public override string ToString()
        {
            return $"position:\t({this.Position.ToString()})\trotation\t({this.Rotation.ToString()})";
        }
    }
}
