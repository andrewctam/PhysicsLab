using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointMass : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector3 pos0, vel0, acc0; 
    public float xAxis, yAxis;
    public int xAxisIndex, yAxisIndex, ID, prefabID;
    public CreateObjects create;
    public bool isPressed, started, canRotate, canTranslateX, canTranslateY, isGraphing, draggable;
    public Graph grapher;
    public Color defaultColor, alphaDefaultColor, graphPointColor;
    public Vector2 currentVelocity, velocityLastFrame, acceleration;
    public GameObject selector;

    public override string ToString() {
        int boolToInt(bool isTrue) { if (isTrue) return 1; else return 0; }
        string formattedName = gameObject.name.Replace(" ", "+");
        return $"{prefabID}:{formattedName}:{rb.mass}:{transform.localScale.x}:{transform.localScale.y}:{pos0.x}:{pos0.y}:{vel0.x}:{vel0.y}:{acc0.x}:{acc0.y}:{xAxisIndex}:{yAxisIndex}:{boolToInt(isGraphing)}:{boolToInt(canRotate)}:{boolToInt(canTranslateX)}:{boolToInt(canTranslateY)}:{grapher.colorToInt(graphPointColor)}~";
    } 

    // Start is called before the first frame update
    void Start()
    {
        create = FindObjectOfType<CreateObjects>();
        grapher = FindObjectOfType<Graph>();
        rb = GetComponent<Rigidbody2D>();
        
        alphaDefaultColor = gameObject.GetComponent<SpriteRenderer>().color;
        defaultColor = new Color(alphaDefaultColor.r, alphaDefaultColor.g, alphaDefaultColor.b, 1f);     
    }
    void FixedUpdate() {
        velocityLastFrame = currentVelocity;
        currentVelocity = rb.velocity;
        acceleration = (currentVelocity - velocityLastFrame) / Time.deltaTime;
        if (started)
            rb.AddForce(acc0 * rb.mass);
    }
    // Update is called once per frame
    void Update()
    {   
        if (isGraphing) {
            xAxis = graphAxisOptions(xAxisIndex);
            yAxis = graphAxisOptions(yAxisIndex);
        }

        if (create.current == ID)  {
            if (isPressed && (Input.GetKey(KeyCode.LeftShift) || draggable)) {
                create.noObjectBeingDragged = false;
                rb.velocity = Vector3.zero;
                Vector2 MousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                Vector2 objPosition = Camera.main.ScreenToWorldPoint(MousePosition);
                transform.position = objPosition;
                if (!started) {
                    pos0 = transform.position;
                    create.newObjectSelected = true; //prevent the editing of the x value from running updateInitialPos();
                    create.posx.text = pos0.x + "";
                    create.posy.text = pos0.y + "";
                    create.newObjectSelected = false;
                }
                
            } else {
                create.noObjectBeingDragged = true;
            }
        }
    }

    void OnMouseDown() {
        if (!create.graphGameObject.activeSelf && !create.cameraSettings.interacting){
            isPressed = true; 
            if (create.updateCurrent(ID)) { //if different object is selected
                draggable = false;
            }
            create.openEditorNewObject();
        }
    }

    void OnMouseUp() {
        isPressed = false;
        draggable = true; //force the user to select the object before dragging it 
    }

    public void initiateMovement() {
        started = true;
        rb.position = pos0;
        rb.velocity = vel0;
    }

    public void updatePosition(Vector3 pos) {
        rb.position = pos;
    }

    public float graphAxisOptions(int index) {
        switch (index) {
            case 0: return create.elapsedTime;
            case 1: return transform.position.x;
            case 2: return transform.position.y;
            case 3: return rb.velocity.x;
            case 4: return rb.velocity.y;
            case 5: return rb.velocity.magnitude;
            case 6: return acceleration.x;
            case 7: return acceleration.y;
            case 8: return (0.5f) * rb.mass * rb.velocity.magnitude * rb.velocity.magnitude;
            case 9: return rb.mass * Physics2D.gravity.y * transform.position.y * -1;
            case 10: return (0.5f) * rb.mass * rb.velocity.magnitude * rb.velocity.magnitude + rb.mass * Physics2D.gravity.y * transform.position.y * -1;
            default: return 0;
        }
    }
    
}
