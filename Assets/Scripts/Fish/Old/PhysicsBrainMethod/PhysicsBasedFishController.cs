using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsBasedFishController : MonoBehaviour {

    public float defaultTailForce;
    public float defaultFinForce;

    public PhysicsBasedJet leftJet;
    public PhysicsBasedJet rightJet;
    public PhysicsBasedJet tailJet;
    public bool ManualControls;

    private Rigidbody2D rigid;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.drag = 1 + transform.localScale.x;
        rigid.angularDrag = 1 + transform.localScale.y;
        tailJet.SetSwimForce(defaultTailForce * rigid.mass);
        leftJet.SetSwimForce(defaultFinForce * rigid.mass);
        rightJet.SetSwimForce(defaultFinForce * rigid.mass);
    }

    // Update is called once per frame
    void Update () {

        Debug.Log(rigid.velocity);
        if (ManualControls)
        {
            if (Input.GetKey(KeyCode.W))
            {
                tailJet.Swim();
            }
            if (Input.GetKey(KeyCode.D))
            {
                rightJet.Swim();
            }
            if (Input.GetKey(KeyCode.A))
            {
                leftJet.Swim();
            }
        }

        else
        {
            //random movements
            if (Random.Range(0f,1f) > 0.5f)
            {
                tailJet.Swim();
            }
            if (Random.Range(0f, 1f) > 0.5f)
            {
                rightJet.Swim();
            }
            if (Random.Range(0f, 1f) > 0.5f)
            {
                leftJet.Swim();
            }
        }
	}
}
