using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour {

	[SerializeField] protected GameManagerMain manager = null;
	public GameObject endGameGO;

	// Use this for initialization
	void Start () 
	{
		if (manager != null) manager.registerEvent(manager.dayToRealSec(2.0f), EnableEnd);

	}

	public IEnumerator EnableEnd()
	{
		endGameGO.SetActive (true);
		yield return null;
	}

}
