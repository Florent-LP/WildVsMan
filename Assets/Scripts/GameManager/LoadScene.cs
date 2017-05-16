using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {

	public void LoadSceneFromIndex(int index)
	{
		SceneManager.LoadScene (index);
	}

	public void ExitGame()
	{
		Application.Quit ();
	}

}
