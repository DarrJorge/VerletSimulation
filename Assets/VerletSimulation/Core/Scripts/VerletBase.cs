using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace VerletSimulation
{
    public abstract class VerletBase : MonoBehaviour
    {
        [SerializeField] protected int iterations = 10;

        protected NativeArray<Point> points;
        protected NativeArray<DistanceConstraint> distanceConstraints;
        protected NativeArray<PositionConstraint> positionConstraints;
        private JobHandle jobHandle;
        
        protected abstract void InitializePointsAndConstraints();
        protected abstract void OnPrepareBeforeSimulation();

        private void Start()
        {
            InitializePointsAndConstraints();
        }

        private void FixedUpdate()
        {
            OnPrepareBeforeSimulation();

            jobHandle = new VerletSimulationJob()
            {
                Points = points,
                DeltaTime = Time.fixedDeltaTime,
                Iterations = iterations,
                Gravity = Physics.gravity,
                DistanceConstraints = distanceConstraints,
                PositionConstraints = positionConstraints
            }.Schedule(jobHandle);
        }

        private void LateUpdate()
        {
            jobHandle.Complete();
        }

        private void OnDestroy()
        {
            if (points.IsCreated) points.Dispose();
            if (distanceConstraints.IsCreated) distanceConstraints.Dispose();
            if (positionConstraints.IsCreated) positionConstraints.Dispose();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < points.Length; i++)
            {
                Gizmos.DrawSphere(points[i].Position, 0.1f);
            }
            
            Gizmos.color = Color.red;
            for (int i = 0; i < distanceConstraints.Length; i++)
            {
                var constraint = distanceConstraints[i];
                Gizmos.DrawLine(points[constraint.Index0].Position, points[constraint.Index1].Position);
            }
        }
    }
}