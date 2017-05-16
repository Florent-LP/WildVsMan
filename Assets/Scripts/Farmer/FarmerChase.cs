using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //NavMeshAgent

public class FarmerChase : MonoBehaviour {

	public List<Transform> visibleTargets = new List<Transform> ();
	public GameObject target;
	private NavMeshAgent agent;

	private float walkSpeed;
	private float chaseSpeedCoeff = 2.0f;

	private float alertStartTime;
	private float safeStartTime;

	private TextMesh alertText;
	public GameObject popPosition;

	// Use this for initialization
	void Start () 
	{
		agent = GetComponent<NavMeshAgent>();
		walkSpeed = agent.speed;
		alertText = gameObject.GetComponent<TextMesh> ();
		alertStartTime = Time.time;
		safeStartTime = 0;
		alertText.transform.position = gameObject.transform.position;
		alertText.text = alertStartTime.ToString ();
	}

	public void ChasePlayer()
	{
		if(GetComponent<Animator> ().GetBool ("isWalking")) 
		{
			GetComponent<AudioSource> ().PlayOneShot (GetComponent<AudioSource> ().clip);
		}
		GetComponent<Animator> ().SetBool ("isWalking", false);
		GetComponent<Animator> ().SetBool ("isAlerted", true);
		agent.speed = chaseSpeedCoeff * walkSpeed;
		agent.destination = target.transform.position;
	}

	public void BackToHouse()
	{
		Debug.Log ("WHERE ARE YOU?!");
		safeStartTime = 0;
		GetComponent<Animator> ().SetBool ("isWalking", true);
		GetComponent<Animator> ().SetBool ("isAlerted", false);
		agent.speed = walkSpeed;
		agent.destination = popPosition.transform.position;
	}

	void LateUpdate()
	{
		visibleTargets = GetComponent<FieldOfViewFarmer> ().visibleTargets;
		if (GetComponent<Animator> ().GetBool ("isAlerted") && !GetComponent<Animator> ().GetBool ("isWalking")) 
		{
			ChasePlayer ();
			if ((visibleTargets.Count <= 0) && (safeStartTime <= 0)) 
			{
				Debug.Log("Player SAFE begins!");
					safeStartTime = Time.time;
			}
			if (visibleTargets.Count > 0) 
			{
				safeStartTime = 0;
			}

			float t = Time.time - safeStartTime;

			if (((int)t%60 >= 10) && (safeStartTime > 0)) 
			{
				BackToHouse ();
			}
		} 
		if (GetComponent<Animator> ().GetBool ("isWalking")) 
		//else
		{
			safeStartTime = 0;
			if (visibleTargets.Count > 0) 
			{
				Debug.Log ("FOUND YOU!");
				GetComponent<AudioSource> ().PlayOneShot (GetComponent<AudioSource> ().clip);
				alertStartTime = Time.time;
				ChasePlayer ();
			} 
			else 
			{
				alertStartTime = 0;
				alertText.text = alertStartTime.ToString ();
				float d = Vector3.Distance (transform.position, popPosition.transform.position);
				if (d < 0.5f) 
				{
					destroyFarmer ();
				}
			}
		}
	}

	void destroyFarmer()
	{
		GetComponent<Animator> ().SetBool ("isWalking", false);
		popFarmer.isFarmer = false;
		/*popPosition.SetActive (false);
		Destroy (gameObject);*/
		gameObject.SetActive (false);
	}

	void Update() 
	{
		if (alertStartTime>0) 
		{
			float t = Time.time - alertStartTime;
			string minutes = ((int)t / 60).ToString ();
			string seconds = (t % 60).ToString ("F2");
			alertText.text = minutes + ":" + seconds;
		}
	}
}
