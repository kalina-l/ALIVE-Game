using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using KKSpeech;

public enum LoadStates { Dummy, CSV, SavedState};

public class ApplicationManager : MonoBehaviour {

    public static ApplicationManager Instance;

    public Text debugText;

    public Canvas UICanvas;
    public LoadStates LoadFrom;

    public float WaitTime = 2;
    public int AutomaticSaveAfterActions = 5;

    public bool resetButton;

    public bool simulateRemote;

    //AI
    private GameData _data;
    private GameLoopController _gameLoop;

    //Animation
    public AnimationController CharacterAnimation { get; set; }

    //UI
    private OutputViewController _output;
    private FeedbackViewController _feedback;
    private ItemBoxViewController _itemBox;
    private ConditionViewController _conditionMonitor;
    private AlertViewController _alert;
    private OptionsMenuController _options;

    //Simulation
    private RemotePersonalitySimulation _simulation;

    //Multiplayer
    private MultiplayerController _multiplayer;
    public MultiplayerController Multiplayer { get { return _multiplayer; } }
    public MultiplayerViewController MultiplayerViewController { get; set; }

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start() {

        //Data
        _data = new GameData(LoadFrom);

        //UI
        _alert = new AlertViewController(UICanvas.transform);
        _output = new OutputViewController(UICanvas.transform);
        _feedback = new FeedbackViewController(UICanvas.transform, _data.Intelligence);
        _itemBox = new ItemBoxViewController(UICanvas.transform, _data.Items, _data.Person);
        _conditionMonitor = new ConditionViewController(UICanvas.transform, _data.Person);
        _options = new OptionsMenuController(UICanvas.transform);

        CharacterAnimation = new AnimationController(GraphicsHelper.Instance.lemo);
        
        if (resetButton)
        {
            new ResetViewController(UICanvas.transform);
        }

        if (_data.Person.GetItems().Count >= 1)
        {
            foreach (KeyValuePair<int, Item> kvp in _data.Person.GetItems())
            {
                _itemBox.AddItemFromPersonality(kvp.Value);
            }
        }

        _multiplayer = new MultiplayerController(_data.Person, "local");
        MultiplayerViewController = new MultiplayerViewController();


        //Simulation
        if (simulateRemote)
        {
            _simulation = new RemotePersonalitySimulation(this, _data.Person);
            _multiplayer.ConnectWithRemote(_simulation.GetController());
            MultiplayerViewController.startMultiplayerView();
        }

        //GameLoop
        _gameLoop = new GameLoopController(this, _data);

        _multiplayer.setGameLoop(_gameLoop);
    }

    public void reset()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            string path = Path.Combine(Application.persistentDataPath, "savestates");
            Directory.Delete(path, true);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

	public void RemoveItem(){
		_itemBox.RemoveItemFromSlot ();
	}

    public void GiveFeedback(int feedback)
    {
        _gameLoop.GiveFeedback(feedback);
    }

    public void ShowFeedback(int feedback)
    {
        _output.ShowFeedback(feedback);
        _feedback.ShowFeedback(feedback);
    }

    public void ShowItemAlert(Item item)
    {
        _alert.ShowAlert(_itemBox.GetItemIcon(item));
    }

    public void UpdateUI()
    {
        _conditionMonitor.UpdateSlider(_data.Person);
    }

    public void ShowMessage(string message)
    {
        _output.DisplayMessage(message);
    }

    public bool getWaitForFeedback()
    {
        return _gameLoop.waitForFeedback;
    }

    public FeedbackViewController getFeedbackController()
    {
        return _feedback;
    }

    public void ToggleMultiplayer()
    {
        if (!Multiplayer.IsConnected)
        {
            _simulation = new RemotePersonalitySimulation(this, _data.Person);
            _multiplayer.ConnectWithRemote(_simulation.GetController());
        }
        else
        {
            Multiplayer.Disconnect();
        }
    }

    void Update() {
        if((Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight) && !Multiplayer.IsConnected)
        {
            _simulation = new RemotePersonalitySimulation(this, _data.Person);
            Multiplayer.ConnectWithRemote(_simulation.GetController());
            MultiplayerViewController.startMultiplayerView();
        }

        if ((Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown) && Multiplayer.IsConnected)
        {
            Multiplayer.Disconnect();
            _simulation.StopSimulation();
            MultiplayerViewController.endMultiplayerView();
        }
    }
}
