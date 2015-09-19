using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using Vectrosity;

public class Road : MonoBehaviour
{
    private Node node1, node2;
    /// <summary>
    /// The two points that the road starts/ends at after pushback
    /// </summary>
    private Vector3 end1, end2;
    public float pushback1, pushback2;

    // bound points for road "rectangle"
    private Dictionary<Node, Vector3Pair> nodeCorners;
    private Dictionary<Vector3Pair, GeneralHelpers.Tuple<GameObject, GameObject>> cornerPoints;


    void Awake()
    {
        nodeCorners = new Dictionary<Node, Vector3Pair>();
        cornerPoints = new Dictionary<Vector3Pair, GeneralHelpers.Tuple<GameObject, GameObject>>();
    }

    void Start()
    {
        gameObject.name = String.Format("Road{0}", gameObject.GetInstanceID());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetNodes(Node n1, Node n2)
    {
        n1.RegisterRoad(this);
        n2.RegisterRoad(this);

        node1 = n1;
        node2 = n2;
        end1 = n1.transform.position;
        end2 = n2.transform.position;

        UpdateBoundPoints();
        transform.position = n1.transform.position;
    }

    void UpdateBoundPoints()
    {
        // pushback
        Pushback(node1, pushback1);
        Pushback(node2, pushback2);

        //var p1 = node1.transform.position;
        //var p2 = node2.transform.position;

        var p1 = end1;
        var p2 = end2;

        var diff = p2 - p1;
        var angle = (float) Math.Atan2(diff.z, diff.x);
        const float rightAngle = (float) Math.PI/2;
        var width = 1f;
        var elevation = 0.02f;

        var p1_1 = new Vector3(width * (float)Math.Cos(angle - rightAngle), elevation,
            width * (float)Math.Sin(angle - rightAngle));
        var p1_2 = new Vector3(width * (float)Math.Cos(angle + rightAngle), elevation,
            width * (float)Math.Sin(angle + rightAngle));
        var p2_1 = new Vector3(width * (float)Math.Cos(angle - rightAngle) + diff.x, elevation,
            diff.z + width * (float)Math.Sin(angle - rightAngle));
        var p2_2 = new Vector3(width * (float)Math.Cos(angle + rightAngle) + diff.x, elevation,
            diff.z + width * (float)Math.Sin(angle + rightAngle));

        nodeCorners.Clear();
        Assert.AreNotEqual(node1, node2);
        var pair1 = new Vector3Pair(p1_1, p1_2);
        var pair2 = new Vector3Pair(p2_1, p2_2);
        Assert.AreNotEqual(pair1, pair2);
        nodeCorners.Add(node1, pair1);
        nodeCorners.Add(node2, pair2);
        Assert.AreNotEqual(nodeCorners[node1], nodeCorners[node2]);

        var randColor = new Color(UnityEngine.Random.Range(0.1f, 0.9f),
            UnityEngine.Random.Range(0.1f, 0.9f),
            UnityEngine.Random.Range(0.1f, 0.9f));

        Func<Vector3, Color, GameObject> CreateCornerPoint = (pos, col) =>
        {
            var point = GameObject.CreatePrimitive(PrimitiveType.Cube);
            point.transform.localScale = new Vector3(0.3f, 0.5f, 0.3f);
            point.transform.position = pos;
            point.transform.parent = transform;
            point.GetComponent<Renderer>().material.color = col;
            return point;
        };

        foreach (var cornerPoint in cornerPoints)
        {
            Destroy(cornerPoint.Value.First);
            Destroy(cornerPoint.Value.Second);
        }
        cornerPoints.Clear();
        cornerPoints.Add(nodeCorners[node1], new GeneralHelpers.Tuple<GameObject, GameObject>(CreateCornerPoint(p1_1, randColor), CreateCornerPoint(p1_2, randColor)));
        cornerPoints.Add(nodeCorners[node2], new GeneralHelpers.Tuple<GameObject, GameObject>(CreateCornerPoint(p2_1, randColor), CreateCornerPoint(p2_2, randColor)));
    }

    void Pushback(Node node, float distance)
    {
        var startNode = node == node1 ? node1 : node2;
        var endNode = node == node1 ? node2 : node1;

        var startingPoint = startNode.transform.position;
        var endingPoint = endNode.transform.position;

        var trueStartingPoint = GetPointOnLineDistFromStart(startingPoint, endingPoint, distance);

        if (startNode == node1)
        {
            end1 = trueStartingPoint;
            pushback1 = distance;
        }
        else
        {
            end2 = trueStartingPoint;
            pushback2 = distance;
        }
    }

    /// <summary>
    /// Returns a point that is <paramref name="dist"/> away from <paramref name="start"/> on the line towards <paramref name="end"/>
    /// </summary>
    /// <returns></returns>
    Vector3 GetPointOnLineDistFromStart(Vector3 start, Vector3 end, float dist)
    {
        var angle = Math.Atan2(end.z - start.z, end.x - start.x);
        var offset = new Vector3(dist*(float)Math.Cos(angle), 0, dist*(float)Math.Sin(angle));
        return start + offset;
    }

    public void SetPushbackFor(Node n, float dist)
    {
        if (n == node1)
            pushback1 = dist;
        else
            pushback2 = dist;

        SetNodes(node1, node2);
    }

}
