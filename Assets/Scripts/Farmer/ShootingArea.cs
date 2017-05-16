using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingArea : MonoBehaviour {

	private SphereCollider areaCollider;
	public float distanceFromRadius;

	public GameObject farmer;
	private FarmerShoot farmerShoot;

	public bool isStop;

	void OnEnable () 
	{
		areaCollider = GetComponent<SphereCollider> ();
		farmer.SetActive (true);
		farmerShoot = farmer.GetComponent<FarmerShoot> ();
		popFarmer.isFarmer = true;
		isStop = false;
		//Debug.Log ("radius: " + areaCollider.radius);

	}

	void OnDisable()
	{
		farmer.SetActive (false);
		popFarmer.isFarmer = false;
	}
		
	void OnTriggerStay(Collider other)
	{
		if (other.transform.gameObject.tag == "Enemy") 
		{
			//Debug.Log ("distance: " + Vector3.Distance(transform.position, other.transform.position));
			if(!isStop)
			{
				if (Vector3.Distance(transform.position, other.transform.position) > areaCollider.radius - distanceFromRadius) 
				{
					farmerShoot.StopFarmer();
					isStop = true;
				}
			}
		}
	}

}
