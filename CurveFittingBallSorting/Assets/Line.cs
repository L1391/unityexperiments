using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{   
    public float speed = 5;

    float oldB = 0;
    float oldM = 0;
    float newB = 0;
    float newM = 0;

    float ratio = 1;

    float t = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (t <= 1) {
            float M = Mathf.Lerp(oldM, newM, t);
            float B = Mathf.Lerp(oldB, newB, t);

            transform.position = new Vector3(transform.position.x, B, transform.position.z);

            float angle = Mathf.Atan(M)*180/Mathf.PI;

            transform.eulerAngles = Vector3.forward * angle;

            t += Time.deltaTime*speed;

        } 
    }

    public void updateLine() {
        t= 0;
    }

    public void leastSquaresRegress(List<Vector2> points) {
        oldM = newM;
        oldB = newB;

        float xsum = 0;
        float ysum = 0;
        float xsquaredsum = 0;
        float xysum = 0;

        int n = points.Count;

        foreach(Vector2 point in points) {
            xsum += point.x;
            ysum += point.y;
            xsquaredsum += point.x * point.x;
            xysum += point.x * point.y;
        }

        newB = ((ysum*xsquaredsum)-(xsum*xysum))/(n*xsquaredsum-xsum*xsum);
        newM = (n*xysum-xsum*ysum)/(n*xsquaredsum-xsum*xsum);

    }

    public void orthogonalRegress(List<Vector2> points) {
        oldM = newM;
        oldB = newB;

        float xsum = 0;
        float ysum = 0;

        foreach(Vector2 point in points) {
            xsum += point.x;
            ysum += point.y;
        }

        float xmean = xsum/points.Count;
        float ymean = ysum/points.Count;

        float mxx = 0;
        float myy = 0;
        float mxy = 0;

        foreach(Vector2 point in points) {
            mxx += (point.x-xmean)*(point.x-xmean);
            myy += (point.y-ymean)*(point.y-ymean);
            mxy += (point.x-xmean)*(point.y-ymean);
        }

        mxx /= points.Count - 1;
        myy /= points.Count - 1;
        mxy /= points.Count - 1;

        if (mxy == 0) {
            newM = 0;
        } else {
            newM = (myy-ratio*mxx + Mathf.Sqrt((myy-ratio*mxx)*(myy-ratio*mxx)+4*ratio*mxy*mxy))/(2*mxy);
        }
        
        newB = ymean - newM*xmean;

    }
}
