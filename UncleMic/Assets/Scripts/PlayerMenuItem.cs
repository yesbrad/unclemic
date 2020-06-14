using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMenuItem : MonoBehaviour 
{
    public Text playerNameText;
    internal string playerName;

    private Player thisPlayer;

    public void Init(Player player)
	{
        thisPlayer = player;
        gameObject.SetActive(true);
        playerNameText.text = player.name;
	}

    public void Delete ()
    {
        PlayerMenu.instance.DeletePlayer(thisPlayer);    
    }
}
