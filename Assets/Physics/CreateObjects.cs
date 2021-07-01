using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CreateObjects : MonoBehaviour
{
    public GameObject announcement, pointMass, ramp, bowlingBall;
    public GameObject bar, editor, buttons, toggleX, toggleY, toggleRot, labGameObject, graphGameObject, graphButton, labBar, graphBar, graphSettings;
    public Text objectIDText, timer, startLabButtonText, graphButtonText, UIToggleText;
    public InputField gravityInput, timeScaleInput, massInput, widthInput, heightInput, posx, posy, velx, vely, accX, accY;
    public List<GameObject> createdObjects;
    public Queue<GameObject> announcementQueue;
    public int count, current;
    public bool newObjectSelected, started, noAnnouncementDisplayed, noObjectBeingDragged;
    public float elapsedTime, lastPlot, timeSpeed;
    public Graph grapher;
    public CameraControls cameraSettings;
    public Vector3 labCameraPos, graphCameraPos;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        Time.timeScale = 0;
        elapsedTime = -0.02f;
        lastPlot = -0.02f;

        timeSpeed = 1f;
        started = false;
        count = -1;
        current = -1;
        createdObjects = new List<GameObject>();
        announcementQueue = new Queue<GameObject>();

        graphCameraPos = new Vector3(0, 0, 0);
        labCameraPos = new Vector3(0, 0, 0);
        noAnnouncementDisplayed = true;
        noObjectBeingDragged = true;
    }
    // Update is called once per frame

    void Update()
    {
        elapsedTime += Time.deltaTime;
        lastPlot += Time.deltaTime;

        timer.text = formatClock(elapsedTime, 3);

        if (noAnnouncementDisplayed && announcementQueue.Count > 0) {
            announcementQueue.Dequeue().SetActive(true);
            noAnnouncementDisplayed = false; 
        }
        

        if (Input.GetKeyDown(KeyCode.R)) {
            elapsedTime = 0f;
            started = false;
            startLabButtonText.text = "Start"; 
            for (int i = 0; i < count + 1; i++)
                if (createdObjects[i] != null)
                    createdObjects[i].GetComponent<PointMass>().started = false;
            Time.timeScale = 0;
            grapher.deletePoints();
            for (int i = 0; i < count + 1; i++)
                if (createdObjects[i] != null)
                    createdObjects[i].transform.position = createdObjects[i].GetComponent<PointMass>().pos0;
        }
            
            
        if (Input.GetKeyDown(KeyCode.Space))
            if (started) {
                togglePause();
            } else
                startLab();
        

    }

    public void newAnnouncement(string message, int framesToDestroy) {
        GameObject created = Instantiate(announcement, announcement.transform.position, Quaternion.identity, gameObject.transform);
        created.GetComponent<Announcement>().setAnnouncement(message, framesToDestroy);
        created.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100f);
        announcementQueue.Enqueue(created);
        
    }

    public void togglePause() {
        if (started)
            if (Time.timeScale == 0)
                Time.timeScale = timeSpeed;
            else
                Time.timeScale = 0f;

    }
    public void openEditorNewObject() {
            newObjectSelected = true;
            editor.SetActive(true);
            graphBar.SetActive(false);
            buttons.SetActive(false);
            labBar.SetActive(true);

            
            PointMass currentPointMassScript = createdObjects[current].GetComponent<PointMass>();
            if (currentPointMassScript.isGraphing) 
                graphButton.GetComponent<Image>().color = new Color(0.5f, 0.8f, 0.8f);
            else
                graphButton.GetComponent<Image>().color = Color.white;

            graphSettings.SetActive(currentPointMassScript.isGraphing);
            grapher.xAxisDropdown.value = currentPointMassScript.xAxisIndex;
            grapher.yAxisDropdown.value = currentPointMassScript.yAxisIndex;


            objectIDText.text = "Object ID: " + createdObjects[current].name;

            // Update Input Fields with current info
            massInput.text = createdObjects[current].GetComponent<Rigidbody2D>().mass + "";
            heightInput.text = createdObjects[current].transform.localScale.y + "";
            widthInput.text = createdObjects[current].transform.localScale.x + "";

            //Update vectors
            posx.text = currentPointMassScript.pos0.x + "";
            posy.text = currentPointMassScript.pos0.y + "";
            velx.text = currentPointMassScript.vel0.x + "";
            vely.text = currentPointMassScript.vel0.y + "";
            accX.text = currentPointMassScript.acc0.x + "";
            accY.text = currentPointMassScript.acc0.y + "";
            
            //Update checkboxes
            toggleRot.GetComponent<Toggle>().isOn = !currentPointMassScript.canRotate;
            toggleX.GetComponent<Toggle>().isOn = !currentPointMassScript.canTranslateX;
            toggleY.GetComponent<Toggle>().isOn = !currentPointMassScript.canTranslateY;
            newObjectSelected = false;
    }


    public void updateEarthGravity() {
        Physics2D.gravity = new Vector2(0, -Mathf.Abs(float.Parse(gravityInput.text)));


    }

    public void updateTimeScale() {
        float newValue = float.Parse(timeScaleInput.text);
        if (newValue > 100f) {
            newValue = 100;
            timeScaleInput.text = "100";
        } else if (newValue < 0.001f) {
            newValue = 0.001f;
            timeScaleInput.text = "0.001";
        }

        timeSpeed = newValue;
        if (Time.timeScale != 0)
            Time.timeScale = timeSpeed;
    }

    public void updateScale() {
        if (!newObjectSelected) {
            float width = float.Parse(widthInput.text);
            float height = float.Parse(heightInput.text);
            createdObjects[current].transform.localScale = new Vector3(width, height, 1); 
        }
    }
        
    public void updateMass() {
        if (!newObjectSelected) {
            createdObjects[current].GetComponent<Rigidbody2D>().mass = float.Parse(massInput.text);
        }
    }
    
    public void updateInitialPos() {
        if (!newObjectSelected) {
            //Update Vectors
            float x = 0;
            float y = 0;
            
            x = float.Parse(posx.text);
            y = float.Parse(posy.text);
            createdObjects[current].GetComponent<PointMass>().pos0 =  new Vector3(x, y, 0);
            if (!started && !createdObjects[current].GetComponent<PointMass>().isPressed)
               createdObjects[current].transform.position = new Vector3(x, y, 0);
        }
    }

    public void updateInitialVel() {
        if (!newObjectSelected) {
            float x = 0;
            float y = 0;
            x = float.Parse(velx.text);
            y = float.Parse(vely.text);

            if (x > 1000) {
                velx.text = "1000";
                x = 1000;
                newAnnouncement("The maximum velocity is 1000. For larger velocities, use another unit for this lab such as kilometers instead of meters.", 360);
            }
            if (y > 1000) {
                vely.text = "1000";
                y = 1000;
                newAnnouncement("The maximum velocity is 1000. For larger velocities, use another unit for this lab such as kilometers instead of meters.", 360);
            }
            createdObjects[current].GetComponent<PointMass>().vel0 = new Vector3(x, y, 0);
        }
    }

    public void updateInitialAcc() {
        if (!newObjectSelected) {
            float x = 0;
            float y = 0;
            x = float.Parse(accX.text);
            y = float.Parse(accY.text);
            createdObjects[current].GetComponent<PointMass>().acc0 = new Vector3(x, y, 0);
        }
    }

    public void toggleRotation() {
        if (!newObjectSelected) { //prevent updating the editor from toggling
            createdObjects[current].GetComponent<Rigidbody2D>().constraints ^= RigidbodyConstraints2D.FreezeRotation ;
            createdObjects[current].GetComponent<PointMass>().canRotate = !createdObjects[current].GetComponent<PointMass>().canRotate; 
        }
    }

    public void toggleVertical() {
        if (!newObjectSelected) {
            createdObjects[current].GetComponent<Rigidbody2D>().constraints ^= RigidbodyConstraints2D.FreezePositionY;
            createdObjects[current].GetComponent<PointMass>().canTranslateY = !createdObjects[current].GetComponent<PointMass>().canTranslateY; 
        }
    }

    public void toggleHorizontal() {
        if (!newObjectSelected) {
            createdObjects[current].GetComponent<Rigidbody2D>().constraints ^= RigidbodyConstraints2D.FreezePositionX;
            createdObjects[current].GetComponent<PointMass>().canTranslateX = !createdObjects[current].GetComponent<PointMass>().canTranslateX; 
        }
    }

    public void toggleGraph() {
        if (graphGameObject.activeSelf) {
            for (int i = 0; i < labGameObject.transform.childCount; i++)
                labGameObject.transform.GetChild(i).GetComponent<Renderer>().enabled = true;

            graphGameObject.SetActive(false);
            graphBar.SetActive(false);
            labBar.SetActive(true);
            cameraSettings.cameraBounded = true;
            graphCameraPos = cameraSettings.setCameraPosition(labCameraPos.x, labCameraPos.y);

            graphButtonText.text = "Graph Display";

            for (int i = 0; i < graphBar.transform.childCount; i++) {
                graphBar.transform.GetChild(i).gameObject.SetActive(true);
            }
            
        } else {   
            for (int i = 0; i < labGameObject.transform.childCount; i++)
                labGameObject.transform.GetChild(i).GetComponent<Renderer>().enabled = false;

            graphGameObject.SetActive(true);

            graphBar.SetActive(true);
            labBar.SetActive(false);
            cameraSettings.cameraBounded = false;
            graphButtonText.text = "Lab Display";
            labCameraPos = cameraSettings.setCameraPosition(graphCameraPos.x, graphCameraPos.y);
            
            for (int i = 0; i < graphBar.transform.childCount; i++) {
                GameObject barChild = graphBar.transform.GetChild(i).gameObject;
                if (barChild.name == "Global Settings")
                    barChild.SetActive(true);
                else
                    barChild.SetActive(false);
            }

                
        }
    }   

    public void toggleGraphSettings() {
        if (labBar.activeSelf) {
            graphBar.SetActive(true);
            labBar.SetActive(false);
        } else {
            graphBar.SetActive(false);
            labBar.SetActive(true);
        }
    }
    

    public void CreatePointMass() {
        count++;
        createdObjects.Add(Instantiate(pointMass, new Vector3(0, 0, 0), Quaternion.identity, labGameObject.transform));
        createdObjects[createdObjects.Count - 1].name = count + "";

    }

    public void CreateRamp() {
        count++;
        createdObjects.Add(Instantiate(ramp, new Vector3(0, 0, 0), Quaternion.identity, labGameObject.transform));
        createdObjects[createdObjects.Count - 1].name = count + "";
    }

    public void CreateBowlingBall() {
        count++;
        createdObjects.Add(Instantiate(bowlingBall, new Vector3(0, 0, 0), Quaternion.identity, labGameObject.transform));
        createdObjects[createdObjects.Count - 1].name = count + "";
    }

    public void closeEditor() {
        editor.SetActive(false);
        buttons.SetActive(true);
        createdObjects[current].GetComponent<PointMass>().draggable = false;
        current = -1;
    }

    public void destroyCurrent() {
        grapher.graphedObjects.Remove(createdObjects[current].gameObject);
        Destroy(createdObjects[current].gameObject);
        createdObjects[current] = null;
        editor.SetActive(false);
        buttons.SetActive(true); 
        current = -1;        
    }

    public bool updateCurrent(int updated) { //returns if current has changed
        if (current == updated)
            return false;
        else
            current = updated;

        return true;
    } 

    public void startLab() {
        if (started) {
            Time.timeScale = 0;
            started = false;
            startLabButtonText.text  = "Start"; 
            for (int i = 0; i < count + 1; i++)
                if (createdObjects[i] != null)
                    createdObjects[i].GetComponent<PointMass>().started = false;
        } else {
            started = true;
            startLabButtonText.text = "Stop";
            for (int i = 0; i < count + 1; i++)
                if (createdObjects[i] != null)
                    createdObjects[i].GetComponent<PointMass>().initiateMovement();
            Time.timeScale = timeSpeed;
            elapsedTime = 0f;
        }
    }

    public string formatClock(float num, int accuracy) {
        string[] decimalParts = (num + ".").Split('.');
        for (int i = 0; i < accuracy; i++)
            decimalParts[1] += "0";

        return ((int) num / 60 ) + ":" + ((int) num % 60) + "." + decimalParts[1].Substring(0, accuracy);        
    }


    public void closeBar() {
        if (bar.activeSelf) {
            bar.SetActive(false);
            UIToggleText.text = "Open UI";
        } else {
            bar.SetActive(true);
            UIToggleText.text = "Close UI";
        }
    }




}
