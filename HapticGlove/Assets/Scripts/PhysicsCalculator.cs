using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class PhysicsCalculator
	{
		public PhysicsCalculator ()
		{
		}

		public static float GetKineticEnergyOfCollision(Rigidbody hapticPoint, Rigidbody collisionObj){
			return 0.5f * (hapticPoint.mass * Mathf.Pow (Vector3.Magnitude(hapticPoint.velocity), 2) + collisionObj.mass * Mathf.Pow (Vector3.Magnitude(collisionObj.velocity), 2));
		}

		public static float GetFrictionForceOfCollision(Vector3 normalPoint, Rigidbody collisionObj, PhysicMaterial materialObj){
			//Create weight vector pointing to the ground
			Vector3 w = new Vector3(0,-1f,0)*collisionObj.mass*9.8f;

			//Get the projection of the vector in the normal direction of the collision point
			Vector3 projectionN = Vector3.Project(w, normalPoint);

			//Normal force magnitude
			return materialObj.dynamicFriction*Vector3.Magnitude(projectionN);
		}
	}
}

