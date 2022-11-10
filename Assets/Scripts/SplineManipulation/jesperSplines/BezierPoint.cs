using UnityEngine;

namespace AnimationSystem.Splines
{
    public struct BezierPoint
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public BezierPoint(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public BezierPoint(Vector3 position, Vector3 forward)
        {
            Position = position;
            Rotation = Quaternion.LookRotation(forward);
        }

        public Vector3 LocalToWorldPosition(Vector3 localSpacePosition)
        {
            return Position + Rotation * localSpacePosition;
        }

        public Vector3 LocalToWorldVector(Vector3 localSpaceVector)
        {
            return Rotation * localSpaceVector;
        }
    }
}