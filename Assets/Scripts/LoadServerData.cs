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

    [Header("Fallback Image")]
    [SerializeField] private Sprite fallbackImageLoading;
    [SerializeField] private Sprite fallbackImageError;

    private void Start()
    {
        SetupButtonListeners();
        GetSlideData();
    }

    private IEnumerator GetImageFromServer(string url = null)
    {
        if (!image) yield break;

        var request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var texture = DownloadHandlerTexture.GetContent(request);
            var rect = new Rect(0, 0, texture.width, texture.height);
            var pivot = new Vector2(texture.width / 2.0f, texture.height / 2.0f);
            var sprite = Sprite.Create(texture, rect, pivot);
            image.sprite = sprite;
        }
        else
        {
            image.sprite = fallbackImageError;
        }
    }

    private IEnumerator GetTextFromServer(string url = null)
    {
        if (!text) yield break;

        var request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        text.text = request.result == UnityWebRequest.Result.Success
            ? request.downloadHandler.text
            : "Error loading text";
    }

    private void SetSlideIndexText(int index = -1)
    {
        var indexText = "??";
        if (index >= 0) indexText = "#" + (index + 1);
        if (textIndex) textIndex.text = indexText;
    }

    private void SetLoadingState(string imageUrl, string textUrl)
    {
        if (image) image.sprite = fallbackImageLoading;
        if (inputImageUrl) inputImageUrl.text = imageUrl;

        if (text) text.text = "Loading...";
        if (inputTextUrl) inputTextUrl.text = textUrl;
    }

    private void GetSlideData(int index = 0)
    {
        var imageUrl = BaseUrl + $"00{index + 1}.png";
        var textUrl = BaseUrl + $"00{index + 1}.txt";

        SetSlideIndexText(index);
        SetLoadingState(imageUrl, textUrl);

        StartCoroutine(GetImageFromServer(imageUrl));
        StartCoroutine(GetTextFromServer(textUrl));
    }

    private void HandleClickPrevButton()
    {
        _index--;
        if (_index < 0) _index = MaxIndex;
        GetSlideData(_index);
    }

    private void HandleClickNextButton()
    {
        _index++;
        if (_index > MaxIndex) _index = 0;
        GetSlideData(_index);
    }

    private void HandleClickLoadButton()
    {
        StartCoroutine(GetImageFromServer(inputImageUrl.text));
        StartCoroutine(GetTextFromServer(inputTextUrl.text));
        SetSlideIndexText();
        _index = -1;
    }

    private void SetupButtonListeners()
    {
        if (buttonPrevious)
            buttonPrevious.onClick.AddListener(HandleClickPrevButton);

        if (buttonNext)
            buttonNext.onClick.AddListener(HandleClickNextButton);

        if (buttonLoad)
            buttonLoad.onClick.AddListener(HandleClickLoadButton);
    }
}
