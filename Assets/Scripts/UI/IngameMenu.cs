using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngameMenu : MonoBehaviour 
{
    public GameObject ingameMenu;

    public string menuLevel;
	

    /* --- Ingame menu ---
     Het ingame menu kan geactiveerd worden met ESC, maar alleen als het spel ook echt ingame is. 
    */
	void Update () 
	{
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool isVisible = ingameMenu.activeInHierarchy;
    
            if (isVisible)
            {
                UnpauseGame();
            }
            else
            {
                PauseGame();
            }
        }
	}


    /* --- Menu acties ---
     Hieronder staan alle acties die het menu kan uitvoeren.
    */
    void PauseGame()
    {
        ingameMenu.gameObject.SetActive(true);
        Time.timeScale = 0;
    }


    void UnpauseGame()
    {
        ingameMenu.gameObject.SetActive(false);
        Time.timeScale = 1;
    }


    public void ToMainMenu()
    {
        UnpauseGame();
        SceneManager.LoadScene(menuLevel);
    }


    public void ResumeGame()
    {
        UnpauseGame();
    }
}
