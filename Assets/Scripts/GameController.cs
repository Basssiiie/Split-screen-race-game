using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour 
{
    // Een list van alle spelers in het level. Ik gebruik het type 'Car', de naam van mijn 
    // auto-script om direct toegang te hebben tot dat script.
    public List<Car> players;

    public ScoreController scoreController;
    

    [Header("Countdown Settings")]
    // Een referentie naar de countdown UI Canvas GameObject en de Textbox.
    public GameObject countDownCanvas;
    public Text countDownText;

    // Het nummer waar de countdown moet starten, bijvoorbeeld 3.
    public int startCount;

    // Vaak telt een countdown niet af per seconde, maar langzamer voor dramatisch effect! :)
    public float timePerCount;

    
    [Space]
    // Een list van alle checkpoints in het spel. Ik gebruik het type 'Checkpoint', de naam van
    // mijn checkpoint-script om direct toegang te hebben tot dat script.
    public List<Checkpoint> checkpoints;

    // Een array voor de huidige progressie van elke speler in het level.
    private int[] currentCheckpoints;

    // Deze boolean gebruiken we zodat de checkpoints niet meer werken nadat de race gewonnen is.
    private bool isRaceFinished = false;



    void Start()
    {
        StartCoroutine(CountDown());

        // Ik zorg ervoor dat de 'currentCheckpoint' array dezelfde lengte heeft als de
        // players-array, zodat ik voor elke speler een aparte progressie kan bijhouden.
        currentCheckpoints = new int[ players.Count ];

        // Ik stel alle checkpoints vervolgens in via de array die ik al in de Inspector had 
        // ingesteld. Tevens weten ze allemaal hun nummer.
        foreach (Checkpoint point in checkpoints)
        {
            point.gameController = this;
            scoreController.AddScore("Test" + Random.Range(15, 150), Random.Range(15, 150));
        }
    }



    /* --- Count down ---
     Er zijn veel verschillende manieren om een countdown te maken. Voor deze versie gebruik ik
     een co-routine.
    */
    IEnumerator CountDown()
    {
        int currentCount = startCount;
        
        while (currentCount > 0)
        {
            countDownText.text = currentCount.ToString();

            currentCount--;
            
            yield return new WaitForSeconds(timePerCount);
        }
        
        countDownText.text = "GO!";

        // Activeer alle spelers, zodat ze kunnen rijden.
        foreach (Car car in players)
        {
            car.isEnabled = true;
        }
        
        yield return new WaitForSeconds(timePerCount);

        countDownCanvas.SetActive(false);
    }


    /* --- Checkpoints aanraken ---
     Deze functie wordt aangeroepen vanuit elk Checkpoint script, zodra er een auto in 
     een checkpoint rijdt.
    */
    public void OnPlayerHitsCheckpoint(Transform transform, Checkpoint checkpoint)
    {
        if (isRaceFinished)
        {
            return;
        }

        // Eerst halen het 'Car' script op van de auto. 
        // Bestaat die niet op dit object? Dan stoppen we de functie met 'return'.
        Car player = transform.GetComponent<Car>();
        if (player == null)
        {
            return;
        }

        /* --- Checkpoint checken ---
         Hieronder kijken we eerst of dit wel de volgende checkpoint voor deze speler is. Dit doen
         we om te voorkomen dat de speler cheat door dezelfde checkpoint meerdere keren aan te 
         raken, of door er eentje over te slaan. Als het de juiste checkpoint is, laten we de 
         speler door naar de volgende checkpoint.
        */
        int playerIndex = players.IndexOf(player);
        int checkpointIndex = checkpoints.IndexOf(checkpoint);
        
        if (currentCheckpoints[playerIndex] == checkpointIndex)
        {
            currentCheckpoints[playerIndex]++;
            
            /* --- Einde ronde ---
             De speler heeft de hele ronde afgelegd! Ik laat de speler hier nu simpelweg winnen, 
             maar je zou hier ook een ronde-teller in kunnen bouwen.
            */
            if (currentCheckpoints[playerIndex] >= checkpoints.Count)
            {
                countDownText.text = player.name + " won!";
                countDownCanvas.SetActive(true);
                isRaceFinished = true;

                scoreController.AddScore("Test", 15);
            }
        }
    }
}
