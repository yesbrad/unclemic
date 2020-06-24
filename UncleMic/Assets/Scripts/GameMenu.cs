using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public static GameMenu instance;
    public Image backgroudImage;
	public GameObject debugTimer;

	void Start () {
        instance = this;
		debugTimer.SetActive(GameManager.instance.useDebugTimes);
	}

    public void UpdateBackground(DrinkTask _task)
	{
        Debug.Log(_task.severity - 2);
        backgroudImage.color = GameManager.instance.backgroundColors.colors[_task.severity - 2];
	}

	public void BackToMenu () 
    {
		GameManager.instance.SetGameState(GameState.Menu);
	}
}
