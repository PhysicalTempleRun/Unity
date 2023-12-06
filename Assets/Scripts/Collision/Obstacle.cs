using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public AudioSource collisionFX;
    public GameObject player;
    public GameObject charModel;
    public GameObject LevelControl;

    void OnTriggerEnter(Collider other)
    {
        collisionFX.Play();
        this.gameObject.GetComponent<BoxCollider>().enabled = false;
        player.GetComponent<PlayerMove>().enabled = false;
        charModel.GetComponent<Animator>().Play("Stumble Backwards");
        LevelControl.GetComponent<IncreaseDistance>().isGameRunning = false;
    }
}
