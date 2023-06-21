using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGeneration : MonoBehaviour
{

    private Tilemap tilemap;

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    // Start is called before the first frame update
    void Start()
    {
        RuleTile outside = Resources.Load<RuleTile>("Tiles/TilePalettes/Prison/OutsideRuleTile");
        Setup(new Vector3Int(0, 0, 0), 10, 10, outside);
    }

    void Setup(Vector3Int corner, int width, int height, TileBase tile)
    {
        for (int x = corner.x; x < corner.x + width; x++)
        {
            for (int y = corner.y; y < corner.y + height; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, corner.z);
                tilemap.SetTile(tilePosition, tile);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
