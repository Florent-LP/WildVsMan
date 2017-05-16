using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //NavMeshAgent

public class SheepTuto : MonoBehaviour {

	public GameObject destination;

	private NavMeshAgent agent;
	private Animator animator;
	private FieldOfViewFarmer fov;

	// Use this for initialization
	void Start () 
	{
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
		fov = GetComponent<FieldOfViewFarmer>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (fov.visibleTargets.Count > 0) 
		{
			animator.SetFloat("Speed", agent.speed);
			animator.SetBool("isMoving", true);
			ReachDestination ();
		}
		if (Vector3.Distance (transform.position, destination.transform.position) <= 1.0f) 
		{
			agent.Stop ();
			animator.SetBool("isMoving", false);
		}
			
	}

	void ReachDestination()
	{
		agent.destination = destination.transform.position;
	}
}
