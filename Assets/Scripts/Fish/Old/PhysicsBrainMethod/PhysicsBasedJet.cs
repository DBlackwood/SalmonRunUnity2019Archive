using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsBasedJet : MonoBehaviour {

    private float swimForce;
    private Rigidbody2D rigid;

	// Use this for initialization
	void Start () {
        rigid = GetComponentInParent<Rigidbody2D>();
	}

    public void Swim()
    {   
        rigid.AddForceAtPosition(transform.up * swimForce, transform.position);
    }

    public void SetSwimForce(float force)
    {
        this.swimForce = force;
    }
}
