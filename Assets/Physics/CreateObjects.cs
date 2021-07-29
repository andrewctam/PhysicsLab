using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;


public class CreateObjects : MonoBehaviour
{
    public GameObject announcementPrefab, bar, editor, buttons, toggleX, toggleY, toggleRot, toggleFrict, labGameObject, graphGameObject, toggleGraphingButton, labBar, graphBar, graphSettingsButton, objectSelector, objectSelectorContainer;
    public TextMeshProUGUI objectIDText, timer, startLabButtonText, graphButtonText, UIToggleText, selectorText;
    public TMP_InputField gravityInput, timeScaleInput, massInput, widthInput, heightInput, posx, posy, velx, vely, accX, accY, frictionInput, nameInput, selectorSearchInput;
    public List<GameObject> createdObjects, selectorsList, objectPrefabs;
    public Queue<GameObject> announcementQueue;
    public int indexOfLast, current, numObjects;
    public bool newObjectSelected, started, noObjectBeingDragged;
    public float elapsedTime, timeSpeed;
    public Graph grapher;
    public CameraControls cameraSettings;
    public Vector3 labCameraPos, graphCameraPos;
    public PhysicsMaterial2D mat, noFriction;

    [DllImport("__Internal")]
    private static extern string SaveLab(string saveInput);

    [DllImport("__Internal")]
    private static extern void LoadNewLab();

    [DllImport("__Internal")]
    private static extern void CheckForLoad();

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        Time.timeScale = 0;
        elapsedTime = -0.02f;
        timeSpeed = 1f;
        started = false;
        indexOfLast = -1;
        current = -1;
        createdObjects = new List<GameObject>();
        selectorsList = new List<GameObject>();
        
        announcementQueue = new Queue<GameObject>();

        graphCameraPos = new Vector3(0, 0, -5f);
        labCameraPos = new Vector3(0, 0, -5f);
        noObjectBeingDragged = true;

        CheckForLoad();
    }
    // Update is called once per frame

    void Update()
    {
        elapsedTime += Time.deltaTime;
        timer.text = formatClock(elapsedTime, 3);

        if (cameraSettings.keyboardAllowed && Input.GetKeyDown(KeyCode.R)) {
            newUrgentAnnouncement("Lab Reset and Graph Cleared", 60);
            elapsedTime = 0f;
            started = false;
            startLabButtonText.text = "Start"; 
            PointMass currentObj;
            for (int i = 0; i < indexOfLast + 1; i++)
                if (createdObjects[i] != null) {
                    currentObj = createdObjects[i].GetComponent<PointMass>();
                    currentObj.gameObject.transform.position = currentObj.pos0;
                    currentObj.started = false;
                    }
            Time.timeScale = 0;
            grapher.deletePoints();
            for (int i = 0; i < indexOfLast + 1; i++)
                if (createdObjects[i] != null)
                    createdObjects[i].transform.position = createdObjects[i].GetComponent<PointMass>().pos0;

                     for (int i = 0; i < indexOfLast + 1; i++)
                if (createdObjects[i] != null)
                    createdObjects[i].GetComponent<PointMass>().started = false;
        }
            
        if (cameraSettings.keyboardAllowed && Input.GetKeyDown(KeyCode.Space))
            if (started) {
                togglePause();
            } else {
                startLab();
            }
        
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (graphGameObject.activeSelf)
                toggleGraph();
            else if (graphBar.activeSelf)
                toggleGraphSettings();
            else if (editor.activeSelf)
                closeEditor();
        }
    }

    /*announcement methods*/

    public void newAnnouncement(string message, int framesToDestroy) {
        GameObject created = Instantiate(announcementPrefab, announcementPrefab.transform.position, Quaternion.identity, gameObject.transform);
        announcementQueue.Enqueue(created);
        created.GetComponent<Announcement>().setAnnouncement(message, framesToDestroy, false);
        
    }
    //urgent announcements are meant for short messages such as errors, actions, buttons clicks, etc.
    public void newUrgentAnnouncement(string message, int framesToDestroy) { 
        Announcement announcementReplaced = FindObjectOfType<Announcement>(); 
        if (announcementReplaced != null)
            if (announcementReplaced.urgent)
                announcementReplaced.destroyNow(); //there can only be one urgent announcement. other urgent messages will be destroyed
            else
                announcementReplaced.gameObject.SetActive(false); //non urgent announcements will be redisplyed once this urgent announcement is done
            
        GameObject created = Instantiate(announcementPrefab, announcementPrefab.transform.position, Quaternion.identity, gameObject.transform);
        created.GetComponent<Announcement>().setAnnouncement(message, framesToDestroy, true);
    }



    /*editor methods*/

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
    public void refreshPos() {
        PointMass obj = createdObjects[current].GetComponent<PointMass>();
        obj.transform.position = obj.pos0;
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
    public void refreshVelocity() {
        PointMass obj = createdObjects[current].GetComponent<PointMass>();
        obj.rb.velocity = obj.vel0;
    }

    public void updateName() {
        string inputtedName = nameInput.text;
        if (inputtedName.Contains("+") || inputtedName.Contains("~") || inputtedName.Contains(":"))
            nameInput.text = createdObjects[current].name;
        else {
            createdObjects[current].name = inputtedName;
            selectorsList[current].GetComponent<Selector>().updateSelectorName(nameInput.text);
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
    public void toggleFriction() {
        if (!newObjectSelected) {
            PointMass obj = createdObjects[current].GetComponent<PointMass>();
            obj.hasFriction = !obj.hasFriction;
            if (obj.hasFriction) 
                createdObjects[current].GetComponent<Collider2D>().sharedMaterial = mat;
            else
                createdObjects[current].GetComponent<Collider2D>().sharedMaterial = noFriction;

        }

    }

    public void updateFriction() {
        mat.friction  = float.Parse(frictionInput.text);
        labGameObject.transform.GetChild(0).GetComponent<Collider2D>().sharedMaterial = mat;

        foreach (GameObject obj in createdObjects) {
            if (obj != null && obj.GetComponent<PointMass>().hasFriction) {
                obj.GetComponent<Collider2D>().sharedMaterial = mat;
            }

        }
    }




    /*graph methods*/

    public void toggleGraph() {
        if (graphGameObject.activeSelf) {
            for (int i = 0; i < labGameObject.transform.childCount; i++)
                labGameObject.transform.GetChild(i).GetComponent<Renderer>().enabled = true;

            graphGameObject.SetActive(false);
            graphBar.SetActive(false);
            labBar.SetActive(true);
            grapher.axisLabelsX.transform.parent.gameObject.SetActive(false);

            cameraSettings.cameraBounded = true;
            
            graphCameraPos = cameraSettings.setCameraPosition(labCameraPos.x, labCameraPos.y, labCameraPos.z);
            
            graphButtonText.text = "Graph Display";

            for (int i = 0; i < graphBar.transform.childCount; i++) {
                GameObject barChild = graphBar.transform.GetChild(i).gameObject;
                if (barChild.name == "Object Specific Settings" ||
                    barChild.name == "Global Settings" || 
                    barChild.name == "Separator")
                    barChild.SetActive(true);
                else
                    barChild.SetActive(false);            
                }
                       
        } else {   
            for (int i = 0; i < labGameObject.transform.childCount; i++)
                labGameObject.transform.GetChild(i).GetComponent<Renderer>().enabled = false;

            graphGameObject.SetActive(true);
            graphBar.SetActive(true);
            labBar.SetActive(false);
            cameraSettings.cameraBounded = false;
            graphButtonText.text = "Lab Display";
            labCameraPos = cameraSettings.setCameraPosition(graphCameraPos.x, graphCameraPos.y, graphCameraPos.z);
            
            for (int i = 0; i < graphBar.transform.childCount; i++) {
                GameObject barChild = graphBar.transform.GetChild(i).gameObject;
                if (barChild.name == "Global Settings" ||
                    barChild.name == "Graph Settings")
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
    

    /*Creation methods*/


    public void createNewObject(GameObject prefabToCreate) {
        indexOfLast++;
        numObjects++;
        createdObjects.Add(Instantiate(prefabToCreate, new Vector3(0, 0, 0), Quaternion.identity, labGameObject.transform));
        createdObjects[indexOfLast].GetComponent<PointMass>().ID = indexOfLast;
        createdObjects[indexOfLast].GetComponent<PointMass>().started = started;

        createdObjects[indexOfLast].name = "Object " + indexOfLast;

        objectSelectorContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 32 * (indexOfLast + 1));


        GameObject createdSelector = Instantiate(objectSelector, new Vector3(0, 0, 0), Quaternion.identity, objectSelectorContainer.transform);
        selectorsList.Add(createdSelector);
        createdSelector.GetComponent<Selector>().updateInfo(indexOfLast, createdObjects[indexOfLast].name, createdObjects[indexOfLast].GetComponent<SpriteRenderer>()); 

        updateSelectors(true);
    }

    /* UI methods*/
    public void closeEditor() {
        editor.SetActive(false);
        buttons.SetActive(true);
        createdObjects[current].GetComponent<PointMass>().draggable = false;
        updateCurrent(-1);
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

    /* methods that control the lab */

    public void startLab() {                
        if (started) {
            newUrgentAnnouncement("Lab Stopped", 30);
            Time.timeScale = 0;
            started = false;
            startLabButtonText.text  = "Start"; 
            for (int i = 0; i < indexOfLast + 1; i++)
                if (createdObjects[i] != null)
                    createdObjects[i].GetComponent<PointMass>().started = false;
        } else {
            newUrgentAnnouncement("Lab Started", 30);
            started = true;
            startLabButtonText.text = "Stop";
            for (int i = 0; i < indexOfLast + 1; i++)
                if (createdObjects[i] != null)
                    createdObjects[i].GetComponent<PointMass>().initiateMovement();
            Time.timeScale = timeSpeed;
            elapsedTime = 0f;
        }
    }

    public void openEditorNewObject() {
        newObjectSelected = true;
        editor.SetActive(true);
        graphBar.SetActive(false);
        buttons.SetActive(false);
        labBar.SetActive(true);

        
        PointMass currentPointMassScript = createdObjects[current].GetComponent<PointMass>();
        if (currentPointMassScript.isGraphing) 
            toggleGraphingButton.GetComponent<Image>().color = new Color(0.5f, 0.8f, 0.8f);
        else
            toggleGraphingButton.GetComponent<Image>().color = Color.white;
            
        createdObjects[current].GetComponent<SpriteRenderer>().color = currentPointMassScript.defaultColor;
        selectorsList[current].GetComponent<Image>().color = new Color(0.5f, 0.8f, 0.8f);
        graphSettingsButton.SetActive(currentPointMassScript.isGraphing);
        grapher.xAxisDropdown.value = currentPointMassScript.xAxisIndex;
        grapher.yAxisDropdown.value = currentPointMassScript.yAxisIndex;
        grapher.colorSlider.value = grapher.colorToInt(createdObjects[current].GetComponent<PointMass>().graphPointColor);
        grapher.pointColorDisplay.color = createdObjects[current].GetComponent<PointMass>().graphPointColor;


        objectIDText.text = "Object ID: " + currentPointMassScript.ID;
        nameInput.text = currentPointMassScript.name;
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
        toggleFrict.GetComponent<Toggle>().isOn = currentPointMassScript.hasFriction;
        toggleRot.GetComponent<Toggle>().isOn = !currentPointMassScript.canRotate; //not since the toggle is Lock Rotation
        toggleX.GetComponent<Toggle>().isOn = !currentPointMassScript.canTranslateX;
        toggleY.GetComponent<Toggle>().isOn = !currentPointMassScript.canTranslateY;
        newObjectSelected = false;
    }

    public void destroyCurrent() {
        numObjects--;
        grapher.graphedObjects.Remove(createdObjects[current].gameObject);
        Destroy(createdObjects[current].gameObject);
        Destroy(selectorsList[current].gameObject);
        selectorsList[current] = null;
        createdObjects[current] = null;
        updateSelectors(true);
        editor.SetActive(false);
        buttons.SetActive(true); 
        current = -1;      
    }

    public bool updateCurrent(int updated) { //returns if current has changed
        if (current == updated)
            return false;
        else {
            if (current != -1) {
                createdObjects[current].GetComponent<SpriteRenderer>().color = createdObjects[current].GetComponent<PointMass>().alphaDefaultColor;
                selectorsList[current].GetComponent<Image>().color = Color.white;
            }

            current = updated;
        }

        return true;
    } 

    /* methods relating to time */
    public string formatClock(float num, int accuracy) {
        string[] decimalParts = (num + ".").Split('.');
        for (int i = 0; i < accuracy; i++)
            decimalParts[1] += "0";

        return ((int) num / 60 ) + ":" + ((int) num % 60) + "." + decimalParts[1].Substring(0, accuracy);        
    }

    public void togglePause() {
        if (started)
            if (Time.timeScale == 0) {
                Time.timeScale = timeSpeed;
                newUrgentAnnouncement("Lab Resumed", 30);
            } else {
                Time.timeScale = 0f;
                newUrgentAnnouncement("Lab Paused", 30);
            }
    }

    public void updateSelectors(bool search) {
        string searchQuery = selectorSearchInput.text.ToUpper();
        int currentIndex = 0;


        if (numObjects < 1)
            selectorText.text = "No Objects Created";
        else if (search || searchQuery != "") {
            for (int i = 0; i < selectorsList.Count; i++) {
                if (createdObjects[i] != null) {
                    if (createdObjects[i].name.ToUpper().Contains(searchQuery) || searchQuery == i + "") {
                        selectorsList[i].SetActive(true);
                        selectorsList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, currentIndex * -32);
                        currentIndex++;
                    } else {
                        selectorsList[i].SetActive(false);
                    }
                }
            }
            
            if (currentIndex == 0)
                selectorText.text = "No Results Found";
            else
                selectorText.text = "";


        } else {
            for (int i = 0; i < selectorsList.Count; i++) {
                if (createdObjects[i] != null) {
                    selectorsList[i].SetActive(true);
                    selectorsList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, currentIndex * -32);
                    currentIndex++;
                }
            }
            selectorText.text = "";
        }

        objectSelectorContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 32 * currentIndex);
    }





    public float roundToTwo(float num) {
        return Mathf.Round(num * 100f) / 100f;
    }

    public void saveLab() {
        string outputString = $"{-Physics2D.gravity.y}:{timeSpeed}:{grapher.xScale}:{grapher.yScale}:{grapher.plotFrequency}:{frictionInput.text}:{labCameraPos.x}:{labCameraPos.y}:{labCameraPos.z}:{graphCameraPos.x}:{graphCameraPos.y}:{graphCameraPos.z}~";
            
        bool savePoints = true;
        if (savePoints) {
            GameObject point;
            Transform pointsContainer = grapher.points.transform;
            for (int i = 0; i < pointsContainer.childCount; i++) {
                point = pointsContainer.GetChild(i).gameObject;
                outputString += $"{roundToTwo(point.transform.position.x)}:{roundToTwo(point.transform.position.y)}:{grapher.colorToInt(point.GetComponent<SpriteRenderer>().color)}:";
            } 
        }
        outputString += "~";

        foreach (GameObject obj in createdObjects) {
            if (obj == null)
                outputString += "~";
            else
                outputString += obj.GetComponent<PointMass>().ToString();
        }

        SaveLab("andrewtam.org/PhysicsLab?" + Compression.Compress(outputString));
    }
    public void loadLabJS() {LoadNewLab();}
    public void loadLab(string inputString) {
        bool strToBool(string one) {return one == "1";}
        
        inputString = Compression.Decompress(inputString);
        
        string[] parts = inputString.Split('~');
        string[] settings = parts[0].Split(':');

        float g = float.Parse(settings[0]);
        Physics2D.gravity = new Vector3(0f, -g, 0f);
        gravityInput.text = g.ToString(); 

        timeSpeed = float.Parse(settings[1]);
        timeScaleInput.text = timeSpeed.ToString();

        grapher.xScale = float.Parse(settings[2]);
        grapher.xScaleInput.text = grapher.xScale.ToString();

        grapher.yScale = float.Parse(settings[3]);
        grapher.yScaleInput.text = grapher.yScale.ToString();
        
        grapher.points.transform.localScale = new Vector3(1/grapher.xScale, 1/grapher.yScale, 1f);
        
        grapher.plotFrequency = float.Parse(settings[4]);
        grapher.plotFreqInput.text = grapher.plotFrequency.ToString(); 

        frictionInput.text = settings[5];
        labCameraPos = new Vector3(float.Parse(settings[6]), float.Parse(settings[7]), float.Parse(settings[8]));
        graphCameraPos = new Vector3(float.Parse(settings[9]), float.Parse(settings[10]), float.Parse(settings[11]));
        
        grapher.deletePoints();
        string[] graphPoints = parts[1].Split(':');
        if (graphPoints[0] != "") {
            for (int i = 0; i < graphPoints.Length - 1; i+=3) {
                grapher.graphPoint(float.Parse(graphPoints[i]), 
                                   float.Parse(graphPoints[i + 1]), 
                                   grapher.intToColor(int.Parse(graphPoints[i + 2])));
            }
        }


        for (int i = 0; i < indexOfLast + 1; i++) {
            Destroy(createdObjects[i]);
            Destroy(selectorsList[i]);
        }
        createdObjects = new List<GameObject>();
        selectorsList = new List<GameObject>();
        indexOfLast = -1;
        current = -1;
        numObjects = 0;
        
        string[] currentObject;
        PointMass objScript;
        GameObject createdSelector;
        for (int i = 2; i < parts.Length - 1; i++) {
            currentObject = parts[i].Split(':');
            if (currentObject[0] == "") {
                createdObjects.Add(null);
                selectorsList.Add(null);
            } else {
                numObjects++;
                createdObjects.Add(Instantiate(objectPrefabs[int.Parse(currentObject[0])], new Vector3(0, 0, 0), Quaternion.identity, labGameObject.transform));
                createdObjects[i - 2].name = currentObject[1].Replace("+", " ");
                createdObjects[i - 2].GetComponent<Rigidbody2D>().mass = float.Parse(currentObject[2]); 
                createdObjects[i - 2].transform.localScale = new Vector3(float.Parse(currentObject[3]), float.Parse(currentObject[4]), 1f);


                Debug.Log(currentObject[5] + " " + currentObject[6]);
                objScript = createdObjects[i - 2].GetComponent<PointMass>();
                objScript.ID = i - 2;
                objScript.pos0 = new Vector3(float.Parse(currentObject[5]), float.Parse(currentObject[6]), 0f);
                objScript.vel0 = new Vector3(float.Parse(currentObject[7]), float.Parse(currentObject[8]), 0f);
                objScript.acc0 = new Vector3(float.Parse(currentObject[9]), float.Parse(currentObject[10]), 0f);

                objScript.xAxisIndex = int.Parse(currentObject[11]);
                objScript.yAxisIndex = int.Parse(currentObject[12]);

                objScript.isGraphing = strToBool(currentObject[13]);
                
                objScript.canRotate = strToBool(currentObject[14]);
                if (!strToBool(currentObject[14]))
                    createdObjects[i - 2].GetComponent<Rigidbody2D>().constraints ^= RigidbodyConstraints2D.FreezeRotation;

                objScript.canTranslateX = strToBool(currentObject[15]);
                if (!strToBool(currentObject[15]))
                    createdObjects[i - 2].GetComponent<Rigidbody2D>().constraints ^= RigidbodyConstraints2D.FreezePositionX;


                objScript.canTranslateY = strToBool(currentObject[16]);
                if (!strToBool(currentObject[16]))
                    createdObjects[i - 2].GetComponent<Rigidbody2D>().constraints ^= RigidbodyConstraints2D.FreezePositionY;


                
                objScript.hasFriction = strToBool(currentObject[17]);

                objScript.graphPointColor = grapher.intToColor(int.Parse(currentObject[18]));

                if (objScript.isGraphing)        
                    grapher.graphedObjects.Add(createdObjects[i - 2]);

                objectSelectorContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 32 * (i - 2 + 1));
                createdSelector = Instantiate(objectSelector, new Vector3(0, 0, 0), Quaternion.identity, objectSelectorContainer.transform);
                selectorsList.Add(createdSelector);
                createdSelector.GetComponent<Selector>().updateInfo(i - 2, createdObjects[i - 2].name, createdObjects[i - 2].GetComponent<SpriteRenderer>()); 
            }
        }

        
        updateSelectors(true);
        updateFriction();
        PointMass currentObj;
        indexOfLast = createdObjects.Count - 1;
        Time.timeScale = 0;
        for (int i = 0; i < indexOfLast + 1; i++)
            if (createdObjects[i] != null) {
                currentObj = createdObjects[i].GetComponent<PointMass>();
                currentObj.gameObject.transform.position = currentObj.pos0;
            }  
    } 
}
