using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwatterMovement : MonoBehaviour
{
    public float hitSpeed = 10f;
    public float returnSpeed = 0.1f;

    bool swatting = false;
    IEnumerator currentRoutine;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            print(currentRoutine);
            if (!swatting) {
                swatting = true;
                currentRoutine = SwatDown();
                StartCoroutine(currentRoutine);
            }
        }
    }

    IEnumerator SwatDown() {
        print("going down");
        while(true) {
            transform.Rotate(Vector3.right*hitSpeed,Space.Self);
            yield return null;
        }
    }

    IEnumerator SwatUp() {
        print("going up");
        float deltaAngle = -returnSpeed;

        while(transform.localRotation != Quaternion.Euler(Vector3.right * -90)) {
            float angle = Vector3.Angle(transform.forward, Vector3.up);
            
            if ( angle + deltaAngle < 0 )
            {
                print("slowing down");
                deltaAngle = -angle;
            }

            transform.Rotate(Vector3.right*deltaAngle,Space.Self);
            
            yield return null;
        }

        print("done going up");
        swatting = false;
    }

    void OnTriggerEnter(Collider other) {
        if(currentRoutine != null) {
            StopCoroutine(currentRoutine);
            StartCoroutine(SwatUp());
        }
    }


}
