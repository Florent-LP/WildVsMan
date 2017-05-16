using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FishingArea : MonoBehaviour {

	private GameObject fishesGO;
	private List<GameObject> fishes;
	private GameObject player;
    private WolfPlayerActions wolfPlayerAct = null;
	public GameObject barrier;

	public float distanceToFish = 8.0f;
	public float fishJump = 3.0f;
	public float distanceToDrink;

	//public bool hasFished;
	public GameObject inputToFish;
	private GameObject fish;

	public GameObject inputToDrink;


	private float elapsedTimed = 0;

	private bool isPlayer;

	void Start()
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
            wolfPlayerAct = player.GetComponent<WolfPlayerActions>();

            isPlayer = true;
		}
	}

	void Update()
	{
        if (player == null) return;
        if (player.gameObject.GetComponent<Animator>().GetBool("isEating")) return;

        fish = isPlayerNear(distanceToFish);

        if (fish == null)
            inputToFish.SetActive(false);
        else if (Input.GetKeyDown(KeyCode.E))
            Fish(fish);

        if (!inputToFish.activeInHierarchy && !wolfPlayerAct.hudEat.activeInHierarchy) {
			//if (Vector3.Distance(transform.position, player.transform.position) < distanceToDrink) {
				//Debug.Log ("1!");
				inputToDrink.SetActive(true);

				if (Input.GetKeyDown(KeyCode.E))
					wolfPlayerAct.Drink();
			/*}
			else {
				//Debug.Log ("2!");
				inputToDrink.SetActive(false);
			}*/
		} else {
			//Debug.Log ("3!");
			inputToDrink.SetActive(false);
		}
	}

	void Fish(GameObject fish)
	{
		fish.transform.Rotate(new Vector3(fish.transform.rotation.x, fish.transform.rotation.x, fish.transform.rotation.z + 90)); 
		fish.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + fishJump, player.transform.position.z + 3);
		fish.GetComponent<CapsuleCollider> ().enabled = true;
		fish.GetComponent<Rigidbody> ().isKinematic = false;
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
				inputToFish.SetActive (true);
				return fish;	
			}
		}
		return null;
	}
}
