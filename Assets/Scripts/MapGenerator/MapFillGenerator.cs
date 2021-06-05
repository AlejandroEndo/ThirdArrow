using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapFillGenerator : MonoBehaviour {

    [Range(1, 10)]
    [SerializeField] private int enemysPerNode;
    [SerializeField] private float spawnRange;
    [SerializeField] private GameObject capsuleEnemy;
    [SerializeField] private List<MapGenerator.Coord> doorPoints;
    [SerializeField] private GameObject enemiesParent;
    private MapGenerator mapGenerator;
    public void SpawnCrucialPoints(Dictionary<string, List<MapGenerator.Coord>> crucialPoints, int[,] map) {
        if (mapGenerator == null) mapGenerator = GetComponent<MapGenerator>();
        doorPoints = crucialPoints["doors"];

        foreach (MapGenerator.Coord door in doorPoints) {
            for (int i = 0; i < enemysPerNode; i++) {
                MapGenerator.Coord convertedPos = GetAvailableSpot(door, map);
                map[convertedPos.x, convertedPos.y] = 1;
                Vector3 pos = CoordToWorldPoint(convertedPos);
                SpawnPoint(pos);
            }
        }
    }

    private void SpawnPoint(Vector3 pos) {
        GameObject enemy = Instantiate(capsuleEnemy, pos, Quaternion.identity);
        enemy.transform.parent = enemiesParent.transform;
    }

    private MapGenerator.Coord GetAvailableSpot(MapGenerator.Coord spawnCoord, int[,] map) {
        Vector2 newPos = GenerateRandomPos(spawnCoord.x, spawnCoord.y);

        if (!IsInsideMapRange((int)newPos.x, (int)newPos.y, map)) return GetAvailableSpot(spawnCoord, map);
        
        if (map[(int)newPos.x, (int)newPos.y] == 0) {
            MapGenerator.Coord randomCoord = new MapGenerator.Coord((int)newPos.x, (int)newPos.y);
            return randomCoord;
        } else {
            return GetAvailableSpot(spawnCoord, map);
        }
    }

    bool IsInsideMapRange(int x, int y, int[,] map) {
        return (x < map.GetLength(0) && x >= 0 && y < map.GetLength(1) && y >= 0);
    }

    private Vector2 GenerateRandomPos(int offsetX, int offsetY) {
        float randomX = Random.Range(-spawnRange, spawnRange);
        float randomY = Random.Range(-spawnRange, spawnRange);
        return new Vector2(randomX + offsetX, randomY + offsetY);
    }

    public Vector3 CoordToWorldPoint(MapGenerator.Coord tile) {
        return new Vector3(-mapGenerator.Width / 2 + mapGenerator.unitSize / 2 + tile.x, 1f, -mapGenerator.Height / 2 + mapGenerator.unitSize / 2 + tile.y);
    }
}
