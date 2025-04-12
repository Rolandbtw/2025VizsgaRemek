using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

public class PauseMenu : MonoBehaviour
{
    [Header("UI objects")]
    [SerializeField] GameObject weaponsUI;
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject baseUI;
    [SerializeField] GameObject runeButton;
    [SerializeField] GameObject circleWipe;
    [SerializeField] GameObject runesButton;

    [Header("Transition variables")]
    [SerializeField] float duration;
    [SerializeField] Image circleWipeImage;
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 endPos;
    [SerializeField] TextMeshProUGUI userNameText;

    [Header("Pause bool")]
    public bool isPaused = false;

    private Runes runesScript;
    private PlayerMovementInputActions movementActions;

    private void Awake()
    {
        if (movementActions == null)
        {
            movementActions = new PlayerMovementInputActions();
        }
    }

    private void OnEnable()
    {
        movementActions.PlayerMap.Enable();
    }

    private void Start()
    {
        runesScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Runes>();
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

        runesScript.inventoryIsOpened = true;

        runesButton.SetActive(false);
        baseUI.SetActive(false);
        circleWipe.SetActive(false);
        pauseMenuUI.SetActive(true);

        isPaused = true;

        weaponsUI.transform.position = new Vector3(100, 100, 0);

        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        runesScript.CloseInventory();

        runesButton.SetActive(true);
        baseUI.SetActive(true);
        circleWipe.SetActive(true);
        pauseMenuUI.SetActive(false);

        isPaused = false;
        Time.timeScale = 1;
    }

    public void BackToMenu()
    {
        Time.timeScale = 1;
        GetComponent<SpawnEnemies>().StopAllCoroutines();
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
        Addressables.ClearResourceLocators();
        Resources.UnloadUnusedAssets();

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
