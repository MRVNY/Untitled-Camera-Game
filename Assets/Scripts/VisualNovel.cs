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

public class VisualNovel : UIPanel
{ 
    public static VisualNovel Instance;
    // Start is called before the first frame update

    private string node;
    private JObject convoTree;

    private TextMeshProUGUI TextBox;
    private List<string> toWrite = new List<string>();
    private string written = "";
    
    private Task writing;
    // public Button skipButton;
    bool skipped = false;
    
    private void Start()
    {
        Instance = this;
        base.Start();
        
        if (GameData.S.convoNode != null)
        {
            StartCoroutine("LoadConvo");
            
            node = GameData.S.convoNode;
            
            TextBox = GetComponentInChildren<TextMeshProUGUI>();
            // skipButton = GetComponent<Button>();
            // skipButton.onClick.AddListener(() => { Next(); });
        }

        setDialog();
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
    
    public void ClearDialog()
    {
        toWrite.Clear();
        written = "";
        TextBox.text = "";
    }
    
    public void setDialog()
    {
        node = GameData.S.convoNode;
        JToken jNode = convoTree[node];
        if (jNode == null)
        {
            UIManager.Instance.SwitchUI(UIType.None);
            return;
        }

        // set text
        if (toWrite.Count == 0)
        {
            string text = jNode[GameData.S.gameLanguage.ToString()].ToString();
            //if (text.Contains("{name}")) text = text.Replace("{name}", Global.GD.player);
            toWrite = text.Split('\n').ToList();
        }

        writing = TypeWriter(toWrite[0]);
    }

    
    async Task TypeWriter(string toType)
    {
        skipped = false;
        // skipButton.enabled = true;
        string story = toType;
        TextBox.text = written;
        foreach (char c in story) 
        {
            if(skipped) break;
            TextBox.text += c;
            await Task.Delay(10);
        }
        written += story + "\n\n";
        skipped = true;
    }
    public async void Next()
    {
        if (skipped && toWrite.Count <= 1)
        {
            UIManager.Instance.SwitchUI(UIType.None);
            toWrite.Clear();
        }

        else if (skipped)
        {
            toWrite.RemoveAt(0);
            // skipButton.enabled = false;
            if (toWrite.Count == 0)
            {
                UIManager.Instance.SwitchUI(UIType.None);
            }
            else setDialog();
        }
		
        else if (writing != null)
        {
            skipped = true;
            await writing;
            TextBox.text = written;
			
            // if (toWrite.Count==1) skipButton.enabled = true;
        }
    }
}
