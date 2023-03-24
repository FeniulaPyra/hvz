using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Vehicle : MonoBehaviour
{
	// Vectors necessary for force-based movement
	protected Vector3 vehiclePosition;
	public Vector3 acceleration;
	public Vector3 direction;
	public Vector3 velocity;

	//floor
	public GameObject floor;
	Vector2 floorDimensions;

	// Floats
	public float mass;
	public float maxSpeed;
	public float radius;

	public Material velocityMaterial;
	public Material directionMaterial;

	public List<GameObject> avoidables;

	GameControls controls;

	// Use this for initialization
	void Start()
	{
		controls = GameObject.Find("GameManagement").GetComponent<GameControls>();
		vehiclePosition = transform.position;
		velocity = Vector3.zero;
		acceleration = new Vector3(.1f, 0, 0);
		floor = GameObject.Find("Floor");
		floorDimensions = new Vector2(floor.GetComponent<MeshRenderer>().bounds.extents.x/2 * floor.transform.localScale.x, floor.GetComponent<MeshRenderer>().bounds.extents.z/2 * floor.transform.localScale.y);//new Vector2(floor.transform.lossyScale.x/2, floor.transform.lossyScale.z/2);
	}

	// Update is called once per frame
	void Update()
	{
		//finds the go direction and how hard he go
		CalcSteeringForces();

		//makes it so how much the boi go is based on time and not whether or not your pc is a potato
		velocity += acceleration * Time.deltaTime;

		//he can't turn back now dundundun....
		if (velocity.magnitude < 0)
			velocity = Vector3.zero;

		//changes where the boi will go
		vehiclePosition += velocity * Time.deltaTime * controls.speedByFps;
		direction = velocity.normalized;

		//animates the boi
		if(velocity.magnitude > .1)
		{
			//f a s t
			gameObject.transform.LookAt(vehiclePosition);
			gameObject.GetComponent<Animator>().SetBool("Idle", false);
			gameObject.GetComponent<Animator>().SetBool("Walk", true);
		}
		else
		{
			//he dont do
			gameObject.GetComponent<Animator>().SetBool("Idle", true);
			gameObject.GetComponent<Animator>().SetBool("Walk", false);
		}

		Debug.DrawLine(transform.position, transform.position + transform.forward * 3);
		Debug.DrawLine(transform.position, transform.position + transform.right * 3);

		//stuff that would have made the bois bounce off the obstacles, but it dont do so its being yeeted
		//AvoidableBind();
		//AvoidableBounce();

		//DON GO YET
		acceleration = Vector3.zero;

		//(to "let it snow") the weather outside is out of bounds, so stay inside the ground area, something i forget the words here, please dont fall pleasee dont fall please dont fall
		Bounce();

		//this ones so they dont speed past the bounds or get stuck rigt at the edge
		Bind();

		//does the actual vroom
		transform.position = vehiclePosition;
		
	}

	// ApplyForce
	// Receive an incoming force, divide by mass, and apply to the cumulative accel vector
	public void ApplyForce(Vector3 force)
	{
		acceleration += force / mass;
	}

	// ApplyForce
	// Receive an incoming force, divide by mass, and apply to the cumulative accel vector
	public void ApplyGravityForce(Vector3 force)
	{
		acceleration += force;
	}

	/// <summary>
	/// Returns a frictional force given the coefficient of friction between this object and another.
	/// </summary>
	/// <param name="coeff">The coefficient of friction</param>
	/// <returns>A vector of the frictional force</returns>
	public Vector3 ApplyFriction(float coeff)
	{
		Vector3 friction = -velocity;

		friction.Normalize();

		friction = friction * coeff;

		return friction;
	}

	/// <summary>
	/// Seek
	/// </summary>
	/// <param name="targetPosition">Vector3 position of desired target</param>
	/// <returns>Steering force calculated to seek the desired target</returns>
	public Vector3 Seek(Vector3 targetPosition)
	{
		// Step 1: Find DV (desired velocity)
		// TargetPos - CurrentPos
		Vector3 desiredVelocity = targetPosition - vehiclePosition;

		// Step 2: Scale vel to max speed
		// desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);
		desiredVelocity.Normalize();
		desiredVelocity = desiredVelocity * maxSpeed;

		// Step 3:  Calculate seeking steering force
		Vector3 seekingForce = desiredVelocity - velocity;

		// Step 4: Return force
		return seekingForce;
	}

	/// <summary>
	/// Overloaded Seek
	/// </summary>
	/// <param name="target">GameObject of the target</param>
	/// <returns>Steering force calculated to seek the desired target</returns>
	public Vector3 Seek(GameObject target)
	{
		return Seek(target.transform.position);
	}

	/// <summary>
	/// Returns a force at which the "vehicle" would flee from the input target position
	/// </summary>
	/// <param name="targetPosition">The position of the target to flee from</param>
	/// <returns>A vector representing how forcefully the "vehicle" will flee from the target position</returns>
	public Vector3 Flee(Vector3 targetPosition)
	{
		// Step 1: Find DV (desired velocity)
		// TargetPos - CurrentPos
		Vector3 desiredVelocity = vehiclePosition - targetPosition;

		// Step 2: Scale vel to max speed
		// desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);
		desiredVelocity.Normalize();
		desiredVelocity = desiredVelocity * maxSpeed;

		// Step 3:  Calculate seeking steering force
		Vector3 fleeingForce = desiredVelocity - velocity;

		
		// Step 4: Return force
		return fleeingForce;
	}

	/// <summary>
	/// Returns a force at which the "vehicle" would flee from the given target
	/// </summary>
	/// <param name="targetPosition">The target to flee from</param>
	/// <returns>A vector representing how forcefully the "vehicle" will flee from the target</returns>
	public Vector3 Flee(GameObject target)
	{
		return Flee(target.transform.position);
	}

	/// <summary>
	/// Bounces the object off of the edge of the area
	/// </summary>
	public void Bounce()
	{

		if (vehiclePosition.x >= floorDimensions.x || vehiclePosition.x <= -floorDimensions.x)
			velocity = (new Vector3(-velocity.x, 0, 0));

		if (vehiclePosition.z >= floorDimensions.y || vehiclePosition.z <= -floorDimensions.y)
			velocity = (new Vector3(0, 0, -velocity.z));

	}

	//keeps objects from shooting out of bounds
	public void Bind()
	{

		//sliiiide to the left!
		if (vehiclePosition.x > floorDimensions.x)
			vehiclePosition.x -= Mathf.Abs(vehiclePosition.x - floorDimensions.x);
		//sliiide to the right!
		else if (vehiclePosition.x < -floorDimensions.x)
			vehiclePosition.x += Mathf.Abs(vehiclePosition.x + floorDimensions.x);

		//How low can you go? (not super low cuz we arent looping the edge)
		if (vehiclePosition.z > floorDimensions.y)
			vehiclePosition.z -= Mathf.Abs(vehiclePosition.z - floorDimensions.y);
		//Can you bring it to the top? (no but he can go a lil bit so hes not toooo low)
		else if (vehiclePosition.z < -floorDimensions.y)
			vehiclePosition.z += Mathf.Abs(vehiclePosition.z + floorDimensions.y);

	}

	private void OnRenderObject()
	{
		if (Input.GetKey(KeyCode.L))
		{
			velocityMaterial.SetPass(0);

			GL.Begin(GL.LINES);
			GL.Vertex(vehiclePosition);
			GL.Vertex(vehiclePosition + velocity);
			GL.End();
		}
	}

	/// <summary>
	/// To be inherited by children.
	/// </summary>
	public abstract void CalcSteeringForces();

	/// <summary>
	/// Finds only the objects that this vehicle needs to avoid.
	/// </summary>
	public void GetAvoidables()
	{
		avoidables.Clear();
		List<GameObject> obstacles = new List<GameObject>(GameObject.FindGameObjectsWithTag("obstacle"));

		foreach (GameObject o in obstacles)
		{
			//gets the center of the obstacle and its radius
			Vector3 obstaclePosition = o.transform.position;
			float obstacleRadius = o.GetComponent<Obstacle>().radius;

			//gets the vector from the vehicle to the obstacle
			Vector3 toObstacle = obstaclePosition - transform.position;

			//toObstacle is projected onto this objects right vector (toObstacle needs to not project so much smh)
			float projectionDistance = Vector3.Dot(toObstacle, transform.right);

			float projectionFront = Vector3.Dot(toObstacle, transform.forward);

			//if the obstacle is too close
			if(projectionDistance < radius + obstacleRadius && projectionFront > 0 && Vector3.Distance(obstaclePosition, transform.position) < velocity.magnitude)
			{
				//yeets it into the o no array
				avoidables.Add(o); // y e e t intensifies
				//yeet intestinesfies
				//yet intestines
				//ye internal organ
				//inside or
				//in or
				//inner
				//in
				//i
				//
			}
		}
	}

	public Vector3 AvoidAvoidables()
	{
		GetAvoidables();

		Vector3 ultForce = Vector3.zero;

		foreach(GameObject o in avoidables)
		{
			if(Vector3.Dot(o.transform.position, transform.right) > 0)
			{
				ultForce += transform.right * maxSpeed;// * 2 / Mathf.Abs(Vector3.Distance(o.transform.position, transform.position));
			}
			else
			{
				ultForce += -transform.right * maxSpeed;// * 2 / Mathf.Abs(Vector3.Distance(o.transform.position, transform.position));
			}
		}

		return ultForce;
	}

	public void AvoidableBounce()
	{
		foreach(GameObject o in avoidables)
		{
			if(Vector3.Distance(o.transform.position, transform.position) <= o.GetComponent<Obstacle>().radius + radius)
			{
				velocity *= -1;
			}
		}
	}
	public void AvoidableBind()
	{
		foreach(GameObject o in avoidables)
		{
			if (Vector3.Distance(o.transform.position, transform.position) <= o.GetComponent<Obstacle>().radius + radius)
			{
				vehiclePosition = (transform.position - o.transform.position).normalized * (o.GetComponent<Obstacle>().radius + radius);
				Debug.Log("inside thing");
			}

		}
	}
}
