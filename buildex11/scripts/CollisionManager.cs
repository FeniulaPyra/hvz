using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
	private List<GameObject> humans;
	private List<GameObject> zombies;
	private List<GameObject> eaten;

	public GameObject humanPrefab;
	public GameObject zombiePrefab;

	// Start is called before the first frame update
	void Start()
    {
		humans = new List<GameObject>();
		zombies = new List<GameObject>();
		eaten = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
		//gets current humans and zombies
		FindCurrentPlayers();

		//checks if humans are being eaten
		CheckForCollisions();
	}

	/// <summary>
	/// Gets the humans and zombies currently on the field
	/// </summary>
	private void FindCurrentPlayers()
	{
		//CollisionManager has forgotten [Everything]!
		humans.Clear();
		zombies.Clear();
		eaten.Clear();

		//CollisionManager has learned [Current Vehicles]!
		humans.AddRange(GameObject.FindGameObjectsWithTag("human"));
		zombies.AddRange(GameObject.FindGameObjectsWithTag("zombie"));
	}

	/// <summary>
	/// Checks for collisions between humans and zombies
	/// </summary>
	private void CheckForCollisions()
	{
		//loops through all the humans
		for(int h = 0; h < humans.Count; h++)
		{
			//gets current human
			GameObject curHuman = humans[h];
			Human hInfo = curHuman.GetComponent<Human>();

			//loops thru all zombies
			//m a y   I   h a v e   s o m e   l o o p s,   b r o t h e r?
			for(int z = 0; z < zombies.Count; z++)
			{
				//gets current zombie
				GameObject curZombie = zombies[z];
				Zombie zInfo = curZombie.GetComponent<Zombie>();

				//the max distance at which a collision will occur
				float distanceBeforeCollision = hInfo.radius + zInfo.radius;
				
				//checks if colliding
				if(Vector3.Distance(curHuman.transform.position, curZombie.transform.position) < distanceBeforeCollision)
				{
					
					Instantiate(zombiePrefab, curHuman.transform.position, Quaternion.identity);
					GameObject.Destroy(curHuman);
				}
			}
		}
	}

}
