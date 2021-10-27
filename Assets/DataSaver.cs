using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

[Serializable]
public class DataSaver : MonoBehaviour
{
    public static DataSaver Instance;
    public static string savePath;
    public string name;
    public int score;
    public bool JsonParsed { get; private set; }

    int bestScore;
    string currentPlayerName;

    private List<PlayerItem> allPlayerItems = new List<PlayerItem>();

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/gamedata2.json";
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        if (File.Exists(savePath))
        {
            FetchAndParseJson();
        }
        else
            Debug.Log("File does not exist yet");
    }

    void OnEnable()
    {
        MyEventBroker.BestScoreChanged += DoOnScoreChanged;
        MyEventBroker.NameChanged += DoOnNameChanged;
    }

    void OnDisable()
    {
        MyEventBroker.BestScoreChanged -= DoOnScoreChanged;
        MyEventBroker.NameChanged -= DoOnNameChanged;
    }


    public PlayerItem GetBestPlayer()
    {
        PlayerItem bestPlayer = new PlayerItem();

        foreach (var item in allPlayerItems)
        {
            if (item.score >= bestScore)
            {
                bestScore = item.score;
                bestPlayer.score = item.score;
                bestPlayer.userName = item.userName;
            }
        }
        return bestPlayer;
    }


    private void DoOnScoreChanged(int newScore)
    {
        // update player data
        PlayerItem p = PlayerExists(currentPlayerName);
        if (p != null && p.score < newScore)
        {
            p.score = newScore;
            SaveToFileAsync();
        }
    }

    private void DoOnNameChanged(string name)
    {
        if (File.Exists(savePath))
        {
            PlayerItem p = PlayerExists(name);
            // check if name already exists
            // if it does not exist, save it
            if (p == null)
                SaveNameAsync(name);
            else currentPlayerName = p.userName;
        }
        else
        {
            SaveNameAsync(name);
        }
    }


    private async void FetchAndParseJson()
    {
        await LoadJsonAsync().ContinueWith(fetched =>
        {
            if (fetched != null && !string.IsNullOrEmpty(fetched.Result))
            {
                ParseJson(fetched.Result);
                fetched.Dispose();
            }
        });
        
    }


    private Task<string> LoadJsonAsync()
    {
        StreamReader stream = new StreamReader(savePath);
        {
            // read the json from the file into a string
            return stream.ReadToEndAsync();
        }
    }

    private void ParseJson(string loadedJsonString)
    {
        Debug.Log("ParseJson: " + loadedJsonString);
        try
        {
            allPlayerItems = JsonConvert.DeserializeObject<List<PlayerItem>>(loadedJsonString);
            if (allPlayerItems == null)
                Debug.Log("playerData after parsing is NULL ");
            else
                Debug.Log($"### Finished parsing. There are {allPlayerItems.Count} players");
            JsonParsed = true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }



    private PlayerItem PlayerExists(string name)
    {
        for (int i = 0; i < allPlayerItems.Count; i++)
        {
            if (allPlayerItems[i].userName.Equals(name))
                return allPlayerItems[i];
        }
        return null;
    }

    private async void SaveNameAsync(string name)
    {
        PlayerItem item = new PlayerItem();
        item.userName = name;
        item.score = 0;
        currentPlayerName = name;
        allPlayerItems.Add(item);

        Debug.Log($"SaveName = {name}; {allPlayerItems.Count} players in array to convert " );
        await SaveToFileAsync();
    }


    private async Task SaveToFileAsync()
    {
        string jsonStringToConvert = JsonConvert.SerializeObject(allPlayerItems);

        Debug.Log("to save. jsonString = " + jsonStringToConvert);
        using (StreamWriter stream = new StreamWriter(savePath))
        {
            await stream.WriteAsync(jsonStringToConvert);
        }
    }
}
