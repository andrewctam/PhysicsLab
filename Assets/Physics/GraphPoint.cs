using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPoint : MonoBehaviour
{
    public Graph graph;
    public float xPos, yPos; //Pos if scales = 1

    // Start is called before the first frame update 
    void Start() 
    {
        /*
        graph = FindObjectOfType<Graph>();
        xPos = transform.position.x * graph.xScale;
        yPos = transform.position.y * graph.yScale;*/
    }
    void Update()
    {
    }

    public void updatePos(float x, float y) {

    }
}
