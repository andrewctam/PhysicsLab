using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Announcement : MonoBehaviour
{
    public int frame, framesToDestroy;

    public void setAnnouncement(string message, int frames) {
        gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = message; 
        framesToDestroy = frames;
    }

    void Update() {
        frame++;
        if (frame > framesToDestroy)
            destroyNow();
    }


    public void destroyNow() {
        FindObjectOfType<CreateObjects>().noAnnouncementDisplayed = true;
        Destroy(gameObject);
    }

}
