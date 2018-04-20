using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour 
{
    public GameObject scoreEntryPrefab;
    public Transform listTransform;

    // Omdat er maar één centrale plek nodig is om de score op te slaan, maak ik deze variable een
    // static. Dit voorkomt ook dat de scores niet verdwijnen als je naar het volgende level gaat.
    private static List<ScoreEntry> scores = new List<ScoreEntry>();


    void Start()
    {
        foreach (ScoreEntry entry in scores)
        {
            Debug.Log("bestaat?" + entry);
        }
    }


    /* --- Score updaten ---
     Elke keer als er iemand finisht, voeg ik zijn score toe aan de lijst en update ik de UI text.
     Ik ga met een loop door de huidige scores heen en voeg hem toe.
    */
    public void AddScore(string name, float time)
    {
        int index = 0;
        foreach (ScoreEntry entry in scores)
        {
            if (time < entry.timeScore)
            {
                index++;
            }
            else break;
        }

        // Maak een nieuw score object.
        ScoreEntry scoreEntry = CreateScoreObject(name, time);

        scores.Insert(index, scoreEntry);
    }


    /* --- Score Entry aanmaken ---
     Hieronder heb ik een aparte functie gemaakt voor het instantiaten van een ScoreEntry object,
     om de code beter te orderen en te voorkomen dat ik niet op twee verschillende plekken dezelfde
     code hoef te schrijven.
    */
    private ScoreEntry CreateScoreObject(string name, float time)
    {
        GameObject scoreObj = Instantiate(scoreEntryPrefab);
        scoreObj.transform.SetParent(listTransform);

        ScoreEntry scoreEntry = scoreObj.GetComponent<ScoreEntry>();
        scoreEntry.SetScore(name, time);

        return scoreEntry;
    }
}
