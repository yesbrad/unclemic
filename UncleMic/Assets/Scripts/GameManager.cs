using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
    public TaskData data;
    public float minTime = 1;
    public float maxTime = 10;

    [Header("UI")]
    public Text taskText;
    public Image backgroundImage;
    public Text debugTimeText;

    float curTime;

	private void Start()
	{
        curTime = GetNewTime();
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
        Task task = data.tasks[Random.Range(0, data.tasks.Length - 1)];

        taskText.text = task.message;
        backgroundImage.color = Random.ColorHSV();

        Speech.instance.Speak(task.message);
    }

    public int GetNewTime ()
    {
        return (int)Random.Range(minTime, maxTime);    
    }
}
