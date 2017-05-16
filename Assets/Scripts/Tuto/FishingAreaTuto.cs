using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FishingAreaTuto : MonoBehaviour {

	private GameObject fishesGO;
	private List<GameObject> fishes;
	private GameObject player;
	public GameObject barrier;

	public float distanceToFish = 8.0f;
	public float fishJump = 8.0f;

	//public bool hasFished;
	public GameObject inputToFish;
	public GameObject inputToDrink;
	private GameObject fish;
	public float distanceToDrink;

	private float elapsedTimed = 0;

	public GameObject tuto1;
	public GameObject tuto2;
	public GameObject tuto3;
	private bool onTuto1 = false;
	private bool onTuto2 = false;
	private bool onTuto3 = false;

	private bool isPlayer;
	private bool isDaddy;
	private bool isDaddyFirst;

	void Awake()
	{
		fishesGO = transform.parent.FindChild ("Fishes").gameObject;
		fishes = new List<GameObject> ();
		foreach (Transform child in fishesGO.transform) 
		{
			if (child.tag == "Fish") 
			{
				fishes.Add (child.gameObject);
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.transform.gameObject.tag == "Player") 
		{
			//Debug.Log ("Player want some fish!!!");
			player = other.gameObject;
			isPlayer = true;
			if (isDaddy)
				isDaddyFirst = true;
		}
		if (other.transform.gameObject.tag == "WolfDaddy") 
		{
			isDaddy = true;
			other.GetComponent<DaddyWalk> ().StopDaddy ();
		}
	}

	void OnTriggerStay(Collider other)
	{

		if (other.transform.gameObject.tag == "Player") 
		{
			Debug.Log ("distance to water: " + Vector3.Distance (transform.position, player.transform.position));

			if (!onTuto1) 
			{
				if (isPlayer && isDaddy && !onTuto1) 
				{
					if(isDaddyFirst)
						player.GetComponent<Rigidbody> ().AddForce (-10*player.GetComponent<Rigidbody> ().GetPointVelocity (transform.position));
					PauseManager.onTuto = true;
					GameManager.tutoFish = true;
					tuto1.SetActive (true);
					onTuto1 = true;
				}
			}


			else if (!other.gameObject.GetComponent<Animator>().GetBool("isEating")) 
			{
				fish = isPlayerNear (distanceToFish);
				if (!inputToFish.activeInHierarchy && !other.gameObject.GetComponent<PlayerActionsTuto>().hudEat.activeInHierarchy) 
				{
					if (Vector3.Distance (transform.position, player.transform.position) < distanceToDrink && !PauseManager.onTuto) 
					{
						Debug.Log ("1!");
						inputToDrink.SetActive (true);
						if (Input.GetKeyDown (KeyCode.E) && !PauseManager.onTuto && !player.GetComponent<PlayerActionsTuto>().onDrink) 
						{
							player.GetComponent<PlayerActionsTuto> ().Drink ();
						}
					}
					else 
					{
						Debug.Log ("2!");
						inputToDrink.SetActive (false);
					}
				} 
				else 
				{
					Debug.Log ("3!");
					inputToDrink.SetActive (false);
				}
					
				if (fish) 
				{
					if (isPlayer && isDaddy && Part1.tuto1 && !onTuto2) 
					{
						PauseManager.onTuto = true;
						tuto2.SetActive (true);
						onTuto2 = true;
					}
				}
				if(Input.GetKeyDown(KeyCode.E) && !PauseManager.onTuto)
				{
					if (fish != null)
					{	
						Fish (fish);
						if (isPlayer && isDaddy && Part1.tuto2 && !onTuto3) 
						{
							PauseManager.onTuto = true;
							tuto3.SetActive (true);
							onTuto3 = true;
						}
					} 
				}
				if (fish == null) 
				{
					inputToFish.SetActive (false);
				}
			}
		}
	}

	void Fish(GameObject fish)
	{
		fish.GetComponent<Rigidbody> ().useGravity = true;
		fish.transform.Rotate(new Vector3(fish.transform.rotation.x, fish.transform.rotation.x, fish.transform.rotation.z + 90)); 
		fish.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z + 3.0f);
		fish.GetComponent<CapsuleCollider> ().enabled = true;
		fish.GetComponent<Animator> ().SetBool ("isDying", true);
		fishes.Remove (fish.gameObject);
	}

	void OnTriggerExit(Collider other)
	{
		if (other.transform.gameObject.tag == "Player") 
		{
			//Debug.Log ("Player has enough of fishes!!!");
			player = null;
			inputToFish.SetActive (false);
			inputToDrink.SetActive (false);
		}
	}

	GameObject isPlayerNear(float distanceToFish)
	{
		foreach (GameObject fish in fishes) 
		{
			if(Vector3.Distance(player.transform.position, fish.transform.position) <= distanceToFish)
			{
				//Debug.Log ("Come on fish!");
				if (onTuto1 && !PauseManager.onTuto && Part1.tuto2) 
				{
					inputToFish.SetActive (true);
				}
				return fish;	
			}
		}
		return null;
	}
}
