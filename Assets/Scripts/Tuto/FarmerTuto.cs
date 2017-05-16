using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //NavMeshAgent
using UnityStandardAssets.ImageEffects;
using UnityEngine.SceneManagement;

public class FarmerTuto : MonoBehaviour {

	private NavMeshAgent agent;

	public Transform daddyWolf;

	public float distanceStop;

	private float walkSpeed;
	private float chaseSpeedCoeff = 4.0f;

	public float waitToShoot;

	private bool isStop;
	private bool hasShot;
	public Camera daddyCamera;
	public Camera farmerCamera;
	public Camera farmerCamera2;

	public GameObject tutoHunt2;

	private ColorCorrectionCurves colorCamera;

	void OnEnable()
	{
		agent = GetComponent<NavMeshAgent>();
		walkSpeed = agent.speed;
		agent.stoppingDistance = distanceStop;
		RunToDaddy();
		colorCamera = farmerCamera2.GetComponent<ColorCorrectionCurves> ();
	}

	void FixedUpdate()
	{
		Debug.Log("distance: " + Vector3.Distance(transform.position, daddyWolf.position));
		if (Vector3.Distance (transform.position, daddyWolf.position) <= distanceStop) {
			if (!isStop) 
			{
				StopFarmer ();
				daddyCamera.gameObject.SetActive (false);
				farmerCamera.gameObject.SetActive (true);
				tutoHunt2.SetActive (true);
				isStop = true;
			}
		} 
		else 
		{
			RunToDaddy();
		}
		if (GameManager.tutoShot) 
		{
			Debug.Log ("YO2!");
			StartCoroutine (StartShoot (waitToShoot));
			if (colorCamera.saturation > 0.0f)
				colorCamera.saturation -= 0.005f;
			else 
			{
				GameManager.tutoShot = false;
				StartCoroutine (EndTuto (5.0f));
			}
		}
	}

	public void RunToDaddy()
	{
		Debug.Log ("Run!");
		GetComponent<Animator> ().SetBool ("isWalking", false);
		GetComponent<Animator> ().SetBool ("isAlerted", true);
		agent.speed = chaseSpeedCoeff * walkSpeed;
		agent.destination = daddyWolf.transform.position;
	}

	public void StopFarmer()
	{
		Debug.Log ("Stop");
		GetComponent<Animator> ().SetBool ("isAlerted", false);
		agent.speed = walkSpeed;
		agent.Stop ();
		GetComponent<Rigidbody> ().AddForce (-150*GetComponent<Rigidbody> ().GetPointVelocity (transform.position));
	}

	public void TurnToDaddy()
	{
		transform.LookAt (daddyWolf.position);
	}
		
	public IEnumerator StartShoot(float waitToShoot)
	{
		if (!hasShot) 
		{
			farmerCamera.gameObject.SetActive (false);
			farmerCamera2.gameObject.SetActive (true);
			TurnToDaddy ();
			yield return new WaitForSeconds (3.0f);
			TurnToDaddy ();
			GetComponent<Animator> ().SetTrigger ("Shoot");
			yield return new WaitForSeconds (waitToShoot);
			hasShot = true;
			GetComponent<Animator> ().ResetTrigger ("Shoot");
		}
	}

	public IEnumerator EndTuto(float wait)
	{
		yield return new WaitForSeconds (wait);
		GameManager.endTuto = true;
		SceneManager.LoadScene (2);
	}

	public void destroyFarmer()
	{
		Destroy (gameObject);
	}
		
}
