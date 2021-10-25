using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

using System.Text;
using System.IO;

public class AStarPathfinding : MonoBehaviour
{
    public Tile checkedTile;
    public Tile finalTile;
    public Tile passableTile;
    public Tile obstacleTile;
    public Tile endpointTile;

    public Text heuristic1;
    public Text heuristic2;
    public Tilemap tilemap;
    private TextMapping textmap;

    public Text display;

    struct Node {
        public Vector3Int position; // location of node on map
        public float g; // distance between current node and start
        public float h; // estimated distance to goal, the heuristic
        public float f; // the total cost of the node
        public Vector3Int parent;

        public Node(Vector3Int position, float g, float h) {
            this.position = position;
            this.g = g;
            this.h = h;
            this.f = g + h;
            this.parent = new Vector3Int(-1, -1, -1);
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

    private void AStar(Vector3Int start, Vector3Int goal) {
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        float dist = Vector3Int.Distance(start, goal);
        Node startNode = new Node(start, 0f, dist);
        openList.Add(startNode);
        textmap.tilemap.SetTile(startNode.position, checkedTile);
        
        Node currentNode = startNode;
        while (openList.Count > 0) {
            if (currentNode.position == goal) {
                print("A* found the goal");

                textmap.tilemap.SetTile(currentNode.position, finalTile);
                Vector3Int parent = currentNode.parent;

                while (parent.x != -1 && parent.y != -1) {
                    Node parentNode = new Node(new Vector3Int(parent.x, parent.y, 0), -1, -1);
                    currentNode = closedList[FindIndex(parentNode, closedList)];
                    textmap.tilemap.SetTile(currentNode.position, finalTile);

                    parent = currentNode.parent;
                }
                return;
            } else {
                int bestNodeIndex = FindSmallestFIndex(openList);
                currentNode = openList[bestNodeIndex];
                closedList.Add(openList[bestNodeIndex]);
                openList.RemoveAt(bestNodeIndex);
                
                // Check all four adjacent tiles if passable
                List<Vector3Int> neighbors = new List<Vector3Int>();
                int x = currentNode.position.x;
                int y = currentNode.position.y;
                if (x < textmap.mapData.GetLength(0)-1 && textmap.mapData[x+1, y] == 0)
                    neighbors.Add(new Vector3Int(x+1, y, 0));
                if (x > 0 && textmap.mapData[x-1, y] == 0)
                    neighbors.Add(new Vector3Int(x-1, y, 0));
                if (y < textmap.mapData.GetLength(1)-1 && textmap.mapData[x, y+1] == 0)
                    neighbors.Add(new Vector3Int(x, y+1, 0));
                if (y > 0 && textmap.mapData[x, y-1] == 0)
                    neighbors.Add(new Vector3Int(x, y-1, 0));

                // Loop through each neighbor
                foreach (Vector3Int neighbor in neighbors) {

                    // Change color of explored tiles
                    textmap.tilemap.SetTile(neighbor, checkedTile);

                    // Check if neighbor is in closed list
                    Node closedNode = new Node(new Vector3Int(-1, -1, -1), -1, -1);
                    foreach (Node n in closedList) {
                        if (n.position.x == neighbor.x && n.position.y == neighbor.y)
                            closedNode = n;
                    }
                    // Check if neighbor is in open list
                    Node openNode = new Node(new Vector3Int(-1, -1, -1), -1, -1);
                    foreach (Node n in openList) {
                        if (n.position.x == neighbor.x && n.position.y == neighbor.y) {
                            openNode = n;
                            openNode.parent = currentNode.position;
                        }
                    }

                    // Not in open list and not in closed list, add it to open list
                    if ((closedNode.g == -1 || closedNode.h == -1) && (openNode.g == -1 || openNode.h == -1)) {
                        float distToParent = Vector3Int.Distance(neighbor, currentNode.position);
                        float distToGoal = Vector3Int.Distance(neighbor, goal);
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

    void Start() {
        GameObject textmapgo = GameObject.Find("MapHandler");
        textmap = textmapgo.GetComponent<TextMapping>();
    }
    

    // 0 = place start point
    // 1 = place goal point
    // 2 = create obstacle
    // 3 = delete obstacle
    public int buttonState = 0;


    private bool startClickAvailable = true;
    private bool endClickAvailable = true;
    private bool aStarStarted = false;
    private Vector3Int startPos = new Vector3Int(-1, -1, -1);
    private Vector3Int endPos = new Vector3Int(-1, -1, -1);
    void Update(){

        if (Input.GetKeyDown("space")) {
            buttonState += 1;
            if (buttonState == 4)
                buttonState = 0;

            if (buttonState == 0) {
                display.text = "Press spacebar to toggle what\nhappens when you right click:\nCurrent mode: Place start point";
            } else if (buttonState == 1) {
                display.text = "Press spacebar to toggle what\nhappens when you right click:\nCurrent mode: Place goal point";
            } else if (buttonState == 2) {
                display.text = "Press spacebar to toggle what\nhappens when you right click:\nCurrent mode: Place obstacles";
            } else if (buttonState == 3) {
                display.text = "Press spacebar to toggle what\nhappens when you right click:\nCurrent mode: Remove obstacles";
            }
        }

        if (Input.GetMouseButton(1)) {
            Vector3Int clickedPosition = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            // Selecting a start of the path within bounds of map
            if(buttonState == 0 && startClickAvailable == true && clickedPosition.x >=0 && clickedPosition.x < textmap.mapData.GetLength(0)
                && clickedPosition.y >=0 && clickedPosition.y < textmap.mapData.GetLength(1)) {

                // Tile selected is a passable tile
                if (textmap.mapData[clickedPosition.x, clickedPosition.y] == 0) {
                    startClickAvailable = false;
                    startPos = clickedPosition;
                    textmap.tilemap.SetTile(startPos, endpointTile);
                } else {
                    print("Invalid start position selected");
                }
            // Selecting an end of the path within bounds of map
            } else if (buttonState == 1 && endClickAvailable == true && clickedPosition.x >=0 && clickedPosition.x < textmap.mapData.GetLength(0)
                && clickedPosition.y >=0 && clickedPosition.y < textmap.mapData.GetLength(1)){

                // Tile selected is a passable tile
                if (textmap.mapData[clickedPosition.x, clickedPosition.y] == 0) {
                    endClickAvailable = false;
                    endPos = clickedPosition;
                    textmap.tilemap.SetTile(endPos, endpointTile);
                } else {
                    print("Invalid end position selected");
                }
            } else if (buttonState == 2) {
                if (textmap.mapData[clickedPosition.x, clickedPosition.y] == 0) {
                    textmap.mapData[clickedPosition.x, clickedPosition.y] = 3;
                    textmap.tilemap.SetTile(clickedPosition, obstacleTile);
                }

            } else if (buttonState == 3) {
                if (textmap.mapData[clickedPosition.x, clickedPosition.y] == 3) {
                    textmap.mapData[clickedPosition.x, clickedPosition.y] = 0;
                    textmap.tilemap.SetTile(clickedPosition, passableTile);
                }
            }

            // If both start and end points are selected, run A* once
            if (!startClickAvailable && !endClickAvailable && !aStarStarted) {
                aStarStarted = true;
                AStar(startPos, endPos);
            }
        }
    }
}
