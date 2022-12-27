using System.Collections;
using System;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public int rollNumber;

    public Action onRollDone;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(Roll());
        }
    }

    public IEnumerator Roll() {
        SpriteRenderer diceMap = GetComponent<SpriteRenderer>();
        float faceWidth = diceMap.bounds.size.x/3f;

        for(int i =0; i < UnityEngine.Random.Range(2,10); i++) {
            transform.position = new Vector2(-UnityEngine.Random.Range(0,3)*faceWidth + faceWidth,-UnityEngine.Random.Range(0,2)*faceWidth + 1.5f*faceWidth);
            yield return new WaitForSeconds(0.02f);
        }

        rollNumber = UnityEngine.Random.Range(1,7);
        transform.position = new Vector2(-((rollNumber-1)%3) * faceWidth + faceWidth, Mathf.Floor(rollNumber/4f)*faceWidth+1.5f*faceWidth);

        //yield return new WaitForSeconds(1);
        onRollDone();
    }
}
