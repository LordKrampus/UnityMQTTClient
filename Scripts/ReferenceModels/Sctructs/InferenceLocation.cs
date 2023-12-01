using UnityEngine;

namespace CoordinateSystem
{
    public struct InferenceLocation
    {
        Quaternion _direction;
        float _distance;

        Quaternion _targetRotation;
        Vector3 _relativePosition;

        public Quaternion Direction => this._direction;
        public float Distance => this._distance;

        public Vector3 RelativePosition => this._relativePosition;
        public Quaternion TargetRotation => this._targetRotation;

        public InferenceLocation(Quaternion targetRotation, Quaternion direction, float distance)
        {
            this._direction = direction;
            this._distance = distance;

            this._targetRotation = targetRotation;

            this._relativePosition = CalcRelativePosition(direction, distance);
        }

        public static Vector3 CalcRelativePosition(Quaternion direction, float distance)
        {
            return Vector3.zero + (direction * Vector3.forward * distance);
        }

    }
}