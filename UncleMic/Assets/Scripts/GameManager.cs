using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
    public TaskData data;
    public float minTime = 1;
    public float maxTime = 10;
    public List<Player> players = new List<Player>();

    public bool isDebug;
    public Task debugTask;

    [Header("UI")]
    public Text taskText;
    public Image backgroundImage;
    public Text debugTimeText;

    float curTime;

	private void Start()
	{
        curTime = 0;//GetNewTime();
	}

	private void Update()
	{
        curTime -= Time.deltaTime;

        if(curTime < 0)
        {
            NextTask();
            curTime = GetNewTime();
        }

        debugTimeText.text = "" + Mathf.Floor(curTime);
	}

    private void NextTask () 
    {
        Task task = GetNewTask();

        string playerMessage = task.message.Replace("PLAYER", SelectPlayer().name);
        playerMessage = playerMessage.Replace("EXTRA", SelectPlayer().name);

        taskText.text = playerMessage;
        backgroundImage.color = Random.ColorHSV();

        Speech.instance.Speak(playerMessage);
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
}

[System.Serializable]
public class Player 
{
    public string name;
}
