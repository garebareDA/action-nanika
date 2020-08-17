using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class startButton : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject GameManager;
    void Start()
    {
        GameManager = GameObject.Find("Manager");
    }
    

    public void StartButton()
    {
        GameManager.SendMessage("LoadScene");
        transform.parent.gameObject.SetActive(false);
    }
}
