using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Class is for keeping the gameobject through the course of the game
/// This will make the object not destructable unless destroy by others
/// Mainly will be used by server service
/// </summary>

public class Make_Singleton : MonoBehaviour {
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this);
	}
}
