using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionArea : MonoBehaviour {

	private GameObject farmerArea;

	void OnTriggerEnter(Collider other)
	{
		farmerArea = transform.parent.parent.parent.FindChild ("FarmerHouse").FindChild ("FarmerOutArea").gameObject;
		//Debug.Log ("isFarmer: " + popFarmer.isFarmer);
		if (!popFarmer.isFarmer) 
		{
			if (other.transform.gameObject.tag == "FarmerHouse") 
			{
				Debug.Log ("Farmer house heard you!!!!!");
				farmerArea.SetActive (true);
			}
		} 
		else 
		{
			FarmerShoot.safeStartTime = 0;
			if (other.transform.gameObject.tag == "Enemy") 
			{
				if (other.GetComponent<FarmerShoot> ().isGoingBack) 
				{
					other.GetComponent<FarmerShoot> ().StopFarmer ();
					other.GetComponent<FarmerShoot> ().isGoingBack = false;
					other.GetComponent<Animator> ().SetBool ("isWalking", false);
					other.GetComponent<Animator> ().SetBool ("isAlerted", false);

				}
			}
		}
	}

}
