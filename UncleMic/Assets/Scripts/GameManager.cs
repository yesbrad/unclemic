using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState {
    Menu,
    Game,
}

public class GameManager : MonoBehaviour 
{
    public static GameManager instance;

    public TaskData data;
    public float minTime = 1;
    public float maxTime = 10;
    public int speechDelayTime = 2;
    public AudioClip taskClip;
    public List<Player> players = new List<Player>();

    public bool isDebug;
    public Task debugTask;

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
        Task task = GetNewTask();

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

    public Task GetNewTask (){
        return isDebug ? debugTask : data.tasks[Random.Range(0, data.tasks.Length - 1)];
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
