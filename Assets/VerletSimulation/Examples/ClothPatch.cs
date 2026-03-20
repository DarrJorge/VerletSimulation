using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace VerletSimulation
{
    public class ClothPatch : VerletBase
    {
        [SerializeField] private Vector2Int stepCount;
        [SerializeField] private Vector2 size;
        
        protected override void InitializePointsAndConstraints()
        {
            var pointCount = new Vector2Int(stepCount.x + 1, stepCount.y + 1);
            points = new NativeArray<Point>(pointCount.x * pointCount.y, Allocator.Persistent);
            var step = new Vector2(size.x / stepCount.x, size.y / stepCount.y);
            
            var distConstraintList = new NativeList<DistanceConstraint>(Allocator.Temp);

            for (int y = 0; y < pointCount.y; y++)
            {
                var offsetY = y * pointCount.y;
                
                for (int x = 0; x < pointCount.x; x++)
                {
                    var index = offsetY + x;
                    points[index] = new Point(transform.TransformPoint(
                        new Vector3(x * step.x, -y * step.y, 0)));

                    if (x > 0)
                    {
                        distConstraintList.Add(new DistanceConstraint(index, index - 1, 
                            new Vector2(0.5f * step.x, step.x)));
                    }
                    
                    if (y > 0)
                    {
                        distConstraintList.Add(new DistanceConstraint(index, index - pointCount.x, 
                            new Vector2(0.5f * step.y, step.y)));
                    }
                }
            }
            
            distanceConstraints = new NativeArray<DistanceConstraint>(distConstraintList.Length, Allocator.Persistent);
            distanceConstraints.CopyFrom(distConstraintList.AsArray());
            
            positionConstraints = new NativeArray<PositionConstraint>(2, Allocator.Persistent);
            positionConstraints[0] = new PositionConstraint(0, points[0].Position);
            positionConstraints[1] = new PositionConstraint(stepCount.x, points[stepCount.x].Position);
        }

        protected override void OnPrepareBeforeSimulation()
        {
            positionConstraints[0] = new PositionConstraint(0, transform.position);
            positionConstraints[1] = new PositionConstraint(stepCount.x, 
                transform.TransformPoint(new Vector3(size.x, 0, 0)));
        }
    }
}
