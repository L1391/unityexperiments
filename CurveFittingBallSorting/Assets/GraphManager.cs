using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public GameObject pointObject;
    public GameObject lineObject;

    public int lifespan;

    List<Vector2> points = new List<Vector2>();

    public enum IterateMode {Click, Time};
    public IterateMode gameMode;

    public enum FitMode {YDistance, Orthogonal};
    public FitMode fitMode;

    public float decayChance;
    public float duplicateChance;

    float prevTime;

    // Start is called before the first frame update
    void Start()
    {
        prevTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameMode == IterateMode.Time) {
            if (Time.time - prevTime > lifespan) {
                iterateLife();
                prevTime = Time.time;
            }
        }

        if (Input.GetMouseButtonDown(0)) {
            if (gameMode == IterateMode.Click) {
                iterateLife();
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            addPoint(mousePos.x, mousePos.y);
        }


    }

    void addPoint(float x, float y) {
        GameObject newPoint = Instantiate(pointObject, new Vector2(x,y),new Quaternion(0,0,0,0),transform);

        points.Add(newPoint.transform.position);

        Line line = lineObject.GetComponent<Line>();

        if (fitMode == FitMode.YDistance) {
            line.leastSquaresRegress(points);
        } else if (fitMode == FitMode.Orthogonal) {
            line.orthogonalRegress(points);
        }

        if (points.Count > 1) line.updateLine();
    }

    void iterateLife() {
        if (Random.value < decayChance) {
            Point decayPoint = FindObjectsOfType<Point>()[Random.Range(0,points.Count)];

            Destroy(decayPoint.gameObject);
            points.Remove(decayPoint.gameObject.transform.position);
        }

        if (Random.value < duplicateChance) {
            Point duplicatePoint = FindObjectsOfType<Point>()[Random.Range(0,points.Count)];

            GameObject newPoint = Instantiate(pointObject, duplicatePoint.gameObject.transform.position, duplicatePoint.gameObject.transform.rotation,transform);
            points.Add(newPoint.transform.position);
        }   

        foreach(Point point in FindObjectsOfType<Point>()) {
            point.increment();

            if (point.age > lifespan && lifespan != 0) {
                Destroy(point.gameObject);
                points.Remove(point.gameObject.transform.position);
            }
        }


    }
}
