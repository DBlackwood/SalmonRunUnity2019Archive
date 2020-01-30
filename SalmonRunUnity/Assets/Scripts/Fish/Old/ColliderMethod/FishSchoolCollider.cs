using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FishSchoolCollider : MonoBehaviour {
    public GameObject fishPrefab;
    public int schoolSize;

    private GameObject[] fish;

	// Use this for initialization
	void Start () {
        fish = new GameObject[schoolSize];
        for (int i = 0; i < fish.Length; i++)
        {
            fish[i] = Instantiate(fishPrefab, Vector3.zero, Quaternion.identity);
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        foreach (GameObject f in fish)
        {
            f.GetComponent<VectorColliderFish>().Move();
        }
	}
}
