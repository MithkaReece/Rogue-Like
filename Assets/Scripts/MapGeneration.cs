using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MapGeneration : MonoBehaviour
{

    private Tilemap ground;
    private Tilemap collisions;

    void Awake()
    {
        ground = transform.GetChild(0).GetComponent<Tilemap>();
        collisions = transform.GetChild(1).GetComponent<Tilemap>();
    }


    RuleTile floorTile;
    RuleTile doorwayTile;
    TileBase edgeTile;
    // Start is called before the first frame update
    void Start()
    {
        floorTile = Resources.Load<RuleTile>("Tiles/Ground/OutsideRuleTile");
        doorwayTile = Resources.Load<RuleTile>("Tiles/Ground/Flooring");
        edgeTile = Resources.Load<TileBase>("Tiles/Collisions/Black");

        //rectangle(new Vector3Int(-5, -5, 0), 10, 10, floor, edges);


        GenerateMap();
    }

    /**Alternate design
     * Essentially the same design but all checks for walls are against tilemap
     * 
     * 
     * Use the tilemap itself
     * Start first room with a frontier tile
     * Expand x amount of times
     * Turn outside tiles into wall
     * Turn inside tiles into floors
     * Pick random wall, turn into doorway
     * Pick side of doorway with no tile, place frontier tile
     * Expand x amount of times
     * 
     * 
     */


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

    public static int NumberOfExpands = 200;
    private int NumberOfRooms = 1;

    private List<Wall> finalWalls;
    private List<Wall> finalFloor;

    //Current outside wall (potential collisions)
    private WallManager outsideWalls;
    //Current frontier of room being generated
    private WallManager wallFrontier;

    void GenerateMap() {
        finalFloor = new List<Wall>();
        finalWalls = new List<Wall>();
        outsideWalls = new WallManager();
        wallFrontier = new WallManager();
        Debug.Log("ROOMS:" + NumberOfRooms.ToString());
        GenerateNextRoom(NumberOfRooms);
        GenerationToTiles();
    }

    void GenerationToTiles() {
        foreach (Wall wall in finalWalls)
        {
            collisions.SetTile(new Vector3Int(wall.Position.x, wall.Position.y, 0), edgeTile);
        }
        Debug.Log("Final Walls:" + finalWalls.Count.ToString());
        foreach (Wall floorWall in finalFloor)
        {
            ground.SetTile(new Vector3Int(floorWall.Position.x, floorWall.Position.y, 0), floorTile);
        }
        Debug.Log("Final Floor:" + finalFloor.Count.ToString());
    }

    

    void GenerateNextRoom(int NoRoomsToAdd) {
        if (NoRoomsToAdd <= 0){
            Debug.Log(outsideWalls.GetWalls());
            Debug.Log(outsideWalls.Removed);
            finalWalls = outsideWalls.GetWalls().Concat(outsideWalls.Removed).ToList();
            finalFloor = wallFrontier.Removed;
            return;
        }
        //Pick random outside wall

        //If first room
        //TODO: Change this to given coord not origin
        if (outsideWalls.Count == 0) {
            wallFrontier.Add(new Wall(new Vector2Int(0,0), 0, 0, 1, 1, 1, 1));
            Debug.Log(">FIRST ROOM");
        }
        else {
            wallFrontier.Add(ExpandRandomWall(outsideWalls, true));
            Debug.Log(">NEW ROOM");
        }
        
        //Now we have a wall picked, and it can't go backwards
        //Expand NumberOfExpands times
        for (int i = 0; i < NumberOfExpands; i++) {
            //Expand random wall
            Wall newWall = ExpandRandomWall(wallFrontier);
            if(newWall == null) {
                finalWalls = outsideWalls.GetWalls().Concat(outsideWalls.Removed).ToList();
                finalFloor = wallFrontier.Removed;
                return;
            }
            wallFrontier.Add(newWall);
 
            //Check borders in all 4 directions
            Vector2Int[] directions = new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };
            foreach(Vector2Int direction in directions) {
                Wall foundWall = outsideWalls.GetWall(newWall.Position + direction);
                if (foundWall != null) {
                    wallFrontier.RemoveDirection(newWall, direction);
                    outsideWalls.RemoveDirection(foundWall, -direction);
                }

                foundWall = wallFrontier.GetWall(newWall.Position + direction);
                if (foundWall != null)
                {
                    wallFrontier.RemoveDirection(newWall, direction);
                    wallFrontier.RemoveDirection(foundWall, -direction);
                }
            }
        }
        Debug.Log("+Add:" + wallFrontier.Count.ToString() + " Walls");
        //All expansion done, add frontier walls to outside walls and clear frontier walls
        outsideWalls.Add(wallFrontier);
        wallFrontier.Clear();

        GenerateNextRoom(NoRoomsToAdd - 1);
    }

    //Expand from a wall
    Wall ExpandRandomWall(WallManager Walls, bool removeWall = false) {
        Wall pickedWall = Walls.GetNextWall();
        if (pickedWall == null) {
            Debug.Log("Can't expand any walls");
            return null;
        }
        Vector2Int castedRndDirection = (Vector2Int)pickedWall.getRndDirection();
        Vector2Int newWallPosition = pickedWall.Position + castedRndDirection;
        if (removeWall) {
            Walls.Remove(pickedWall);
            ground.SetTile(new Vector3Int(pickedWall.Position.x, pickedWall.Position.y, 0), doorwayTile);
            Debug.Log("Added doorway:"+pickedWall.Position.ToString());
        }
        
        return new Wall(pickedWall.RoomStartPosition, newWallPosition.x, newWallPosition.y, -castedRndDirection);
    }
    
}


class WallManager {
    public List<Wall> Walls { get; }
    public List<Wall> Removed { get; }

    public WallManager() {
        Walls = new List<Wall>();
        Removed = new List<Wall>();
    }

    public int Count
    {
        get { return Walls.Count;  }
    }

    public List<Wall> GetWalls() {
        return Walls;
    }

    public Wall GetWall(Vector2Int position)
    {
        return Walls.Find(wall => wall.Position == position);
    }

    public bool Add(Wall wall) {
        if (!wall.HasAvailableDirections())
        {
            Removed.Add(wall);
            return false;
        }
        Walls.Add(wall);
        return true;
    }

    public bool Add(WallManager walls) {
        Walls.AddRange(walls.GetWalls());
        return true;
    }


    public bool RemoveDirection(Wall wall, Vector2Int direction) {
        return RemoveDirection(wall.Position, direction);    
    }
    //Removes direction, may be displayed to removed list
    //Returns false if wall gets removed
    public bool RemoveDirection(Vector2Int wallPosition, Vector2Int direction) {
        Wall foundWall = Walls.Find(wall => wall.Position == wallPosition);
        if(foundWall != null) {
            foundWall.RemoveDirection(direction);
            Walls.Remove(foundWall);
            return Add(foundWall);
        }
        return true;
    }

    //Manual Removal
    public void Remove(Wall removingWall) {
        Wall foundWall = Walls.Find(wall => wall.Position == removingWall.Position);
        if (foundWall != null)
        {
            Debug.Log("Removed");
            Walls.Remove(foundWall);
        }
    }

    //Pick a random wall from a list of the walls with the smallest available directions
    public Wall GetNextWall() {
        Wall pickedWall = ChooseWall();
        if(pickedWall == null) {
            return null;
        }
        if (!pickedWall.HasAvailableDirections()) {
            Debug.Log("Random wall has no directions - Shouldn't happen");
            return null;
        }
        return pickedWall;
    }

    //Choose wall based on weighted probability
    //Higher probability the closer the wall is to the room start
    //Should make the room more space like
    private Wall ChooseWall() {
        if(Walls.Count <= 0) {
            Debug.Log("Wall empty");
            return null;
        }
        List<double> weightedProbabilities = new List<double>();
        foreach(Wall wall in Walls) {
            weightedProbabilities.Add(wall.ChooseChance);
        }
        double randomValue = new System.Random().NextDouble() * weightedProbabilities.Sum();

        double cumulativeProbability = 0.0;
        for (int i=0; i<weightedProbabilities.Count; i++) {
            cumulativeProbability += weightedProbabilities[i];
            if (randomValue <= cumulativeProbability)
                return Walls[i];
        }
        return Walls[^1];
    }

    public void Clear() {
        Walls.Clear();
    }

}

class Wall {
    public Vector2Int RoomStartPosition;
    public Vector2Int Position;
    public Direction outside; //North, east, south, west (CHANGE back to private)
    public double ChooseChance {
        get {
            return 0.1*MapGeneration.NumberOfExpands*Mathf.Pow(Vector2Int.Distance(RoomStartPosition, Position), -2);
        }
    }

    public Wall(Vector2Int roomStartPosition, int x, int y, int north, int east, int south, int west) {
        RoomStartPosition = roomStartPosition;
        Position = new Vector2Int(x, y);
        outside = new Direction(north, east, south, west);
    }

    public Wall(Vector2Int roomStartPosition, int x, int y) {
        RoomStartPosition = roomStartPosition;
        Position = new Vector2Int(x, y);
        outside = new Direction(1, 1, 1, 1);
    }

    public Wall(Vector2Int roomStartPosition, int x, int y, Vector2Int blockedDirection) {
        RoomStartPosition = roomStartPosition;
        Position = new Vector2Int(x, y);
        if (blockedDirection.y == 1)
            outside = new Direction(0, 1, 1, 1);
        else if (blockedDirection.x == 1)
            outside = new Direction(1, 0, 1, 1);
        else if (blockedDirection.y == -1)
            outside = new Direction(1, 1, 0, 1);
        else if (blockedDirection.x == -1)
            outside = new Direction(1, 1, 1, 0);
        else
            Debug.Log("Invalid direction given to wall");
    }


    public Vector2Int? getRndDirection() {
        return outside.getRndDirection();
    }

    public bool HasAvailableDirections() {
        return outside.HasAvailableDirections();
    }

    public void RemoveDirection(Vector2Int direction) {
        outside.RemoveDirection(direction);
    }

    public int NumberOfDirections() {
        return outside.NumberOfDirections();
    }
}

class Direction {

    public List<Vector2Int> AvailableDirections;

    public Direction(int north, int east, int south, int west) {
        AvailableDirections = new List<Vector2Int>();
        if (north == 1)
            AvailableDirections.Add(new Vector2Int(0, 1));
        if (east == 1)
            AvailableDirections.Add(new Vector2Int(1, 0));
        if (south == 1)
            AvailableDirections.Add(new Vector2Int(0, -1));
        if (west == 1)
            AvailableDirections.Add(new Vector2Int(-1, 0));
    }

    public int NumberOfDirections() {
        return AvailableDirections.Count;
    }

    public bool HasAvailableDirections() {
        return AvailableDirections.Count > 0;
    }


    public Vector2Int? getRndDirection() {
        if (AvailableDirections.Count <= 0)
            return null;
        System.Random rnd = new System.Random();
        int randomIndex = rnd.Next(0, AvailableDirections.Count);
        return AvailableDirections[randomIndex];
    }
    public void RemoveDirection(Vector2Int direction) {
        AvailableDirections.Remove(direction);
    }

}