using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI objects")]
    [SerializeField] GameObject weaponsUI;
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject baseUI;
    [SerializeField] GameObject runeButton;
    [SerializeField] GameObject circleWipe;

    [Header("Transition variables")]
    [SerializeField] float duration;
    [SerializeField] Image circleWipeImage;
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 endPos;
    [SerializeField] TextMeshProUGUI userNameText;

    [Header("Pause bool")]
    public bool isPaused = false;

    private PlayerMovementInputActions movementActions;

    private void Awake()
    {
        movementActions = new PlayerMovementInputActions();
    }

    private void OnEnable()
    {
        movementActions.PlayerMap.Enable();
    }

    private void Start()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    private void Update()
    {
        if(movementActions.PlayerMap.Pause.triggered)
        {
            if (!isPaused && !GetComponent<SpawnEnemies>().isInTransition)
            {
                PauseGame();
            }
            else if (isPaused)
            {
                ResumeGame();
            }
        }
    }

    void PauseGame()
    {
        userNameText.text = PlayerPrefs.GetString("userName");

        GameObject.FindGameObjectWithTag("Player").GetComponent<Runes>().CloseInventory();
        GameObject.FindGameObjectWithTag("Player").GetComponent<Runes>().inventoryIsOpened = true;

        baseUI.SetActive(false);
        circleWipe.SetActive(false);
        pauseMenuUI.SetActive(true);

        isPaused = true;
        Time.timeScale = 0;

        weaponsUI.transform.position = new Vector3(100, 100, 0);
    }

    public void ResumeGame()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Runes>().CloseInventory();

        baseUI.SetActive(true);
        circleWipe.SetActive(true);
        pauseMenuUI.SetActive(false);

        isPaused = false;
        Time.timeScale = 1;
    }

    public void BackToMenu()
    {
        GetComponent<SpawnEnemies>().StopAllCoroutines();
        Time.timeScale = 1;
        circleWipe.SetActive(true);
        StartCoroutine(Transition(true));
    }

    public void NewRoom()
    {
        circleWipe.SetActive(true);
        GetComponent<SpawnEnemies>().StopAllCoroutines();
        Time.timeScale = 1;
        StartCoroutine(Transition(false));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator Transition(bool backToMenu)
    {
        SpawnEnemies sc = GameObject.FindGameObjectWithTag("Generator").GetComponent<SpawnEnemies>();
        StartCoroutine(sc.DeathCircleWipe());
        yield return new WaitForSeconds(1.5f);
        if (backToMenu)
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
