using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [HideInInspector] // Dit zorgt ervoor dat de variable nog steeds public is, maar ik kan hem
					  // niet bewerken in de inspector
    public GameController gameController;


	/* --- Checkpoint trigger ---
     Om het registreren van checkpoints centraal te houden gebruik ik dit script enkel als 
	 'postbode', die vervolgens aan de GameController doorgeeft welke auto door welke checkpoint
	 is gereden. Op deze manier voorkom ik dat meerdere objecten individueel van elkaar kijken waar
	 alle auto's zijn.
    */
	void OnTriggerEnter (Collider collider)
    {
        if (collider.tag == "Player")
        {
            gameController.OnPlayerHitsCheckpoint(collider.transform.root, this);
        }
	}
}
