using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class option : MonoBehaviour
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
        GameObject options = transform.parent.transform.parent.Find("options").gameObject;
        EventSystem.current.SetSelectedGameObject(null);
        options.SetActive(true);
        transform.parent.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(options.transform.Find("Slider").GetComponent<Slider>().gameObject, null);
    }
}
