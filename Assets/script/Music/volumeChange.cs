using UnityEngine;
using System.Collections;

public class volumeChange : MonoBehaviour
{

    [SerializeField]
    UnityEngine.Audio.AudioMixer mixer;

    public float masterVolume
    {
        set { mixer.SetFloat("MasterVol",value); }
    }

    public float SEVolume
    {
        set { mixer.SetFloat("SEVol", value); }
    }

    public float BGMVolume
    {
        set { mixer.SetFloat("BGMVol", value); }
    }
}
