using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MainCamera))]
public class CameraEditor: Editor
{
    private string _angleName;

    private JObject _jObject;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MainCamera myScript = (MainCamera)target;
        
        //A list of CameraAngle struct
        GUILayout.Label("Camera Angles");
        var transform = myScript.transform;
        
        for(int i = 0; i < MainCamera.Instance.Angles.Count; i++)
        {
            var cameraAngle = MainCamera.Instance.Angles[i];
            //editable textfield
            
            string tmp = EditorGUILayout.TextField(""+cameraAngle.Name,"");
            if(tmp != "")
            {
                _angleName = tmp;
            }
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Go"))
            {
                transform.position = cameraAngle.Position;
                transform.eulerAngles = cameraAngle.Rotation;
            }
            if(GUILayout.Button("Rename"))
            {
                // cameraAngle.name = GUILayout.TextField(angleName);
                MainCamera.Instance.Angles.Insert(i+1,new CameraAngle()
                {
                    Name = _angleName,
                    Position = cameraAngle.Position,
                    Rotation = cameraAngle.Rotation
                });
                MainCamera.Instance.Angles.RemoveAt(i);
            }
            if (GUILayout.Button("Delete"))
            {
                MainCamera.Instance.Angles.RemoveAt(i);
            }
            GUILayout.EndHorizontal();
        }
        
        GUILayout.Label("Current Angle");
        GUILayout.Label(""+transform.position + transform.eulerAngles);
        if (GUILayout.Button("Add Angle"))
        {
            MainCamera.Instance.AddAngle();
        }
        if(GUILayout.Button("Save All"))
        {
            //save as JObject
            _jObject = new JObject();
            for (int i = 0; i < MainCamera.Instance.Angles.Count; i++)
            {
                _jObject.Add(MainCamera.Instance.Angles[i].Name, new JObject()
                {
                    {"position", new JArray(MainCamera.Instance.Angles[i].Position.x, MainCamera.Instance.Angles[i].Position.y, MainCamera.Instance.Angles[i].Position.z)},
                    {"rotation", new JArray(MainCamera.Instance.Angles[i].Rotation.x, MainCamera.Instance.Angles[i].Rotation.y, MainCamera.Instance.Angles[i].Rotation.z)}
                });
            }
            
            File.WriteAllText(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "/CameraAngles.json", _jObject.ToString());
        }

        if (_jObject != null)
        {
            GUILayout.TextArea(_jObject.ToString());
        }
    }
}

public struct CameraAngle
{
    public Vector3 Position;
    public Vector3 Rotation;
    public string Name;
}
