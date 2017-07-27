using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public enum LoadStates { Dummy, CSV, SavedState};

public class ApplicationManager : MonoBehaviour {

    public static ApplicationManager Instance;

    public Canvas UICanvas;
    public LoadStates LoadFrom;

    public float WaitTime = 2;
    public int AutomaticSaveAfterActions = 5;

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
    private GameOverViewController _gameOver;

    //Simulation
    private GameData _remoteData;
    private RemotePersonalitySimulation _simulation;
    private SimulationController _remoteSimulationContoller;
    private MultiplayerController _remoteMultiplayerController;

    //Multiplayer
    private bool rotated;
    private HappeningController _happeningController;
    private MultiplayerController _multiplayer;
    public MultiplayerController Multiplayer { get { return _multiplayer; } }
    public MultiplayerViewController MultiplayerViewController { get; set; }

    public bool SimulateMultiplayer { get; set; }

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start() {

        //Data
        _data = new GameData(LoadFrom);

        CharacterAnimation = new AnimationController(GraphicsHelper.Instance.lemo);

        //UI
        _alert = new AlertViewController(UICanvas.transform);
        _output = new OutputViewController(UICanvas.transform);
        _feedback = new FeedbackViewController(UICanvas.transform, _data.Intelligence);
        _itemBox = new ItemBoxViewController(UICanvas.transform, _data.Items, _data.Person, CharacterAnimation);
        _conditionMonitor = new ConditionViewController(UICanvas.transform, _data.Person);
        _options = new OptionsMenuController(UICanvas.transform, _data.Person);


        if (_data.Person.GetItems().Count >= 1)
        {
            foreach (KeyValuePair<int, Item> kvp in _data.Person.GetItems())
            {
                _itemBox.AddItemFromPersonality(kvp.Value);
            }
        }
        
        _happeningController = GameObject.Find("Happening").GetComponent<HappeningController>();
        _multiplayer = new MultiplayerController(_data, _happeningController, "local");
        _happeningController.Lemo = Multiplayer;
        MultiplayerViewController = new MultiplayerViewController(UICanvas.transform);


        //Simulation
        _remoteSimulationContoller = new SimulationController();
        _remoteData = new GameData(LoadStates.CSV);
        _remoteMultiplayerController = new MultiplayerController(_remoteData, _remoteSimulationContoller, "remote");

        _gameOver = new GameOverViewController(UICanvas.transform, CharacterAnimation);

        //GameLoop
        _gameLoop = new GameLoopController(this, _data);

        _multiplayer.setGameLoop(_gameLoop);

        
    }

    public void GameOver()
    {
        _gameOver.GameOver();
    }

    public void EndGame()
    {
        _data.Person.IsAlive = false;
    }

    public IEnumerator StartGame()
    {
        return _gameOver.StartSequence();
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

    public void ShowEmotion(int emotion)
    {
        if(emotion < 0)
        {
            GraphicsHelper.Instance.negativeEmotionFX.Play();
        } else if(emotion > 0)
        {
            GraphicsHelper.Instance.positiveEmotionFX.Play();
        }
    }

    public void ShowRemoteFeedback(int feedback)
    {
        _feedback.ShowRemoteFeedback(feedback);
    }

    public void ShowItemAlert(Item item)
    {
        _alert.ShowAlert(_itemBox.GetItemIcon(item));
    }

    public void MoveItemAlert(bool multiplayer)
    {
        if(multiplayer)
        {
            _alert.setMultiplayer();
        }
        else
        {
            _alert.setSingleplayer();
        }
    }

    public void UpdateUI()
    {
        _conditionMonitor.UpdateSlider(_data.Person);
        _itemBox.UpdateBox(_data.Person);
    }

    public void ShowMessage(string message, bool isMultiplayer)
    {
        _output.DisplayMessage(message, isMultiplayer);
    }

    public bool getWaitForFeedback()
    {
        return _gameLoop.waitForFeedback;
    }

    public FeedbackViewController getFeedbackController()
    {
        return _feedback;
    }

    public void SetupRemoteTexture(Material material)
    {
        MultiplayerViewController.setupTexture(material);
    }

    public void AddRemoteItem(int itemId)
    {
        MultiplayerViewController.addItem(itemId);
    }

    public void RemoveRemoteItem()
    {
        MultiplayerViewController.removeItem();
    }

    void Update() {

        bool debugLandscape = false;
        bool debugPortrait = false;

#if UNITY_EDITOR
        debugLandscape = (Screen.width > Screen.height);
        debugPortrait = (Screen.width <= Screen.height) ;
#endif

        if ((Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight || debugLandscape) && !rotated)
        {
            DebugController.Instance.Log("Activate Multiplayer: " + Input.deviceOrientation.ToString() + " " + debugLandscape.ToString() + " " + rotated.ToString(), DebugController.DebugType.Multiplayer);

            rotated = true;

            

            if (SimulateMultiplayer)
            {
                StartCoroutine(ConnectRemoteSimulation());
            }
            else
            {
                Multiplayer.SetConnectionController(_happeningController);
                Multiplayer.StartMultiplayer();
            }

            MultiplayerViewController.startMultiplayerView();
        }

        if ((Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown || debugPortrait) && rotated)
        {
            DebugController.Instance.Log("Dectivate Multiplayer: " + Input.deviceOrientation.ToString() + " " + debugPortrait.ToString() + " " + rotated.ToString(), DebugController.DebugType.Multiplayer);


            rotated = false;

            if (SimulateMultiplayer)
            {
                if (_simulation != null)
                {
                    _simulation.StopSimulation();
                    _simulation = null;
                    _remoteMultiplayerController.EndMultiplayer();
                }
            }

            Multiplayer.EndMultiplayer();
            MultiplayerViewController.endMultiplayerView();
            
        }
    }

    IEnumerator ConnectRemoteSimulation ()
    {
        Debug.Log("Start Routine !!!! " + rotated);
        yield return new WaitForSeconds(2);

        // Remote
        _remoteSimulationContoller.SetMultiplayerController(Multiplayer);

        // Local
        SimulationController localSimulationContoller = new SimulationController();
        localSimulationContoller.SetMultiplayerController(_remoteMultiplayerController);
        Multiplayer.SetConnectionController(localSimulationContoller);

        _simulation = new RemotePersonalitySimulation(_remoteData, this, _remoteMultiplayerController);
        _remoteMultiplayerController.StartMultiplayer();
        Multiplayer.StartMultiplayer();
    }
}
