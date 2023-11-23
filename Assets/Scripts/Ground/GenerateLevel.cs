using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{
    public GameObject[] section;
    public int zPos = 33;
    public bool creatingSection = false;
    public int secNum;

    // Update is called once per frame
    void Update()
    {
        if(creatingSection == false)
        {
            creatingSection = true;
            StartCoroutine(GenerateSection());
        }
    }

    IEnumerator GenerateSection(){
        secNum = Random.Range(0, section.Length);
        zPos += 112;
        Instantiate(section[secNum], new Vector3(-8, 0, zPos), Quaternion.identity);
        
        yield return new WaitForSeconds(2);
        creatingSection = false;
    }
}
