using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public Vector2 aspectRatio;
    [Range(7, 20)]
    public int resolution;

    private int Width { get { return (int)aspectRatio.x * resolution; } }
    private int Height { get { return (int)aspectRatio.y * resolution; } }

    [SerializeField] private string randomSeed;
    [SerializeField] private bool useRandomSeed;

    [Range(0, 100)]
    [SerializeField] private int randomFillPercent;
    [Range(1, 10)]
    [SerializeField] private int smoothIterations;
    [SerializeField] private int unitSize;
    [SerializeField] private int borderSize;
    [SerializeField] private int wallTresholdSize;
    [SerializeField] private int roomTresholdSize;
    [Range(1, 5)]
    [SerializeField] private int passageRadius;

    private int[,] map;

    void Start() {
        GenerateMap();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) GenerateMap();
    }

    private void GenerateMap() {
        map = new int[Width, Height];
        RandomFillMap();

        for (int i = 0; i < smoothIterations; i++) {
            SmoothMap();
        }

        ProcessMap();

        int[,] borderedMap = new int[Width + borderSize * 2, Height + borderSize * 2];
        for (int x = 0; x < borderedMap.GetLength(0); x++) {
            for (int y = 0; y < borderedMap.GetLength(1); y++) {
                if (x >= borderSize && x < Width + borderSize && y >= borderSize && y < Height + borderSize) {
                    borderedMap[x, y] = map[x - borderSize, y - borderSize];
                } else {
                    borderedMap[x, y] = 1;
                }
            }
        }

        MeshGenerator meshGenerator = GetComponent<MeshGenerator>();
        meshGenerator.GenerateMesh(borderedMap, unitSize);
    }

    void ProcessMap() {
        List<List<Coord>> wallRegions = GetRegions(1);

        foreach (List<Coord> wallRegion in wallRegions) {
            if (wallRegion.Count < wallTresholdSize) {
                foreach (Coord tile in wallRegion) {
                    map[tile.x, tile.y] = 0;
                }
            }
        }

        List<List<Coord>> roomRegions = GetRegions(0);
        List<Room> survivingRooms = new List<Room>();

        foreach (List<Coord> roomRegion in roomRegions) {
            if (roomRegion.Count < roomTresholdSize) {
                foreach (Coord tile in roomRegion) {
                    map[tile.x, tile.y] = 1;
                }
            } else {
                survivingRooms.Add(new Room(roomRegion, map));
            }
        }
        survivingRooms.Sort();
        survivingRooms[0].isMainRoom = true;
        survivingRooms[0].isAccessibleFromMainRoom = true;
        ConnectClosestRooms(survivingRooms);
    }

    void ConnectClosestRooms(List<Room> rooms, bool forceAccessibilityFromMainRoom = false) {
        List<Room> roomsA = new List<Room>();
        List<Room> roomsB = new List<Room>();

        if (forceAccessibilityFromMainRoom) {
            foreach (Room room in rooms) {
                if (room.isAccessibleFromMainRoom) {
                    roomsB.Add(room);
                } else {
                    roomsA.Add(room);
                }
            }
        } else {
            roomsA = rooms;
            roomsB = rooms;
        }

        int bestDistance = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;

        foreach (Room roomA in roomsA) {
            if (!forceAccessibilityFromMainRoom) {
                possibleConnectionFound = false;
                if (roomA.connectedRooms.Count > 0) continue;
            }

            foreach (Room roomB in roomsB) {
                if (roomA == roomB || roomA.IsConnected(roomB)) continue;

                for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++) {
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++) {
                        Coord tileA = roomA.edgeTiles[tileIndexA];
                        Coord tileB = roomB.edgeTiles[tileIndexB];
                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.x - tileB.x, 2) + Mathf.Pow(tileA.y - tileB.y, 2));

                        if (distanceBetweenRooms < bestDistance || !possibleConnectionFound) {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                            bestTileA = tileA;
                            bestTileB = tileB;
                        }
                    }
                }
            }
            if (possibleConnectionFound && !forceAccessibilityFromMainRoom) {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }

        if (possibleConnectionFound && forceAccessibilityFromMainRoom) {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(rooms, true);
        }

        if (!forceAccessibilityFromMainRoom) {
            ConnectClosestRooms(rooms, true);
        }
    }

    List<Coord> GetLine(Coord from, Coord to) {
        List<Coord> line = new List<Coord>();

        int x = from.x;
        int y = from.y;

        int dx = to.x - from.x;
        int dy = to.y - from.y;

        bool inverted = false;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if (longest < shortest) {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++) {
            line.Add(new Coord(x, y));

            if (inverted) {
                y += step;
            } else {
                x += step;
            }

            gradientAccumulation += shortest;
            if (gradientAccumulation >= longest) {
                if (inverted) {
                    x += gradientStep;
                } else {
                    y += gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }

        return line;
    }

    void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB) {
        Room.ConnectRooms(roomA, roomB);

        List<Coord> line = GetLine(tileA, tileB);
        foreach(Coord coord in line) {
            DrawCircle(coord, passageRadius);
        }
    }

    void DrawCircle(Coord coord, int r) {
        for (int x = -r; x <= r; x++) {
            for (int y = -r; y <= r; y++) {
                if (x * x + y * y <= r * r) {
                    int drawX = coord.x + x;
                    int drawY = coord.y + y;
                    if (IsInMapRange(drawX, drawY)) map[drawX, drawY] = 0;
                }
            }
        }
    }

    Vector3 CoordToWorldPoint(Coord tile) {
        return new Vector3(-Width / 2 + unitSize / 2 + tile.x, 2f, -Height / 2 + unitSize / 2 + tile.y);
    }

    List<List<Coord>> GetRegions(int tileType) {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[Width, Height];

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                if (mapFlags[x, y] == 0 && map[x, y] == tileType) {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);
                    foreach (Coord tile in newRegion) {
                        mapFlags[tile.x, tile.y] = 1;
                    }
                }
            }
        }
        return regions;
    }

    List<Coord> GetRegionTiles(int startX, int startY) {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[Width, Height];
        int tileType = map[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0) {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.x - 1; x <= tile.x + 1; x++) {
                for (int y = tile.y - 1; y <= tile.y + 1; y++) {
                    if (IsInMapRange(x, y) && (x == tile.x || y == tile.y)) {
                        if (mapFlags[x, y] == 0 && map[x, y] == tileType) {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }
        return tiles;
    }

    bool IsInMapRange(int x, int y) {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    private void RandomFillMap() {
        if (useRandomSeed) randomSeed = Time.time.ToString();

        System.Random rng = new System.Random(randomSeed.GetHashCode());

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1) map[x, y] = 1;
                else map[x, y] = rng.Next(0, 100) < randomFillPercent ? 1 : 0;
            }
        }
    }

    private void SmoothMap() {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                int wallCount = WallCount(x, y);
                if (wallCount > 4) map[x, y] = 1;
                else if (wallCount < 4) map[x, y] = 0;
            }
        }
    }

    private int WallCount(int posX, int posY) {
        int wallCount = 0;
        for (int x = posX - 1; x <= posX + 1; x++) {
            for (int y = posY - 1; y <= posY + 1; y++) {
                if (IsInMapRange(x, y)) {
                    if (x != posX || y != posY) {
                        wallCount += map[x, y];
                    }
                } else wallCount++;
            }
        }
        return wallCount;
    }

    struct Coord {
        public int x;
        public int y;
        public Coord(int _x, int _y) {
            x = _x;
            y = _y;
        }
    }

    class Room : IComparable<Room> {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<Room> connectedRooms;

        public int roomSize;
        public bool isAccessibleFromMainRoom;
        public bool isMainRoom;

        public Room() { }

        public Room(List<Coord> roomTiles, int[,] map) {
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();
            edgeTiles = new List<Coord>();

            foreach (Coord tile in tiles) {
                for (int x = tile.x - 1; x <= tile.x + 1; x++) {
                    for (int y = tile.y - 1; y <= tile.y + 1; y++) {
                        if (x == tile.x || y == tile.y) {
                            if (map[x, y] == 1) {
                                edgeTiles.Add(tile);
                            }
                        }
                    }
                }
            }
        }

        public void SetAccessibleFromMainRoom() {
            if (!isAccessibleFromMainRoom) {
                isAccessibleFromMainRoom = true;
                foreach (Room connectedRoom in connectedRooms) {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }

        public static void ConnectRooms(Room roomA, Room roomB) {
            if (roomA.isAccessibleFromMainRoom) {
                roomB.SetAccessibleFromMainRoom();
            } else if (roomB.isAccessibleFromMainRoom) {
                roomA.SetAccessibleFromMainRoom();
            }

            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        public bool IsConnected(Room otherRoom) {
            return connectedRooms.Contains(otherRoom);
        }

        public int CompareTo(Room oterRoom) {
            return oterRoom.roomSize.CompareTo(roomSize);
        }
    }

}
