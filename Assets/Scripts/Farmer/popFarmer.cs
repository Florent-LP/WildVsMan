using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popFarmer : MonoBehaviour {

	//public GameObject farmerPrefab;
	public static bool isFarmer;

	void OnEnable () 
	{
		isFarmer = true;
		/*GameObject instanceTile = UnityEngine.GameObject.Instantiate(farmerPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity) as GameObject;
		instanceTile.transform.parent = gameObject.transform;
		instanceTile.transform.localScale = new Vector3 (1.34f, 1.34f, 1.34f);
		instanceTile.transform.localRotation = new Quaternion (instanceTile.transform.localRotation.x, 0, instanceTile.transform.localRotation.z, instanceTile.transform.localRotation.w);*/
	}

	void OnDisable()
	{
		isFarmer = false;
	}
}
