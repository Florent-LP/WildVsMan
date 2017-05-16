/* TODO:
 * Limitate height & distance separately
 * SphereCast->CapsuleCast
 */

#define DEBUG_TP
//#define CAPSULE_CAST // For mark placement

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Powers {
    public class Teleport : Power
    {
        // User settings
        public float maxDistance = 10f;
        public float teleportTime = 0.5f;

        // Prefabs
        protected List<GameObject> originalAppearance = new List<GameObject>(); // GameObjects to hide while teleporting (in order to change appearance)
        [SerializeField] protected  GameObject altAppearancePrefab; // Model prefab used while teleporting (e.g bat)
        protected GameObject altAppearanceInstance = null;
        [SerializeField] protected  GameObject teleportMarkPrefab; // Mark model
        protected GameObject teleportMarkInstance = null;

        // State variables
        protected enum State
        {
            idle,
            placingMark,
            requestingTP,
            teleporting
        }
        protected State currentState = State.idle;

        // GameObject attributes
        protected int playerLayer;
        protected Camera mainCamera;
        protected Rigidbody rbody;
        protected CharacterController charCtr;
        protected CapsuleCollider capsule;

        // Use this for initialization
        protected void Start()
        {
            SkinnedMeshRenderer[] appearanceMeshes = manager.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer mesh in appearanceMeshes)
                originalAppearance.Add(mesh.gameObject);

            playerLayer = manager.gameObject.layer;
            mainCamera = Camera.main;
            rbody = manager.gameObject.GetComponent<Rigidbody>();
            charCtr = manager.gameObject.GetComponent<CharacterController>();
            capsule = manager.gameObject.GetComponent<CapsuleCollider>();
        }

        // Update is called once per frame
        protected void Update()
        {
            if (powerEnabled)
            {
                if (CrossPlatformInputManager.GetButtonDown(manager._triggerKey) && currentState == State.idle)
                    currentState = State.placingMark;
                else if (CrossPlatformInputManager.GetButtonUp(manager._triggerKey) && currentState == State.placingMark)
                    currentState = State.requestingTP;
                else if (CrossPlatformInputManager.GetButtonDown(manager._cancelKey) && currentState == State.placingMark && !useTriggerKeyOnly)
                    currentState = State.idle;
            }
        }

        // Fixed update is called in sync with physics
        protected void FixedUpdate()
        {
            if (currentState == State.placingMark)
            {
                // Choose the location of the mark
                Vector3 destination = getDestination();

                // Check if destination is in front of the player
                Vector3 toDest = (destination - transform.position).normalized;
                if (Vector3.Dot(toDest, transform.forward) <= 0)
                    destination = transform.position;

                // Create the mark or place it
                if (teleportMarkInstance == null)
                    teleportMarkInstance = Instantiate(teleportMarkPrefab, destination, manager.gameObject.transform.rotation);
                else
                    teleportMarkInstance.transform.position = destination;

            }
            else if (currentState == State.requestingTP)
            {
                // Teleport player v1
                /*transform.position = teleportMark_instance.transform.position;
                transform.rotation = teleportMark_instance.transform.rotation;*/

                // Teleport player v2
                if (manager.RequestMana(manaCost))
                    manager.StartCoroutine(Dash(teleportMarkInstance.transform.position));
                else
                    currentState = State.idle;
                Destroy(teleportMarkInstance);
            }
            else if (currentState == State.idle)
            {
                if (teleportMarkInstance != null)
                    Destroy(teleportMarkInstance);
            }
        }

        protected Vector3 getDestination()
        {
            //Ray aiming = mainCamera.ScreenPointToRay(Input.mousePosition);
            Ray aiming = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            LayerMask layersToIgnore = 0;
            layersToIgnore |= 1 << LayerMask.NameToLayer("Ignore Raycast");
            layersToIgnore |= 1 << LayerMask.NameToLayer("UI");
            layersToIgnore |= 1 << LayerMask.NameToLayer("Player");
            layersToIgnore |= 1 << LayerMask.NameToLayer("Hitbox");
            layersToIgnore = ~layersToIgnore;

            #if !CAPSULE_CAST
                bool castTest = Physics.Raycast(aiming, out hit, maxDistance, layersToIgnore);
            #else
                CapsuleCollider playerCapsule = manager.gameObject.GetComponent<CapsuleCollider>();
                if (playerCapsule == null) throw new System.Exception("Player has no CapsuleCollider");
                Vector3 capsuleTop = mainCamera.transform.position + (playerCapsule.height * 0.5f - playerCapsule.radius) * Vector3.up;
                Vector3 capsuleBot = mainCamera.transform.position + (playerCapsule.height * 0.5f - playerCapsule.radius) * Vector3.down;

                bool castTest = Physics.CapsuleCast(capsuleBot, capsuleTop, playerCapsule.radius, aiming.direction, out hit, maxDistance, layersToIgnore);
            #endif

            Vector3 destination = castTest
                ? hit.point // An obstacle has been encountered
                : aiming.origin + aiming.direction.normalized * maxDistance; // No obstacle

             #if DEBUG_TP
                if (castTest)
                    Debug.Log("Teleport - cast hit: " + hit.collider.gameObject.name);
             #endif

            return destination;
        }

        protected IEnumerator Dash(Vector3 destination)
        {
            currentState = State.teleporting;

            // Disable physics
            if (rbody != null) rbody.isKinematic = true;
            if (charCtr != null) charCtr.enabled = false;
            if (capsule != null) capsule.enabled = false;

            // Hotfix to avoid warnings (conflict with PlayerController)
            PlayerController playerCtr = manager.GetComponent<PlayerController>();
            if (playerCtr != null) playerCtr.enabled = false;

            // Some useful variables
            Vector3 origin = manager.transform.position;
            Vector3 direction = destination - origin;
            float distance = Vector3.Distance(destination, origin);
            float speed = distance / teleportTime;
            float startTime = Time.time;

            // Morph
            if (altAppearancePrefab != null)
            {
                foreach (GameObject prefab in originalAppearance)
                    prefab.SetActive(false);

                if (altAppearanceInstance != null)
                    Destroy(altAppearanceInstance);

                altAppearanceInstance = Instantiate(
                    altAppearancePrefab,
                    mainCamera.transform.position + direction.normalized * 0.8f + new Vector3(0, -0.5f, 0),
                    mainCamera.transform.rotation,
                    manager.transform
                );
            }

            // Execute move
            //for (float curTime = 0;  curTime < teleportTime; curTime = Time.time - startTime) { // Time limiter version
            while (Vector3.Distance(manager.transform.position, origin) < distance) // Distance limiter version
            {
                manager.transform.position += direction.normalized * speed * Time.deltaTime;
                yield return null;
            }

#if DEBUG_TP
                Debug.Log(
                    "Teleport:\n" +
                    "    Distance - wanted: " + distance + ", real: " + Vector3.Distance(transform.position, origin) + "\n" +
                    "    Time - wanted: " + teleportTime + ", real: " + (Time.time - startTime)
                );
#endif

            // Morph back to original
            if (altAppearanceInstance != null)
            {
                Destroy(altAppearanceInstance);
                foreach (GameObject prefab in originalAppearance)
                    prefab.SetActive(true);
            }

            // Restore physics
            if (rbody != null) rbody.isKinematic = false;
            if (charCtr != null) charCtr.enabled = true;
            if (capsule != null) capsule.enabled = true;

            // Hotfix to avoid warnings (conflict with PlayerController)
            if (playerCtr != null) playerCtr.enabled = true;

            // Prevent Abuse
            yield return new WaitForSeconds(timeBetweenUses);

            currentState = State.idle;
        }
    }
}

// Former version
/*//#define CAPSULE_CAST // For mark placement

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(CapsuleCollider))]
public class Teleport : MonoBehaviour
{
    // State variables
    protected bool placingMark = false;
    protected bool teleporting = false;
    protected bool dashRunning = false;

    // GameObject attributes
    protected Camera mainCamera;
    protected Rigidbody rbody;
    protected CapsuleCollider capsule;
    protected int playerLayer;

    // Prefabs
    public GameObject teleportModelPrefab; // Model prefab used while teleporting (e.g bat)
    protected GameObject teleportModelInstance = null;
    public GameObject teleportMarkPrefab; // Mark model
    protected GameObject teleportMarkInstance = null;
    public GameObject[] humanoidPrefabs; // Model instances to hide while teleporting (default ones)

    // User settings
    public float maxDistance = 10f;
    public float teleportTime = 0.5f;
    public float delayBetweenDashes = 1f;

    // Use this for initialization
    protected void Start()
    {
        mainCamera = Camera.main;
        rbody = gameObject.GetComponent<Rigidbody>();
        capsule = gameObject.GetComponent<CapsuleCollider>();
        playerLayer = gameObject.layer;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Ability1") && !placingMark && !teleporting)
        {
            placingMark = true;
        }
        else if (CrossPlatformInputManager.GetButtonUp("Ability1") && placingMark && !teleporting)
        {
            placingMark = false;
            teleporting = true;
        }
    }

    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        if (placingMark)
        {
            // Choose the location of the mark
            Vector3 destination = getDestination();

            // Create the mark or place it
            if (teleportMarkInstance == null)
                teleportMarkInstance = Instantiate(teleportMarkPrefab, destination, gameObject.transform.rotation);
            else
                teleportMarkInstance.transform.position = destination;

        }
        else if (teleporting)
        {
            // Teleport player v1
            //transform.position = teleportMark_instance.transform.position;
            //transform.rotation = teleportMark_instance.transform.rotation;

            // Teleport player v2
            if (!dashRunning) StartCoroutine("Dash");

            Destroy(teleportMarkInstance);
            teleporting = false;
        }
    }

    protected Vector3 getDestination()
    {
        Ray aiming = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        LayerMask layersToIgnore = ~(1 << playerLayer | 1 << 2); // Ignore the player and the mark (layer 2 = Ignore Raycast)

#if !CAPSULE_CAST
        bool castTest = Physics.Raycast(aiming, out hit, maxDistance, layersToIgnore);
#else
        CapsuleCollider playerCapsule = gameObject.GetComponent<CapsuleCollider>();
        if (playerCapsule == null) return;
        Vector3 capsuleTop = mainCamera.transform.position + (playerCapsule.height * 0.5f - playerCapsule.radius) * Vector3.up;
        Vector3 capsuleBot = mainCamera.transform.position + (playerCapsule.height * 0.5f - playerCapsule.radius) * Vector3.down;

        bool castTest = Physics.CapsuleCast(capsuleBot, capsuleTop, playerCapsule.radius, aiming.direction, out hit, maxDistance, layersToIgnore);
#endif

        Vector3 destination = castTest
            ? hit.point // An obstacle has been encountered
            : aiming.origin + aiming.direction.normalized * maxDistance; // No obstacle

        return destination;
    }

    protected IEnumerator Dash()
    {
        dashRunning = true;

        // Disable physics
        rbody.isKinematic = true;
        capsule.enabled = false;

        // Some useful variables
        Vector3 origin = transform.position;
        Vector3 dest = teleportMarkInstance.transform.position;
        Vector3 direction = dest - origin;
        float distance = Vector3.Distance(dest, origin);
        float speed = distance / teleportTime;
        float startTime = Time.time;

        // Morph
        if (teleportModelPrefab != null)
        {
            for (int i = 0; i < humanoidPrefabs.Length; ++i)
                humanoidPrefabs[i].SetActive(false);

            if (teleportModelInstance != null)
                Destroy(teleportModelInstance);

            teleportModelInstance = Instantiate(
                teleportModelPrefab,
                mainCamera.transform.position + direction.normalized * 0.8f + new Vector3(0, -0.5f, 0),
                mainCamera.transform.rotation,
                transform
            );
        }

        // Execute move
        //for (float curTime = 0;  curTime < teleportTime; curTime = Time.time - startTime) { // Time limiter version
        while (Vector3.Distance(transform.position, origin) < distance) // Distance limiter version
        {
            transform.position += direction.normalized * speed * Time.deltaTime;
            yield return null;
        }

        //Debug.Log(
        //    "Teleport:\n" +
        //    "    Distance - wanted: " + distance + ", real: " + Vector3.Distance(transform.position, origin) + "\n" +
        //    "    Time - wanted: " + teleportTime + ", real: " + (Time.time - startTime)
        //);

        // Morph back to original
        if (teleportModelInstance != null)
        {
            Destroy(teleportModelInstance);
            for (int i = 0; i < humanoidPrefabs.Length; ++i)
                humanoidPrefabs[i].SetActive(true);
        }

        // Restore physics
        rbody.isKinematic = false;
        capsule.enabled = true;

        // Prevent Abuse
        yield return new WaitForSeconds(delayBetweenDashes);

        dashRunning = false;
    }
}*/
