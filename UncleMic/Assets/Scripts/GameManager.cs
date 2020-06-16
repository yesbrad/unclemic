using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleSheetsToUnity;
using UnityEngine.Events;

public enum GameState {
    Menu,
    Game,
}

public class GameManager : MonoBehaviour 
{
    public static GameManager instance;

    public const string C_Player = "PLAYER";
    public const string C_ExtraPlayer = "EXTRA";

    private List<DrinkTask> data = new List<DrinkTask>();
    private List<DrinkTaskCue> cueData = new List<DrinkTaskCue>();

    public float minTime = 1;
    public float maxTime = 10;
    public int speechDelayTime = 2;
    public AudioClip taskClip;
    public List<Player> players = new List<Player>();

    public bool useDebugSheet;

    [Header("UI")]
    public Text taskText;
    public Image backgroundImage;
    public Text debugTimeText;

    int currentStep;
    private AudioSource audioSource;
    public List<Player> recentPlayers = new List<Player>();

    float curTime;
    internal GameState gameState = GameState.Menu;

	private void Start()
	{
        instance = this;
        audioSource = GetComponent<AudioSource>();
        SetGameState(GameState.Menu);
	}

	public void BeginGame (UnityAction<bool> startCallback) {
		FetchTaskData((success) => {
			startCallback.Invoke(success);
			if(success){
				SetGameState(GameState.Game);
			}
		});
	}

    public void FetchTaskData(UnityAction<bool> callback)
    {
		SpreadsheetManager.Read(new GSTU_Search("1rdp9VNO-s_Uxn3YFYZoLaVCRTudt19UQi4FwEs8BL-U", useDebugSheet ? "DebugTasks" : "Tasks"), (GstuSpreadSheet sheet) =>
		{
			print("Finished SpreadSheet Fetch");
			int i = 0;
			foreach (var val in sheet.columns["A"])
			{
				i++;
				if (!string.IsNullOrEmpty(val.value))
				{
					if (IsValidCell(sheet["B" + i].value) && IsValidCell(sheet["C" + i].value))
					{
						data.Add(new DrinkTask(val.value, new DrinkTaskCue(new DrinkTask(sheet["B" + i].value), int.Parse(sheet["C" + i].value))));
					}
					else
					{
						data.Add(new DrinkTask(val.value));
					}

				}
			}
			callback.Invoke(true);
		}, (RequestErrorResponse error) => {
			Debug.LogError("Google Sheets Response Failed: " + error.statusCode);
			callback.Invoke(false);
		});
	}

    private bool IsValidCell (string cellVal){
        return !string.IsNullOrWhiteSpace(cellVal) && cellVal != "NULL" && cellVal != "null";
    }


	private void Update()
	{
        if(gameState == GameState.Game) 
        {
            curTime -= Time.deltaTime;

            if(curTime < 0)
            {
                NextTask();
                curTime = GetNewTime();
            }

            debugTimeText.text = "" + Mathf.Floor(curTime);
		}
	}

    private void NextTask () 
    {
        DrinkTask task = GetNewTask();

        audioSource.PlayOneShot(taskClip);

        taskText.text = task.message;
        backgroundImage.color = Random.ColorHSV();
        StartCoroutine(DelaySpeech(task.message));
        currentStep++;
    }

    IEnumerator DelaySpeech (string playerMessage) {
        yield return new WaitForSeconds(speechDelayTime);
        Speech.instance.Speak(playerMessage);
        yield return null;
    }

    public void AddTask (DrinkTask task){
        data.Add(task);
    }

    public DrinkTask GetNewTask (){
        // See if we have any Queued Tasks
        for (int i = 0; i < cueData.Count; i++)
        {
            if(cueData[i].steps <= currentStep){
                DrinkTask task = cueData[i].task;
                cueData.RemoveAt(i);
                return task;
            }
        }

        DrinkTask selectedTask = new DrinkTask(data[Random.Range(0, data.Count)]);

        //Format names in
        Player p1 = SelectPlayer();
        Player p2 = SelectPlayer(selectedTask.message.Contains(C_ExtraPlayer));

        string playerMessage = selectedTask.message.Replace(C_Player, p1.name);
        playerMessage = playerMessage.Replace(C_ExtraPlayer, p2.name);
        selectedTask.message = playerMessage;

        // If the Selected task comes with a Queued Task make sure we get ready to show it later. Also set it up with the corrent names!
        if(selectedTask.HasCueTask()){
            string modMessage = selectedTask.qTask.task.message.Replace(C_Player, p1.name);
            modMessage = modMessage.Replace(C_ExtraPlayer, p2.name);

            DrinkTaskCue newC = new DrinkTaskCue(new DrinkTask(modMessage), selectedTask.qTask.steps + currentStep);
            cueData.Add(newC);
        }

        return selectedTask;
    }

    public Player SelectPlayer(bool addToHistory = true)
    {
        Player player = players[Random.Range(0, players.Count - 1)];
        bool freshPlayer = true;
        bool breakLoop = false;

        if(players.Count < 3 || !addToHistory ){
            return player;
        }

        // Select a player not from the recents
        while(breakLoop == false){
            player = players[Random.Range(0, players.Count - 1)];
            freshPlayer = true;

            if(recentPlayers.Count > 0){
                for (int i = 0; i < recentPlayers.Count; i++)
                {
                    if(recentPlayers[i].name == player.name){
                        freshPlayer = false;
                    }
                }
            } 

            if (freshPlayer) {
                Debug.Log("We found a fresh Player");
                breakLoop = true;
            }
        }

        recentPlayers.Add(player);

        // Remove recentPlayers from the list so they can be reused
        if(recentPlayers.Count > players.Count - (players.Count < 4 ? 2 : (players.Count / 2))){
            print("Removing Recent Player: " + recentPlayers[0].name);
            recentPlayers.RemoveAt(0);
        }

        return player;
    }

    public int GetNewTime ()
    {
        return (int)Random.Range(minTime, maxTime);    
    }

    public void SetGameState (GameState state)
    {
        gameState = state;

		if(state == GameState.Menu){
			currentStep = 0;
			curTime = 0;

			if(Speech.instance)
			{
				Speech.instance.audioSource.Stop();
				Speech.instance.audioSource.clip = null;
			}
			
			cueData.Clear();
			data.Clear();
		}

        UIManager.instance.UpdatePanel();
    }
}

[System.Serializable]
public class Player
{
    public string name;

    public Player(string newName) {
        name = newName;
    }
}

[System.Serializable]
public class DrinkTask
{
    [TextArea]
    public string message = "";
    public DrinkTaskCue qTask = null;
    public bool hasQTask;

    public DrinkTask(string newMessage){
        message = newMessage;
        qTask = null;
        hasQTask = false;
    }

    public DrinkTask(DrinkTask drinkTask)
    {
        message = drinkTask.message;
        qTask = drinkTask.qTask;
        hasQTask = drinkTask.hasQTask;
    }

    public DrinkTask(string newMessage, DrinkTaskCue taskCue){
        message = newMessage;
        qTask = taskCue;
        hasQTask = true;
    }

    public bool HasCueTask(){
        return hasQTask;
    }
}


[System.Serializable]
public class DrinkTaskCue {
    public DrinkTask task;
    public int steps;// { get; private set; }

    public DrinkTaskCue(DrinkTask drinkTask, int _steps) {
        task = drinkTask;
        steps = _steps;

    }

    public void SetSteps (int _steps){
        steps = _steps;
    }
}
