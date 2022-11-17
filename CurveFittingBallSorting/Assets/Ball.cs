using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public int id;
    public int sortId;

    public delegate void ZoneCollideEvent(int id, int ballSortId, int zoneSortId);
	    public event ZoneCollideEvent OnZoneCollide;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other) {
        Zone collidedZone = other.gameObject.GetComponent<Zone>();

        OnZoneCollide(id, sortId, collidedZone.sortId);
    }
}
