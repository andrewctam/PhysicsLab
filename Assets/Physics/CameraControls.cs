using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CameraControls : MonoBehaviour
{
    public float speed, xBoundMin, xBoundMax, yBoundMin, yBoundMax, dragSpeed, dragKey;
    public bool cameraBounded, dragging;
    public Vector3 currentPos, posLastFrame;
    public CreateObjects create;
    public bool interacting, keyboardAllowed;
    
 
    void Start() {
        speed = 0.03f;
        cameraBounded = true;
        xBoundMin = -100000f;
        xBoundMax = 100000f;
        yBoundMin = -5f;
        yBoundMax = 100000f;
    }
    
    void Update()
    {
        if (cameraBounded) {
            if (Camera.main.transform.position.x < xBoundMin)
                Camera.main.transform.position = new Vector3(xBoundMin, Camera.main.transform.position.y, -Camera.main.orthographicSize);
            if (Camera.main.transform.position.x > xBoundMax)
                Camera.main.transform.position = new Vector3(xBoundMax, Camera.main.transform.position.y, -Camera.main.orthographicSize);

            if (Camera.main.transform.position.y < yBoundMin)
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, yBoundMin, -Camera.main.orthographicSize);
            if (Camera.main.transform.position.y > yBoundMax)
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, yBoundMax, -Camera.main.orthographicSize);
        }
        
        interacting = EventSystem.current.IsPointerOverGameObject();
        GameObject UISelected = EventSystem.current.currentSelectedGameObject;
        keyboardAllowed = UISelected == null || UISelected.GetComponent<TMP_InputField>() == null;
        
        if (keyboardAllowed) {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                transform.Translate(new Vector3(speed  *  Camera.main.orthographicSize, 0, 0));

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                transform.Translate(new Vector3(-speed  *  Camera.main.orthographicSize, 0, 0));

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                transform.Translate(new Vector3(0, -speed  *  Camera.main.orthographicSize, 0));

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                transform.Translate(new Vector3(0, speed  *  Camera.main.orthographicSize, 0));
                
            if (Input.GetKey(KeyCode.E))
                Camera.main.orthographicSize -= 0.1f;
            else if (Input.GetKey(KeyCode.Q))
                Camera.main.orthographicSize += 0.1f;

            if (Input.GetKey(KeyCode.Z)) {
                setCameraPosition(0f, 0f, -5f);
            }
        }




        if (Input.GetMouseButtonDown(0) && (!interacting || (
            create.graphGameObject.activeSelf && keyboardAllowed
            ))) //Graph is shown, and a label is selected.
            dragging = true;
        else if (Input.GetMouseButtonUp(0)) 
            dragging = false;

        
        if (create.noObjectBeingDragged) {
            posLastFrame = currentPos;
            currentPos = Input.mousePosition;
            if (dragging && currentPos != posLastFrame)   
                transform.Translate(Camera.main.ScreenToWorldPoint(posLastFrame) - Camera.main.ScreenToWorldPoint(currentPos)); 
        }
        
        Camera.main.orthographicSize += -Input.mouseScrollDelta.y;
        if (Camera.main.orthographicSize < 1f)
            Camera.main.orthographicSize = 1f;
        else if (Camera.main.orthographicSize > 100f) {
            Camera.main.orthographicSize = 100f;
            create.newUrgentAnnouncement("Scroll Limit Reached", 30);    
        }
        
    }
    
    public Vector3 setCameraPosition (float x, float y, float zoom) {
        Vector3 oldPos = Camera.main.transform.position;
        float oldZoom = Camera.main.orthographicSize;
        Camera.main.transform.position = new Vector3(x, y, zoom);
        Camera.main.orthographicSize = -zoom;

        return new Vector3(oldPos.x, oldPos.y, -oldZoom);
    }

    
}