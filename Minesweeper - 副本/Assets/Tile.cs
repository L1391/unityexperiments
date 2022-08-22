using UnityEngine;
using System;
using System.Collections;

public class Tile : MonoBehaviour
{
    public bool isMine = false;
    public int mineNumber = 0;
    public Color revealColor = Color.grey;

    public int x;
    public int y;

    public delegate void OnRevealDelegate(Tile tile);
    public OnRevealDelegate onReveal;

    public delegate void OnGameOverDelegate(Tile tile);
    public OnGameOverDelegate onGameOver;

    // Start is called before the first frame update
    void Start()
    {
        onReveal += RevealSelf;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            Reveal();
        }
    }

    public void Reveal() {
        onReveal(this);
    }

    void RevealSelf(Tile tile) {
        StartCoroutine(Bounce());
            if (isMine) {
                GetComponent<SpriteRenderer>().color = Color.red;
                onGameOver(tile);
            } else {
                GetComponent<SpriteRenderer>().color = revealColor;
           }
    }

    public float bounceTime = 0.5f;
    public float bounceScale = 0.5f;

    public IEnumerator Bounce() {

        float startTime = Time.time;
        Vector3 origScale = transform.localScale;

        while (transform.localScale != origScale + Vector3.one * bounceScale) {
            
            //transform.localScale += Vector3.one * 0.1f;

            transform.localScale = Vector3.Slerp(transform.localScale, origScale + Vector3.one * bounceScale, (Time.time-startTime)/bounceTime);

            yield return null;
        }

        while (transform.localScale != origScale) {
            //transform.localScale -= Vector3.one * 0.1f;

            transform.localScale = Vector3.Slerp(transform.localScale, origScale, (Time.time-startTime)/bounceTime);


            yield return null;
        }

    }
}
