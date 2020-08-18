using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class retry : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject gameManger;
    public void OnButtonClick()
    {   gameManger = GameObject.Find("Game Manager");
        gameManger.SendMessage("restart");
    }
}
