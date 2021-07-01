using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointMass : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector3 pos0, vel0, acc0; 
    public float xAxis, yAxis;
    public int xAxisIndex, yAxisIndex, ID;
    public CreateObjects create;
    public bool isPressed, started, canRotate, canTranslateX, canTranslateY, isGraphing, draggable;
    public Graph grapher;
    public Color defaultColor, alphaDefaultColor;
    public Vector2 currentVelocity, velocityLastFrame, acceleration;

    // Start is called before the first frame update
    void Start()
    {
        create = FindObjectOfType<CreateObjects>();
        grapher = FindObjectOfType<Graph>();
        started = false;
        rb = GetComponent<Rigidbody2D>();
        pos0 = new Vector3(0, 0, 0);
        vel0 = new Vector3(0, 0, 0);
        acc0 = new Vector3(0, 0, 0);
        xAxisIndex = 0;
        yAxisIndex = 1;
        ID = int.Parse(gameObject.name);
        defaultColor = gameObject.GetComponent<SpriteRenderer>().color;
        alphaDefaultColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0.6f);
    
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
            if (create.graphBar.activeSelf && create.current == ID) {
                xAxisIndex = grapher.xAxisDropdown.GetComponent<Dropdown>().value;
                yAxisIndex = grapher.yAxisDropdown.GetComponent<Dropdown>().value;
            }
            
            xAxis = graphAxisOptions(xAxisIndex);
            yAxis = graphAxisOptions(yAxisIndex);
        }
    
        if (create.current == ID)  {
            gameObject.GetComponent<SpriteRenderer>().color = alphaDefaultColor;
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

            } else
                create.noObjectBeingDragged = true;
        }

        else 
            gameObject.GetComponent<SpriteRenderer>().color = defaultColor;
    }

    void OnMouseDown() {
        isPressed = true; 
        if (create.updateCurrent(ID)) { //if different object is selected
            draggable = false;
        }
        //create.GetComponent<CreateObjects>().newObjectSelected = true;
        create.openEditorNewObject();
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

