//#define DEBUG_PREY

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(FieldOfViewFarmer))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
//[RequireComponent(typeof(Rigidbody))]
public class PreyAI : MonoBehaviour {
    protected FieldOfViewFarmer fov = null;
    protected NavMeshAgent agent = null;
    protected Animator anim = null;
    //protected Rigidbody rbody = null;
    protected Transform predator = null;
    [SerializeField] protected GameObject ragdollPrefab = null;
    [SerializeField] protected float stopFleeDelay = 5f;
    protected float safetyTime;
    public GameObject inputToAct = null;
	public bool isAlive;

    #if DEBUG_PREY
        GameObject destMarker = null;
    #endif

    // Use this for initialization
    void Start () {
		isAlive = true;
        fov = GetComponent<FieldOfViewFarmer>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        //rbody = GetComponent<Rigidbody>();
        safetyTime = stopFleeDelay;
	}

    // Update is called once per frame
    void Update()
    {
        // Keep an eye at surroundings (while resting)
        if (predator == null)
        {
            if (fov.visibleTargets.Count > 0)
                predator = fov.visibleTargets[0];
        }
        // At least one predator nearby
        else
        {
            Vector3 toPredator = predator.position - transform.position;
            bool withinDangerRange = (toPredator.sqrMagnitude < fov.viewRadius*fov.viewRadius);
            //bool withinCriticalRange = (toPredator.sqrMagnitude < fov.viewRadius*fov.viewRadius/2);

            // Danger in sight
            if (withinDangerRange)
            {
                Vector3 runTo = transform.position - toPredator.normalized*agent.speed;

                NavMeshHit hit;
                NavMesh.SamplePosition(runTo, out hit, fov.viewRadius, 1 << NavMesh.GetAreaFromName("Walkable"));
                agent.SetDestination(hit.position);

                // Visualize destination (debug)
                #if DEBUG_PREY
                    Destroy(destMarker);
                    destMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    destMarker.GetComponent<SphereCollider>().enabled = false;
                    destMarker.transform.position = hit.position;
                #endif

                //inputToAct.SetActive(withinCriticalRange);
            }
            // Danger out of sight
            else
            {
                // Keep running for a while
                if (safetyTime > 0)
                {
                    Vector3 runTo = transform.position + transform.forward;

                    NavMeshHit hit;
                    NavMesh.SamplePosition(runTo, out hit, fov.viewRadius, 1 << NavMesh.GetAreaFromName("Walkable"));
                    agent.SetDestination(hit.position);

                    safetyTime -= Time.deltaTime;
                }
                // Get back to resting
                else
                {
                    agent.velocity = Vector3.zero;
                    predator = null;

                    safetyTime = stopFleeDelay;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (agent.velocity.sqrMagnitude > 1)
        {
            //Debug.Log(agent.velocity + " velocity");
            anim.SetFloat("Speed", 15);
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetFloat("Speed", 1);
            anim.SetBool("isMoving", false);
        }
    }

    public void OnKill()
    {
        Destroy(gameObject);
		if (ragdollPrefab != null) 
		{
			isAlive = false;
			Instantiate (ragdollPrefab, transform.position, transform.rotation);
		}
    }


    /* TODO: Refactor AI using State Pattern

    public class WanderBehaviour : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {

        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {

        }
    }

    public class FleeBehaviour : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {

        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {

        }
    }
    */
}
