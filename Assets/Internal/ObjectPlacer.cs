using System;
using UnityEngine;
using System.Collections;

public class ObjectPlacer : MonoBehaviour
{
    /// <summary>
    /// Callback for when an item is placed. The returned values are:
    /// bool : whether the object was placed or placement canceled (by user (ie esc) or system (placing other object))
    /// GameObject : the Gameobject that was placed
    /// bool : True if placement was to an existing snapped object.
    /// </summary>
    private Action<bool, GameObject, bool> objectPlacedCallback;

    private GameObject placingObject;

    private WorldManager worldMgr;

    private ObjectPicker picker;

    private Type snapToType;
    private GameObject snappedToObj;

    // Use this for initialization
    void Start()
    {
        worldMgr = FindObjectOfType<WorldManager>();
        picker = FindObjectOfType<ObjectPicker>();
    }

    // Update is called once per frame
    void Update()
    {
        if (placingObject == null)
            return;

        if (Input.GetMouseButtonDown(0) && worldMgr.TrySetInputDirty())
        {
            var coll = placingObject.GetComponent<Collider>();
            if (coll != null)
            {
                coll.enabled = true;
            }

            GameObject oldObjRef;
            
            if (snappedToObj == null)
            {
                oldObjRef = placingObject;
            }
            else
            {
                oldObjRef = snappedToObj;
                // if the object we are placing is not getting sent back to the callback, destroy it
                Destroy(placingObject);
            }
            placingObject = null;
            objectPlacedCallback(true, oldObjRef, snappedToObj != null);
        }

        if (placingObject != null)
        {
            Vector3 placeToPut;

            if (snapToType != null && picker.Hit.collider)
            {
                var comp = picker.Hit.collider.gameObject.GetComponent(snapToType);
                
                if (comp != null)
                {
                    placeToPut = comp.gameObject.transform.position;
                    snappedToObj = comp.gameObject;
                    Debug.Log("Snapped");
                }
                else
                {
                    placeToPut = picker.Hit.point;
                    snappedToObj = null;
                }
            }
            else
            {
                placeToPut = picker.Hit.point;
                snappedToObj = null;
            }

            placingObject.transform.position = placeToPut;
        }
    }

    public void PlaceObject(GameObject obj, Action<bool, GameObject, bool> callback, Type snapToType = null)
    {
        this.snapToType = snapToType;

        if (placingObject != null)
        {
            Destroy(placingObject.gameObject);
            placingObject = null;
        }
        placingObject = obj;

        // disable the collider during placement
        var coll = obj.GetComponent<Collider>();
        if (coll != null)
        {
            coll.enabled = false;
        }
        objectPlacedCallback = callback;
    }
}
