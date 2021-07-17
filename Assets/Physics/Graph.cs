using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class Graph : MonoBehaviour
{
    public float lastPlot, plotFrequency, xScale, yScale, canvasWidth, canvasHeight;
    public GameObject graphGameObject, coordinatePoint, points, axisLabelsX, axisLabelsY, axisLabel, axisGridsX, axisGridsY, XAxisPrefab, YAxisPrefab;
    public CreateObjects create;
    public TMP_InputField xScaleInput, yScaleInput, plotFreqInput;
    public List<GameObject> graphedObjects;
    public TMP_Dropdown xAxisDropdown, yAxisDropdown;
    public int minimumXAxis, minimumYAxis, step;
   
    // Start is called before the first frame update
    void Start()
    {
        plotFrequency = 0.5f;
        lastPlot = 0f;
        xScale = 1f;
        yScale = 1f;
        step = 1;
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
            create.toggleGraphingButton.GetComponent<Image>().color = new Color(0.5f, 0.8f, 0.8f);
        } else {
            graphedObjects.Remove(currentGameObj);
            create.graphSettings.SetActive(false);
            currentGameObj.GetComponent<PointMass>().isGraphing = false;
            create.toggleGraphingButton.GetComponent<Image>().color = Color.white;

        }
    }
    
    public void updateScale() {
        xScale = float.Parse(xScaleInput.text);
        yScale = float.Parse(yScaleInput.text);
        
        if (xScale > 10000) {
            xScale = 10000;
            xScaleInput.text = "10000";
            create.newAnnouncement("Scale Limit is 10,000", 120);

        } else if (xScale < -10000) {
            xScale = -10000;
            xScaleInput.text = "-10000";
            create.newAnnouncement("Scale Limit is -10,000", 120);
        } else if (xScale == 0) {
            xScale = 1;
            xScaleInput.text = "1";
            create.newAnnouncement("Scale can not be 0", 120);
        }

        if (yScale > 10000) {
            yScale = 10000;
            yScaleInput.text = "10000";
            create.newAnnouncement("Scale Limit is 10,000", 120);

        } else if (yScale < -10000) {
            yScale = -10000;
            yScaleInput.text = "-10000";
            create.newAnnouncement("Scale Limit is -10,000", 120);
        } else if (yScale == 0) {
            yScale = 1;
            yScaleInput.text = "1";
            create.newAnnouncement("Scale can not be 0", 120);
        }

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
        float zoomLevel = Camera.main.orthographicSize;
            if      (zoomLevel > 90f) step = 50;
            else if (zoomLevel > 80f) step = 45;
            else if (zoomLevel > 70f) step = 35;
            else if (zoomLevel > 60f) step = 30;
            else if (zoomLevel > 40f) step = 25;
            else if (zoomLevel > 25f) step = 15;
            else if (zoomLevel > 20f) step = 10;
            else if (zoomLevel > 15f) step = 5; 
            else if (zoomLevel > 10f) step = 3;  
            else if (zoomLevel > 5f)  step = 2;
            else                      step = 1;

        //Create or Destroy Extra labels if the canvas size changes or if the user zooms in/out.
        canvasWidth = GetComponent<RectTransform>().rect.width * transform.localScale.x;
        int minimumLabels = 2 + (int) Mathf.Ceil( canvasWidth / 
            (Camera.main.WorldToScreenPoint(new Vector3(step, 0, 0)).x - Camera.main.WorldToScreenPoint(Vector3.zero).x) );
        
        if (axisLabelsX.transform.childCount < minimumLabels) {
            for (int i = 0; i < minimumLabels - axisLabelsX.transform.childCount; i++) {
                Instantiate(axisLabel, new Vector3(0, 0, 0), Quaternion.identity, axisLabelsX.transform);
                Instantiate(XAxisPrefab, new Vector3(0, 0, 0), Quaternion.identity, axisGridsX.transform);
            }
        } else if (axisLabelsX.transform.childCount > minimumLabels) {
            for (int i = minimumLabels; i < axisLabelsX.transform.childCount; i++) {
                Destroy(axisLabelsX.transform.GetChild(i).gameObject);
                Destroy(axisGridsX.transform.GetChild(i).gameObject);
            }
        }

        canvasHeight = GetComponent<RectTransform>().rect.height * transform.localScale.y;
        minimumLabels = 2 + (int) Mathf.Ceil( canvasHeight / 
            (Camera.main.WorldToScreenPoint(new Vector3(0, step, 0)).y - Camera.main.WorldToScreenPoint(Vector3.zero).y) );

        if (axisLabelsY.transform.childCount < minimumLabels) {
            for (int i = 0; i < minimumLabels - axisLabelsY.transform.childCount; i++) {
                Instantiate(axisLabel, new Vector3(0, 0, 0), Quaternion.identity, axisLabelsY.transform);
                Instantiate(YAxisPrefab, new Vector3(0, 0, 0), Quaternion.identity, axisGridsY.transform);
            
            }
        } else if (axisLabelsY.transform.childCount > minimumLabels) {
            for (int i = minimumLabels; i < axisLabelsY.transform.childCount; i++) {
                Destroy(axisLabelsY.transform.GetChild(i).gameObject);
                Destroy(axisGridsY.transform.GetChild(i).gameObject);
            }
        }
        
        //Determine the text and position of labels. There are always two extra labels to allow for the "wrap around" effect
        //Once the camera scrolls by the value of step, minimumXAxis will increase by 1. The label at (i-1) will be moved to the label at i, and each label will have their text
        //increased appropriately based on the scale and step.
        int padding  = 35;
        minimumXAxis = (int) (Camera.main.ScreenToWorldPoint(Vector3.zero).x / step); //At the bottom left corner of the screen (0,0) find out this coordinate in the world
        for (int i = 0; i < axisLabelsX.transform.childCount; i++) {
            Vector3 pos = Camera.main.WorldToScreenPoint(new Vector3(step * (minimumXAxis + i - 1), 0, 0));
            if (pos.y < padding)
                pos.y = padding;
            else if (pos.y > canvasHeight - padding)
                pos.y = canvasHeight - padding;
            
            axisLabelsX.transform.GetChild(i).position = pos;
            axisGridsX.transform.GetChild(i).position = new Vector3(pos.x, canvasHeight / 2, 0);

            axisLabelsX.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = formatLabel(xScale * step *(minimumXAxis + i - 1));
        }
        
        minimumYAxis = (int) (Camera.main.ScreenToWorldPoint(Vector3.zero).y / step);
        for (int i = 0; i < axisLabelsY.transform.childCount; i++) {
            Vector3 pos = Camera.main.WorldToScreenPoint(new Vector3(0, step * (minimumYAxis + i - 1), 0));
            if (pos.x < padding)
                pos.x = padding;
            else if (pos.x > canvasWidth - padding)
                pos.x = canvasWidth - padding;

            axisLabelsY.transform.GetChild(i).position = pos;
            axisGridsY.transform.GetChild(i).position = new Vector3(canvasWidth / 2, pos.y,  0);

            axisLabelsY.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = formatLabel(yScale * step * (minimumYAxis + i - 1));
        }
    }

    public string formatLabel(float num) {
        if (num >= 10000 ||
            num <= -10000 ||
            (num > 0 && num < 0.01) ||
            (num < 0 && num > -0.01)) {
            string result = num.ToString("e1");

            if (result.Contains("+00"))
                result = result.Replace("+00", "");
            else if (result.Contains("-00"))
                result = result.Replace("-00", "-");
            else if (result.Contains("+0"))
                result = result.Replace("+0", "");
            else if (result.Contains("-0"))
                result = result.Replace("-0", "-");

            return result;
        } else
            return Mathf.Round(num * 100f) / 100f + "";
    }
    
}
