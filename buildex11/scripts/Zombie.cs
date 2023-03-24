using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//zombies are cats and they eat the ducks. this is rude, so the ducks run away.
public class Zombie : Vehicle
{
	public GameObject brains;
	public float frictionCoefficient;
	public Material targetLineMaterial;

	/// <summary>
	/// Applies a seeking force to the zombie, such that it can catch and consume the b r a i n s.
	/// </summary>
	override public void CalcSteeringForces()
	{
		GetClosestTarget();
		Vector3 ultForce = Vector3.zero;
		if (brains != null)
			ultForce = (Seek(brains) + ApplyFriction(frictionCoefficient));
		else
			ultForce = ApplyFriction(frictionCoefficient);
		ultForce += AvoidAvoidables();
		//makes sure it doesn't yeet into the ground
		ultForce.y = 0;

		ApplyForce(ultForce);
	}

	/// <summary>
	/// Finds the closest human to eat.
	/// </summary>
	public void GetClosestTarget()
	{
		List<GameObject> allTheBrains = new List<GameObject>(GameObject.FindGameObjectsWithTag("human"));

		//no index errors
		if (allTheBrains.Count > 0)
		{
			//first thing to check against, otherwise might get a sad boi
			brains = allTheBrains[0];

			//check through all the humans to find the closest one.
			foreach (GameObject b in allTheBrains)
			{
				if (Vector3.Distance(b.transform.position, transform.position) < Vector3.Distance(brains.transform.position, transform.position))
					brains = b;
			}
		}
		
	}

	private void OnRenderObject()
	{
		if(brains != null && Input.GetKey(KeyCode.L))
		{
			targetLineMaterial.SetPass(0);

			GL.Begin(GL.LINES);
			GL.Vertex(vehiclePosition);
			GL.Vertex(brains.transform.position);
			GL.End();
		}
	}

}
