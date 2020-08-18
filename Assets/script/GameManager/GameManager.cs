using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager: MonoBehaviour
{
    public int currentStageNum = 0;
    [SerializeField]
    string[] stageName;
    private GameObject Player;
    private Vector3 restartPostion;
    private GameObject gameOverCanvasPrefab;
    private GameObject fadeCanvasPrefab;
    public GameObject missEffect;
    private bool missPlayer;
    private Vector3 restartPostionButton;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        DontDestroyOnLoad(gameObject);
        restartPostion = Player.transform.position;
        restartPostionButton = Player.transform.position;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        restartPostionButton = Player.transform.position;
    }

    IEnumerator miss(float[] times)
    {
        missPlayer = true;
        GameObject missEffects = Instantiate(missEffect);
        DontDestroyOnLoad(missEffects);
        yield return new WaitForSeconds(0.8f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(stageName[currentStageNum]);

        while (true)
        {
            yield return null;
            if (asyncLoad.progress >= 0.9f)
            {
                Player.transform.position = restartPostion;
                missEffects.GetComponent<Animator>().SetBool("out", true);
                yield return new WaitForSeconds(1f);
                Player.SendMessage("respwarn", times);
                asyncLoad.allowSceneActivation = true;
                Destroy(missEffects);
                missPlayer = false;
                break;
            }
        }
        
    }

    public IEnumerator restart()
    {
        Player.SendMessage("unPause");
        GameObject missEffects = Instantiate(missEffect);
        DontDestroyOnLoad(missEffects);
        missEffects.transform.Find("Text").gameObject.GetComponent<Text>().text = "Restart";
        yield return new WaitForSeconds(0.8f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(stageName[currentStageNum]);

        while (true)
        {
            yield return null;
            if (asyncLoad.progress >= 0.9f)
            {
                Player.transform.position = restartPostionButton;
                missEffects.GetComponent<Animator>().SetBool("out", true);
                yield return new WaitForSeconds(1f);
                asyncLoad.allowSceneActivation = true;
                Destroy(missEffects);
                break;
            }
        }
    }

    public IEnumerator title()
    {
        GameObject missEffects = Instantiate(missEffect);
        Time.timeScale = 1f;
        DontDestroyOnLoad(missEffects);
        missEffects.transform.Find("Text").gameObject.GetComponent<Text>().text = "Title";
        yield return new WaitForSeconds(0.8f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("title");

        while (true)
        {
            yield return null;
            if (asyncLoad.progress >= 0.9f)
            {
                Player.transform.position = restartPostionButton;
                missEffects.GetComponent<Animator>().SetBool("out", true);
                yield return new WaitForSeconds(1f);
                asyncLoad.allowSceneActivation = true;
                Destroy(missEffects);
                break;
            }
        }
    }

    public IEnumerator goal(string time)
    {
        GameObject missEffects = Instantiate(missEffect);
        Time.timeScale = 1f;
        DontDestroyOnLoad(missEffects);
        missEffects.transform.Find("Text").gameObject.GetComponent<Text>().text = "Goal";
        missEffects.transform.Find("Text (1)").gameObject.GetComponent<Text>().text = time;
        yield return new WaitForSeconds(0.8f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("title");

        while (true)
        {
            yield return null;
            if (asyncLoad.progress >= 0.9f)
            {
                Player.transform.position = restartPostionButton;
                missEffects.GetComponent<Animator>().SetBool("out", true);
                yield return new WaitForSeconds(1f);
                asyncLoad.allowSceneActivation = true;
                Destroy(missEffects);
                break;
            }
        }
    }

    IEnumerator waitForLoadScene(int stageNum)
    {
        yield return new WaitForSeconds(20);
        yield return SceneManager.LoadSceneAsync(stageName[stageNum]);
    }
}
