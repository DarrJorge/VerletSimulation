using Unity.Collections;
using UnityEngine;

namespace VerletSimulation
{
    public class Rope : VerletBase
    {
        [SerializeField] private int stepCount = 10;
        [SerializeField] private Transform end;
        
        protected override void InitializePointsAndConstraints()
        {
            if (stepCount < 1) stepCount = 1;
            points = new NativeArray<Point>(stepCount + 1, Allocator.Persistent);
            distanceConstraints = new NativeArray<DistanceConstraint>(stepCount, Allocator.Persistent);
            
            var dir = end.position - transform.position;
            var distance = dir.magnitude;
            var step = distance / stepCount;
            var normalizedDir = dir / distance;

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Point(i * step * normalizedDir);
                if (i > 0)
                {
                    distanceConstraints[i - 1] = new DistanceConstraint(i - 1, i, new Vector2(0.5f * step, step));
                }
            }
            
            positionConstraints = new NativeArray<PositionConstraint>(2, Allocator.Persistent);
            positionConstraints[0] = new PositionConstraint(0, points[0].Position);
            positionConstraints[1] = new PositionConstraint(points.Length - 1, points[^1].Position);
        }
        

        protected override void OnPrepareBeforeSimulation()
        {
            positionConstraints[0] = new PositionConstraint(0, transform.position);
            positionConstraints[1] = new PositionConstraint(points.Length - 1, end.position);
        }
        
    }
}