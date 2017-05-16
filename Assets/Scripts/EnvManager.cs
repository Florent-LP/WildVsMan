using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvManager : MonoBehaviour {
    [SerializeField] protected GameManagerMain manager = null;
    public int oneSwitchEvery = 60; // InGame minutes

    // Use this for initialization
    void Start () {
        if (manager != null) manager.registerEvent(0, switchOneByOne);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public IEnumerator switchOneByOne()
    {
        for (int i = 0; enabled && i < transform.childCount; ++i)
        {
            yield return new WaitForSeconds(manager.minToRealSec(oneSwitchEvery));
            //Debug.Log("Switch");
            EnvSwitch envSwitch = transform.GetChild(i).GetComponent<EnvSwitch>();
            if (envSwitch != null) envSwitch.switchUrban();
        }
    }
}
