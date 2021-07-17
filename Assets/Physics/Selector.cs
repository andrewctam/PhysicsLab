using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Selector : MonoBehaviour
{
    public int id;
    public TextMeshProUGUI idText, objectName;
    public Image img;

    public void updateInfo(int changeID, string changeName, SpriteRenderer sr)
    {
        id = changeID;
        idText.text = "ID: " + changeID;
        updateSelectorName(changeName);
        img.sprite = sr.sprite;
        img.color = sr.color;
        //GetComponent<RectTransform>().anchoredPosition = new Vector2(0, id * -32);
        GetComponent<Button>().onClick.AddListener(setToCurrent);
        gameObject.SetActive(false);
    }

    public void setToCurrent() {
        CreateObjects create = FindObjectOfType<CreateObjects>();
        if (create.current == id) {
            Vector3 pos = create.createdObjects[id].transform.position;
            Camera.main.transform.position = new Vector3(pos.x, pos.y, Camera.main.transform.position.z);
        } else {
            create.updateCurrent(id);
            create.createdObjects[id].GetComponent<PointMass>().draggable = true;
            create.openEditorNewObject();
        }

    }

    public void updateSelectorName(string changeName) { //only objectName needs to be updated. the others don't
        objectName.text = changeName;
    }
    
}


