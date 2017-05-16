using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WolfPackManager : MonoBehaviour {

	public GameObject wolfNPCPrefab;
	private List<Transform> popPositionsList;
	private GameObject popPositions;
	private int randomSpawn;

	public int nbWolf = 3;
	private List<int> indexTab;
	[HideInInspector]
	public List<GameObject> wolfList;
	private GameObject wolfPack;
	public GameObject panel;

	public Transform wolfWomanPop;
	public bool isWomanWolf;

	void Start () 
	{
		init ();
	}

	void init()
	{
		popPositions = GameObject.Find ("PopPositions");
		popPositionsList = new List<Transform>(popPositions.GetComponentsInChildren<Transform> ());
		popPositionsList.RemoveAt (0);
		indexTab = new List<int>();
		wolfList = new List<GameObject> ();
		createWolfPack ();
		fillerIndexTab (indexTab, popPositionsList.Count);
	}

	void createWolfPack()
	{
		wolfPack = new GameObject ();
		wolfPack.name = "WolfPack";
		wolfPack.transform.parent = transform.parent;
	}

	/*void OnTriggerEnter(Collider other)
	{
		if (other.transform.gameObject.tag == "Player") 
		{
			panel.SetActive (true);
		}
	}*/

	/*void OnTriggerStay(Collider other)
	{
		if (other.transform.gameObject.tag == "Player") 
		{
			setUIActive ();
		}
	}*/

	/*void OnTriggerExit(Collider other)
	{
		if (other.transform.gameObject.tag == "Player") 
		{
			panel.SetActive (false);
		}
	}*/

	public void popAWolf()
	{
		chooseRandom (0, popPositionsList.Count, indexTab);
		indexTab.Remove (randomSpawn);
		instantiateWolf (popPositionsList[randomSpawn].position.x, popPositionsList[randomSpawn].position.y, popPositionsList[randomSpawn].position.z);
		//Debug.Log ("Wolf pop!");
	}

	public void popAllWolfs()
	{
		for (int i = 0; i < nbWolf; i++) 
		{
			popAWolf ();
		}
	}

	void instantiateWolf(float x, float y, float z)
	{
		GameObject instanceWolf = UnityEngine.GameObject.Instantiate(wolfNPCPrefab, new Vector3(x,y,z), Quaternion.identity) as GameObject;
		instanceWolf.transform.parent = gameObject.transform.parent;
		//instanceWolf.transform.localScale = new Vector3 (0.02f,0.02f,0.02f);
		instanceWolf.transform.localScale = new Vector3 (0.015f,0.015f,0.015f);
		instanceWolf.transform.parent = wolfPack.transform;
		wolfList.Add (instanceWolf);
		TextMesh wolfName = instanceWolf.GetComponent<TextMesh> ();
		wolfName.text = "Wolf " + randomSpawn;
	}

	void chooseRandom(int fromVal, int toVal, List<int> intList)
	{
		randomSpawn = Random.Range (fromVal, toVal);
		while (!intList.Contains (randomSpawn)) 
		{
			randomSpawn = Random.Range (fromVal, toVal);
		}
	}

	void fillerIndexTab(List<int> indexTab, int length)
	{
		indexTab.Clear ();
		for (int i=0; i < length; i++) 
		{
			indexTab.Add(i);
			//Debug.Log ("index i: " + indexTab[i]);
		}
	}

	public void setUIActive()
	{
		if (isCampFull ()) 
		{
			panel.transform.FindChild ("DoWolfUIButton").gameObject.GetComponent<Button> ().interactable = false;
			panel.transform.FindChild ("DoAllWolfsUIButton").gameObject.GetComponent<Button> ().interactable = false;
			panel.transform.FindChild ("EraseAllWolfsUIButton").gameObject.GetComponent<Button> ().interactable = true;
		} 
		else 
		{
			panel.transform.FindChild ("DoWolfUIButton").gameObject.GetComponent<Button> ().interactable = true;
			if (wolfList.Count == 0) {
				panel.transform.FindChild ("DoAllWolfsUIButton").gameObject.GetComponent<Button> ().interactable = true;
				panel.transform.FindChild ("EraseAllWolfsUIButton").gameObject.GetComponent<Button> ().interactable = false;
			} 
			else 
			{
				panel.transform.FindChild ("DoAllWolfsUIButton").gameObject.GetComponent<Button> ().interactable = false;
				panel.transform.FindChild ("EraseAllWolfsUIButton").gameObject.GetComponent<Button> ().interactable = true;
			}
		}
	}

	public void killAllWolfs()
	{
		Destroy (wolfPack);
		fillerIndexTab (indexTab, popPositionsList.Count);
		wolfList.Clear();
		createWolfPack ();
	}

	public bool isCampFull()
	{
		if (wolfList.Count == nbWolf) 
		{
			return true;
		}
		return false;
	}
}
