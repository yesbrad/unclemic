using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMenu : MonoBehaviour 
{
    public static PlayerMenu instance;
    public GameObject playerMenuPrefab;
    public Transform playerNameContainer;
    public InputField nameInput;

	public Image playIcon;
	public Image loadingIcon;
	public Text errorText;

	private List<PlayerMenuItem> items = new List<PlayerMenuItem>();

	private void Start()
	{
        instance = this;
        RefreshUI();
	}

	public void RefreshUI ()
    {
        for (int x = 0; x < items.Count; x++)
        {
            Destroy(items[x].gameObject);
        }

        items.Clear();

        for (int i = 0; i < GameManager.instance.players.Count; i++)
        {
            PlayerMenuItem item = Instantiate(playerMenuPrefab).GetComponent<PlayerMenuItem>();
			item.transform.SetParent(playerNameContainer);
            item.transform.SetSiblingIndex(0);
            item.Init(GameManager.instance.players[i]);
            items.Add(item);
        }
    }

    public void AddPlayer () 
    {
        GameManager.instance.players.Add(new Player(nameInput.text));
        nameInput.text = "";
        RefreshUI();
    }

    public void DeletePlayer (Player player)
    {
        Player deletePlayer = null;

        for (int i = 0; i < GameManager.instance.players.Count; i++)
        {
            if (GameManager.instance.players[i].name == player.name)
            {
                deletePlayer = GameManager.instance.players[i];
            }
        }

        if (deletePlayer != null)
        {
            GameManager.instance.players.Remove(deletePlayer);
			RefreshUI();             
        }
    }

    public void Play()
    {
		playIcon.gameObject.SetActive(false);
		loadingIcon.gameObject.SetActive(true);
		errorText.gameObject.SetActive(false);

		GameManager.instance.BeginGame((success) => {
			playIcon.gameObject.SetActive(true);
			loadingIcon.gameObject.SetActive(false);
			errorText.gameObject.SetActive(!success);
		});
	}
}
