using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//zombies are cats and they eat the ducks. this is rude, so the ducks run away.
public class Zombie : Vehicle
{
	public float frictionCoefficient;
	public Material targetLineMaterial;

	/// <summary>
	/// Applies a seeking force to the zombie, such that it can catch and consume the b r a i n s.
	/// </summary>
	override public void CalcSteeringForces()
	{
		target = GetClosestTarget("human");

		Vector3 ultForce = Vector3.zero;

		if (target != null && Vector3.Distance(transform.position, target.transform.position) < radiusOfAwareness)
		{
			Vector3 seekingForce = Pursue(target);
			float distToFood = Vector3.Distance(transform.position, target.transform.position + target.GetComponent<Vehicle>().velocity);
			if (distToFood < arrivalDistance)
				seekingForce *= distToFood;
			ultForce += seekingForce;
		}
		else
			ultForce += Wander();

		ultForce += ApplyFriction(frictionCoefficient);

		foreach(GameObject o in GameObject.FindGameObjectsWithTag("obstacle"))
			ultForce += ObstacleAvoidance(o);

		ultForce += Separate();

		//makes sure it doesn't yeet into the ground
		ultForce.y = 0;

		ApplyForce(ultForce);
	}

	private void OnRenderObject()
	{
		if(target != null && Input.GetKey(KeyCode.L))
		{
			targetLineMaterial.SetPass(0);

			GL.Begin(GL.LINES);
			GL.Vertex(vehiclePosition);
			GL.Vertex(target.transform.position);
			GL.End();
		}
	}

}
