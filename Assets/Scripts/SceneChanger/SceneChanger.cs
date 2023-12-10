using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    public void StartGame() {
        Debug.Log("Start Game");
        SceneManager.LoadScene("GameScene");
    }

    public void InitGame() {
        Debug.Log("Init Game");
        SceneManager.LoadScene("StartScene");
    }
}
