using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour 
{
    public string raceLevel;	

    /* --- Menu acties ---
     Hieronder staan alle acties die het menu kan uitvoeren.
    */

    public void StartGame()
    {
        SceneManager.LoadScene(raceLevel);
    }


    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Application has quit.");
    }
}
