using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameZone : MonoBehaviour {

	public GameObject inputToEndGame;
	public GameObject blackBackground;
	public GameObject victoryHUD;
	private TextMesh distance;
	public Transform player;

	void OnEnable()
	{
		distance = GetComponent<TextMesh> ();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player") 
		{
			inputToEndGame.SetActive (true);
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player") 
		{
			inputToEndGame.SetActive (true);
			if (Input.GetKeyDown (KeyCode.E)) 
			{
				inputToEndGame.SetActive (false);
				blackBackground.SetActive (true);
				victoryHUD.SetActive (true);
				StartCoroutine (RestartGame (5.0f));
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player") 
		{
			inputToEndGame.SetActive (false);
		}
	}

	public IEnumerator RestartGame(float wait)
	{
		yield return new WaitForSeconds (wait);
		Debug.Log ("GO MAIN SCREEN.");
		SceneManager.LoadScene (0);
	}

	void Update()
	{
        int dist = (int)Vector3.Distance(transform.position, player.position);
        distance.text = "" + dist;
        distance.fontSize = dist;
        transform.parent.LookAt(player.position);
	}
}
