using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerMain : MonoBehaviour
{
    public delegate IEnumerator Callback();
    protected List<TimedEvent> eventsList = new List<TimedEvent>();
    public float realTimeElapsed { get; protected set; }

	public Text sentenceSurvival;

	public GameObject inputFollowHUD;
	public GameObject inputGetOutHUD;
	public GameObject player;

	public static GameObject inputFollowHUDGO;
	public static GameObject inputGetOutHUDGO;
	public static GameObject playerGO;

    [SerializeField] protected float realTimeToGameDay_minutes = 15;

    // real to game Converters
    const float oneDay_seconds = 24 * 3600;
    public float gameTimeElapsed
    {
        get
        {
            float realTimeToGameDay_seconds = realTimeToGameDay_minutes * 60;
            return realTimeElapsed * oneDay_seconds / realTimeToGameDay_seconds;
        }
    }
    public float days
    {
        get
        {
            return Mathf.Floor(gameTimeElapsed / oneDay_seconds);
        }
    }
    public float hours
    {
        get
        {
            return Mathf.Floor(gameTimeElapsed % oneDay_seconds / 3600);
        }
    }
    public float minutes
    {
        get
        {
            return Mathf.Floor(gameTimeElapsed % oneDay_seconds % 3600 / 60);
        }
    }
    public float seconds
    {
        get
        {
            return Mathf.Floor(gameTimeElapsed % oneDay_seconds % 3600 % 60);
        }
    }

    // game to real Converters
    public float dayToRealSec(float gameDays)
    {
        return gameDays * realTimeToGameDay_minutes * 60;
    }
    public float hourToRealSec(float gameHours)
    {
        return gameHours * realTimeToGameDay_minutes * 60 / 24;
    }
    public float minToRealSec(float gameMinutes)
    {
        return gameMinutes * realTimeToGameDay_minutes / 24;
    }
    public float secToRealSec(float gameSeconds)
    {
        return gameSeconds * realTimeToGameDay_minutes * 60 / (24 * 3600);
    }

    // to string
    public string gameTimeToStr()
    {
        return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", days, hours, minutes, seconds); // dd:hh:mm:ss
    }

    // events handlers
    public class TimedEvent
    {
        public bool done = false;
        public float atTime = 0; // Real seconds
        public Callback callback = null; // Must return be Coroutine
    }

    public void registerEvent(float atRealTime, Callback functionToCall)
    {
        TimedEvent gameEvent = new TimedEvent();
        gameEvent.atTime = atRealTime;
        gameEvent.callback = functionToCall;

        eventsList.Add(gameEvent);
    }

    protected void browseEvents()
    {
        for (int i = 0; i < eventsList.Count; ++i)
        {
            if (!eventsList[i].done && eventsList[i].atTime < realTimeElapsed)
            {
                eventsList[i].done = true;
                StartCoroutine(eventsList[i].callback());
            }
        }
    }

    // callback example
    protected IEnumerator LogGameTime()
    {
        Debug.Log("Date is: " + gameTimeToStr());
        yield return null;
    }

	void Awake()
	{
		inputFollowHUDGO = inputFollowHUD;
		inputGetOutHUDGO = inputGetOutHUD;
		playerGO = player;
	}

    // Use this for initialization
    void Start()
    {
        realTimeElapsed = 0.0f;

        // Comment executer une fonction (ex : LogGameTime) a 5min dans le jeu ?
        // Exemple (voir plus haut pour le prototype de LogGameTime) :
        registerEvent(minToRealSec(5), LogGameTime);
        // Soit ici, soit dans un autre Start (voir EnvManager.cs).

    }

    // Update is called once per frame
    void Update()
    {
		sentenceSurvival.text = "Tu as survécu " + (days+1) + " jour(s).";
        realTimeElapsed += Time.deltaTime;
        browseEvents();
    }
}
