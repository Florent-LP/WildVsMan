using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


namespace Powers
{
    public class NightVision : Power
    {
        // User settings
        public float maxDuration = 10f;

        // NightVision component
        protected DeferredNightVisionEffect nvComp = null;


        // State variables
        protected enum State
        {
            disabled,
            enabled,
            cancelling
        }
        protected State currentState = State.disabled;

        // Use this for initialization
        protected void Start()
        {
            nvComp = Camera.main.GetComponent<DeferredNightVisionEffect>();
            if (nvComp == null)
                throw new System.Exception("NightVision Power: You must attach a DeferredNightVisionEffect script to the MainCamera.");
        }

        // Update is called once per frame
        protected void Update()
        {
            if (powerEnabled)
            {
                string cancelKey = useTriggerKeyOnly ? manager._triggerKey : manager._cancelKey;

                // Trigger
                if (CrossPlatformInputManager.GetButtonDown(manager._triggerKey) && currentState == State.disabled)
                {
                    if (manager.RequestMana(manaCost))
                        manager.StartCoroutine(ExecuteVision());
                    else
                        currentState = State.disabled;
                }
                // Cancel
                else if (CrossPlatformInputManager.GetButtonDown(cancelKey) && currentState == State.enabled)
                    currentState = State.cancelling;
            }
        }

        // Fixed update is called in sync with physics
        protected void FixedUpdate()
        {

        }

        protected IEnumerator ExecuteVision()
        {
            currentState = State.enabled;

            // Step 1 : Enable vision
            nvComp.enabled = true;

            // Step 2 : Wait for vision timeout
            //yield return new WaitForSeconds(maxDuration); // version without cancel
            float startTime = Time.time;
            while (Time.time - startTime < maxDuration && currentState != State.cancelling)
                yield return null;

            // Step 3 : Disable vision
            nvComp.enabled = false;

            // Prevent abuse
            yield return new WaitForSeconds(timeBetweenUses);

            currentState = State.disabled;
        }
    }
}
