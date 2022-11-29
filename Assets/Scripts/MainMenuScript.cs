using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public GameObject SettingsMenu;
    public GameObject ExitMenu;
    public GameObject MainMenu;

    public void LoadScene(string Name)
    {
        SceneManager.LoadScene(Name);
    }
    public void SettingsButton()
    {
        MainMenu.SetActive(!MainMenu.activeInHierarchy);
        SettingsMenu.SetActive(!SettingsMenu.activeInHierarchy);
    }
    public void ExitButton()
    {
        MainMenu.SetActive(!MainMenu.activeInHierarchy);
        ExitMenu.SetActive(!ExitMenu.activeInHierarchy);
    }
}

