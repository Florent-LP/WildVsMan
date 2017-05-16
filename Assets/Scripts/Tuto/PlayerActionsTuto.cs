using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionsTuto : MonoBehaviour {

	protected Animator animator;
	protected Rigidbody rbody;
	protected Collider playerCol;
	protected PlayerController playerCtr;
	protected FieldOfViewPlayer fov;

	public float fishHungerUp;
	public float waterThirstUp;
	public float fishHealthUp;
	public GameObject hudEat;

	private bool onEat;
	public bool onDrink;

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

		// Pick up dead animal / food
		if (fov.visibleTargets.Count > 0 && !PauseManager.onTuto) 
		{
			Debug.Log ("Found food!");
			hudEat.SetActive (true);

			Transform food = fov.visibleTargets[0];
			if (food != null && Input.GetKeyDown (KeyCode.E) && !onEat) 
			{
				Eat (food);
			}
			return;
		}
	}

	void Eat(Transform food)
	{
		Debug.Log("Eating food.");
		onEat = true;
		animator.SetTrigger("isEating");
		StartCoroutine (DisableControllerOnAction (2.75f));
		GetComponent<PlayerHunger> ().hunger += fishHungerUp;
		GetComponent<PlayerHealth> ().health += fishHealthUp;

		Destroy (food.gameObject);
	}

	IEnumerator DisableControllerOnAction(float wait)
	{
		PauseManager.onTuto = true;
		yield return new WaitForSeconds (wait);
		PauseManager.onTuto = false;
		onEat = false;
		onDrink = false;
	}

	public void Drink()
	{
		Debug.Log("Drinking water.");
		animator.SetTrigger("isEating");
		onDrink = true;
		StartCoroutine (DisableControllerOnAction (2.0f));
		GetComponent<PlayerThirst> ().thirst += waterThirstUp;
	}
}