using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class manager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject missEffect;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator LoadScene()
    {
        GameObject missEffects = Instantiate(missEffect);
        DontDestroyOnLoad(missEffects);
        missEffects.transform.Find("Text").gameObject.GetComponent<Text>().text = "Blue City";
        yield return new WaitForSeconds(0.8f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("action");

        while (true)
        {
            yield return null;
            if (asyncLoad.progress >= 0.9f)
            {
                missEffects.GetComponent<Animator>().SetBool("out", true);
                yield return new WaitForSeconds(1f);
                asyncLoad.allowSceneActivation = true;
                Destroy(missEffects);
                Destroy(gameObject);
                break;
            }
        }
    }

    public void OnApplicationQuit()
    {
        UnityEngine.Application.Quit();
    }
}
