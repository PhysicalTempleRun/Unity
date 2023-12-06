using System.IO.Ports;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class PlayerMove : MonoBehaviour
// {
//     SerialPort serialPort;
//     string portName = "/dev/cu.usbmodem142101"; // Change to your Arduino's port
//     int baudRate = 9600;

//     public GameObject charModel;
//     public float moveSpeed = 20;
//     public float leftRightSpeed = 25;
//     public float jump = 20;
//     private bool isJumping = false;
//     private bool isComingDown= false;
//     private bool isRunning = true;

//     void Start() {
//         serialPort = new SerialPort(portName, baudRate);
//         serialPort.Open();
//     }
    
//     // Update is called once per frame
//     void Update()
//     {
//         if(isRunning) {
//             transform.Translate(Vector3.forward*moveSpeed*Time.deltaTime);
//         }

//         if (serialPort.IsOpen) {
//             try {
//                 string message = serialPort.ReadLine();
//                 HandleMessage(message);
//             } catch (System.Exception) { }
//         }
        
//         if(isJumping==true) {
//             if(isComingDown==false) {
//                 transform.Translate(Vector3.up*Time.deltaTime*jump,Space.World);
//             } else {
//                 transform.Translate(Vector3.down*Time.deltaTime*jump,Space.World);
//             }
//         }
//     }

//     void HandleMessage(string message) {
//         switch (message.Trim()) {
//             case "jump":
//                 if(!isJumping)
//                 {
//                     Jump();
//                 }
//                 break;
//             case "stop":
//                 if(isRunning) {
//                     isRunning=false;
//                     charModel.GetComponent<Animator>().Play("Idle");
//                 }
//                 break;
//             case "start":
//                 if(!isRunning) {
//                     isRunning=true;
//                     charModel.GetComponent<Animator>().Play("Standard Run");
//                 }
//                 break;
//         }
//     }

//     void Jump(){
//         isJumping = true;
//         charModel.GetComponent<Animator>().Play("Jump");
//         StartCoroutine(JumpSequence());
//     }

//     IEnumerator JumpSequence() {
//         yield return new WaitForSeconds(0.6f);
//         isComingDown = true;
//         yield return new WaitForSeconds(0.6f);
//         isComingDown = false;
//         isJumping = false;
//         charModel.GetComponent<Animator>().Play("Standard Run");
//     }

    
// }

public class PlayerMove : MonoBehaviour
{
    public GameObject charModel;
    public float moveSpeed = 20;
    public float leftRightSpeed = 25;
    public float jump = 20;
    private bool isJumping = false;
    private bool isComingDown= false;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward*moveSpeed*Time.deltaTime);
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if(this.gameObject.transform.position.x > LevelBoundary.leftSide)
            { 
                transform.Translate(Vector3.left*leftRightSpeed*Time.deltaTime);
            }
        }
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if(this.gameObject.transform.position.x < LevelBoundary.rightSide)
            {
                transform.Translate(Vector3.right*leftRightSpeed*Time.deltaTime);
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
    
}
