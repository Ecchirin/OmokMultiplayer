using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    OmokManager _manager;

    Vector3 mousePos;
    GameObject _stonePos;

	public Stone PlayerStone;

    public GameObject stone;
    void Awake()
    {
        _manager = GameObject.Find("OmokManager").GetComponent<OmokManager>();
    }

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (_manager.isPlaying)
		{
			if (Input.GetMouseButton(0))
			{
				_stonePos = StonePos();
				if (_stonePos != null)
				{
					mousePos = _stonePos.transform.position;
				}
			}
			if (_manager.isPlayerTurn && Input.GetMouseButtonUp(0) && _stonePos != null && _stonePos.tag == "Board")
			{
				_manager.PutStone(StonePos(), PlayerStone);
			}
		}
	}


    GameObject StonePos() //돌을 놓을 위치를 가져옴
    {
        RaycastHit hit;
        GameObject target = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(true == (Physics.Raycast(ray.origin, ray.direction, out hit))) 
         {
            target = hit.collider.gameObject;
            if(target.gameObject.tag == "Board")
                return target;
        }
        return target;
    }
}
