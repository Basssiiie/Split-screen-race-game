using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Car : MonoBehaviour
{
    // --- Publieke variabelen ---

    [Header("Player Settings")]
    // De naam van de input axis voor links en rechts voor deze auto.
    public string horizontalInput;

    // De naam van de input axis voor vooruit en achteruit voor deze auto.
    public string verticalInput;

    // De KeyCode om de camera mee te veranderen voor deze auto.
    public KeyCode cameraInput;                 

    
    [Header("Vehicle Properties")]
    // Deze variable houdt bij of de speler mag rijden of niet. (Bijvoorbeeld tijdens de countdown.)
    public bool isEnabled;

    // Dit is de kracht van de motor, hoe hoger en hoe sneller de auto optrekt.
    public float motorPower;

    // Dit is de hoek van de wielen als je maximaal naar links of rechts zou sturen.
    public float steeringAngle;

    // De lokale offset van het middenpunt van de zwaartekracht. Normaal wordt deze automatisch
    // berekend, maar voor mijn auto's ligt hij te hoog, waardoor de auto's sneller over de kop
    // gaan. Vandaar dat ik hem handmatig wil instellen.
    public Vector3 centerOfMass;

    
    [Header("Wheel References")]
    // Dit zijn de referenties naar alle models voor de wielen op deze auto.
    public GameObject wheelObjFrontRight;       
    public GameObject wheelObjFrontLeft;
    public GameObject wheelObjRearRight;
    public GameObject wheelObjRearLeft;

    // Dit zijn de referenties naar alle WheelColliders op deze auto.
    public WheelCollider wheelColFrontRight;    
    public WheelCollider wheelColFrontLeft;
    public WheelCollider wheelColRearRight;
    public WheelCollider wheelColRearLeft;

    
    [Header("Camera References")]
    // Dit is een lijst van alle camera's voor deze auto.
    public GameObject[] allCameras;             

    
    [Header("UI References")]
    // Dit is een referentie naar de UI text voor de snelheidsmeter.
    public Canvas speedoCanvas;
    public Text speedometer;


    // --- Interne variabelen ---

    // Dit is een interne referentie naar de RigidBody. Zo hoef ik maar 1x GetComponent aan te roepen, in plaats van elke update.
    private Rigidbody rigidBody;

    // Dit is de index van de huidige camera die nu aanstaat. 0 = de eerste camera, 1 = de tweede, 2 = de derde, etc.
    private int currentCameraIndex = 0;         



    void Start ()
    {
        // Ik sla de RigidBody op in een interne variabel voor later gebruik.
        rigidBody = GetComponent<Rigidbody>();

        rigidBody.centerOfMass = centerOfMass;
    }
	


	void Update () 
	{
        /* --- Camera switchen ---
         Voor de camera's gebruik ik een array en daar switch ik dan vervolgens doorheen. Het 
         systeem is erg gelijk aan het switchen van wapens dat jullie gebruikt hebben in je 
         first-person shooter.
        */
        if (Input.GetKeyDown(cameraInput))
        {
            // Eerst verhogen we de index met 1 op het moment dat de knop ingedrukt is.
            currentCameraIndex++;

            if (currentCameraIndex >= allCameras.Length)
            {
                currentCameraIndex = 0;
            }

            // Vervolgens activeren we de nieuwe camera en deactiveren we de anderen.
            for (int i = 0; i < allCameras.Length; i ++)
            {
                if (i == currentCameraIndex)
                {
                    allCameras[i].SetActive(true);

                    // We kunnen tegen het Canvas zeggen op welke camera hij gerenderd moet 
                    // worden, als de Canvas is ingesteld op 'Screen Space - Camera'.
                    speedoCanvas.worldCamera = allCameras[i].GetComponent<Camera>();
                }
                else
                {
                    allCameras[i].SetActive(false);
                }
            }
        }
	}



    void FixedUpdate()
    {
        /* --- Besturing ---
         Als eerste check ik met een '!isEnabled' of de speler de auto wel mag besturen. Dit mag 
         bijvoorbeeld niet als de countdown nog aan het aftellen is. Ik gebruik een 'return' om te 
         zeggen dat het spel de rest van deze functie niet meer mag uitlezen.
        */
        if (!isEnabled)
        {
            return;
        }

        /* --- Motor ---
         Hieronder berekenen we de motorkracht in combinatie met de input. De input is altijd een 
         nummer tussen -1 (down), 0 (niks) en 1 (up). Het resultaat geven we door aan de colliders
         van de achterwielen van dit voertuig.
        */
        float motor = motorPower * Input.GetAxis(verticalInput);

        wheelColRearLeft.motorTorque = motor;
        wheelColRearRight.motorTorque = motor;

        /* --- Sturen ---
         Voor het sturen doen we hetzelfde: we berekenen de stuurhoek aan de hand van de input.
         De input is altijd een nummer tussen -1 (down), 0 (niks) en 1 (up). Het resultaat geven 
         we door aan de colliders van de voorwielen van dit voertuig.
        */
        float steering = steeringAngle * Input.GetAxis(horizontalInput);

        wheelColFrontLeft.steerAngle = steering;
        wheelColFrontRight.steerAngle = steering;

        /* --- Modelen updaten ---
         We hebben nog enkel de colliders geupdate, nu moeten we de zichtbare wiel-models updaten.
         Gelukkig kunnen we aan de colliders vragen hoe deze moeten staan. Let erop dat je wielen 
         standaard geen rotatie of scale hebben, en de pivot moet in het midden staan. Geef ze een 
         Empty GameObject als parent met de juiste positie/rotatie/scale als dit wel zo is.

         Ik gebruik hier een aparte functie om te voorkomen dat ik de regels code voor elk wiel 
         moet herhalen.
        */
        UpdateWheel(wheelColFrontLeft, wheelObjFrontLeft);
        UpdateWheel(wheelColFrontRight, wheelObjFrontRight);
        UpdateWheel(wheelColRearLeft, wheelObjRearLeft);
        UpdateWheel(wheelColRearLeft, wheelObjRearLeft);

        /* --- UI updaten
         Als laatste update ik de snelheid van de auto in de UI. Dit doe ik door de snelheid
         van de RigidBody te pakken in meter per seconde, en deze om te vormen naar kilometer
         per uur. Dit getal rond ik vervolgens af tot een integer en zet ik in mijn textbox.
        */
        float speed = (rigidBody.velocity.magnitude * 3.6f);
        int speedRounded = Mathf.RoundToInt(speed);

        speedometer.text = (speedRounded + " km/u");
    }



    /* --- Update Wheel functie ---
     Als deze functie wordt aangeroepen, dan wordt het meegegeven wheelObj gepositioneerd aan de 
     hand van de meegegeven collider.
    */
    void UpdateWheel(WheelCollider collider, GameObject wheelObj)
    {
        Vector3 position;
        Quaternion rotation;

        // We kunnen de benodigde positie en rotatie uit de WheelCollider uitlezen.
        collider.GetWorldPose(out position, out rotation);

        // Hier zeggen we dat het model deze informatie moet overnemen.
        wheelObj.transform.position = position;
        wheelObj.transform.rotation = rotation;
    }
}