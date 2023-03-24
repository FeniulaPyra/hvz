using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControls : MonoBehaviour
{

	GameObject floor;
	Vector2 bounds;

	public GameObject humanPrefab;
	public GameObject zombiePrefab;
	public GameObject seedPrefab;

	public float speedByFps;

	// Start is called before the first frame update
	void Start()
    {
		speedByFps = 1;

		floor = GameObject.Find("Floor");
		bounds = new Vector2(floor.GetComponent<MeshRenderer>().bounds.extents.x / 2 * floor.transform.localScale.x, floor.GetComponent<MeshRenderer>().bounds.extents.z / 2 * floor.transform.localScale.y);

	}

	// Update is called once per frame
	void Update()
    {
		if (Input.GetKeyDown(KeyCode.H))
		{
			Instantiate(humanPrefab, new Vector3(Random.Range(-bounds.x, bounds.x), 0, Random.Range(-bounds.y, bounds.y)), Quaternion.identity);
			if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				foreach(GameObject h in GameObject.FindGameObjectsWithTag("human"))
				{
					Destroy(h);
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.Z))
		{
			Instantiate(zombiePrefab, new Vector3(Random.Range(-bounds.x, bounds.x), 0, Random.Range(-bounds.y, bounds.y)), Quaternion.identity);
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				foreach (GameObject z in GameObject.FindGameObjectsWithTag("zombie"))
				{
					Destroy(z);
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.F))
		{
			Instantiate(seedPrefab, new Vector3(Random.Range(-bounds.x, bounds.x), 0, Random.Range(-bounds.y, bounds.y)), Quaternion.identity);

			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				foreach (GameObject f in GameObject.FindGameObjectsWithTag("food"))
				{
					Destroy(f);
				}
			}
		}

		//resets when r is pressed
		if (Input.GetKeyDown(KeyCode.R))
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);

		//slow the game
		if (Input.GetKeyDown(KeyCode.Minus))
		{
			if (speedByFps > 0)
				speedByFps -= .1f;
		}
		//speed up the game
		else if (Input.GetKeyDown(KeyCode.Equals))
		{
			speedByFps += .1f;
		}
		//pause the game
		else if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			speedByFps = 0;
		}
		//play game at regular speed
		else if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			speedByFps = 1;
		}
	}


	private void OnGUI()
	{
		GUI.Box(new Rect(16, 16, 300, 140), "R to reset\nH to add Human(duck) (+shift to delete all ducks)\nZ to add Zombie(cat) (+shift to delete all zombies)\nF to add Food(seed pile) (+shift to delete all food)\n- to slow everything down\n+ to speed everything up\n0 to pause\n1 to go at regular speed\nHold L to show lines");
	}
}
