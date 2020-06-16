using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
	public void BackToMenu () {
		GameManager.instance.SetGameState(GameState.Menu);
	}
}
