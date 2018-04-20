using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [HideInInspector]
    public GameController gameController;
    
	

	void OnTriggerEnter (Collider collider)
    {
        if (collider.tag == "Player")
        {
            gameController.OnPlayerHitsCheckpoint(collider.transform.root, this);
        }
	}
}
