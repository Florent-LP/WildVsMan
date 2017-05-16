using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerThirst : MonoBehaviour {

	public Slider thirstBar;

	public float thirstReducer;
	public float thirst;
	public float thirstValue;
	private float timer;
	public float timerValue = 5.0f;
	private bool isThirsty;

	void Start()
	{
		thirst = thirstValue;
		timer = timerValue;
	}

	public void ThirstReduce()
	{
		thirst -= thirstReducer;
	}

	void FixedUpdate () 
	{
		thirstBar.value = thirst;
		if (!PauseManager.onDialog) 
		{
			if (thirst > 0) 
			{
				if (thirst > thirstValue) 
				{
					thirst = thirstValue;
				}
				timer -= Time.deltaTime;
				if (timer < 0) 
				{
					ThirstReduce ();
					timer = timerValue;
				}
			} 
			else 
			{
				thirst = 0;
				StartCoroutine (HealthReduce (2.0f));
			}
		}
	}

	IEnumerator HealthReduce(float wait)
	{
		if (!isThirsty) 
		{
			isThirsty = true;
			yield return new WaitForSeconds (wait);
			GetComponent<PlayerHealth> ().healthBar.value -= 1;
			isThirsty = false;
		}

	}
}
