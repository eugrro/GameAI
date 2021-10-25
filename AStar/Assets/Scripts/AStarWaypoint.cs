using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AStarWaypoint : MonoBehaviour
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


    public Text heuristic1;
    public Text heuristic2;
    private TextMapping textmap; 
    private bool chnagingWhich = true;
    private GameObject startWaypoint;
    private GameObject endWaypoint;
    private List<GameObject> lines = new List<GameObject>();
    public List<GameObject> waypoints = new List<GameObject>();


    struct Node {
        public Vector3 position; // location of node on map
        public float g; // distance between current node and start
        public float h; // estimated distance to goal, the heuristic
        public float f; // the total cost of the node
        public Vector3 parent;

        public Node(Vector3 position, float g, float h) {
            this.position = position;
            this.g = g;
            this.h = h;
            this.f = g + h;
            this.parent = new Vector3(-1, -1, -1);
        }
    }

    private int FindIndex(Node nodeToFind, List<Node> list) {
        for (int i = 0; i < list.Count; i++) {
            if (list[i].position.x == nodeToFind.position.x && list[i].position.y == nodeToFind.position.y) {
                return i;
            }
        }
        return -1;
    }

    private int FindSmallestFIndex(List<Node> list) {
        float smallestF = 9999f;
        if (heuristic1.text != "") {
            smallestF = float.Parse(heuristic1.text);
        }
        int index = 0;
        for (int i = 0; i < list.Count; i++) {
            if (list[i].f < smallestF) {
                smallestF = list[i].f;
                index = i;
            }
        }
        return index;
    }
    private bool couldBeNeighbor(Vector3 p1, Vector3 p2){
        if(Vector3.Distance(p1, p2) > 7){
            return false;
        }
        if(Mathf.Abs(p1.x-p2.x) > 5 && Mathf.Abs(p1.y-p2.y) > 5){
            return false;
        }
        return true;
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
                        if (mapWidth == mapHeight)
                        {
                            if (vec3int.x >= mapWidth) {
                                vec3int.x = 0;
                                vec3int.y += 1;
                            }
                        }
                        else
                        {
                            if (vec3int.x > mapWidth)
                            {
                                vec3int.x = 0;
                                vec3int.y += 1;
                            }
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

    private void AStar(GameObject start, GameObject goal) {
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        Vector3 startPos = start.transform.position;
        Vector3 goalPos = goal.transform.position;

        float dist = Vector3.Distance(startPos, goalPos);
        Node startNode = new Node(startPos, 0f, dist);
        openList.Add(startNode);
        //textmap.tilemap.SetTile(startNode.position, checkedTile);
        
        Node currentNode = startNode;
        while (openList.Count > 0) {
            if (currentNode.position == goalPos) {
                print("A* found the goal");

                //textmap.tilemap.SetTile(currentNode.position, finalTile);
                Vector3 parent = currentNode.parent;
                DrawLine(currentNode.position, parent);


                while (parent.x != -1 && parent.y != -1) {
                    Node parentNode = new Node(new Vector3(parent.x, parent.y, 0), -1, -1);
                    currentNode = closedList[FindIndex(parentNode, closedList)];
                    if(currentNode.parent == new Vector3(-1, -1, -1)){
                        DrawLine(parent, currentNode.position);
                    }else{
                        DrawLine(parent, currentNode.parent);
                    }
                    //textmap.tilemap.SetTile(currentNode.position, finalTile);

                    parent = currentNode.parent;
                }
                return;
            } else {
                int bestNodeIndex = FindSmallestFIndex(openList);
                currentNode = openList[bestNodeIndex];
                closedList.Add(openList[bestNodeIndex]);
                openList.RemoveAt(bestNodeIndex);
                
                // Check all four adjacent tiles if passable
                List<Vector3> neighbors = new List<Vector3>();
                float x = currentNode.position.x;
                float y = currentNode.position.y;
                /*
                if (x < textmap.mapData.GetLength(0)-1 && textmap.mapData[x+1, y] == 0)
                    neighbors.Add(new Vector3Int(x+1, y, 0));
                if (x > 0 && textmap.mapData[x-1, y] == 0)
                    neighbors.Add(new Vector3Int(x-1, y, 0));
                if (y < textmap.mapData.GetLength(1)-1 && textmap.mapData[x, y+1] == 0)
                    neighbors.Add(new Vector3Int(x, y+1, 0));
                if (y > 0 && textmap.mapData[x, y-1] == 0)
                    neighbors.Add(new Vector3Int(x, y-1, 0));
                */
                var currentNodePosition = currentNode.position;
                foreach (GameObject otherWaypoint in waypoints) {
                    if(couldBeNeighbor(otherWaypoint.transform.position, currentNode.position)){
                        neighbors.Add(otherWaypoint.transform.position);
                    }
                }
                // Loop through each neighbor
                foreach (Vector3 neighbor in neighbors) {

                    // Change color of explored tiles
                    //textmap.tilemap.SetTile(neighbor, checkedTile);

                    // Check if neighbor is in closed list
                    Node closedNode = new Node(new Vector3(-1, -1, -1), -1, -1);
                    foreach (Node n in closedList) {
                        if (n.position.x == neighbor.x && n.position.y == neighbor.y)
                            closedNode = n;
                    }
                    // Check if neighbor is in open list
                    Node openNode = new Node(new Vector3(-1, -1, -1), -1, -1);
                    foreach (Node n in openList) {
                        if (n.position.x == neighbor.x && n.position.y == neighbor.y) {
                            openNode = n;
                            openNode.parent = currentNode.position;
                        }
                    }

                    // Not in open list and not in closed list, add it to open list
                    if ((closedNode.g == -1 || closedNode.h == -1) && (openNode.g == -1 || openNode.h == -1)) {
                        float distToParent = Vector3.Distance(neighbor, currentNode.position);
                        float distToGoal = Vector3.Distance(neighbor, goalPos);
                        Node newNode = new Node(neighbor, currentNode.g + distToParent, distToGoal); // 
                        newNode.parent = currentNode.position; // Set parent to current node
                        openList.Add(newNode); // Add to open list, and set the g value
                        openNode = newNode;
                    }

                    float weight = 0f;
                    if (heuristic2.text != "") {
                        weight = float.Parse(heuristic2.text);
                    }

                    // Neighbor is in closed list and current g value is lower
                    if (currentNode.g + weight < closedNode.g) {
                        closedNode.g = currentNode.g;
                        closedNode.parent = currentNode.position;

                        closedList[FindIndex(closedNode, closedList)] = closedNode;

                    // Neighbor is in open list and current g value is lower
                    } else if (currentNode.g + weight < openNode.g) {
                        openNode.g = currentNode.g;
                        openNode.parent = currentNode.position;

                        openList[FindIndex(openNode, openList)] = openNode;
                    }
                }
            }
        }
        print("A* could not find goal");
    }

    void DrawLine(Vector3 pos1, Vector3 pos2){
        pos1.z = -1; 
        pos2.z = -1;
        GameObject newLine = new GameObject("MyGameObject");
        LineRenderer lRend = newLine.AddComponent<LineRenderer>();
        lRend.SetColors (Color.red,Color.red);
        lRend.SetWidth(1f, 1f);
        lRend.SetPosition(0,pos1);
        lRend.SetPosition(1,pos2);
        lines.Add(newLine);
    }
    void clearLines(){
        foreach(GameObject oj in lines){
            Destroy (oj);
        }
        lines.Clear();
    }

    void Start() {
        GameObject textmapgo = GameObject.Find("MapHandler");
        textmap = textmapgo.GetComponent<TextMapping>();
        //waypoints = textmap.waypoints;

        mapData = Load(Application.dataPath + "\\" + fileNameToLoad);
        var ss = 2;
        var spacing = 6;
        if(mapHeight < 100){
            spacing = 6;
            ss = 2;
        }else{
            spacing = 4;
            ss = 2;
        }
        bool isWaypoint = true;
        if(SceneManager.GetActiveScene().buildIndex == 1){

            //if waypoint map
            for(int i = 0;i < mapHeight-ss;i+=2){
                for(int j = 0;j < mapWidth-ss;j+=2){
                    isWaypoint = true;
                    for(int y = 0;y < ss;y++){
                        for(int x = 0;x < ss;x++){
                            //Debug.Log(string.Format("i: {0} j: {1} x: {2} y: {3}",i, j, x,y));
                            if(mapData[j+x, i+y] != 0){
                                isWaypoint = false;
                            }
                        }
                    }
                    if(isWaypoint){
                        bool addWaypoint = true;
                        foreach (GameObject otherWaypoint in waypoints) {
                            if(Vector3.Distance(otherWaypoint.transform.position, new Vector3(j + .5f + ss/2,i +.5f + ss/2, 0)) < spacing){
                                addWaypoint = false;
                            }
                        }
                        if(addWaypoint){
                            GameObject newWaypoint = Instantiate(waypoint, new Vector3(j + .5f + ss/2,i + .5f + ss/2 , 0), Quaternion.identity);
                            waypoints.Add(newWaypoint);
                        }
                    }
                }      
            }
        }
        startWaypoint = waypoints[0];
        endWaypoint = waypoints[10];
        //DrawLine(waypoints[0].transform.position, waypoints[10].transform.position);
        AStar(startWaypoint, endWaypoint);

    }
    private void Clear() {
        for (int i = 0; i < mapWidth+1; i++) {
            for (int  j = 0; j < mapHeight+11; j++) {
                tilemap.SetTile(new Vector3Int(i, j, 0), null);
            }
        }
    }
    
    private bool startClickAvailable = true;
    private bool endClickAvailable = true;
    private bool aStarStarted = false;
    private Vector3 startPos = new Vector3(-1, -1, -1);
    private Vector3 endPos = new Vector3(-1, -1, -1);
    void Update(){

        if (Input.GetMouseButtonDown(0)) {
            Vector3 clickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            GameObject closestObject = waypoints[0];
            float closestDistance = 9999f;
            foreach (GameObject otherWaypoint in waypoints) {
                if(Vector3.Distance(clickedPosition, otherWaypoint.transform.position) < closestDistance){
                    closestDistance = Vector3.Distance(clickedPosition, otherWaypoint.transform.position);
                    closestObject = otherWaypoint;
                }
            }
            // If both start and end points are selected, run A* once
            if (chnagingWhich) {
                startWaypoint = closestObject;
                chnagingWhich = !chnagingWhich;
            }else{
                endWaypoint = closestObject;
                chnagingWhich = !chnagingWhich;
            }
            clearLines();
            AStar(startWaypoint, endWaypoint);
        }
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
