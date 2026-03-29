using System.Collections.Generic;
using UnityEngine;

public class BSPDungeonGenerator : MonoBehaviour
{
    public enum TileType { Empty, RoomFloor, CorridorFloor, RoomWall, CorridorWall, Lift }

    [Header("Dungeon Settings")]
    public int gridWidth = 50;
    public int gridHeight = 50;
    public int numberOfFloors = 3;
    public float floorHeightOffset = 6f;

    [Header("BSP Settings")]
    public int minPartitionSize = 10;
    public int minRoomSize = 5;

    [Header("Prefabs")]
    public GameObject roomFloorPrefab;
    public GameObject roomWallPrefab;
    public GameObject corridorFloorPrefab;
    public GameObject corridorWallPrefab;
    public GameObject liftPrefab;

    private TileType[,,] grid;
    private Vector2Int[] liftPositions;

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        grid = new TileType[numberOfFloors, gridWidth, gridHeight];

        liftPositions = new Vector2Int[numberOfFloors - 1];

        for (int f = 0; f < numberOfFloors; f++)
        {
            List<Leaf> leaves = new List<Leaf>();
            Leaf root = new Leaf(0, 0, gridWidth, gridHeight, minPartitionSize, minRoomSize);
            leaves.Add(root);

            bool didSplit = true;
            while (didSplit)
            {
                didSplit = false;
                List<Leaf> newLeaves = new List<Leaf>();
                foreach (Leaf l in leaves)
                {
                    if (l.leftChild == null && l.rightChild == null)
                    {
                        if (l.width > minPartitionSize * 2 || l.height > minPartitionSize * 2 || Random.value > 0.1f)
                        {
                            if (l.Split())
                            {
                                newLeaves.Add(l.leftChild);
                                newLeaves.Add(l.rightChild);
                                didSplit = true;
                            }
                            else newLeaves.Add(l);
                        }
                        else newLeaves.Add(l);
                    }
                    else
                    {
                        newLeaves.Add(l.leftChild);
                        newLeaves.Add(l.rightChild);
                    }
                }
                leaves = newLeaves;
            }

            root.CreateRooms();
            DrawRoomsToGrid(leaves, f);


            List<Vector2Int> roomCenters = new List<Vector2Int>();
            foreach (Leaf l in leaves)
            {
                if (l.room != null)
                {
                    int cx = l.room.x + l.room.width / 2;
                    int cy = l.room.y + l.room.height / 2;
                    roomCenters.Add(new Vector2Int(cx, cy));
                }
            }


            if (f < numberOfFloors - 1 && roomCenters.Count > 0)
            {
                int randomRoomIndex = Random.Range(0, roomCenters.Count);

                liftPositions[f] = roomCenters[randomRoomIndex];
                grid[f, liftPositions[f].x, liftPositions[f].y] = TileType.Lift;
            }

            if (f > 0)
            {
                roomCenters.Add(liftPositions[f - 1]);
                grid[f, liftPositions[f - 1].x, liftPositions[f - 1].y] = TileType.RoomFloor;
            }

            for (int i = 0; i < roomCenters.Count - 1; i++)
            {
                ConnectWithAStar(roomCenters[i], roomCenters[i + 1], f);
            }
        }


        InstantiateDungeon();
    }


    void ConnectWithAStar(Vector2Int start, Vector2Int end, int floor)
    {
        List<Vector2Int> path = FindPath(start, end, floor);
        if (path != null)
        {
            foreach (Vector2Int p in path)
            {
                if (grid[floor, p.x, p.y] == TileType.Empty)
                {
                    grid[floor, p.x, p.y] = TileType.CorridorFloor;
                }
            }
        }
    }

    List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, int floor)
    {

        List<Vector2Int> openSet = new List<Vector2Int> { start };
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float>();
        gScore[start] = 0;

        Vector2Int[] neighbors = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (openSet.Count > 0)
        {
            Vector2Int current = openSet[0];
            foreach (Vector2Int pos in openSet)
            {
   
                float fCurrent = gScore[current] + Mathf.Abs(current.x - end.x) + Mathf.Abs(current.y - end.y);
                float fPos = gScore.ContainsKey(pos) ? gScore[pos] + Mathf.Abs(pos.x - end.x) + Mathf.Abs(pos.y - end.y) : float.MaxValue;
                if (fPos < fCurrent) current = pos;
            }

            if (current == end)
            {
    
                List<Vector2Int> path = new List<Vector2Int>();
                while (cameFrom.ContainsKey(current))
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                return path;
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Vector2Int dir in neighbors)
            {
                Vector2Int neighbor = current + dir;

                if (neighbor.x < 1 || neighbor.x >= gridWidth - 1 || neighbor.y < 1 || neighbor.y >= gridHeight - 1)
                    continue; 

                if (closedSet.Contains(neighbor)) continue;


                float moveCost = (grid[floor, neighbor.x, neighbor.y] == TileType.Empty) ? 5f : 1f;
                float tentativeG = gScore[current] + moveCost;

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                }
            }
        }
        return null;
    }


    void InstantiateDungeon()
    {
        for (int f = 0; f < numberOfFloors; f++)
        {
            float yPos = f * floorHeightOffset;

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    TileType type = grid[f, x, y];
                    Vector3 worldPos = new Vector3(x, yPos, y);

                    bool isLiftArrival = (f > 0 && x == liftPositions[f - 1].x && y == liftPositions[f - 1].y);

                    if (type == TileType.RoomFloor)
                    {
                        if (!isLiftArrival)
                        {
                            Instantiate(roomFloorPrefab, worldPos, Quaternion.identity, transform);
                        }
                    }
                    else if (type == TileType.CorridorFloor)
                    {
                        if (!isLiftArrival)
                        {
                            Instantiate(corridorFloorPrefab, worldPos, Quaternion.identity, transform);
                        }
                    }
                    else if (type == TileType.Lift)
                    {
    
                        Vector3 liftPos = new Vector3(x, yPos + floorHeightOffset / 2f, y);
                        GameObject lift = Instantiate(liftPrefab, liftPos, Quaternion.identity, transform);
                    }
                    else if (type == TileType.Empty)
                    {
       
                        if (IsAdjacentTo(f, x, y, TileType.RoomFloor))
                        {
                            Vector3 wallPos = new Vector3(x, yPos + 1f, y);
                            Instantiate(roomWallPrefab, wallPos, Quaternion.identity, transform);
                        }
                        else if (IsAdjacentTo(f, x, y, TileType.CorridorFloor))
                        {
                            Vector3 wallPos = new Vector3(x, yPos + 1f, y);
                            Instantiate(corridorWallPrefab, wallPos, Quaternion.identity, transform);
                        }

                    }
                }
            }
        }
    }

    bool IsAdjacentTo(int f, int x, int y, TileType checkType)
    {

        if (x > 0 && grid[f, x - 1, y] == checkType) return true;
        if (x < gridWidth - 1 && grid[f, x + 1, y] == checkType) return true;
        if (y > 0 && grid[f, x, y - 1] == checkType) return true;
        if (y < gridHeight - 1 && grid[f, x, y + 1] == checkType) return true;
        return false;
    }

   
    void DrawRoomsToGrid(List<Leaf> leaves, int floor)
    {
        foreach (Leaf l in leaves)
        {
            if (l.room != null)
            {
                for (int x = l.room.x; x < l.room.x + l.room.width; x++)
                {
                    for (int y = l.room.y; y < l.room.y + l.room.height; y++)
                    {
                        grid[floor, x, y] = TileType.RoomFloor;
                    }
                }
            }
        }
    }

    class Rect
    {
        public int x, y, width, height;
        public Rect(int x, int y, int w, int h) { this.x = x; this.y = y; width = w; height = h; }
    }

    class Leaf
    {
        public int x, y, width, height;
        public int minPartitionSize, minRoomSize;
        public Leaf leftChild, rightChild;
        public Rect room;

        public Leaf(int x, int y, int w, int h, int minP, int minR)
        {
            this.x = x; this.y = y; width = w; height = h;
            minPartitionSize = minP; minRoomSize = minR;
        }

        public bool Split()
        {
            if (leftChild != null || rightChild != null) return false;

            bool splitH = Random.value > 0.5f;
            if (width > height && (float)width / height >= 1.25f) splitH = false;
            else if (height > width && (float)height / width >= 1.25f) splitH = true;

            int max = (splitH ? height : width) - minPartitionSize;
            if (max <= minPartitionSize) return false;

            int split = Random.Range(minPartitionSize, max);

            if (splitH)
            {
                leftChild = new Leaf(x, y, width, split, minPartitionSize, minRoomSize);
                rightChild = new Leaf(x, y + split, width, height - split, minPartitionSize, minRoomSize);
            }
            else
            {
                leftChild = new Leaf(x, y, split, height, minPartitionSize, minRoomSize);
                rightChild = new Leaf(x + split, y, width - split, height, minPartitionSize, minRoomSize);
            }
            return true;
        }

        public void CreateRooms()
        {
            if (leftChild != null || rightChild != null)
            {
                if (leftChild != null) leftChild.CreateRooms();
                if (rightChild != null) rightChild.CreateRooms();
            }
            else
            {
                int roomW = Random.Range(minRoomSize, width - 2);
                int roomH = Random.Range(minRoomSize, height - 2);
                int roomX = Random.Range(1, width - roomW - 1);
                int roomY = Random.Range(1, height - roomH - 1);
                room = new Rect(x + roomX, y + roomY, roomW, roomH);
            }
        }
    }
}