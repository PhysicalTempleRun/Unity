using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncreaseDistance : MonoBehaviour
{
    public GameObject distanceDisplay;
    public GameObject finalScoreDisplay;
    public GameObject scoreboard;
    public int disRun;
    public bool isPlayerRunning = false;
    public bool isGameRunning = true;
    public bool increasingDistance = false;

    void Start()
    {
        scoreboard.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {   
        if(isGameRunning && increasingDistance==false && isPlayerRunning==true) {
            increasingDistance=true;
            StartCoroutine(AddingDis());
        }
        if(isGameRunning==false) {
            scoreboard.SetActive(true);
            finalScoreDisplay.GetComponent<Text>().text = disRun + "m";
        } 
    }

    IEnumerator AddingDis() {
        disRun+=2;
        distanceDisplay.GetComponent<Text>().text = disRun + "m";
        yield return new WaitForSeconds(0.25f);
        increasingDistance=false;
    }
}
