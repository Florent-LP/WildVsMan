using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHunger : MonoBehaviour {

	public Slider hungerBar;

	public float hungerReducer;
	public float hunger;
	public float hungerValue;
	private float timer;
	public float timerValue = 5.0f;

	private bool isHunger;

	void Start()
	{
		hunger = hungerValue;
		timer = timerValue;
	}

	public void HungerReduce()
	{
		hunger -= hungerReducer;
	}

	void FixedUpdate () 
	{
		hungerBar.value = hunger;
		if (!PauseManager.onDialog) 
		{
			if (hunger > 0) 
			{
				if (hunger > hungerValue) 
				{
					hunger = hungerValue;
				}
				timer -= Time.deltaTime;
				if (timer < 0) 
				{
					HungerReduce ();
					timer = timerValue;
				}
			} 
			else 
			{
				hunger = 0;
				StartCoroutine (HealthReduce (2.0f));
			}
		}
	}

	IEnumerator HealthReduce(float wait)
	{
		if (!isHunger) 
		{
			isHunger = true;
			yield return new WaitForSeconds (wait);
			GetComponent<PlayerHealth> ().healthBar.value -= 1;
			isHunger = false;
		}

	}
}
