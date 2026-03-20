using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace VerletSimulation
{
    [BurstCompile]
    public struct VerletSimulationJob : IJob
    {
        public NativeArray<Point> Points;
        public NativeArray<DistanceConstraint> DistanceConstraints;
        public NativeArray<PositionConstraint> PositionConstraints;
        public Vector3 Gravity;
        public float DeltaTime;
        public int Iterations;
        
        public void Execute()
        {
            if (Iterations < 1) Iterations = 1;
            DeltaTime /= Iterations;
            float sqrDeltaTime = DeltaTime * DeltaTime;

            for (int i = 0; i < Iterations; i++)
            {
                for (int j = 0; j < Points.Length; j++)
                {
                    var point = Points[j];
                    var position = point.Position;
                    point.Position += (position - point.PreviousPosition) + Gravity * sqrDeltaTime;
                    point.PreviousPosition = position;
                    Points[j] = point;
                }
                ApplyConstraints();
            }
        }

        [BurstCompile]
        private void ApplyConstraints()
        {
            for (int i = 0; i < DistanceConstraints.Length; i++)
            {
                var constraint = DistanceConstraints[i];
                var point0 = Points[constraint.Index0];
                var point1 = Points[constraint.Index1];
                
                var dir = point1.Position - point0.Position;
                var sqrDistance = dir.sqrMagnitude;
                
                if (sqrDistance > constraint.SqrDistanceRange.y)
                {
                    var distance = Mathf.Sqrt(sqrDistance);
                    var normalizedDir = dir / distance;
                    
                    var offset = (distance - constraint.DistanceRange.y) * 0.5f;
                    point0.Position = point0.Position + offset * normalizedDir;
                    point1.Position = point1.Position - offset * normalizedDir;
                    
                    Points[constraint.Index0] = point0;
                    Points[constraint.Index1] = point1;
                }
                else if (sqrDistance < constraint.SqrDistanceRange.x)
                {
                    var distance = Mathf.Sqrt(sqrDistance);
                    var normalizedDir = dir / distance;
                    
                    var offset = (distance - constraint.DistanceRange.x) * 0.5f;
                    point0.Position = point0.Position + offset * normalizedDir;
                    point1.Position = point1.Position - offset * normalizedDir;
                    
                    Points[constraint.Index0] = point0;
                    Points[constraint.Index1] = point1;
                }
            }
            
            for (int i = 0; i < PositionConstraints.Length; i++)
            {
                var constraint = PositionConstraints[i];
                var point = Points[constraint.Index];
                point.Position = constraint.Position;
                Points[constraint.Index] = point;
            }
        }
    }
}