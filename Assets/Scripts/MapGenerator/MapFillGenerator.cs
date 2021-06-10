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
            GameObject parentObj = new GameObject {
                name = "spawnPoint_" + door.x + "_" + door.y
            };
            //MapGenerator.Coord convertedPos = GetAvailableSpot(door, map);
            map[door.x, door.y] = 1;
            Vector3 pos = CoordToWorldPoint(door);
            parentObj.transform.position = pos;
            parentObj.transform.parent = enemiesParent.transform;
            for (int i = 0; i < enemysPerNode; i++) {
                SpawnPoint(pos, parentObj.transform);
            }
        }
    }

    private void SpawnPoint(Vector3 pos, Transform parent) {
        GameObject enemy = Instantiate(capsuleEnemy, pos, Quaternion.identity);
        enemy.transform.parent = enemiesParent.transform;
        enemy.transform.parent = parent;
        enemy.GetComponent<NPC>().spawnPoint = parent;
    }

    private MapGenerator.Coord GetAvailableSpot(MapGenerator.Coord spawnCoord, int[,] map) {
        Vector2 newPos = GenerateRandomPos(spawnCoord.x, spawnCoord.y);

        int r = 2;
        if (!IsInsideMapRange((int)newPos.x, (int)newPos.y, r, map)) {
            return GetAvailableSpot(spawnCoord, map);
        }

        for (int x = (int)newPos.x - r; x <= (int)newPos.x + r; x++) {
            for (int y = (int)newPos.y - r; y <= (int)newPos.y + r; y++) {
                if ((Mathf.Pow(x - newPos.x, 2) + Mathf.Pow(y - newPos.y, 2)) <= Mathf.Pow(r,2))
                    if (map[x, y] == 0) return GetAvailableSpot(spawnCoord, map);
            }
        }

        if (map[(int)newPos.x, (int)newPos.y] == 1) {
            MapGenerator.Coord randomCoord = new MapGenerator.Coord((int)newPos.x, (int)newPos.y);
            return randomCoord;
        } else {
            spawnRange += Time.deltaTime;
            return GetAvailableSpot(spawnCoord, map);
        }
    }

    bool IsInsideMapRange(int x, int y, int r, int[,] map) {
        return (x < map.GetLength(0) - r && x > r && y < map.GetLength(1) - r && y > r);
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
