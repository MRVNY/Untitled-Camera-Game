using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class VisualNovel : UIPanel
{ 
    public static VisualNovel Instance;
    // Start is called before the first frame update
    private string node;
    private JObject convoTree;
    private JObject cameraTree;

    private TextMeshProUGUI _textBox;
    private List<string> _toWrite = new List<string>();
    private string _toType = "";
    private string _written = "";
    
    private Task writing;
    bool skipped = false;
    private State _state = State.Done;
    
    private new void Start()
    {
        Instance = this;
        base.Start();
        
        if (GameData.S.convoNode != null)
        {
            StartCoroutine(LoadConvo());
            
            node = GameData.S.convoNode;
            
            _textBox = GetComponentInChildren<TextMeshProUGUI>();
        }

        SetDialog();
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && UIManager.CurrentUI == UIType.VisualNovel)
        {
            // Next();
        }
    }

    private IEnumerator LoadConvo()
    {
        if(Application.platform == RuntimePlatform.WebGLPlayer)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Dialog.json");
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load data from URL: " + webRequest.error);
                yield break;
            }

            convoTree = JObject.Parse(webRequest.downloadHandler.text);
        }
        else
            convoTree = JObject.Parse(File.ReadAllText(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Dialog.json"));
    }
    
    private void TestCamera()
    {
        // for
    }
    
    public void ClearDialog()
    {
        _toWrite.Clear();
        _written = "";
        _textBox.text = "";
    }

    private void SetDialog()
    {
        node = GameData.S.convoNode;
        JToken jNode = convoTree[node];
        if (jNode == null)
        {
            UIManager.Instance.SwitchUI(UIType.None);
            return;
        }

        // set text
        if (_toWrite.Count == 0)
        {
            string text = jNode[GameData.S.gameLanguage.ToString()]?.ToString();
            if (text != null) _toWrite = text.Split('\n').ToList();
        }

        writing = TypeWriter(_toWrite[0], Color.black);
    }
    
    async Task TypeWriter(string text, Color color)
    {
        _state = State.Type;
        
        _toType = text;
        _written = "";
        _textBox.text = "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + _written + "</color>" + "<color=#00000000>" + _toType+ "</color>" ;

        while (_toType.Length > 0)
        {
            if(_state == State.Skip) break;
            
            //pop first character
            char c = _toType[0];
            _toType = _toType.Remove(0, 1);
            _written += c;
            _textBox.text = "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + _written + "</color>" + "<color=#00000000>" + _toType+ "</color>" ;
            
            await Task.Delay(GameData.Instance.textSpeed);
        }
        
        if(_state == State.Skip) _textBox.text = "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + text + "</color>";
        _state = State.Done;
    }
    public async void Next()
    {
        if (skipped && _toWrite.Count <= 1)
        {
            UIManager.Instance.SwitchUI(UIType.None);
            _toWrite.Clear();
        }

        else if (skipped)
        {
            _toWrite.RemoveAt(0);
            if (_toWrite.Count == 0)
            {
                UIManager.Instance.SwitchUI(UIType.None);
            }
            else SetDialog();
        }
		
        else if (writing != null)
        {
            skipped = true;
            await writing;
            _textBox.text = _written;
        }
    }
    
    private enum State
    {
        Type,
        Skip,
        Done
    }
}
