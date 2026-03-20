using System;
using UnityEngine;

namespace VerletSimulation
{
    public readonly struct DistanceConstraint
    {
        public readonly int Index0;
        public readonly int Index1;
        public readonly Vector2 DistanceRange;
        public readonly Vector2 SqrDistanceRange;

        public DistanceConstraint(int index0, int index1, Vector2 distanceRange)
        {
            Index0 = index0;
            Index1 = index1;
            DistanceRange = distanceRange;
            SqrDistanceRange = new Vector2(
                distanceRange.x * distanceRange.x, 
                distanceRange.y * distanceRange.y);
        }
    }
    
    public readonly struct PositionConstraint
    {
      public readonly int Index;
      public readonly Vector3 Position;

      public PositionConstraint(int index, Vector3 position)
      {
          Index = index;
          Position = position;
      }
    }
}