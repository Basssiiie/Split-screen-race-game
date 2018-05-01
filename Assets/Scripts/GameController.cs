using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour 
{
    // Een list van alle spelers in het level. Ik gebruik het type 'Car', de naam van mijn 
    // auto-script om direct toegang te hebben tot dat script.
    public List<Car> players;

	
	[Space] // Dit zorgt voor een kleine witregel in de Inspector (voor beter overzicht).

	// Een referentie naar de countdown UI GameObject en de Textbox.
	public GameObject centerPanelObj;
    public Text centerPanelText;

    // Het nummer waar de countdown moet starten, bijvoorbeeld 3.
    public int startCount;

    // Vaak telt een countdown niet af per seconde, maar langzamer voor dramatisch effect! :)
    public float secondsPerCount;

	
	[Space]

	// Alle variabelen nodig voor het updaten van de timer.
	public GameObject timerObj;
	public Text timerText;

	private bool startTimer = false;
	private float currentTimer = 0;

	// De variabel 'BestTimeScore' heb ik een 'static' gemaakt, om ervoor te zorgen dat deze waarde 
	// voor de rest van de speelsessie wordt onthouden.
	private static float BestTimeScore = float.MaxValue;


	[Space]

    // Een list van alle checkpoints in het spel. Ik gebruik het type 'Checkpoint', de naam van
    // mijn checkpoint-script om direct toegang te hebben tot dat script.
    public List<Checkpoint> checkpoints;

    // Een array voor de huidige progressie van elke speler in het level.
    private int[] currentCheckpoints;

    // Deze boolean gebruiken we zodat de checkpoints niet meer werken nadat de race gewonnen is.
    private bool isRaceFinished = false;



	private void Start()
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
        }
    }


	/* --- Update voor Timer ---
     Ik gebruik de Update in dit object eigenlijk alleen voor de Timer.
    */
	private void Update()
	{
		if (!startTimer)
		{
			return;
		}

		currentTimer += Time.deltaTime;

		string curTime = ChangeFloatIntoTime(currentTimer);
		string bestTime = "not set";

		if (BestTimeScore != float.MaxValue)
		{
			bestTime = ChangeFloatIntoTime(BestTimeScore);
		}

		timerText.text = (curTime + "\n" + bestTime); // De '\n' betekend 'nieuwe regel'.
	}


	/* --- Timer naar Text ---
	 Om de Timer-float gemakkelijk om te zetten naar tekst, heb ik besloten een functie te 
	 schrijven die om een tijd-'float' vraagt, en deze vervolgens omzet in een tekst-'string' met 
	 de juiste tijd. 
	*/
	private string ChangeFloatIntoTime(float time)
	{
		int minutes = Mathf.FloorToInt(time / 60);
		float seconds = time % 60;

		// Met ToString kun je verschillende regels zetten over hoe de waarde wordt omgevormt naar
		// tekst. Hieronder geef ik met '00' aan dat 'seconds' altijd 2 van die karakters heeft op 
		// die plek, die pas worden overschreven als de 'seconds' variabele groter is dan nul. 
		// (Meer informatie: https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings )
		string text = minutes + ":" + seconds.ToString("00.00"); // + milli.ToString(".00");

		return text;
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
            centerPanelText.text = currentCount.ToString();

            currentCount--;
            
            yield return new WaitForSeconds(secondsPerCount);
        }
        
        centerPanelText.text = "GO!";

		startTimer = true;
		timerObj.SetActive(true);

        // Activeer alle spelers, zodat ze kunnen rijden.
        foreach (Car car in players)
        {
            car.isEnabled = true;
        }
        
        yield return new WaitForSeconds(secondsPerCount);

        centerPanelObj.SetActive(false);
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
                centerPanelText.text = player.name + " won!";
                centerPanelObj.SetActive(true);

				if (currentTimer < BestTimeScore)
				{
					BestTimeScore = currentTimer;
				}

                isRaceFinished = true;
            }
        }
    }
}
