using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollButtons : MonoBehaviour
{
    public int indexOfCurrent;

    public void Start() {
        indexOfCurrent = 0;
    }

    public void increaseCurrent() {
        indexOfCurrent = (indexOfCurrent + 1) % transform.childCount;
        changePage();
    }

    public void decreaseCurrent() {
        indexOfCurrent -= 1;
        if (indexOfCurrent < 0)
            indexOfCurrent = transform.childCount - 1;
        changePage();
    }

    public void changePage() {
        for (int i = 0; i < transform.childCount; i++)
            if (i == indexOfCurrent)
                transform.GetChild(i).gameObject.SetActive(true);
            else
                transform.GetChild(i).gameObject.SetActive(false);
    }
}
