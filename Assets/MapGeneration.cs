using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGeneration : MonoBehaviour
{

    private Tilemap ground;
    private Tilemap collisions;

    void Awake()
    {
        ground = transform.GetChild(0).GetComponent<Tilemap>();
        collisions = transform.GetChild(1).GetComponent<Tilemap>();
    }

    // Start is called before the first frame update
    void Start()
    {
        RuleTile floor = Resources.Load<RuleTile>("Tiles/Ground/OutsideRuleTile");
        TileBase edges = Resources.Load<TileBase>("Tiles/Collisions/Black");

        rectangle(new Vector3Int(-5, -5, 0), 10, 10, floor, edges);
    }

    void rectangle(Vector3Int corner, int width, int height, TileBase floor, TileBase edges)
    {
        for (int x = corner.x; x < corner.x + width; x++)
        {
            for (int y = corner.y; y < corner.y + height; y++)
            {
                if (x == corner.x || x == corner.x+width-1 || y == corner.y || y == corner.y + height - 1)
                    collisions.SetTile(new Vector3Int(x, y, corner.z), edges);
                else
                    ground.SetTile(new Vector3Int(x, y, corner.z), floor);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
