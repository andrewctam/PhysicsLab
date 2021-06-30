using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollButtons : MonoBehaviour
{
    public GameObject page1, page2;
    public int count, current;

    public void Start() {
        count = 2;
        current = 0;
    }
    public void increaseCurrent() {
        current = (current + 1) % count;
        updateButtons();
    }

    public void decreaseCurrent() {
        current -= 1;
        if (current < 0)
            current = count - 1;
        updateButtons();
    }

    public void updateButtons() {
        if (current == 0) {
            page1.SetActive(true);
            page2.SetActive(false);
        }
        else if (current == 1) {
            page2.SetActive(true);
            page1.SetActive(false);
        }
    }
}
