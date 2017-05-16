using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : MonoBehaviour {
    [SerializeField] protected GameObject inputToBreak = null;
    [SerializeField] protected List<Rigidbody> fencePieces = new List<Rigidbody>();
    [SerializeField] protected List<PreyAI> enableSheeps = new List<PreyAI>();
    [SerializeField] protected GameObject farmerArea  = null;
    protected GameObject player = null;
    [HideInInspector] public bool broken = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!broken && player != null && Input.GetKeyDown(KeyCode.E))
        {
            foreach (Rigidbody piece in fencePieces)
                piece.isKinematic = false;
            foreach (PreyAI sheepAI in enableSheeps)
                sheepAI.enabled = true;
            farmerArea.SetActive(true);

            broken = true;
            player = null;
            inputToBreak.SetActive(false);
            StartCoroutine(delayedDisable());
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (!broken && other.tag == "Player")
        {
            player = other.gameObject;
            inputToBreak.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            player = null;
            inputToBreak.SetActive(false);
        }
    }

    protected IEnumerator delayedDisable()
    {
        yield return new WaitForSeconds(60);
        gameObject.SetActive(false);
    }
}
