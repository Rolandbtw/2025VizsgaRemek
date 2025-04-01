using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
using SimpleJSON;
using System.Net;
using System;

public class MainMenu : MonoBehaviour
{
    [Header("Transition variables")]
    [SerializeField] Image circleWipeImage;
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 endPos;
    [SerializeField] float moveDuration;

    [Header("Knight spinning variables")]
    [SerializeField] GameObject knight;
    [SerializeField] GameObject effect;
    bool isRotating=false;

    [Header("UI variables")]
    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject loginButton;
    [SerializeField] TextMeshProUGUI errorMessage;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TMP_InputField email;
    [SerializeField] TMP_InputField password;

    public bool guest = true;

    private void Start()
    {
        StartCoroutine(Transition(endPos, startPos, moveDuration, false));
        PlayerPrefs.SetString("userName","Guest");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isRotating)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePos);
            if (hitCollider != null && hitCollider.gameObject == knight)
            {
                StartCoroutine(FlipKnight(0.5f));
                Instantiate(effect, knight.transform.position, effect.transform.rotation);
                isRotating = true;
            }
        }
    }

    public void StartGame()
    {
        StartCoroutine(Transition(startPos, endPos, moveDuration, true));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator Transition(Vector3 startPos, Vector3 endPos, float duration, bool nextScene)
    {
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            circleWipeImage.rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        if (nextScene)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    
    public void LoginPanelOn()
    {
        loginPanel.SetActive(true);
    }

    public void LoginPanelOff()
    {
        errorMessage.text = "";
        loginPanel.SetActive(false);
    }

    public void SettingsPanelOn()
    {
        settingsPanel.SetActive(true);
    }

    public void SettingsPanelOff()
    {
        settingsPanel.SetActive(false);
    }

    public void LoginButton()
    {
        if (email.text == "" || password.text == "")
        {
            errorMessage.text = "Please do not leave any field empty.";
        }
        else
        {
            guest = false;
            StartCoroutine(Login());
        }
    }
    IEnumerator Login()
    {
        string url = "http://localhost:8000/api/user/login";
        string postdata = "{ \"email\": \""+email.text+"\",\"password\": \""+password.text+"\"}";
        Debug.Log(postdata);
        using (UnityWebRequest request = UnityWebRequest.Post(url, postdata, "application/json"))
        {
 
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                errorMessage.text = request.error;
            }
            else
            {
                JSONNode info = JSON.Parse(request.downloadHandler.text);

                Debug.Log(info);

                PlayerPrefs.SetString("userName", info["user"]["name"].ToString().Replace("\"", ""));
                nameText.text = $"Welcome {PlayerPrefs.GetString("userName")}";
                
                PlayerPrefs.SetInt("id", int.Parse(info["user"]["id"].ToString()));
                PlayerPrefs.SetInt("deathcount", int.Parse(info["user"]["deaths"].ToString()));

                PlayerPrefs.SetString("token", info["user"]["token"].ToString().Replace("\"", ""));

                PlayerPrefs.SetInt("bestWaves", int.Parse(info["user"]["waves"].ToString()));
                PlayerPrefs.SetInt("boss1lvl", int.Parse(info["user"]["boss1lvl"].ToString()));
                PlayerPrefs.SetInt("boss2lvl", int.Parse(info["user"]["boss2lvl"].ToString()));
                PlayerPrefs.SetInt("boss3lvl", int.Parse(info["user"]["boss3lvl"].ToString()));
                PlayerPrefs.SetInt("kills", int.Parse(info["user"]["kills"].ToString()));

                PlayerPrefs.SetString("user", "loggedin");

                Debug.Log(PlayerPrefs.GetString("token")); 
                loginPanel.SetActive(false);
                loginButton.SetActive(false);
                StartCoroutine(Transition(startPos, endPos, moveDuration, true));
            }
        }
    }   

                

    IEnumerator FlipKnight(float duration)
    {
        float timeElapsed = 0f;
        Quaternion startRotation = knight.transform.rotation;

        while (timeElapsed < duration)
        {
            float angle = Mathf.Lerp(0, 360, timeElapsed / duration);
            knight.transform.rotation = startRotation * Quaternion.Euler(0, angle, 0);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        knight.transform.rotation = startRotation * Quaternion.Euler(0, 360, 0);
        isRotating = false;
    }
}
