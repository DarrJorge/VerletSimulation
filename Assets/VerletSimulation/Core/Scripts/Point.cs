using UnityEngine;

namespace VerletSimulation
{
    public struct Point
    {
        public Vector3 Position;
        public Vector3 PreviousPosition;
        public Quaternion Orientation;

        public Point(Vector3 position)
        {
            Position = position;
            PreviousPosition = position;
            Orientation = Quaternion.identity;
        }

        public Point(Vector3 position, Quaternion orientation)
        {
            Position = position;
            PreviousPosition = position;
            Orientation = orientation;
        }
    }
}