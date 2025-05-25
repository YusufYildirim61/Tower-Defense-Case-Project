using UnityEngine;

public class Waypoints : MonoBehaviour
{
    [SerializeField] static Transform[] points;
    [SerializeField] GameObject waypoints;

    public Transform[] GetPoints()
    {
        return points;
    }
    void Awake()
    {
        points = new Transform[waypoints.transform.childCount];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = waypoints.transform.GetChild(i);
        }
    }

    
}
