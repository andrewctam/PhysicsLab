using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class Graph : MonoBehaviour
{
    public float lastPlot, plotFrequency, xScale, yScale;
    public GameObject graphGameObject, coordinatePoint, points, axisLabelsX, axisLabelsY, axisLabel;
    public CreateObjects create;
    public TMP_InputField xScaleInput, yScaleInput, plotFreqInput;
    public List<GameObject> graphedObjects;
    public TMP_Dropdown xAxisDropdown, yAxisDropdown;
    public int minimumXAxis, minimumYAxis;
   
    // Start is called before the first frame update
    void Start()
    {
        plotFrequency = 0.5f;
        lastPlot = 0f;
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
                graphPoint(script.xAxis / xScale, script.yAxis / yScale);
            }
            lastPlot = 0f;
        }

        if (Input.GetKeyDown(KeyCode.X))
            deletePoints();

        if (graphGameObject.activeSelf) {
            updateNumberLabels(); 

            minimumXAxis = (int) Camera.main.ScreenToWorldPoint(Vector3.zero).x;
            for (int i = 0; i < axisLabelsX.transform.childCount; i++) {
                axisLabelsX.transform.GetChild(i).position = Camera.main.WorldToScreenPoint(new Vector3((minimumXAxis + i - 1), 0, 0));
                axisLabelsX.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = (xScale * (minimumXAxis + i - 1)).ToString("F2");
            }
            minimumYAxis = (int) Camera.main.ScreenToWorldPoint(Vector3.zero).y;
            for (int i = 0; i < axisLabelsY.transform.childCount; i++) {
                axisLabelsY.transform.GetChild(i).position = Camera.main.WorldToScreenPoint(new Vector3(0, (minimumYAxis + i - 1), 0));
                axisLabelsY.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = (yScale * (minimumYAxis + i - 1)).ToString("F2");
            }
        }

            
    }

    public void graphPoint(float x, float y) {
        GameObject createdPoint = Instantiate(coordinatePoint, new Vector3(x, y, 0), Quaternion.identity, points.transform);
        createdPoint.transform.localScale = new Vector3(0.1f * xScale, 0.1f * yScale, 1f);

    }

    public void deletePoints() {
        Destroy(points.gameObject);
        points = new GameObject("points");
        points.transform.SetParent(graphGameObject.transform);
        lastPlot = 0f;
        points.transform.localScale = new Vector3(1/xScale, 1/yScale, 1f);

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


        points.transform.localScale = new Vector3(1/xScale, 1/yScale, 1f);
        for (int i = 0; i < points.transform.childCount; i++) {
            points.transform.GetChild(i).transform.localScale = new Vector3(0.1f * xScale, 0.1f * yScale, 1f);
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

    public void updateXAxis() {
        create.createdObjects[create.current].GetComponent<PointMass>().xAxisIndex = xAxisDropdown.GetComponent<TMP_Dropdown>().value;
    }

    public void updateYAxis() {
        create.createdObjects[create.current].GetComponent<PointMass>().yAxisIndex = yAxisDropdown.GetComponent<TMP_Dropdown>().value;
    }

    public void updateNumberLabels() {
        int minimumLabels = 2 + (int) Mathf.Ceil(GetComponent<RectTransform>().rect.width * transform.localScale.x / (
            Camera.main.WorldToScreenPoint(new Vector3(1, 0, 0)).x - Camera.main.WorldToScreenPoint(Vector3.zero).x) );
        if (axisLabelsX.transform.childCount < minimumLabels) {
            for (int i = 0; i < minimumLabels - axisLabelsX.transform.childCount; i++)
                Instantiate(axisLabel, new Vector3(0, 0, 0), Quaternion.identity, axisLabelsX.transform);
        } else if (axisLabelsX.transform.childCount > minimumLabels) {
            for (int i = minimumLabels; i < axisLabelsX.transform.childCount; i++)
                Destroy(axisLabelsX.transform.GetChild(i).gameObject);
        }

        minimumLabels = 2 + (int) Mathf.Ceil(GetComponent<RectTransform>().rect.height * transform.localScale.y / (
            Camera.main.WorldToScreenPoint(new Vector3(0, 1, 0)).y - Camera.main.WorldToScreenPoint(Vector3.zero).y) );
        if (axisLabelsY.transform.childCount < minimumLabels) {
            for (int i = 0; i < minimumLabels - axisLabelsY.transform.childCount; i++)
                Instantiate(axisLabel, new Vector3(0, 0, 0), Quaternion.identity, axisLabelsY.transform);
        } else if (axisLabelsY.transform.childCount > minimumLabels) {
            for (int i = minimumLabels; i < axisLabelsY.transform.childCount; i++) 
                Destroy(axisLabelsY.transform.GetChild(i).gameObject);
        }
    }
    
}
