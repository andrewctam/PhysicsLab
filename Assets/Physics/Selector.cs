using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
public class Selector : MonoBehaviour
{
    public int id;
    public Text idText, name;
    public Image img;
    public CreateObjects create;


    public void updateInfo(int changeID, string changeName, SpriteRenderer sr)
    {
        id = changeID;
        idText.text = "ID: " + changeID;
        updateSelectorName(changeName);
        img.sprite = sr.sprite;
        img.color = sr.color;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, id * -32);
        GetComponent<Button>().onClick.AddListener(setToCurrent);
    }

    public void setToCurrent() {
        create = FindObjectOfType<CreateObjects>();
        create.updateCurrent(id);
        create.openEditorNewObject();

    }

    public void updateSelectorName(string changeName) { //only name needs to be updated. the others don't
        name.text = changeName;
    }
    
}


