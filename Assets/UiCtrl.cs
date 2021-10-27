using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiCtrl : MonoBehaviour
{
    [SerializeField] public static string nameOfCurentUser;
    public GameObject messageBox;
    public Text bestPlayerText;
    public Button startBtn;
    private bool isValid;

    private void Start()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return new WaitWhile(() => DataSaver.Instance == null);
        yield return new WaitWhile(() => DataSaver.Instance.JsonParsed == false);
        PlayerItem player = DataSaver.Instance.GetBestPlayer();
        string bestMsg = player == null ? "no score available yet." :
            $"Best score : {player.userName} : {player.score}";
        bestPlayerText.text = bestMsg;
    }

    public void NameEntered(InputField field)
    {
        string inputText = field.text;
        isValid = !string.IsNullOrEmpty(inputText) && !inputText.Contains(" ") &&
            inputText.Length >= 2;

        messageBox.SetActive(!isValid);
        startBtn.enabled = isValid;
        startBtn.transform.GetComponent<Image>().color = Color.green;
        nameOfCurentUser = inputText;

        MyEventBroker.CallNameChanged(nameOfCurentUser);
    }

    public void StartScene()
    {
        if (isValid)
            SceneManager.LoadScene("main");
    }
}
