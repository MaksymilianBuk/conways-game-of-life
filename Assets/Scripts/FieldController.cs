using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldController : MonoBehaviour
{
    public void ToggleColorOnClick()
    {
        Color colorDied = new Color32(213, 30, 51, 255);
        Color colorLiving = new Color32(73, 149, 7, 255);
        if (gameObject.GetComponent<Image>().color.Equals(colorDied))
        {
            gameObject.GetComponent<Image>().color = colorLiving;
        }
        else
        {
            gameObject.GetComponent<Image>().color = colorDied;
        }
    }
}
