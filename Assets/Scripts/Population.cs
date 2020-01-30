using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour {

    public GameObject MemberPrefab;
    public int PopSize;
    private int Generation;
    private List<GameObject> Members;
    public float GenerationLength;
    
	// Use this for initialization
	void Start () {
        NewGeneration();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void NewGeneration()
    {
        List<GameObject> OldMembers = Members;
        Members = new List<GameObject>();

        for (int i = 0; i < PopSize; i++)
        {
            Members.Add(Instantiate(MemberPrefab));
        }

        if (OldMembers != null)
        {
            foreach (GameObject Member in OldMembers)
            {
                Destroy(Member);
            }
        }

        StartCoroutine(GenerationCoroutine());
    }

    private IEnumerator GenerationCoroutine()
    {
        yield return new WaitForSeconds(GenerationLength);
        NewGeneration();
    }
}
