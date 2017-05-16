using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Powers
{

    public class PowersManager : MonoBehaviour
    {
        [SerializeField] protected string triggerKey = "";
        [SerializeField] protected string cancelKey = "";
        [HideInInspector] public string _triggerKey { get {  return triggerKey; } }
        [HideInInspector] public string _cancelKey { get {  return cancelKey; } }
        [SerializeField] protected bool enableNumKeys = false; // Select power with alphanumeric keys

        public int activePower = 0;
        [SerializeField] protected List<Power> powersPrefabs = new List<Power>();
        protected List<Power> powersInstances = new List<Power>();
        protected GameObject powersContainer;

        //protected PlayerMana playerMana;

        protected void Awake()
        {
            powersContainer = new GameObject("Powers");
            powersContainer.transform.parent = gameObject.transform;
            powersContainer.transform.localPosition = Vector3.zero;
            powersContainer.transform.localRotation = Quaternion.identity;

            Power instance;
            foreach (Power p in powersPrefabs)
            {
                instance = Instantiate(p, powersContainer.transform, false);
                instance.manager = this;
                powersInstances.Add(instance);
            }
        }

        // Use this for initialization
        protected void Start()
        {
            //playerMana = gameObject.GetComponent<PlayerMana>();
        }

        // Update is called once per frame
        protected void Update()
        {
            if (powersInstances.Count > 0)
                for (int i = 0; i < powersInstances.Count; ++i)
                    powersInstances[i].powerEnabled = (i == activePower);
        }

        // Fixed update is called in sync with physics
        protected void FixedUpdate()
        {

        }

        protected void OnGUI()
        {
            if (enableNumKeys)
            {
                Event e = Event.current;
                if (e != null && e.isKey && (int)e.keyCode >= (int)KeyCode.Alpha0 && (int)e.keyCode <= (int)KeyCode.Alpha9)
                    for (int i = 0; i < powersInstances.Count; ++i)
                        if ((int)e.keyCode == (int)KeyCode.Alpha1 + i)
                            activePower = i;
            }
        }

        public bool RequestMana(int amount)
        {
            /*if (playerMana != null)
                return playerMana.UseMana(amount);
            return false;*/
            return true;
        }
    }

    public abstract class Power : MonoBehaviour
    {
        [HideInInspector] public PowersManager manager;
        [HideInInspector] public bool powerEnabled = false;
        public bool useTriggerKeyOnly = false;
        public int manaCost = 0;
        public float timeBetweenUses = 1f;
    }
}
