using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //NavMeshAgent

public class DaddyWalk : MonoBehaviour {

	private NavMeshAgent agent;
	public float walkSpeed = 4.0f;
	public float runSpeedCoeff = 12.0f;

	private Animator animator;

	public List<Transform> visibleTargets = new List<Transform> ();
	private FieldOfViewPlayer fov;
	public Transform fishingArePosition;
	private bool daddyStop;

	public GameManager gm;

	public GameObject player;
	public GameObject tutoPart1;
	public bool tuto1;

	public GameObject tutoPart2;
	public bool tuto2;

	public bool tuto3;

	private Rigidbody rbody;
	private Collider playerCol;

	public float dashDistance = 20f;
	public float dashTime = 1f;
	public float dashDelay = 2f;
	private Transform preyTuto = null;

	public Camera daddyCamera;
	public Camera playerCamera;

	// State variables
	protected enum State
	{
		other,
		dashing
	}
	protected State currentState = State.other;

	void Awake () 
	{
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
		fov = GetComponent<FieldOfViewPlayer> ();
		rbody = GetComponent<Rigidbody> ();
		playerCol = GetComponent<CapsuleCollider>();
		ResetBool ();
	}

	public void StopDaddy()
	{
		ResetBool ();
		agent.speed = walkSpeed;
		agent.Stop ();
	}

	void Update () 
	{
		if (!PauseManager.onTuto && !GameManager.tutoHunt) 
		{ 
			if (!daddyStop) 
			{
				ReachPositionByWalk (fishingArePosition);
				daddyStop = true;
			}
			/*if (Vector3.Distance(transform.position, fishingArePosition.position) < 5)
			{
				StopDaddy ();
			}*/
		} 
		if (GameManager.tutoHunt) 
		{
			if (!tuto1) 
			{
				transform.LookAt (gm.sheep.transform.position);
				agent.Resume ();
				ReachPositionByWalk (gm.sheep.transform);
				if (Vector3.Distance (transform.position, gm.sheep.transform.position) < 50) 
				{
					StopDaddy ();
					tutoPart1.SetActive (true);
					tuto1 = true;
				}
			}

			if (tuto1 && !tuto2) 
			{
				if (Vector3.Distance (transform.position, player.transform.position) < 10) 
				{
					PauseManager.onTuto = true;
					player.GetComponent<Rigidbody> ().AddForce (-50*player.GetComponent<Rigidbody> ().GetPointVelocity (transform.position));
					tutoPart2.SetActive (true);
					tuto2 = true;
				}

			}

			if (tuto1 && tuto2 && PauseManager.onTuto == false) 
			{
				if (!tuto3) 
				{
					player.GetComponent<PlayerActionsTuto> ().enabled = false;
					daddyCamera.gameObject.SetActive (true);
					playerCamera.gameObject.SetActive (false);
					agent.Resume ();
					ReachPositionByRun (gm.sheep.transform);
					preyTuto = FindClosestObject ("Enemy", "MammalPrey");
				}
					//if (Vector3.Distance (transform.position, gm.sheep.transform.position) < 3) 
					//	{
				if (preyTuto != null) 
				{
					StartCoroutine (Dash (preyTuto));
					GameManager.tutoHunt = false;
				}
					//}
			}
		}

		else 
		{
			if (tuto3) 
			{
				StopDaddy ();
				GameManager.tutoDeath = true;
			}
		}
	}

	void ReachPositionByWalk(Transform transformPosition)
	{
		animator.SetBool("isWalking", true);
		agent.speed = walkSpeed;
		agent.destination = transformPosition.position;
	}

	void ReachPositionByRun(Transform transformPosition)
	{
		animator.SetBool("isRunning", true);
		agent.speed = runSpeedCoeff;
		agent.destination = transformPosition.position;
	}

	void ResetBool()
	{
		animator.SetBool("isWalking", false);
		animator.SetBool("isCrouching", false);
		animator.SetBool("isRunning", false);
	}



	protected Transform FindClosestObject(string layer, string tag)
	{
		Transform closestObject = null;

		// Get all objects on layer within radius
		List<Transform> objectsInReach = new List<Transform>();
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, dashDistance, 1 << LayerMask.NameToLayer(layer));
		for (int i = 0; i < hitColliders.Length; ++i)
		{
			// Process only visible tagged objects
			if (hitColliders[i].tag == tag)
			{
				Plane[] camPlanes = GeometryUtility.CalculateFrustumPlanes(daddyCamera);
				bool isVisible = GeometryUtility.TestPlanesAABB(camPlanes, hitColliders[i].bounds);

				if (isVisible)
					objectsInReach.Add(hitColliders[i].transform);
			}
		}

		// Choose closest object
		float objectDist;
		float closestDist = dashDistance;

		foreach (Transform nearbyObj in objectsInReach) {
			objectDist = (transform.position - nearbyObj.position).magnitude;

			if (objectDist < closestDist) {
				closestDist = objectDist;
				closestObject = nearbyObj;
			}
		}

		return closestObject;
	}


	protected IEnumerator Dash(Transform prey)
	{
		if (currentState == State.dashing) yield break;
		currentState = State.dashing;

		// Disable physics
		//if (playerCol != null) playerCol.isTrigger = true;
		if (rbody != null) {
			//rbody.isKinematic = true;
			rbody.velocity = Vector3.zero;
		}

		// Animate
		animator.SetTrigger("isAttacking");

		// Prepare move
		while (!animator.GetAnimatorTransitionInfo (0).IsUserName ("DashBegin")) 
		{
			tuto3 = true;
			yield return null;
		}

		// Some useful variables
		Vector3 origin = transform.position, direction;
		float distance, speed;

		Vector3 playerPoint, preyPoint;
		float playerRadius, preyRadius;

		// Execute move
		do
		{
			playerPoint = playerCol.ClosestPointOnBounds(prey.position);
			playerRadius = Vector3.Distance(transform.position, playerPoint);

			preyPoint = prey.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
			preyRadius = Vector3.Distance(prey.position, preyPoint);

			direction = (prey.position - transform.position).normalized;
			distance = Vector3.Distance(origin, prey.position);
			speed = distance / dashTime;

			transform.LookAt(prey.position);
			transform.position += direction * speed * Time.deltaTime;
			yield return null;
		}
		while (Vector3.Distance(transform.position, prey.position) > playerRadius + preyRadius
			&& !animator.GetAnimatorTransitionInfo(0).IsUserName("DashToBite"));
		StopDaddy ();

		// Kill prey
		PreyAI foodAI = prey.GetComponent<PreyAI>();
		if (foodAI != null) foodAI.OnKill();

		// Finish move
		while (!animator.GetAnimatorTransitionInfo (0).IsUserName ("BiteEnd")) 
		{
			StopDaddy ();
			yield return null;
		}

		// Restore physics
		//if (playerCol != null) playerCol.isTrigger = false;
		//if (rbody != null) rbody.isKinematic = false;

		// Prevent Abuse
		yield return new WaitForSeconds(dashDelay);
		StopDaddy ();
		preyTuto = null;
		currentState = State.other;
	}

}
