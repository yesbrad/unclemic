using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour 
{
    public static UIManager instance;
    public GameObject playerPanel;
    public GameObject ingamePanel;

	private void Start()
	{
        instance = this;
	}

	public void UpdatePanel ()
    {
        ingamePanel.SetActive(GameManager.instance.gameState == GameState.Game);
        playerPanel.SetActive(GameManager.instance.gameState == GameState.Menu);
    }
}
