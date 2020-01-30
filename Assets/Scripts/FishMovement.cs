using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour {

    private Rigidbody2D rigid;

	// Use this for initialization
	void Start () {
        rigid = GetComponentInParent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Swim()
    {
        rigid.AddForceAtPosition(transform.up, transform.position);
    }
}
