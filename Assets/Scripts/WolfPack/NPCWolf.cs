using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCWolf : MonoBehaviour {

	private GameObject inputFollowHUD;
	private GameObject inputGetOutHUD;
	private GameObject player;

	private bool isFollowingPlayer;
	private NavMeshAgent agent;
	protected Animator animator;

	public float crouchSpeed;
	public float walkSpeed;
	public float runSpeed;

	private Vector3 initialPosition;

	public float stoppingDistance;

	private Animator playerAnimator;

	private bool startFollow;
	private bool goHome;

	void Start () 
	{
		inputFollowHUD = GameManagerMain.inputFollowHUDGO;
		inputGetOutHUD = GameManagerMain.inputGetOutHUDGO;
		player = GameManagerMain.playerGO;
		agent = GetComponent<NavMeshAgent> ();
		animator = gameObject.GetComponent<Animator> ();
		initialPosition = transform.position;
		agent.stoppingDistance = stoppingDistance;
		playerAnimator = GameManagerMain.playerGO.GetComponent<Animator> ();
	}

	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player") 
		{
			if (!isFollowingPlayer) 
			{						
				inputGetOutHUD.SetActive (false);
				inputFollowHUD.SetActive (true);
				if (Input.GetKeyDown (KeyCode.E)) 
				{
					goHome = false;
					startFollow = true;
				}
			} 
			else 
			{				
				inputFollowHUD.SetActive (false);
				inputGetOutHUD.SetActive (true);
				if (Input.GetKeyDown (KeyCode.E)) 
				{
					startFollow = false;
					goHome = true;
				}
			}
		}
	}

	void Update()
	{
		if (startFollow) 
		{
			FollowPlayer ();
		}
		if (goHome) 
		{
			GoHome ();
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.transform.gameObject.tag == "Player") 
		{
			inputFollowHUD.SetActive (false);
			inputGetOutHUD.SetActive (false);
		}
	}

	public void FollowPlayer()
	{
		goHome = false;
		isFollowingPlayer = true;
		player.GetComponent<WolfPlayerActions> ().isWithNPC = true;
		transform.LookAt (player.transform.position);

        animator.SetBool("isWalking", false);
        animator.SetBool("isCrouching", false);
        animator.SetBool("isRunning", false);

        if (playerAnimator.GetBool ("isWalking"))
			ReachPositionByWalk (player.transform);
		else if (playerAnimator.GetBool ("isCrouching"))
			ReachPositionByCrouch(player.transform);
		else if (playerAnimator.GetBool ("isRunning"))
			ReachPositionByRun(player.transform);
		else if (agent.velocity.sqrMagnitude > walkSpeed*walkSpeed)
			animator.SetBool("isRunning", true);
        else if (agent.velocity.sqrMagnitude > 0.1f)
            animator.SetBool("isWalking", true);
    }

	public void GoHome()
	{
		startFollow = false;
		isFollowingPlayer = false;
		player.GetComponent<WolfPlayerActions> ().isWithNPC = false;
		transform.LookAt (initialPosition);
		animator.SetBool("isRunning", true);
		agent.Resume ();
		agent.speed = runSpeed;
		agent.destination = initialPosition;		
		if (Vector3.Distance (transform.position, initialPosition) < 3) 
		{
			StopWolf ();
			goHome = false;
		}
	}


	void ReachPositionByCrouch(Transform transformPosition)
	{
		agent.Resume ();
		animator.SetBool("isCrouching", true);
		agent.speed = crouchSpeed;
		agent.destination = transformPosition.position - agent.stoppingDistance*(transformPosition.position - transform.position).normalized;
	}

	void ReachPositionByWalk(Transform transformPosition)
	{
		agent.Resume ();
		animator.SetBool("isWalking", true);
		agent.speed = walkSpeed;
		agent.destination = transformPosition.position - agent.stoppingDistance * (transformPosition.position - transform.position).normalized;
	}

	void ReachPositionByRun(Transform transformPosition)
	{
		agent.Resume ();
		animator.SetBool("isRunning", true);
		agent.speed = runSpeed;
		agent.destination = transformPosition.position - agent.stoppingDistance * (transformPosition.position - transform.position).normalized;
	}

	void ResetBool()
	{
		animator.SetBool("isWalking", false);
		animator.SetBool("isCrouching", false);
		animator.SetBool("isRunning", false);
	}

	void StopWolf()
	{
		ResetBool ();
		agent.speed = walkSpeed;
		agent.Stop ();
	}
}
