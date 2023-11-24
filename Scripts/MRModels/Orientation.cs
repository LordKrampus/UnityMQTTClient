using System;
using System.Linq;

using UnityEngine;

namespace MRModels
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

        public Vector3 Rotation
        {
            get => new Vector3(this._rotation[0], this._rotation[1], this._rotation[2]);
            set => this._rotation = new float[] { value.x, value.y, value.z };
        }

        public Orientation(Vector3 position, Vector3 rotation)
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
