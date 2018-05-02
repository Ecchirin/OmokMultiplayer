using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script is used for changing scene
/// Nothing else should be here
/// </summary>

public class SceneChange : MonoBehaviour {

    //Change scene using name
    public void ChangeScene(string levelName)
    {
        Debug.Log("Scene change to: " + levelName);
        SceneManager.LoadScene(levelName);
    }

    //Application exit button
    public void QuitApplication()
    {
        Debug.Log("Quitting Application...");
        Application.Quit();
    }
}
