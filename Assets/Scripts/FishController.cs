using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour {

    public FishMovement LeftJet;
    public FishMovement RightJet;
    public FishMovement TailJet;
    public bool ManualControls;
	
	// Update is called once per frame
	void Update () {
        if (ManualControls)
        {
            if (Input.GetKey(KeyCode.W))
            {
                TailJet.Swim();
            }
            else if (Input.GetKey(KeyCode.D))
            {
                RightJet.Swim();
            }
            else if (Input.GetKey(KeyCode.A))
            {
                LeftJet.Swim();
            }
        }

        else
        {
            //random movements
            if (Random.Range(0f,1f) > 0.5f)
            {
                TailJet.Swim();
            }
            if (Random.Range(0f, 1f) > 0.5f)
            {
                RightJet.Swim();
            }
            if (Random.Range(0f, 1f) > 0.5f)
            {
                LeftJet.Swim();
            }
        }
	}
}
