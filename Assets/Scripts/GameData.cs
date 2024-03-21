using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public struct SaveData
{
    public char gameLanguage;
    public string convoNode;
}
public class GameData : MonoBehaviour
{
    public static SaveData S;
    public static GameData Instance;
    private static string savePath;
    
    
    public float focusSpeed = 1;
    public int textSpeed = 100;
    public UIType currentUI = UIType.None;
    
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        savePath = Application.persistentDataPath + "/saves/";
        Load();
        
        if (S.gameLanguage  == '\0')
        {
            S = new SaveData()
            {
                gameLanguage = 'e',
                convoNode = "0"
            };
        }

    }
    
    public static void Save()
    {
        Save(S, "GameSave");
    }

    public static void Load()
    {
        //DeleteAllSaveFiles();
        S = Load<SaveData>("GameSave");
    }
    
    public static bool SaveExists(string key)
    {
        string path = savePath + key + ".txt";
        return File.Exists(path);
    }
    
    public static void Save<T>(T objectToSave, string key)
    {
        Directory.CreateDirectory(savePath);
        BinaryFormatter formatter = new BinaryFormatter();

        using FileStream fileStream = new FileStream(savePath + key + ".txt", FileMode.Create);
        formatter.Serialize(fileStream, objectToSave);
    }
    
    public static T Load<T>(string key)
    {
        if (SaveExists(key))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using FileStream fileStream = new FileStream(savePath + key + ".txt", FileMode.Open);
            var returnValue = (T)formatter.Deserialize(fileStream);

            return returnValue;
        }
        else return default(T);
    }
    
    public static void ClearSave()
    {
        DirectoryInfo dir = new DirectoryInfo(savePath);
        dir.Delete(true);
        Directory.CreateDirectory(savePath);
        PlayerPrefs.DeleteAll();
    }
}




