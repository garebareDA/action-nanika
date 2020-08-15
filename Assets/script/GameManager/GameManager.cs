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
        DontDestroyOnLoad(gameObject);
        Player = GameObject.FindGameObjectWithTag("Player");
        restartPostion = Player.transform.position;
        restartPostionButton = Player.transform.position;
        SceneManager.sceneLoaded += OnSceneLoaded;
        miss();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        restartPostionButton = Player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator miss()
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

    public void setRestartPotiosn(Vector3 potion)
    {
        restartPostion = potion;
    }

    IEnumerator waitForLoadScene(int stageNum)
    {
        yield return new WaitForSeconds(20);
        yield return SceneManager.LoadSceneAsync(stageName[stageNum]);
    }
}
