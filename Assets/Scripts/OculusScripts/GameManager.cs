using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour
{
    public enum GameTypes { Sandbox, SkeetShooter, None } //GameTypes public enum

    //Public Variables
    [Header("UI Elements")]
    [Header("UI Buttons")]
    public Button sandboxButton;
    public Button skeetButton, startButton;
    [Header("UI Texts")]
    public Text scoreText;
    public Text targetsText, timeText;
    [Space(10)]
    public Text targetsCaption;
    public Text timeCaption;
    [Header("UI Sliders")]
    public Slider targetsSlider;
    public Slider timeSlider;

    [Header("Sandbox Objects")]
    public GameObject[] StationaryTargets;
    public GameObject[] MovingTargets;

    [Header("Skeet Shooter Objects")]
    public GameObject SkeetTarget;
    public GameObject[] SkeetPositions;
    public OVRInput.Button SkeetPullButton;

    //Private Variables
    private int score = 0, targetsHit = 0;
    private bool gameInProgress = false, singleTargetUp = false;
    private float time, targets;
    private float currentTime = 0f;
    private GameTypes currentGameType;
    private GameObject[] AllTargets;
    private GameObject currentTargetUp;
    private OVRInput.Controller currentHand;

    // Start is called before the first frame update
    void Start()
    {
        // Creating an array for both stationary and moving targets
        AllTargets = new GameObject[StationaryTargets.Length + MovingTargets.Length];
        StationaryTargets.CopyTo(AllTargets, 0);
        MovingTargets.CopyTo(AllTargets, StationaryTargets.Length);

        // Hide all options on first frame
        HideSandboxOptions();
        HideSkeetOptions();

        // Attaching UI buttons to correct methods
        Button sandbox = sandboxButton.GetComponent<Button>();
        sandbox.onClick.AddListener(HideSkeetOptions);
        sandbox.onClick.AddListener(ShowSandboxOptions);

        Button skeet = skeetButton.GetComponent<Button>();
        skeet.onClick.AddListener(HideSandboxOptions);
        skeet.onClick.AddListener(ShowSkeetOptions);

        Button start = startButton.GetComponent<Button>();
        start.onClick.AddListener(StartGame);

        // Resetting game mode and score
        currentGameType = GameTypes.None; // Set gamemode to none

        LoadSandboxLeaderboard();
        LoadSkeetLeaderboard();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameInProgress && currentTime > 0)
        {
            currentTime -= 1 * Time.deltaTime;
        }
        // Update all text displays
        UpdateTextDisplays();

        // Update correct game mode
        if (currentGameType == GameTypes.Sandbox && gameInProgress == true)
        {
            SandboxGameMode();
        }
        else if (currentGameType == GameTypes.SkeetShooter && gameInProgress == true)
        {
            SkeetGameMode();
        }else if(gameInProgress == false)
        {
            NoGameMode();
        }
    }

    /// <summary>
    /// Offshore method to update all the UI text displays
    /// </summary>
    void UpdateTextDisplays()
    {
        scoreText.text = score.ToString();
        targetsText.text = targetsSlider.value.ToString();
        timeText.text = timeSlider.value.ToString();
    }

    /// <summary>
    /// Shows all the relevant UI elements for the sandbox game mode
    /// </summary>
    void ShowSandboxOptions()
    {
        currentGameType = GameTypes.Sandbox;

        targetsText.gameObject.SetActive(true);
        targetsSlider.gameObject.SetActive(true);
        targetsCaption.gameObject.SetActive(true);

        timeText.gameObject.SetActive(true);
        timeSlider.gameObject.SetActive(true);
        timeCaption.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides all the relevant UI elements for the sandbox game mode
    /// </summary>
    void HideSandboxOptions()
    {
        targetsText.gameObject.SetActive(false);
        targetsSlider.gameObject.SetActive(false);
        targetsCaption.gameObject.SetActive(false);

        timeText.gameObject.SetActive(false);
        timeSlider.gameObject.SetActive(false);
        timeCaption.gameObject.SetActive(false);
    }

    /// <summary>
    /// Shows all the relevant UI elements for the skeet shooter game mode
    /// </summary>
    void ShowSkeetOptions()
    {
        currentGameType = GameTypes.SkeetShooter;

        targetsText.gameObject.SetActive(true);
        targetsSlider.gameObject.SetActive(true);
        targetsCaption.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides all the relevant UI elements for the skeet shooter game mode
    /// </summary>
    void HideSkeetOptions()
    {
        targetsText.gameObject.SetActive(false);
        targetsSlider.gameObject.SetActive(false);
        targetsCaption.gameObject.SetActive(false);
    }

    /// <summary>
    /// Is used by the start button to decypher which gamemode is selected and the correct logic to use
    /// </summary>
    void StartGame()
    {
        if (!gameInProgress)
        {
            gameInProgress = true;

            if (currentGameType == GameTypes.Sandbox) // Any setups for the game mode in this if statement
            {
                // Get values from UI elements
                time = timeSlider.value;
                currentTime = time;
                targets = targetsSlider.value;
                targetsHit = 0;
                score = 0;

                // Command all targets to go down
                foreach(GameObject target in StationaryTargets)
                {
                    target.GetComponentInChildren<StationaryTarget>().TargetDown();
                }

                // Hide all UI elements
                HideSandboxOptions();
                HideSkeetOptions();
            }
            else if (currentGameType == GameTypes.SkeetShooter) // Any setups for the game mode in this if statement
            {
                targets = targetsSlider.value;
                targetsHit = 0;
                score = 0;

                foreach (GameObject target in StationaryTargets)
                {
                    target.GetComponentInChildren<StationaryTarget>().TargetDown();
                }

                HideSandboxOptions();
                HideSkeetOptions();
            }else
            {
                print("No gamemode selected!");
            }
        }
    }

    /// <summary>
    /// Holds all the logic for the sandbox game mode
    /// </summary>
    void SandboxGameMode()
    {
        if (currentTime > 0 && targetsHit < targets)
        {
            singleTargetUp = false;
            foreach(GameObject target in StationaryTargets)
            {
                if(target.GetComponentInChildren<StationaryTarget>().TargetStatus == StationaryTarget.TargetState.Down)
                {
                    continue;
                }
                else
                {
                    singleTargetUp = true;
                    break;
                }
            }

            if(singleTargetUp == false)
            {
                GameObject newTargetUp = StationaryTargets[Random.Range(0, StationaryTargets.Length)];
                while(newTargetUp == currentTargetUp)
                {
                    newTargetUp = StationaryTargets[Random.Range(0, StationaryTargets.Length)];
                }
                newTargetUp.GetComponentInChildren<StationaryTarget>().TargetUp();
                currentTargetUp = newTargetUp;
            }
        }
        else
        {
            gameInProgress = false;
            print("Sandbox game has finished");
            foreach(GameObject target in StationaryTargets)
            {
                target.GetComponentInChildren<StationaryTarget>().TargetUp();
            }
            targetsHit = 0;

            SaveSandbox();
        }
        
    }

    /// <summary>
    /// Holds all the logic for the skeet shooter game mode
    /// </summary>
    void SkeetGameMode()
    {
        if(targetsHit < targets)
        {
            GameObject pos = SkeetPositions[Random.Range(0, SkeetPositions.Length)];
            if(OVRInput.GetDown(SkeetPullButton, currentHand))
            {
                print("Button pressed");
                GameObject skeetObject = Instantiate(SkeetTarget, pos.transform);
                switch (pos.name)
                {
                    case "SkeetPos_1":
                        skeetObject.GetComponentInChildren<Rigidbody>().AddForce(new Vector3(1, 1, -1) * 1000);
                        break;
                    case "SkeetPos_2":
                        skeetObject.GetComponentInChildren<Rigidbody>().AddForce(new Vector3(-1, 1, -1) * 1000);
                        break;
                    case "SkeetPos_3":
                        skeetObject.GetComponentInChildren<Rigidbody>().AddForce(new Vector3(1, 1, 0) * 1000);
                        break;
                    case "SkeetPos_4":
                        skeetObject.GetComponentInChildren<Rigidbody>().AddForce(new Vector3(-1, 1, 0) * 1000);
                        break;
                }
            }
        }else
        {
            gameInProgress = false;
            print("Skeet shooter game has finished");
            foreach (GameObject target in StationaryTargets)
            {
                target.GetComponentInChildren<StationaryTarget>().TargetUp();
            }
            targetsHit = 0;
        }

        SaveSkeetShooter();
    }

    void NoGameMode()
    {
        foreach(GameObject target in StationaryTargets)
        {
            if(target.transform.rotation == target.GetComponentInChildren<StationaryTarget>().RotateTarget && target.GetComponentInChildren<StationaryTarget>().TargetStatus == StationaryTarget.TargetState.Down)
            {
                target.GetComponentInChildren<StationaryTarget>().TargetUp();
            }
        }
    }

    public int Score
    {
        get { return score; }
        set { score = value; }
    }

    public int TargetsHit
    {
        get { return targetsHit; }
        set { targetsHit = value; }
    }

    public OVRInput.Controller CurrentHand
    {
        get { return currentHand; }
        set { currentHand = value; }
    }

    void SaveSandbox()
    {

    }

    void SaveSkeetShooter()
    {

    }

    void LoadSandboxLeaderboard()
    {

    }

    void LoadSkeetLeaderboard()
    {

    }
}