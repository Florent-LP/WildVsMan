using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //NavMeshAgent

public class FarmerShoot : MonoBehaviour {

	public List<Transform> visibleTargets = new List<Transform> ();
	public GameObject target;
	private NavMeshAgent agent;

	public GameObject farmerArea;


	private float walkSpeed;
	private float chaseSpeedCoeff = 4.0f;

	private float alertStartTime;
	static public float safeStartTime;

	//private TextMesh alertText;
	public GameObject popPosition;

	public float timeFarmerOut;

	public float distanceToShoot;

	private float startToShoot;
	private float waitToShoot;

	private float distanceShootMin = 0.0f;
	private float distanceShootMed = 20.0f;
	private float distanceShootMax = 40.0f;

	private float probaMin = 0.01f;
	private float probaMed = 0.05f;

	public bool isGoingBack;

	public float damage;


	private bool hasShoot;

	void OnEnable()
	{
		if (target == null)
			target = GameObject.FindGameObjectWithTag ("Player");
		startToShoot = 0.8f;
		waitToShoot = 5.0f;
		agent = GetComponent<NavMeshAgent>();
		walkSpeed = agent.speed;
		//alertText = gameObject.GetComponent<TextMesh> ();
		//alertText.transform.position = transform.position;
		alertStartTime = Time.time;
		safeStartTime = 0;
		//alertText.text = alertStartTime.ToString ();
		RunToPlayer ();
	}

	public void RunToPlayer()
	{
		Debug.Log ("Run!");
		GetComponent<Animator> ().SetBool ("isWalking", false);
		GetComponent<Animator> ().SetBool ("isAlerted", true);
		agent.speed = chaseSpeedCoeff * walkSpeed;
		agent.destination = target.transform.position;
	}

	public void StopFarmer()
	{
		Debug.Log ("Stop");
		GetComponent<Animator> ().SetBool ("isAlerted", false);
		agent.speed = walkSpeed;
		agent.Stop ();
	}

	public void BackToHouse()
	{
		Debug.Log ("WHERE ARE YOU?!");
		transform.LookAt (popPosition.transform.position);
		agent.speed = walkSpeed;
		agent.Resume ();
		safeStartTime = 0;
		GetComponent<Animator> ().SetBool ("isWalking", true);
		GetComponent<Animator> ().SetBool ("isAlerted", false);
		agent.destination = popPosition.transform.position;
	}

	public void TurnToPlayer()
	{
		transform.LookAt (target.transform.position);
	}

	void FixedUpdate()
	{
		if (!isGoingBack) 
		{
			TurnToPlayer ();
		}
		visibleTargets = GetComponent<FieldOfViewFarmer> ().visibleTargets;

		if ((visibleTargets.Count <= 0) && (safeStartTime <= 0)) 
		{
			Debug.Log("Player SAFE begins!");
			safeStartTime = Time.time;
		}

		if (visibleTargets.Count > 0) 
		{
			safeStartTime = 0;
			float distance = Vector3.Distance (transform.position, visibleTargets [0].position);
			float proba1 = Random.Range (0.0f, 1.0f);
			float proba2 = Random.Range (0.0f, 1.0f);
			//Debug.Log ("(distance,proba1, proba2): " + "(" + distance + "," + proba1 + "," + proba2 + ")");

			if (distance >= distanceShootMax) //wolf far from farmer
			{
				if (proba1 < probaMin && proba2 < probaMin) 
				{
					if(GetComponent<Animator> ().GetBool("Shoot") == false)
					{
						//Invoke ("StartShoot", startToShoot);
						StartCoroutine (Shoot (startToShoot, waitToShoot));

					}
				}
			}

			if (distance < distanceShootMax && distance >= distanceShootMed) //wolf not so far from farmer
			{
				if (proba1 < probaMed && proba2 < probaMed) 
				{
					if(GetComponent<Animator> ().GetBool("Shoot") == false)
					{
						//Invoke ("StartShoot", startToShoot);
						StartCoroutine (Shoot (startToShoot, waitToShoot));

					}
				}
			}

			if (distance < distanceShootMed && distance >= distanceShootMin) //wolf near from farmer
			{
				if (proba1 < probaMed && proba2 > probaMed) 
				{
					if(GetComponent<Animator> ().GetBool("Shoot") == false)
					{
						//Invoke ("StartShoot", startToShoot);
						StartCoroutine (Shoot (startToShoot, waitToShoot));
					}
				}
			}
		}

		float t = Time.time - safeStartTime;
		if (((int)t%60 >= timeFarmerOut) && (safeStartTime > 0)) 
		{
			if(!isGoingBack)
				BackToHouse ();
			isGoingBack = true;
		}

		if (isGoingBack) 
		{
			//Debug.Log ("d: " + d);
			if (Vector3.Distance (transform.position, popPosition.transform.position) < 0.5f) {
				destroyFarmer ();
			} 
			else 
			{
				Invoke ("destroyFarmer", 3.0f);
			}
		}
	}
		

	public IEnumerator Shoot(float waitTransition, float waitBetweenShots)
	{
		if (!hasShoot)
		{
			hasShoot = true;
			GetComponent<Animator> ().SetTrigger ("Shoot");
			yield return new WaitForSeconds (waitTransition);
			GetComponent<Animator> ().ResetTrigger ("Shoot");
			PlayerDamage ();
			yield return new WaitForSeconds (waitBetweenShots);
			hasShoot = false;
		}
	}

	/*public void StartShoot()
	{
		GetComponent<Animator> ().SetTrigger ("Shoot");
		Invoke ("StopShoot", waitToShoot);
	}

	public void StopShoot()
	{
		GetComponent<Animator> ().ResetTrigger ("Shoot");
		PlayerDamage ();
	}*/
		
	public void PlayerDamage()
	{
		target.GetComponent<PlayerHealth> ().health -= damage;
	}

	public void destroyFarmer()
	{
		/*popPosition.SetActive (false);
		Destroy (gameObject);*/
		isGoingBack = false;
		farmerArea.SetActive (false);
	}

	void Update() 
	{
		/*if (alertStartTime>0) 
		{
			float t = Time.time - alertStartTime;
			string minutes = ((int)t / 60).ToString ();
			string seconds = (t % 60).ToString ("F2");
			//alertText.text = minutes + ":" + seconds;
		}*/
	}
}
