using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorColliderFish : MonoBehaviour {

    public Vector2 movementVector;

    // Use this for initialization
    void Awake () {
        movementVector = Vector2.zero;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Move()
    {
        GetComponent<Rigidbody2D>().AddForce(movementVector);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        VectorCollider vc = collision.gameObject.GetComponent<VectorCollider>();
        if (vc != null)
        {
            movementVector = vc.vector;
        }
    }
}
