using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class title : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        Button button = transform.Find("Button").GetComponent<Button>();
        EventSystem.current.SetSelectedGameObject(button.gameObject, null);
    }
}
