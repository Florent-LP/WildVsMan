using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfPlayerActions : MonoBehaviour {

    protected Animator animator;
    protected Rigidbody rbody;
    protected Collider playerCol;
    protected PlayerController playerCtr;
    protected FieldOfViewPlayer fov;

    public float fishHungerUp;
	public float fishHealthUp;
	public float waterThirstUp;


	public GameObject hudEat;
	public GameObject hudDrink;
    public GameObject hudHunt;

	public GameObject hudBreed;
	public Transform campPosition;
	public GameObject blackBackground;

	private bool isBreed;

    public float dashDistance = 20f;
    public float dashTime = 1f;
    public float dashDelay = 2f;

	public WolfPackManager campScript;

	public bool isWithNPC;

    // State variables
    protected enum State
    {
        other,
        dashing
    }
    protected State currentState = State.other;

    void Start()
	{
		animator = gameObject.GetComponent<Animator> ();
        rbody = gameObject.GetComponent<Rigidbody>();
        playerCol = gameObject.GetComponent<Collider>();
        playerCtr = gameObject.GetComponent<PlayerController>();
        fov = gameObject.GetComponent<FieldOfViewPlayer>();

        animator.SetFloat("attackAcceleration", 1.8f );
	}

	void Update()
    {
        hudEat.SetActive(false);
		hudHunt.SetActive(false);
		hudBreed.SetActive(false);

        // Pick up dead animal / food
        if (fov.visibleTargets.Count > 0) 
		{
			if (fov.visibleTargets [0] != null) 
			{
				if (fov.visibleTargets [0].tag == "WolfWoman") 
				{
					Breed (fov.visibleTargets [0]);
				} 

				else if(fov.visibleTargets [0].tag != "NPCWolf")
				{
					Debug.Log ("Found food!");
					hudEat.SetActive (true);

					Transform food = fov.visibleTargets[0];
					if (food != null && Input.GetKeyDown(KeyCode.E))
						Eat (food);
				}

				return;
			}
		}

        // Enable dash to prey
        Transform prey = FindClosestObject("Enemy", "MammalPrey");
        if (prey != null) 
		{
			bool isHorse = false;
			bool isSheep = false;

			if(prey.GetComponent<HorseAI> ())
				isHorse = prey.GetComponent<HorseAI> ().isAlive;
			if(prey.GetComponent<PreyAI> ())
				isSheep = prey.GetComponent<PreyAI> ().isAlive;
			if (isHorse) 
			{
				if (isWithNPC) 
				{
					Debug.Log ("PEUT ATTAQUER CHEVAL!");
					hudHunt.SetActive(true);

					if (Input.GetKeyDown(KeyCode.E))
						StartCoroutine(Dash(prey));

					return;
				}

			}
			if(isSheep)
			{
				Debug.Log ("OH UN MOUTON!");
				hudHunt.SetActive(true);

				if (Input.GetKeyDown(KeyCode.E))
					StartCoroutine(Dash(prey));

				return;
			}
        }
    }

	void Eat(Transform food)
    {
        Debug.Log("Eating food.");
        animator.SetTrigger("isEating");

		GetComponent<PlayerHunger> ().hunger += fishHungerUp;
		GetComponent<PlayerHealth> ().health += fishHealthUp;

		Destroy (food.gameObject);
	}

	public void Drink()
	{
		Debug.Log("Drinking water.");
		animator.SetTrigger("isEating");

		GetComponent<PlayerThirst> ().thirst += waterThirstUp;
	}

	public void Breed(Transform wolfWoman)
	{
		if (!campScript.isCampFull()) 
		{
			hudBreed.SetActive(true);
			if (Input.GetKeyDown (KeyCode.E) && !isBreed) 
			{
				//Debug.Log("Let's breed!");
				isBreed = true;
				campScript.popAWolf ();
				hudBreed.SetActive(false);
				StartCoroutine (blackTransition (2.5f, wolfWoman));
				transform.position = campPosition.position;
			}
		}
	}

	public IEnumerator blackTransition(float wait, Transform wolfWoman)
	{
		blackBackground.SetActive (true);

		if (!campScript.isWomanWolf) 
		{
			wolfWoman.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
			wolfWoman.position = campScript.wolfWomanPop.position;
			campScript.isWomanWolf = true;
		}
		yield return new WaitForSeconds (wait);
		wolfWoman.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
		PauseManager.onTuto = true;
		blackBackground.SetActive (false);
		PauseManager.onTuto = false;
		isBreed = false;
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
                Plane[] camPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
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

        // Disable controls
        if (playerCtr != null) playerCtr.enabled = false;

        // Disable physics
        //if (playerCol != null) playerCol.isTrigger = true;
        if (rbody != null) {
            //rbody.isKinematic = true;
            rbody.velocity = Vector3.zero;
        }

        // Animate
        animator.SetTrigger("isAttacking");

        // Prepare move
        while (!animator.GetAnimatorTransitionInfo(0).IsUserName("DashBegin"))
            yield return null;

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

        // Kill prey
        PreyAI foodAI = prey.GetComponent<PreyAI>();
		HorseAI horseAI = prey.GetComponent<HorseAI>();

        if (foodAI != null) foodAI.OnKill();
		if (horseAI != null) horseAI.OnKill();


        // Finish move
        while (!animator.GetAnimatorTransitionInfo(0).IsUserName("BiteEnd"))
            yield return null;

        // Restore physics
        //if (playerCol != null) playerCol.isTrigger = false;
        //if (rbody != null) rbody.isKinematic = false;

        // Restore controls
        if (playerCtr != null) playerCtr.enabled = true;

        // Prevent Abuse
        yield return new WaitForSeconds(dashDelay);

        currentState = State.other;
    }
}