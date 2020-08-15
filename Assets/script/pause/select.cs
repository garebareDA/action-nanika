using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class select : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject GameMnager;
    public void OnButtonClick()
    {
        GameMnager = GameObject.Find("GameMnager");
        GameMnager.SendMessage("backToSelect");
    }
}
