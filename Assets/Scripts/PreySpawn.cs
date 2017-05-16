using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreySpawn : MonoBehaviour {
    [HideInInspector] public GameObject prey = null;
    protected bool visible = false;
    protected bool locked = false;
    protected bool tooClose = false;
    public bool available {
        get {
            return visible && !locked && !tooClose;
        }
    }
    [HideInInspector] public PreyPool pool = null;

	// Use this for initialization
	void Start () {
        initialize();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ClippingField")
            visible = true;
        else if (other.gameObject.tag == "Player")
            tooClose = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "ClippingField") {
            visible = false;
            Destroy(prey);
            prey = null;
        } else if (other.gameObject.tag == "Player")
            tooClose = false;
    }

    private void OnEnable()
    {
        initialize();
    }

    private void OnDisable()
    {
        visible = false;
        Destroy(prey);
        prey = null;
    }

    protected void initialize()
    {
        if (pool == null)
            pool = GameObject.FindWithTag("PreyPool").GetComponent<PreyPool>();

        if (pool != null)
            pool.spawnList.Add(this);
    }

    public void lockFor(float seconds) {
        locked = true;
        StartCoroutine(timedLock(seconds));
    }

    protected IEnumerator timedLock(float seconds) {
        yield return new WaitForSeconds(seconds);
        locked = false;
    }
}
