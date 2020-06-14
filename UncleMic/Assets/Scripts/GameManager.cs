using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleSheetsToUnity;

public enum GameState {
    Menu,
    Game,
}

public class GameManager : MonoBehaviour 
{
    public static GameManager instance;

    //public TaskData data;
    public List<DrinkTask> data = new List<DrinkTask>();
    public float minTime = 1;
    public float maxTime = 10;
    public int speechDelayTime = 2;
    public AudioClip taskClip;
    public List<Player> players = new List<Player>();

    public bool isDebug;
    public DrinkTask debugTask;

    [Header("UI")]
    public Text taskText;
    public Image backgroundImage;
    public Text debugTimeText;

    private AudioSource audioSource;

    float curTime;
    internal GameState gameState = GameState.Menu;

	private void Start()
	{
        instance = this;
        curTime = 0;
        SetGameState(GameState.Menu);
        audioSource = GetComponent<AudioSource>();

        FetchTaskData();
	}

    public void FetchTaskData()
    {
        SpreadsheetManager.Read(new GSTU_Search("1rdp9VNO-s_Uxn3YFYZoLaVCRTudt19UQi4FwEs8BL-U", "Tasks"), (GstuSpreadSheet sheet) =>
        {
            foreach (var val in sheet.columns["A"])
            {
                data.Add(new DrinkTask(val.value));
            }
        });
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

        string playerMessage = task.message.Replace("PLAYER", SelectPlayer().name);
        playerMessage = playerMessage.Replace("EXTRA", SelectPlayer().name);

        taskText.text = playerMessage;
        backgroundImage.color = Random.ColorHSV();
        StartCoroutine(DelaySpeech(playerMessage));
    }

    IEnumerator DelaySpeech (string playerMessage) {
        yield return new WaitForSeconds(speechDelayTime);
        Speech.instance.Speak(playerMessage);
        yield return null;
    }

    public DrinkTask GetNewTask (){
        return isDebug ? debugTask : data[Random.Range(0, data.Count - 1)];
    }

    public Player SelectPlayer () {
        return players[Random.Range(0, players.Count - 1)];
    }

    public int GetNewTime ()
    {
        return (int)Random.Range(minTime, maxTime);    
    }

    public void SetGameState (GameState state)
    {
        gameState = state;
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
