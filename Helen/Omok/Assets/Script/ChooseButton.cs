using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChooseButton : MonoBehaviour {

	OmokManager _manager;
	public GameObject panel;
	
	void Awake()
	{
		_manager = GameObject.Find("OmokManager").GetComponent<OmokManager>();
	}
	public void FirstButton()
	{
		_manager._ai.AIStone = Stone.White;
		_manager._player.PlayerStone = Stone.Black;
		_manager.AIStone = _manager._ai.AIStone;
		_manager.PlayerStone = _manager._player.PlayerStone;
		_manager.isPlayerTurn = true;
		_manager.isAITurn = false;
		panel.SetActive(false);
		_manager.isPlaying = true;
	}

	public void SecondButton()
	{
		_manager._ai.AIStone = Stone.Black;
		_manager._player.PlayerStone = Stone.White;
		_manager.AIStone = _manager._ai.AIStone;
		_manager.PlayerStone = _manager._player.PlayerStone;
		_manager.isPlayerTurn = false;
		_manager.isAITurn = true;
		panel.SetActive(false);
		_manager.isPlaying = true;
	}

	public void RandomButton()
	{
		int rand = Random.Range(0, 1);
		if (rand == 0)
			SecondButton();
		else
			FirstButton();

	}
}
