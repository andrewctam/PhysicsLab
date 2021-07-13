using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Announcement : MonoBehaviour
{
    public int frame, framesToDestroy;
    public bool urgent;

    void Start() {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100f);
    }
    
    void Update() {
        frame++;
        if (frame > framesToDestroy)
            destroyNow();
    }

    public void setAnnouncement(string message, int frames, bool isUrgent) {
        urgent = isUrgent;
        gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message; 
        framesToDestroy = frames;

        if (urgent || FindObjectOfType<CreateObjects>().announcementQueue.Count == 1) //if urgent or this announcement is at the front of the queue 
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }

    public void destroyNow() {
        CreateObjects create = FindObjectOfType<CreateObjects>();
        if (!urgent) { 
            create.announcementQueue.Dequeue();        //remove this announcement from the queue
        }
        
        if (create.announcementQueue.Count > 0) 
            create.announcementQueue.Peek().SetActive(true);
        Destroy(gameObject);
    }

}
