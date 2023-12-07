using System.IO.Ports;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class PlayerMove : MonoBehaviour
{
    SerialPort serialPort;
    string portName = "/dev/cu.usbmodem142101"; // Change to your Arduino's port
    int baudRate = 9600;

    public GameObject charModel;
    public float moveSpeed = 20;
    public float leftRightSpeed = 25;
    public float jump = 20;
    private bool isJumping = false;
    private bool isComingDown= false;
    private bool isRunning = true;

    void Start() {
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();
    }
    
    // Update is called once per frame
    void Update()
    {
        if(isRunning) {
            transform.Translate(Vector3.forward*moveSpeed*Time.deltaTime);
        }

        if (serialPort.IsOpen) {
            try {
                string message = serialPort.ReadLine();
                HandleMessage(message);
            } catch (System.Exception) { }
        }
        
        if(isJumping==true) {
            if(isComingDown==false) {
                transform.Translate(Vector3.up*Time.deltaTime*jump,Space.World);
            } else {
                transform.Translate(Vector3.down*Time.deltaTime*jump,Space.World);
            }
        }
    }

    void HandleMessage(string message) {
        switch (message.Trim()) {
            case "jump":
                if(!isJumping)
                {
                    Jump();
                }
                break;
            case "stop":
                if(isRunning) {
                    isRunning=false;
                    charModel.GetComponent<Animator>().Play("Idle");
                }
                break;
            case "start":
                if(!isRunning) {
                    isRunning=true;
                    charModel.GetComponent<Animator>().Play("Standard Run");
                }
                break;
        }
    }

    void Jump(){
        isJumping = true;
        charModel.GetComponent<Animator>().Play("Jump");
        StartCoroutine(JumpSequence());
    }

    IEnumerator JumpSequence() {
        yield return new WaitForSeconds(0.6f);
        isComingDown = true;
        yield return new WaitForSeconds(0.6f);
        isComingDown = false;
        isJumping = false;
        charModel.GetComponent<Animator>().Play("Standard Run");
    }
}
*/


public class PlayerMove : MonoBehaviour
{
    public GameObject charModel;
    public float moveSpeed = 20;
    public float leftRightSpeed = 25;
    public float jump = 20;
    private bool isJumping = false;
    private bool isComingDown= false;

    private bool isRotating = false;
    public float rotateDuration = 0.05f;


    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward*moveSpeed*Time.deltaTime);
        if (!isRotating)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                StartCoroutine(Rotate(Quaternion.Euler(0, -90, 0)));
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                StartCoroutine(Rotate(Quaternion.Euler(0, 90, 0)));
            }
        }

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            if(!isJumping)
            {
                Jump();
            }
        }
        if(isJumping==true) {
            if(isComingDown==false) {
                transform.Translate(Vector3.up*Time.deltaTime*jump,Space.World);
            } else {
                transform.Translate(Vector3.down*Time.deltaTime*jump,Space.World);
            }
        }
    }

    void Jump(){
        isJumping = true;
        charModel.GetComponent<Animator>().Play("Jump");
        StartCoroutine(JumpSequence());
    }

    IEnumerator JumpSequence() {
        yield return new WaitForSeconds(0.6f);
        isComingDown = true;
        yield return new WaitForSeconds(0.6f);
        isComingDown = false;
        isJumping = false;
        charModel.GetComponent<Animator>().Play("Standard Run");
    }

    IEnumerator Rotate(Quaternion deltaRotation)
    {
        isRotating = true;

        Quaternion originalRotation = transform.rotation;
        Quaternion targetRotation = transform.rotation * deltaRotation;

        for (float t = 0; t < 1; t += Time.deltaTime / rotateDuration)
        {
            transform.rotation = Quaternion.Lerp(originalRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;
        isRotating = false;
    }
    
}
