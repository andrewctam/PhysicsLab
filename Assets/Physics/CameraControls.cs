using UnityEngine;
 
public class CameraControls : MonoBehaviour
{
    public float speed, xBoundMin, xBoundMax, yBoundMin, yBoundMax, dragSpeed, dragKey;
    public bool cameraBounded, dragging;
    public Vector3 currentPos, posLastFrame;
    public CreateObjects create;
    
 
    void Start() {
        speed = 0.03f;
        cameraBounded = true;
        xBoundMin = -100f;
        xBoundMax = 100f;
        yBoundMin = -5f;
        yBoundMax = 100f;
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            dragging = true;
        else if (Input.GetMouseButtonUp(0)) 
            dragging = false;

        if (create.noObjectBeingDragged || !cameraBounded) {
            posLastFrame = currentPos;
            currentPos = (Input.mousePosition);
            if (dragging && currentPos != posLastFrame)   
                transform.Translate(Camera.main.ScreenToWorldPoint(posLastFrame) - Camera.main.ScreenToWorldPoint(currentPos)); 
        }



        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            transform.Translate(new Vector3(speed  *  Camera.main.orthographicSize, 0, 0));

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            transform.Translate(new Vector3(-speed  *  Camera.main.orthographicSize, 0, 0));

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            transform.Translate(new Vector3(0, -speed  *  Camera.main.orthographicSize, 0));

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            transform.Translate(new Vector3(0, speed  *  Camera.main.orthographicSize, 0));



        if (Input.GetKey(KeyCode.Z))
            setCameraPosition(0f, 0f);
        
        if (cameraBounded) {
            if (Camera.main.transform.position.x < xBoundMin)
                Camera.main.transform.position = new Vector3(xBoundMin, Camera.main.transform.position.y, -1);
            if (Camera.main.transform.position.x > xBoundMax)
                Camera.main.transform.position = new Vector3(xBoundMax, Camera.main.transform.position.y, -1);

            if (Camera.main.transform.position.y < yBoundMin)
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, yBoundMin, -1);
            if (Camera.main.transform.position.y > yBoundMax)
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, yBoundMax, -1);
        }

        Camera.main.orthographicSize += -Input.mouseScrollDelta.y;
        if (Camera.main.orthographicSize < 1f)
            Camera.main.orthographicSize = 1f;
        else if (cameraBounded && Camera.main.orthographicSize > 30f)
            Camera.main.orthographicSize = 30f;
    }


    
    public Vector3 setCameraPosition (float x, float y) {
        Vector3 oldPos = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(x, y, -1f);
        return oldPos;
    }

    
}