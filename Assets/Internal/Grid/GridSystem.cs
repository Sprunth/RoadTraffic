using UnityEngine;
using System.Collections;
using Vectrosity;

public class GridSystem : MonoBehaviour
{
    private VectorLine gridLine;

    public Vector2 GridSize { get; private set; }

    public Material GridMaterial;

    // Use this for initialization
    void Start()
    {
        gridLine = new VectorLine("GridLine", new Vector3[0], GridMaterial, 1);
        GridSize = new Vector2(100, 100);
        MakeGrid();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {

    }

    void MakeGrid()
    {
        gridLine.Resize(((int)GridSize.x + (int)GridSize.y) * 2);

        var index = 0;
        const float height = 0.1f;
        for (var x = 0; x < GridSize.x; x++)
        {
            gridLine.points3[index++] = new Vector3(x, height, 0);
            gridLine.points3[index++] = new Vector3(x, height, GridSize.y);
        }
        for (var y = 0; y < GridSize.y; y++)
        {
            gridLine.points3[index++] = new Vector3(0, height, y);
            gridLine.points3[index++] = new Vector3(GridSize.x, height, y);
        }

        // offset so tiles are between lines rather than on intersections
        for (var i = 0; i < gridLine.points3.Count; i++)
        {
            gridLine.points3[i] = gridLine.points3[i] + new Vector3(0.5f, 0, 0.5f);
        }

        gridLine.Draw3D();
    }
}
