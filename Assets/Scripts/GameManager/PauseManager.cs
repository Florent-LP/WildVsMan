using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour {

	public static bool onPause;
	public static bool onDialog;
	public static bool onTuto;

	public GameObject menuPanel;

	public void setTrueOnDialog()
	{
		onDialog = true;
	}

	public void setFalseOnDialog()
	{
		onDialog = false;
	}

	public void setTrueOnTuto()
	{
		onTuto = true;
	}

	public void setFalseOnTuto()
	{
		onTuto = false;
    }

    public void setTrueOnPause()
    {
        onPause = true;
    }

    public void setFalseOnPause()
    {
        onPause = false;
    }

    public void EndDialogTuto()
	{
		setFalseOnDialog ();
		setFalseOnTuto ();
		GetComponent<RPGTalk> ().enabled = false;
	}

	void Update () 
	{
		if (onPause) 
		{
			Time.timeScale = 0;
		} 
		else 
		{
			Time.timeScale = 1;
		}

		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			menuPanel.SetActive (!menuPanel.activeInHierarchy);
			onPause = !onPause;
		}
	}
}
