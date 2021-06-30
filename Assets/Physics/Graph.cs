using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour
{
    public float lastPlot, plotFrequency, xScale, yScale;
    public GameObject graphGameObject, coordinatePoint, points;
    public CreateObjects create;
    public InputField xScaleInput, yScaleInput, plotFreqInput;
    public List<GameObject> graphedObjects;
    public Dropdown xAxisDropdown, yAxisDropdown;

    // Start is called before the first frame update
    void Start()
    {
        plotFrequency = 0.5f;
        lastPlot = 0f;
        //create = FindObjectOfType<CreateObjects>();
        xScale = 1f;
        yScale = 1f;
        graphedObjects = new List<GameObject>();
        
    
    }

    // Update is called once per frame
    void Update()
    {
        lastPlot += Time.deltaTime;

        if (lastPlot >= plotFrequency && create.started && graphedObjects.Count > 0) {
            foreach (GameObject element in graphedObjects) {
                PointMass script = element.GetComponent<PointMass>();
                graphPoint(script.xAxis * xScale, script.yAxis * yScale);
            }
            lastPlot = 0f;
        }

        if (Input.GetKeyDown(KeyCode.X))
            deletePoints();
            
    }

    public void graphPoint(float x, float y) {
        GameObject createdPoint = Instantiate(coordinatePoint, new Vector3(x, y, 0), Quaternion.identity, points.transform);
        createdPoint.transform.localScale = new Vector3(0.1f / xScale, 0.1f / yScale, 1f);

    }

    public void deletePoints() {
        Destroy(points.gameObject);
        points = new GameObject("points");
        points.transform.SetParent(graphGameObject.transform);
        lastPlot = 0f;
        points.transform.localScale = new Vector3(xScale, yScale, 1f);

    }

    public void toggleGraphing() {
        GameObject currentGameObj = create.createdObjects[create.current];
        if (!currentGameObj.GetComponent<PointMass>().isGraphing) {
            graphedObjects.Add(currentGameObj);
            create.graphSettings.SetActive(true);
            currentGameObj.GetComponent<PointMass>().isGraphing = true;
            create.graphButton.GetComponent<Image>().color = new Color(0.5f, 0.8f, 0.8f);
        } else {
            graphedObjects.Remove(currentGameObj);
            create.graphSettings.SetActive(false);
            currentGameObj.GetComponent<PointMass>().isGraphing = false;
            create.graphButton.GetComponent<Image>().color = Color.white;

        }
    }
    
    public void updateScale() {
        xScale = float.Parse(xScaleInput.text);
        yScale = float.Parse(yScaleInput.text);


        points.transform.localScale = new Vector3(xScale, yScale, 1f);
        for (int i = 0; i < points.transform.childCount; i++) {
            points.transform.GetChild(i).transform.localScale = new Vector3(0.1f / xScale, 0.1f / yScale, 1f);
        }

    }

    public void updateFrequency() {
        float newFreq = float.Parse(plotFreqInput.text);
        if (newFreq < 0.02f) {
            newFreq = 0.02f;
            plotFreqInput.text = "0.02";
            create.newAnnouncement("The minimum plot frequency is 0.02", 180);
        
        }
        plotFrequency = newFreq;
       

    }
}
