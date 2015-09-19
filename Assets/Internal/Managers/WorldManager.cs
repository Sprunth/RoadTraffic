using UnityEngine;
using System.Collections;

public class WorldManager : MonoBehaviour
{

    public bool InputDirty { get; private set; }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        InputDirty = false;
    }

    public bool TrySetInputDirty()
    {
        if (InputDirty)
            return false;

        InputDirty = true;
        return true;
    }
}
