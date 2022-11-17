using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{   
    public int score = 0;

    public GameObject zoneObject;
    public GameObject ballObject;

    [System.Serializable]
    public class ZoneCoords
    {
        public List<float> zoneCoords;
    }
    public List<ZoneCoords> zonePos;

    public List<float> zoneSize;
    
    public enum SpawnMode {Defined, Infinite};
    public SpawnMode gameMode;

    public enum SpawnRateMode {Fixed, Random};
    public SpawnRateMode spawnRateMode;
    public float spawnRate = 1.0f;

    float prevTime;
    float spawnTime;

    public List<int> spawnSortOrder;
    int spawnIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        prevTime = Time.time;
        spawnTime = Random.Range(0.1f, spawnRate);

        for (int i = 0; i < zoneSize.Count; i++) {

            GameObject newZone = Instantiate(zoneObject, getBoundFromLeft(zonePos[i].zoneCoords[0]) + getBoundFromBottom(zonePos[i].zoneCoords[1]),new Quaternion(0,0,0,0),transform);
            newZone.transform.localScale = new Vector3(0.1f,zoneSize[i],1);
            newZone.transform.eulerAngles = Vector3.forward * zonePos[i].zoneCoords[2];
            newZone.GetComponent<SpriteRenderer>().color = new Color(i/zoneSize.Count,i/zoneSize.Count,i/zoneSize.Count, 1);
            newZone.GetComponent<Zone>().sortId = i;
        }

    }

    Vector2 getBoundFromLeft(float frac) {
        return Camera.main.ScreenToWorldPoint(new Vector2(Screen.width*frac,Screen.height/2));
    }

    Vector2 getBoundFromBottom(float frac) {
        return Camera.main.ScreenToWorldPoint(new Vector2(Screen.width/2, Screen.height*frac));
    }

    public void UpdateScore(int ballId, int ballSortId, int zoneSortId) {
        Debug.Log("update score");

        if (ballSortId == zoneSortId) {
            score++;
            Debug.Log("Correct sort");
        } else {
            Debug.Log("Incorrect sort");
        }

        foreach(Ball b in FindObjectsOfType<Ball>()) {
            if (b.id == ballId) {
                Destroy(b.gameObject);
                break;
            }
        }
    }

    void SpawnBall(int id, int sortId) {
        GameObject newBall = Instantiate(ballObject, new Vector2(0,3), new Quaternion(0,0,0,0), transform);
        Ball ball = newBall.GetComponent<Ball>();
        ball.id = id;
        ball.sortId = sortId;
        ball.OnZoneCollide += UpdateScore;

        newBall.GetComponent<SpriteRenderer>().color = new Color(sortId/zoneSize.Count,sortId/zoneSize.Count,sortId/zoneSize.Count, 1);

        spawnIndex++;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnRateMode == SpawnRateMode.Fixed) {
            if (Time.time - prevTime > spawnRate) {

                if (gameMode == SpawnMode.Defined && spawnIndex < spawnSortOrder.Count) {
                    SpawnBall(spawnIndex, spawnSortOrder[spawnIndex]);
                } else if (gameMode == SpawnMode.Infinite) {
                    SpawnBall(spawnIndex, Random.Range(0,zoneSize.Count+1));
                }

                prevTime = Time.time;  
            }

        } else if (spawnRateMode == SpawnRateMode.Random) {
            if (Time.time - prevTime > spawnTime) {

                if (gameMode == SpawnMode.Defined && spawnIndex < spawnSortOrder.Count) {
                    SpawnBall(spawnIndex, spawnSortOrder[spawnIndex]);
                } else if (gameMode == SpawnMode.Infinite) {
                    SpawnBall(spawnIndex, Random.Range(0,zoneSize.Count+1));
                }

                prevTime = Time.time; 
                spawnTime = Random.Range(0.1f, spawnRate); 
            }
        }

    }
}
