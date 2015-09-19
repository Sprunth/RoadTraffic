using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class RoadManager : MonoBehaviour
{
    private ObjectPlacer placer;

    enum RoadPlacingStatus { PlacingStartingNode, PlacingEndingNode, NotPlacing }

    private RoadPlacingStatus roadPlacingStatus;

    private GameObject startNodeTemp;

    private List<Node> nodesWithNewRoads;

    // Use this for initialization
    void Start()
    {
        placer = FindObjectOfType<ObjectPlacer>();

        roadPlacingStatus = RoadPlacingStatus.NotPlacing;

        nodesWithNewRoads = new List<Node>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlaceNewRoad();
        }
    }

    public void PlaceNewRoad(Node startingNode = null)
    {
        var placeholder = GenerateNodePlaceholder();

        if (startingNode == null)
        {
            roadPlacingStatus = RoadPlacingStatus.PlacingStartingNode;
            startNodeTemp = null;
        }
        else
        {
            roadPlacingStatus = RoadPlacingStatus.PlacingEndingNode;
            startNodeTemp = startingNode.gameObject;
        }

        placer.PlaceObject(placeholder, PlacedNodeCallback, typeof(Node));
    }

    GameObject GenerateNodePlaceholder()
    {
        var placeholder = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // do more?
        return placeholder;
    }

    GameObject CreateNode(Vector3 pos)
    {
        var nodeObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        nodeObj.transform.position = pos;
        var nodeCom = nodeObj.AddComponent<Node>();
        return nodeObj;
    }

    void PlacedNodeCallback(bool success, GameObject obj, bool placedWhileSnapped)
    {
        if (!success)
            return;

        switch (roadPlacingStatus)
        {
            case RoadPlacingStatus.PlacingStartingNode:
            {
                startNodeTemp = obj;

                var placeholder = GenerateNodePlaceholder();
                roadPlacingStatus = RoadPlacingStatus.PlacingEndingNode;
                placer.PlaceObject(placeholder, PlacedNodeCallback, typeof(Node));

                break;
            }
            case RoadPlacingStatus.PlacingEndingNode:
            {
                // when using existing node to start or snapped to ending node, skip node creation
                // if either node contains a node component, then we don't need to create a new one

                // swap startingNodeTemp/obj with real node
                GameObject n1, n2;

                if (startNodeTemp.GetComponent<Node>() == null)
                {
                    n1 = CreateNode(startNodeTemp.transform.position);
                    Destroy(startNodeTemp);
                }
                else
                {
                    n1 = startNodeTemp;
                }

                if (obj.GetComponent<Node>() == null)
                {
                    n2 = CreateNode(obj.transform.position);
                    Destroy(obj);
                }
                else
                {
                    n2 = obj;
                }

                startNodeTemp = null;

                // todo: create road
                var roadObj = new GameObject();
                var roadCom = roadObj.AddComponent<Road>();
                roadCom.SetNodes(n1.GetComponent<Node>(), n2.GetComponent<Node>());

                nodesWithNewRoads.Add(n1.GetComponent<Node>());
                nodesWithNewRoads.Add(n2.GetComponent<Node>());

                roadPlacingStatus = RoadPlacingStatus.NotPlacing;
                break;
            }
            case RoadPlacingStatus.NotPlacing:
            {
                Debug.LogError("Road Callback called while not placing any node");
                break;
            }
        }

        CleanUpNodesWithNewRoads();
    }

    void CleanUpNodesWithNewRoads()
    {
        nodesWithNewRoads.ForEach(node => node.FixupIntersection());
    }
}
