using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class backMenue: MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClick()
    {
        GameObject buttons = transform.parent.transform.parent.Find("buttons").gameObject;
        EventSystem.current.SetSelectedGameObject(null);
        buttons.SetActive(true);
        transform.parent.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(buttons.transform.Find("Button").GetComponent<Button>().gameObject, null);
    }
}
