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
			return 0.5f * (hapticPoint.mass * Mathf.Pow (hapticPoint.velocity, 2) + collisionObj.mass * Mathf.Pow (collisionObj.velocity, 2));
		}

		public static float GetFrictionForceOfCollision(Rigidbody object1, Rigidbody object2, PhysicMaterial material1, PhysicMaterial material2){
			
		}
	}
}

