using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextDisplay : MonoBehaviour {

    //private Text textBox = null;
    private TextMeshProUGUI textBox = null;

	// Use this for initialization
	void Start () {
        textBox = this.GetComponent<TextMeshProUGUI>();
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
