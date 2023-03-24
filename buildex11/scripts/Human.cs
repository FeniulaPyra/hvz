using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//humans are ducks. [x-files theme plays]
public class Human : Vehicle
{
	public GameObject zombie;
	public float frictionCoefficient;
	public float awarenessRange;


	/// <summary>
	/// Applies a fleeing force to the human, which is slowed down by friction.
	/// </summary>
	override public void CalcSteeringForces()
	{
		//finds the closest thing to run away from
		GetClosestZombie();

		//stops the go
		Vector3 ultimateForce = Vector3.zero;

		//goes the go
		if (Vector3.Distance(zombie.transform.position, transform.position) < awarenessRange)
		{ 
			ultimateForce += Flee(zombie);
		}
		//slows the go
		else
			ultimateForce = ApplyFriction(frictionCoefficient);
		ultimateForce += AvoidAvoidables();
		//makes sure it doesn't yeet into the ground 
		ultimateForce.y = 0;

		ApplyForce(ultimateForce);
	}


	private void GetClosestZombie()
	{
		List<GameObject> zombies = new List<GameObject>(GameObject.FindGameObjectsWithTag("zombie"));

		//no index errors
		if (zombies.Count > 0)
		{
			//first thing to check against, otherwise might get a sad boi
			zombie = zombies[0];

			//check through all the humans to find the closest one.
			foreach (GameObject z in zombies)
			{
				if (Vector3.Distance(z.transform.position, transform.position) < Vector3.Distance(zombie.transform.position, transform.position))
					zombie = z;
			}
		}
	}

	
}
