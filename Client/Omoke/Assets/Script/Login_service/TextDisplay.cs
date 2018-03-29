using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDisplay : MonoBehaviour {

    private Text textBox = null;

	// Use this for initialization
	void Start () {
        textBox = this.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public IEnumerator DisplayText(string text, float time)
    {
        textBox.enabled = true;
        textBox.text = text;
        yield return new WaitForSeconds(time);
        textBox.text = "";
        textBox.enabled = false;
    }
}
