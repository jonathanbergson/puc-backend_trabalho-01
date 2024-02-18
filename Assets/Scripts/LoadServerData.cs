using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadServerData : MonoBehaviour
{
    private int _index;
    private const int MaxIndex = 4;
    private const string BaseUrl =
        "https://raw.githubusercontent.com/jonathanbergson/puc-backend_trabalho-01/main/Assets/Data/";

    [Header("History Image")]
    [SerializeField] private Image image;
    [SerializeField] private InputField inputImageUrl;

    [Header("History Text")]
    [SerializeField] private Text text;
    [SerializeField] private InputField inputTextUrl;

    [Header("History Controls")]
    [SerializeField] private Text textIndex;
    [SerializeField] private Button buttonNext;
    [SerializeField] private Button buttonPrevious;
    [SerializeField] private Button buttonLoad;

    private void Start()
    {
        SetupButtonClicks();
        LoadData();
    }

    private IEnumerator LoadImage(string url = null)
    {
        if (inputImageUrl) inputImageUrl.text = url;
        var request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var texture = DownloadHandlerTexture.GetContent(request);
            var rect = new Rect(0, 0, texture.width, texture.height);
            var pivot = new Vector2(texture.width / 2.0f, texture.height / 2.0f);
            var sprite = Sprite.Create(texture, rect, pivot);
            if (image) image.sprite = sprite;
        }
    }

    private IEnumerator LoadText(string url = null)
    {
        if (inputTextUrl) inputTextUrl.text = url;
        var request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (text) text.text = request.downloadHandler.text;
        }
    }

    private void SetupButtonClicks()
    {
        if (buttonPrevious)
        {
            buttonPrevious.onClick.AddListener(() =>
            {
                _index--;
                if (_index < 0) _index = MaxIndex;
                LoadData(_index);
            });
        }

        if (buttonNext)
        {
            buttonNext.onClick.AddListener(() =>
            {
                _index++;
                if (_index > MaxIndex) _index = 0;
                LoadData(_index);
            });
        }

        if (buttonLoad)
        {
            buttonLoad.onClick.AddListener(LoadDataFromInput);
        }
    }

    private void LoadData(int index = 0)
    {
        var imageUrl = BaseUrl + $"00{index + 1}.png";
        var textUrl = BaseUrl + $"00{index + 1}.txt";
        StartCoroutine(LoadImage(imageUrl));
        StartCoroutine(LoadText(textUrl));
        if (textIndex) textIndex.text = "#" + (_index + 1);
    }

    private void LoadDataFromInput()
    {
        StartCoroutine(LoadImage(inputImageUrl.text));
        StartCoroutine(LoadText(inputTextUrl.text));
        if (textIndex) textIndex.text = "??";
    }
}
