using System.Collections.Generic;
using HighlightingSystem;
using UnityEngine;
using System.Collections;

public class Node : WorldObject
{
    private RoadManager roadMgr;

    private Highlighter highlighter;

    private List<Road> connectedRoads;


    void Awake()
    {
        connectedRoads = new List<Road>();

        roadMgr = FindObjectOfType<RoadManager>();
        highlighter = gameObject.AddComponent<Highlighter>();

        gameObject.name = string.Format("Node{0}", gameObject.GetInstanceID());
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RegisterRoad(Road r)
    {
        if (connectedRoads.Contains(r))
            return;

        connectedRoads.Add(r);
    }

    public void FixupIntersection()
    {
        if (connectedRoads.Count >= 2)
        {
            connectedRoads.ForEach(road => road.SetPushbackFor(this, 1));
        }
    }

    public override void OnSelect()
    {
        roadMgr.PlaceNewRoad(this);
    }

    public override void OnDeselect()
    {
        
    }

    public override void OnHoverEnter()
    {
        highlighter.ConstantOn(Color.blue);
    }

    public override void OnHoverExit()
    {
        highlighter.ConstantOff();
    }
}
