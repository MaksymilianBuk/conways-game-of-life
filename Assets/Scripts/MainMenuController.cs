using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public InputField width;
    public InputField height;

    public void OnStartGameClick()
    {
        if (IsValid())
        {
            PlayerPrefs.SetInt("width", int.Parse(width.text));
            PlayerPrefs.SetInt("height", int.Parse(height.text));
            SceneManager.LoadScene("MainScreen");
        }
    }

    private bool IsValid()
    {
        return !string.IsNullOrEmpty(width.text) && !string.IsNullOrEmpty(height.text);
    }

}
