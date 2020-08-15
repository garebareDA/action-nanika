using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class back : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject player; 

    public void OnButtonClick()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.SendMessage("unPause");
    }
}
