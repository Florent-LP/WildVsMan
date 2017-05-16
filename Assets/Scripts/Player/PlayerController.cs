using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private Animator animator;

	private float speed;
	public float sneakySpeed = 1.25f;
	public float walkSpeed = 2.0f;
	public float runSpeed = 30.0f;

	public float inputDelay = 0.1f;
	public float rotateVel = 100;

	Quaternion targetRotation;
	Rigidbody rBody;
	float forwardInput, turnInput;

	public Quaternion TargetRotation
	{
		get{ return targetRotation;}
	}

	void Start () 
	{
		animator = GetComponent<Animator>();
		speed = walkSpeed;
		targetRotation = transform.rotation;
		rBody = GetComponent<Rigidbody> ();
		forwardInput = turnInput = 0;
	}

	void GetInput()
	{
		forwardInput = Input.GetAxis ("Vertical");
		turnInput = Input.GetAxis ("Horizontal");

		if (forwardInput != 0)
		{
			animator.SetBool("isWalking", true);

			if (Input.GetKey(KeyCode.LeftShift))
			{
				animator.SetBool("isWalking", false);
				animator.SetBool ("isCrouching", false);
				animator.SetBool("isRunning", true);
			}
			else
			{
				animator.SetBool("isRunning", false);
			}
			if (Input.GetKey(KeyCode.LeftControl))
			{
				animator.SetBool("isWalking", false);
				animator.SetBool("isRunning", false);
				animator.SetBool("isCrouching", true);
			}
			else
			{
				animator.SetBool("isCrouching", false);
			}
		}
		else
		{
			animator.SetBool("isWalking", false);
			//animator.SetBool("isCrouching", false);
			animator.SetBool ("isRunning", false);
		}

		if (!Input.GetKey (KeyCode.LeftShift)) 
		{
			animator.SetBool ("isRunning", false);
		}
		if (!Input.GetKey (KeyCode.LeftControl)) 
		{
			animator.SetBool ("isCrouching", false);
		}

		if(animator.GetBool("isWalking"))
		{
			speed = walkSpeed;
		}
		if(animator.GetBool("isRunning"))
		{
			speed = runSpeed;
		}
		if(animator.GetBool("isCrouching"))
		{
			speed = sneakySpeed;
		}
	}

	void Run()
	{
		if (Mathf.Abs (forwardInput) > inputDelay) 
		{
			rBody.velocity = transform.forward * forwardInput * speed + new Vector3(0, rBody.velocity.y, 0);
		} 
	}

	void Turn()
	{
		targetRotation *= Quaternion.AngleAxis (rotateVel * turnInput * Time.deltaTime, Vector3.up);
		transform.rotation = targetRotation;
	}

	void Update () 
	{
        bool isEating = animator.GetCurrentAnimatorStateInfo(0).IsName("Eat");

        if (!PauseManager.onTuto && !isEating) 
		{
			GetInput ();
			Turn ();
			Run ();
		} 
		else 
		{
			animator.SetBool("isWalking", false);
			animator.SetBool("isRunning", false);
			animator.SetBool ("isCrouching", false);
		}
	}
}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private Animator animator;

	private float speed;
	public float sneakySpeed = 1.25f;
	public float walkSpeed = 2.0f;
	public float runSpeed = 30.0f;

	public float inputDelay = 0.1f;
	public float rotateVel = 100;

	Quaternion targetRotation;
	Rigidbody rBody;
    CapsuleCollider capsule;
	float forwardInput, turnInput;

	public Quaternion TargetRotation
	{
		get{ return targetRotation;}
	}

	void Start () 
	{
		animator = GetComponent<Animator>();
		speed = walkSpeed;
		targetRotation = transform.rotation;
		rBody = GetComponent<Rigidbody> ();
		forwardInput = turnInput = 0;
        capsule = GetComponent<CapsuleCollider>();
	}

	void GetInput()
	{
		forwardInput = Input.GetAxis ("Vertical");
		turnInput = Input.GetAxis ("Horizontal");

		if (forwardInput != 0)
		{
			animator.SetBool("isWalking", true);

			if (Input.GetKey(KeyCode.LeftShift))
			{
				animator.SetBool("isWalking", false);
				animator.SetBool ("isCrouching", false);
				animator.SetBool("isRunning", true);
			}
			else
			{
				animator.SetBool("isRunning", false);
			}
			if (Input.GetKey(KeyCode.LeftControl))
			{
				animator.SetBool("isWalking", false);
				animator.SetBool("isRunning", false);
				animator.SetBool("isCrouching", true);
			}
			else
			{
				animator.SetBool("isCrouching", false);
			}
		}
		else
		{
			animator.SetBool("isWalking", false);
			//animator.SetBool("isCrouching", false);
			animator.SetBool ("isRunning", false);
		}

		if (!Input.GetKey (KeyCode.LeftShift)) 
		{
			animator.SetBool ("isRunning", false);
		}
		if (!Input.GetKey (KeyCode.LeftControl)) 
		{
			animator.SetBool ("isCrouching", false);
		}

		if(animator.GetBool("isWalking"))
		{
			speed = walkSpeed;
		}
		if(animator.GetBool("isRunning"))
		{
			speed = runSpeed;
		}
		if(animator.GetBool("isCrouching"))
		{
			speed = sneakySpeed;
		}
	}

	void Run()
	{
		if (Mathf.Abs (forwardInput) > inputDelay) 
		{
            //rBody.velocity = transform.forward * forwardInput * speed + new Vector3(0, rBody.velocity.y, 0);
            Vector3 move = transform.forward * forwardInput * speed * Time.deltaTime;

            RaycastHit hitInfo;
            //if (Physics.Raycast(transform.position + capsule.center + Vector3.forward * capsule.height, Vector3.down, out hitInfo, capsule.radius * 1.5f))
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, 0.3f)) {
                Vector3 groundProj = Vector3.ProjectOnPlane(move, hitInfo.normal);
                //transform.LookAt(groundProj);
                transform.position += groundProj; // Project on ground to avoid bounce
            } else
                transform.position += move;
		} 
	}

	void Turn()
	{
        //targetRotation *= Quaternion.AngleAxis (rotateVel * turnInput * Time.deltaTime, Vector3.up);
        //transform.rotation = targetRotation;
        transform.rotation *= Quaternion.AngleAxis(rotateVel * turnInput * Time.deltaTime, transform.up);
	}

	void FixedUpdate ()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position + (Vector3.up * 0.5f), Vector3.down, out hitInfo, 0.8f))
            Debug.DrawLine(transform.position, transform.position + Vector3.ProjectOnPlane(transform.forward * 4, hitInfo.normal), Color.blue);

        Debug.DrawLine(transform.position + transform.forward * capsule.height + transform.up,
                    transform.position + transform.forward * capsule.height - transform.up * 0.3f,
                    Color.red);

        if (!PauseManager.onTuto) 
		{
			GetInput ();
			Turn ();
			Run ();
		} 
		else 
		{
			animator.SetBool("isWalking", false);
			animator.SetBool("isRunning", false);
			animator.SetBool ("isCrouching", false);
		}
	}
}
*/
