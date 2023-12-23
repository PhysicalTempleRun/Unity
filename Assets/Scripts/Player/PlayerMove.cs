using UnityEngine;
using System;
using System.IO.Ports;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.SceneManagement;


public class PlayerMove : MonoBehaviour
{
    SerialPort serialPortUSB;
    string portNameUSB = "/dev/cu.usbmodem141101";
    int baudRateUSB = 9600;
    
    SerialPort serialPortBLT;
    string portNameBLT = "/dev/cu.usbmodem141401";////"/dev/cu.usbmodem142401"; //"/dev/cu.MyBT2";
    int baudRateBLT = 115200;

    public GameObject charModel;
    public float moveSpeed = 20;
    public float leftRightSpeed = 25;
    public float jump = 20;
    private bool isJumping = false;
    private bool isComingDown= false;
    public bool isRunning = false;
    public float rotateDuration = 0.05f;

    private Thread usbThread;
    private Thread bltThread;
    private bool isReadingUSB = false;
    private bool isReadingBLT = false;

    private Queue<string> usbQueue = new Queue<string>();
    private Queue<string> bltQueue = new Queue<string>();

    private Coroutine rotateCoroutine;
    private Coroutine jumpCoroutine;
    
    public GameObject levelControl;


    // void Start() {
    //     Debug.Log("PlayerMove.cs : Start() called");
    //     isRunning=false;
    //     charModel.GetComponent<Animator>().Play("Idle");
    //     levelControl.GetComponent<IncreaseDistance>().isPlayerRunning = false;
    // }
    
    void Update()
    {
        if(isRunning) {
            transform.Translate(Vector3.forward*moveSpeed*Time.deltaTime);
        }

        while (usbQueue.Count > 0)
        {
            string message = "";
            lock (usbQueue)
            {
                message = usbQueue.Dequeue();
            }
            HandleMessageUSB(message);
        }

        while (bltQueue.Count > 0)
        {
            string message = "";
            lock (bltQueue)
            {
                message = bltQueue.Dequeue();
            }
            HandleMessageBluetooth(message);
        }
        
        if(isJumping==true) {
            if(isComingDown==false) {
                transform.Translate(Vector3.up*Time.deltaTime*jump,Space.World);
            } else {
                transform.Translate(Vector3.down*Time.deltaTime*jump,Space.World);
            }
        }
    }


    void OnApplicationPause() {
        Debug.Log("PlayerMove.cs : OnApplicationPause() called");
        //CleanUpResources();
    }

    void OnDestroy() {
        Debug.Log("PlayerMove.cs : OnDestroy() called");
        CleanUpResources();
    }

    private void CleanUpResources() {
        
        Debug.Log("Cleaning up resources...");

        // if (rotateCoroutine != null) {
        //     Debug.Log("rotate coroutine stopped");
        //     StopCoroutine(rotateCoroutine);
        // }
        // if (jumpCoroutine != null) {
        //     Debug.Log("jump coroutine stopped");
        //     StopCoroutine(jumpCoroutine);
        // }
        // 시리얼 포트 닫기
        if (serialPortUSB != null && serialPortUSB.IsOpen) {
            Debug.Log("USB Port Closed");
            serialPortUSB.Close();
        }
        if (serialPortBLT != null && serialPortBLT.IsOpen) {
            Debug.Log("Bluetooth Port Closed"); 
            serialPortBLT.Close();
        }

        // 스레드 종료 처리
        isReadingUSB = false;
        isReadingBLT = false;
        if (usbThread != null && usbThread.IsAlive) {
            Debug.Log("USB Thread Close");
            usbThread.Abort();
        }
        if (bltThread != null && bltThread.IsAlive) {
            Debug.Log("Bluetooth Thread Close");
            bltThread.Abort();
        }
    }

    private void ReadUSB()
    {
        Debug.Log("ReadUSB() called");
        while (isReadingUSB)
        {
            try
            {
                if (serialPortUSB.IsOpen)
                {
                    string message = serialPortUSB.ReadLine();
                    lock (usbQueue)
                    {
                        Debug.Log("1 - USB Message: " + message);
                        usbQueue.Enqueue(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("USB Read Thread Error: " + ex.Message);
            }
        }
        Debug.Log("ReadUSB() closed");
    }

    private void ReadBLT()
    {
        Debug.Log("ReadBLT() called");
        while (isReadingBLT)
        {
            try
            {
                if (serialPortBLT.IsOpen)
                {
                    string message = serialPortBLT.ReadLine();
                    lock (bltQueue)
                    {
                        Debug.Log("1 - BLT Message: " + message);
                        bltQueue.Enqueue(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("BLT Read Thread Error: " + ex.Message);
            }
        }
        Debug.Log("ReadBLT() closed");
    }

    void HandleMessageUSB(string message) {
        Debug.Log("USB Message: " + message);
        string msg = message.Trim();
        if(isSubsequence(msg, "jump")) {
            if(!isJumping)
            {
                Jump();
            }
        } else if(isSubsequence(msg, "stop")) {
            if(isRunning) {
                isRunning=false;
                charModel.GetComponent<Animator>().Play("Idle");
                levelControl.GetComponent<IncreaseDistance>().isPlayerRunning = false;
            }
        } else if(isSubsequence(msg, "start")) {
            if(!isRunning) {
                isRunning=true;
                charModel.GetComponent<Animator>().Play("Standard Run");
                levelControl.GetComponent<IncreaseDistance>().isPlayerRunning = true;
            }
        }
    }

    void HandleMessageBluetooth(string message) {
        Debug.Log("Bluetooth Message: " + message);
        switch (message.Trim()) {
            case "left":
                rotateCoroutine = StartCoroutine(Rotate(Quaternion.Euler(0, -90, 0)));
                break;
            case "right":
                rotateCoroutine = StartCoroutine(Rotate(Quaternion.Euler(0, 90, 0)));
                break;
        }
    }

    void Jump(){
        isJumping = true;
        charModel.GetComponent<Animator>().Play("Jump");
        jumpCoroutine = StartCoroutine(JumpSequence());
    }

    IEnumerator JumpSequence() {
        yield return new WaitForSeconds(0.6f);
        isComingDown = true;
        yield return new WaitForSeconds(0.611f);
        isComingDown = false;
        isJumping = false;
        charModel.GetComponent<Animator>().Play("Standard Run");
        jumpCoroutine = null;
    }

    IEnumerator Rotate(Quaternion deltaRotation)
    {
        Quaternion originalRotation = transform.rotation;
        Quaternion targetRotation = transform.rotation * deltaRotation;

        for (float t = 0; t < 1; t += Time.deltaTime / rotateDuration)
        {
            transform.rotation = Quaternion.Lerp(originalRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;
        rotateCoroutine = null; 
    }

    private bool isSubsequence(string source, string target)
    {
        int sourceIndex = 0;
        int targetIndex = 0;

        while (sourceIndex < source.Length && targetIndex < target.Length)
        {
            if (source[sourceIndex] == target[targetIndex])
            {
                targetIndex++;
            }
            sourceIndex++;
        }

        return targetIndex == target.Length;
    }

    void Start() {
        Debug.Log("PlayerMove.cs : OnStart() called");
        isRunning=false;
        charModel.GetComponent<Animator>().Play("Idle");
        levelControl.GetComponent<IncreaseDistance>().isPlayerRunning = false;
        // USB 시리얼 포트 재연결 시도
        if (serialPortUSB == null || !serialPortUSB.IsOpen) {
            try {
                serialPortUSB = new SerialPort(portNameUSB, baudRateUSB);
                serialPortUSB.Open();
                Debug.Log("USB Port Reopened");
            }
            catch (Exception ex) {
                Debug.LogError("Error reopening USB port: " + ex.Message);
            }
        }

        // Bluetooth 시리얼 포트 재연결 시도
        if (serialPortBLT == null || !serialPortBLT.IsOpen) {
            try {
                serialPortBLT = new SerialPort(portNameBLT, baudRateBLT);
                serialPortBLT.Open();
                Debug.Log("Bluetooth Port Reopened");
            }
            catch (Exception ex) {
                Debug.LogError("Error reopening Bluetooth port: " + ex.Message);
            }
        }

        // 스레드 재시작
        if (!isReadingUSB && (usbThread == null || (usbThread != null && !usbThread.IsAlive))) {
            usbThread = new Thread(ReadUSB);
            usbThread.Start();
            isReadingUSB = true;
            Debug.Log("USB Thread Reopened");
        }

        if (!isReadingBLT && (bltThread == null || (bltThread != null && !bltThread.IsAlive))) {
            bltThread = new Thread(ReadBLT);
            bltThread.Start();
            isReadingBLT = true;
            Debug.Log("Bluetooth Thread Reopened");
        }
    }

}