using System.Collections;
using UnityEngine;
using System;
public class BugMovement : MonoBehaviour
{
    public float maxSpeed = 1.2f;
    public float steerStrength = 0.05f;
    bool rotating = false;

    public int id;
    public delegate void BugDeathEvent(int id);
	public event BugDeathEvent OnBugDeath;

    // Update is called once per frame
    void Update()
    {
        if (!rotating)
        {
            float wander = (2 * steerStrength * Mathf.PerlinNoise(Time.time,0) - steerStrength);
            transform.RotateAround(transform.position,transform.up,wander);

            transform.position += transform.forward * maxSpeed * Time.deltaTime;
        }
    }

    IEnumerator rotateOntoWall(GameObject wall) {
        rotating = true;
        Vector3 wallNormal = wall.transform.up;
        Vector3 rotationAxis = Vector3.Cross(wallNormal,transform.up).normalized;
        float deltaAngle = -maxSpeed;
        float angle = Vector3.Angle(transform.up, wallNormal);
        while (angle != 0) {
            angle = Vector3.Angle(transform.up, wallNormal);
            if ( angle + deltaAngle < 0 )
            {
                deltaAngle = -angle;
            }

            transform.RotateAround(transform.position,rotationAxis,deltaAngle);
            yield return null;
        }

        //transform.up = wallNormal;

        rotating = false;
    }


    void OnTriggerEnter(Collider other) {
        if(other.gameObject.name == "Player" || other.gameObject.name == "Swatter") {
            print("killed a bug");

            OnBugDeath(id);

        } else if (!rotating) {
            print("starting rotation");
            StartCoroutine(rotateOntoWall(other.gameObject));
        }
    }
}
