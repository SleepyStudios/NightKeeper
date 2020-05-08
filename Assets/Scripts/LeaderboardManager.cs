using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.InputSystem;

public struct LeaderboardEntry {
    public string name;
    public int score;
    public DateTime date;
}

public class LeaderboardManager : MonoBehaviour {
    public string url;
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
    public Transform leaderboardContent;
    public GameObject textEntryPrefab;
    public TMP_InputField nameInput, secretInput;
    public Button submitButton, refreshButton;
    public TextMeshProUGUI scoreText;
    private int score;
    public ScoreManager scoreManager;
    public TextMeshProUGUI infoMessage;
    private bool isRefreshing, isSubmitting;
    private string myName;
    private List<LeaderboardEntry> myEntries = new List<LeaderboardEntry>();

    private void Start() {
        StartCoroutine(GetEntries());
        SetupUI();
    }

    public void SetScore() {
        score = scoreManager.CalculateScore();
        scoreText.text = $"The darkness was too much to handle - it drove you insane\nYou scored {score} points";
    }

    IEnumerator GetEntries() {
        UnityWebRequest www = UnityWebRequest.Get(url + "/leaderboard?sort=top");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
            GameObject textEntry = Instantiate(textEntryPrefab, leaderboardContent);
            textEntry.GetComponent<TextMeshProUGUI>().text = "Something went wrong - please try again later or let us know on Twitter @SleepyStudios";
        } else {
            ParseEntries(www.downloadHandler.text);
        }
    }

    private void ParseEntries(string jsonString) {
        entries.Clear();
        foreach (Transform child in leaderboardContent) {
            Destroy(child.gameObject);
        }

        isSubmitting = false;
        if (refreshButton.gameObject.activeSelf) {
            isRefreshing = false;
            refreshButton.GetComponentInChildren<TextMeshProUGUI>().text = "refresh leaderboard";
        }

        JSONArray json = JSON.Parse(jsonString) as JSONArray;
        int index = 0;
        foreach (JSONNode node in json.Children) {
            index++;
            LeaderboardEntry entry = new LeaderboardEntry();
            entry.name = node["name"].Value;
            entry.score = node["score"].AsInt;
            entry.date = DateTime.Parse(node["date"].Value);
            entries.Add(entry);

            GameObject textEntry = Instantiate(textEntryPrefab, leaderboardContent);
            textEntry.GetComponent<LeaderboardEntryText>().AddEntry(entry, entry.name == myName, index);

            if (entry.name == myName) {
                myEntries.Add(entry);
            }
        }
    }

    public void NewEntry(string name, int score, string secret) {
        StartCoroutine(AddEntry(name, score, secret));
    }

    IEnumerator AddEntry(string name, int score, string secret) {
        JSONNode json = new JSONObject();
        json.Add("name", name);
        json.Add("score", score);
        json.Add("secret", secret);

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json.ToString());

        UnityWebRequest www = new UnityWebRequest(url + "/leaderboard?sort=top");
        www.method = UnityWebRequest.kHttpVerbPOST;
        www.downloadHandler = new DownloadHandlerBuffer();
        www.uploadHandler = new UploadHandlerRaw(bytes) {
            contentType = "application/json"
        };

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
            if (www.error.Contains("403")) {
                infoMessage.text = "That name already exists. If this is your name, make sure you entered the correct secret";
                isSubmitting = false;
                submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "submit";
            } else {
                infoMessage.text = "Something went wrong - please try again later or let us know on Twitter @SleepyStudios";
                HandleEntrySubmitted(false);
            }
        } else {
            myName = name;
            ParseEntries(www.downloadHandler.text);
            HandleEntrySubmitted(true);
        }
        yield return null;
    }

    private void SetupUI() {
        submitButton.onClick.AddListener(() => {
            if (!isSubmitting) {
                if (nameInput.text.Trim().Length > 0 && secretInput.text.Trim().Length > 0) {
                    isSubmitting = true;
                    submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "submitting...";
                    NewEntry(nameInput.text, score, secretInput.text);
                } else {
                    infoMessage.text = "Please enter a name and secret";
                }
            }
        });

        refreshButton.onClick.AddListener(() => {
            if (!isRefreshing) {
                isRefreshing = true;
                refreshButton.GetComponentInChildren<TextMeshProUGUI>().text = "refreshing...";
                StartCoroutine(GetEntries());
            }
        });
        refreshButton.gameObject.SetActive(false);
    }

    private void HandleEntrySubmitted(bool success) {
        if (success) {
            LeaderboardEntry topEntry = myEntries.OrderByDescending(e => e.score).First();
            int index = entries.IndexOf(topEntry) + 1;
            if (myEntries.Count == 1) {
                infoMessage.text = $"You've placed #{index} out of " + entries.Count + " entries";
            } else {
                LeaderboardEntry newestEntry = myEntries.OrderByDescending(e => e.date).First();
                int newestEntryIndex = entries.IndexOf(newestEntry) + 1;
                infoMessage.text = $"You've placed #{newestEntryIndex} out of " + entries.Count + $" entries. Your highest placement is #{index}";
            }
        }

        nameInput.gameObject.SetActive(false);
        secretInput.gameObject.SetActive(false);
        refreshButton.gameObject.SetActive(true);
        
        submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "play again";
        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(() => SceneManager.LoadScene("Game"));
    }

    private void Update() {
        if (Keyboard.current.spaceKey.wasPressedThisFrame) {
            SceneManager.LoadScene("Game");
        }
    }
}
