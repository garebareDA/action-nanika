using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boom : MonoBehaviour
{
    public AnimationClip anim;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, anim.length);
    }
}