using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODOptimization : MonoBehaviour
{
    public List<GameObject> disableWhenFar = new List<GameObject>();

    // Use this for initialization
    void Start () {
        toggleFarLOD();
	}
	
	// Update is called once per frame
	/*void Update () {

    }*/

    public void toggleCloseLOD()
    {
        foreach (GameObject go in disableWhenFar)
            go.SetActive(true);
    }

    public void toggleFarLOD()
    {
        foreach (GameObject go in disableWhenFar)
            go.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ClippingField")
            toggleCloseLOD();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "ClippingField")
            toggleFarLOD();
    }
}