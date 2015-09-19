using UnityEngine;
using System.Collections;

public class ObjectPicker : MonoBehaviour
{
    private Camera cam;
    public RaycastHit Hit { get; private set; }
    public WorldObject SelectedWorldObject;

    private WorldManager worldMgr;

    // Use this for initialization
    void Start()
    {
        cam = GetComponent<Camera>();
        worldMgr = FindObjectOfType<WorldManager>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f) && hit.collider != null)
        {
            #region handle onhover

            var worldObj = hit.collider.gameObject.GetComponent<WorldObject>();
            if (worldObj != null && (worldObj != SelectedWorldObject))
            {
                WorldObject oldHoverObj = null;
                if (Hit.collider)
                    oldHoverObj = Hit.collider.gameObject.GetComponent<WorldObject>();

                if (!worldObj.Equals(oldHoverObj))
                {
                    // call hover enter on new obj
                    worldObj.OnHoverEnter();

                    // call hover exit on old obj
                    if (oldHoverObj)
                    {
                        oldHoverObj.OnHoverExit();
                    }
                }
            }


            #endregion
        }

        #region handle onclick

        if (Input.GetMouseButtonDown(0) && worldMgr.TrySetInputDirty() && hit.collider)
        {
            var obj = hit.collider.gameObject.GetComponent<WorldObject>();
            if (obj != null)
            {
                if (SelectedWorldObject != null && SelectedWorldObject != obj)
                {
                    SelectedWorldObject.OnDeselect();
                }
                SelectedWorldObject = obj;

                obj.OnSelect();
            }
        }

        #endregion

        Hit = hit;
    }
}
