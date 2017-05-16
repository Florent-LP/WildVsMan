using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part1 : MonoBehaviour {

	public static bool tuto1;
	public static bool tuto2;

	public void setTrueOnDialog()
	{
		PauseManager.onDialog = true;
	}

	public void setFalseOnDialog()
	{
		PauseManager.onDialog = false;
	}

	public void setTrueOnTuto()
	{
		PauseManager.onTuto = true;
	}

	public void setFalseOnTuto()
	{
		PauseManager.onTuto = false;
	}

	public void EndDialogTuto()
	{
		setFalseOnDialog ();
		setFalseOnTuto ();
		GetComponent<RPGTalk> ().enabled = false;
	}

	public void EndDialogTuto1()
	{
		setFalseOnDialog ();
		setFalseOnTuto ();
		GetComponent<RPGTalk> ().enabled = false;
		tuto1 = true;
	}

	public void EndDialogTuto12()
	{
		setFalseOnDialog ();
		setFalseOnTuto ();
		GetComponent<RPGTalk> ().enabled = false;
		tuto2 = true;
	}

	public void EndDialogTuto2()
	{
		setFalseOnDialog ();
		setFalseOnTuto ();
		GameManager.tutoFish = false;
		GameManager.tutoHunt = true;
		GetComponent<RPGTalk> ().enabled = false;
	}

	public void EndDialogTuto3()
	{
		setFalseOnDialog ();
		setFalseOnTuto ();
		GameManager.tutoShot = true;
		GetComponent<RPGTalk> ().enabled = false;
	}
}
