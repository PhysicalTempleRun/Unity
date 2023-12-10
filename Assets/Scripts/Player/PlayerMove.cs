using UnityEngine;
using System;
using System.IO.Ports;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class PlayerMove : MonoBehaviour
{
    SerialPort serialPortUSB;
    string portNameUSB = "/dev/cu.usbmodem141101";
    int baudRateUSB = 9600;
    
    SerialPort serialPortBLT;
    string portNameBLT = "/dev/cu.usbmodem141401";////"/dev/cu.usbmodem142401"; //"/dev/cu.MyBT2";
    int baudRateBLT = 9600;

    public GameObject charModel;
    public float moveSpeed = 20;
    public float leftRightSpeed = 25;
    public float jump = 20;
    private bool isJumping = false;
    private bool isComingDown= false;
    private bool isRunning = true;
    public float rotateDuration = 0.05f;

    private Thread usbThread;
    private Thread bltThread;
    private bool isReadingUSB = false;
    private bool isReadingBLT = false;

    private Queue<string> usbQueue = new Queue<string>();
    private Queue<string> bltQueue = new Queue<string>();


    void Start() {
        try {
            serialPortUSB = new SerialPort(portNameUSB, baudRateUSB);
            serialPortUSB.Open();
            Debug.Log("USB Port Opened");
            usbThread = new Thread(ReadUSB);
            usbThread.Start();
            isReadingUSB = true;
        }
        catch (Exception ex) {
            Debug.LogError("Error opening USB port: " + ex.Message);
        }

        try {
            Debug.Log("Bluetooth Port Opening");
            serialPortBLT = new SerialPort(portNameBLT, baudRateBLT);
            serialPortBLT.Open();
            serialPortBLT.DtrEnable = true;
            serialPortBLT.ReadTimeout = 100;
            Debug.Log("Bluetooth Port Opened");
            bltThread = new Thread(ReadBLT);
            bltThread.Start();
            isReadingBLT = true;
        }
        catch (Exception ex) {
            Debug.LogError("Error opening Bluetooth port: " + ex.Message);
        }
    }
    
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

    void OnDestroy()
    {
        // 스레드 종료 처리
        isReadingUSB = false;
        isReadingBLT = false;
        if (usbThread != null && usbThread.IsAlive)
            usbThread.Join();
        if (bltThread != null && bltThread.IsAlive)
            bltThread.Join();

        // 시리얼 포트 닫기
        if (serialPortUSB != null && serialPortUSB.IsOpen)
            serialPortUSB.Close();
        if (serialPortBLT != null && serialPortBLT.IsOpen)
            serialPortBLT.Close();
    }

    private void ReadUSB()
    {
        while (isReadingUSB)
        {
            try
            {
                if (serialPortUSB.IsOpen)
                {
                    string message = serialPortUSB.ReadLine();
                    lock (usbQueue)
                    {
                        usbQueue.Enqueue(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("USB Read Thread Error: " + ex.Message);
            }
        }
    }

    private void ReadBLT()
    {
        while (isReadingBLT)
        {
            try
            {
                if (serialPortBLT.IsOpen)
                {
                    string message = serialPortBLT.ReadLine();
                    lock (bltQueue)
                    {
                        bltQueue.Enqueue(message);
                    }
                }
            }
            catch (TimeoutException)
            {
                // Timeout 처리
            }
            catch (Exception ex)
            {
                Debug.LogError("BLT Read Thread Error: " + ex.Message);
            }
        }
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
            }
        } else if(isSubsequence(msg, "start")) {
            if(!isRunning) {
                isRunning=true;
                charModel.GetComponent<Animator>().Play("Standard Run");
            }
        } 
    }

    void HandleMessageBluetooth(string message) {
        Debug.Log("Bluetooth Message: " + message);
        switch (message.Trim()) {
            case "left":
                StartCoroutine(Rotate(Quaternion.Euler(0, -90, 0)));
                break;
            case "right":
                StartCoroutine(Rotate(Quaternion.Euler(0, 90, 0)));
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

}

/*

public class PlayerMove : MonoBehaviour
{
    public GameObject charModel;
    public float moveSpeed = 20;
    public float leftRightSpeed = 25;
    public float jump = 20;
    private bool isJumping = false;
    private bool isComingDown= false;

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
*/