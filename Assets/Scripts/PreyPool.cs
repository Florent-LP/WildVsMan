using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyPool : MonoBehaviour {
    public int maxActivePreys = 10;
    public int delayBtSpawns = 120; //real seconds
    [HideInInspector] public List<PreySpawn> spawnList = new List<PreySpawn>();
    protected List<PreySpawn> availables = new List<PreySpawn>();
    [SerializeField] protected List<GameObject> preysPrefabs = new List<GameObject>();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        int curActivePreys = 0;
        availables.Clear();

        foreach (PreySpawn spawn in spawnList)
            if (spawn.enabled && spawn.available) {
                if (spawn.prey == null)
                    availables.Add(spawn);
                else
                    curActivePreys++;
            }

        if (curActivePreys < maxActivePreys
            && availables.Count > 0
            && preysPrefabs.Count > 0)
        {
            PreySpawn randSpawn = availables[Random.Range(0, availables.Count)];
            GameObject prey = preysPrefabs[Random.Range(0, preysPrefabs.Count)];
            if (prey != null) {
                GameObject preyInst = Instantiate(
                    prey,
                    randSpawn.transform.position,
                    randSpawn.transform.rotation,
                    transform
                );
                randSpawn.prey = preyInst;
                randSpawn.lockFor(delayBtSpawns);
            }
        }
	}
}
