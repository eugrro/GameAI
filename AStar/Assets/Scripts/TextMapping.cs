using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


using System.Text;
using System.IO;

public class TextMapping : MonoBehaviour
{
    public string fileNameToLoad = "";
    public GameObject waypoint;
    private int mapWidth;
    private int mapHeight;

    public Tile passableTile;
    public Tile impassableTile;
    public Tile outOfBoundsTile;

    public Tilemap tilemap;
    public int[,] mapData;

    public int[,] getMapData(){
        new WaitForSeconds(3);
        return mapData;
    }
    public int getMapHeight(){
        return mapHeight;
    }
    public int getMapWidth(){
        return mapWidth;
    }

    private int[,] Load(string filePath) {
        try {
            using(StreamReader sr = new StreamReader(filePath) ) {
                string input = sr.ReadToEnd();
                string[] lines = input.Split(new[] {'\n'}, System.StringSplitOptions.RemoveEmptyEntries);

                mapHeight = int.Parse(lines[1].Split(' ')[1]);
                mapWidth = int.Parse(lines[2].Split(' ')[1]);

                int[,] mapData = new int[mapWidth+1, mapHeight+1];
                for (int i = 0; i < mapWidth; i++) {
                    for (int j = 0; j < mapHeight; j++) {
                        mapData[i, j] = -1;
                    }
                }

                Vector3Int vec3int = Vector3Int.zero;
                for(int i = lines.Length-1; i >= 4; i--) {
                    string st = lines[i];

                    foreach (char c in st) {
                        if (mapWidth == mapHeight && vec3int.x >= mapWidth) {
                            vec3int.x = 0;
                            vec3int.y += 1;
                        } else if (vec3int.x > mapWidth) {
                            vec3int.x = 0;
                            vec3int.y += 1;
                        }
                        if (c == '.') {
                            tilemap.SetTile(vec3int, passableTile);
                            mapData[vec3int.x, vec3int.y] = 0;
                        } else if (c == 'T') {
                            tilemap.SetTile(vec3int, impassableTile);
                            mapData[vec3int.x, vec3int.y] = 1;
                        } else {
                            tilemap.SetTile(vec3int, outOfBoundsTile);
                            mapData[vec3int.x, vec3int.y] = 2;
                        }
                        vec3int.x += 1;
                    }
                }
                return mapData;
            }
        } catch(IOException e) {
            Debug.Log(e.Message);
        }
        return null;
    }
    private void Clear() {
        for (int i = 0; i < mapWidth+1; i++) {
            for (int  j = 0; j < mapHeight+11; j++) {
                tilemap.SetTile(new Vector3Int(i, j, 0), null);
            }
        }
    }

    void Start() {
        mapData = Load(Application.dataPath + "\\" + fileNameToLoad);
        
    }

    void Update() {
        if (Input.GetKeyDown("m")) {
            if (fileNameToLoad == "map1.map") {
                Clear();
                fileNameToLoad = "map2.map";
                mapData = Load(Application.dataPath + "\\" + fileNameToLoad);
            } else if (fileNameToLoad == "map2.map") {
                Clear();
                fileNameToLoad = "map1.map";
                mapData = Load(Application.dataPath + "\\" + fileNameToLoad);
            }
        }
    } 
}
