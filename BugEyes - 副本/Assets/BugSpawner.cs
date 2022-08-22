using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public int totalBugs = 1;
    int numBugs;
    public LayerMask mask;
    public GameObject bugPrefab;

    public delegate void UpdateScoreEvent(string score);
	public event UpdateScoreEvent OnUpdateScore;

    public event System.Action OnAllBugsDead;

    Dictionary<int,GameObject> bugs;
    void Start()
    {        
        NewGame();
    }

    // Update is called once per frame
    public void NewGame()
    {
        numBugs = totalBugs;
        bugs = new Dictionary<int, GameObject>(totalBugs);

        for(int i = 0; i < totalBugs; i++) {
            print("Bug " + i);
            Vector3 direction = Random.onUnitSphere;
            Vector3 rotation = Random.insideUnitCircle;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, 50, mask)){
                GameObject wall = hit.collider.gameObject;
                Vector3 spawnPos = hit.point + wall.transform.up * 0.1f;
                Quaternion spawnRot = Quaternion.LookRotation(rotation, wall.transform.up);

                GameObject newBug = Instantiate(bugPrefab,spawnPos,spawnRot);
                bugs.Add(i,newBug);

                BugMovement bug = newBug.GetComponent<BugMovement>();
                bug.id = i;
                bug.OnBugDeath += DeleteBug;

                Camera bugCam = newBug.GetComponentInChildren<Camera>();
                bugCam.rect = new Rect(1.0f*i/totalBugs, 0,1.0f/totalBugs,1);
            }
        }
        
        OnUpdateScore("0/" + totalBugs.ToString());
    }

    public void DeleteBug(int id) {
        GameObject deadBug = bugs[id];
        Destroy(deadBug);
        bugs.Remove(id);

        numBugs--;

        int count = 0;
        foreach(int key in bugs.Keys) {
            print(key);
            Camera bugCam = bugs[key].GetComponentInChildren<Camera>();
            bugCam.rect = new Rect(1.0f*count/numBugs, 0,1.0f/numBugs,1);

            count++;
        }

        OnUpdateScore((totalBugs - numBugs).ToString() + "/" + totalBugs.ToString());

        if (numBugs == 0) {
            OnAllBugsDead();
        }
    }
}
