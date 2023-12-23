using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;



public class IncreaseDistance : MonoBehaviour
{
    public string portName = "/dev/cu.usbmodem14301";
    SerialPort arduino;
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
        arduino = new SerialPort (portName, 9600);
        arduino.Open();
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
            string result = toArduinoScore(disRun);
            arduino.Write(result);
        } 
    }

    IEnumerator AddingDis() {
        disRun+=2;
        distanceDisplay.GetComponent<Text>().text = disRun + "m";
        yield return new WaitForSeconds(0.25f);
        increasingDistance=false;
    }

    string toArduinoScore (int scoreNum) {
        if(scoreNum < 100) {
            return "D";
        } else if (scoreNum < 200) {
            return "C";
        } else if (scoreNum < 450) {
            return "B";
        } else {
            return "A";
        }
    } 
}
