using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cinemachine;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MainCamera : MonoBehaviour
{
    private Camera _cam;
    private DepthOfField _dof;
    private Transform _sphere;
    private int _priority = 10;
    
    private JObject cameraTree;
    public List<CameraAngle> Angles = new List<CameraAngle>();
    // [SerializeField] private UnityEvent OnCapture;

    public static MainCamera Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _sphere = GetComponentInChildren<MeshFilter>().transform;
        _cam = GetComponent<Camera>();
        GetComponentInChildren<Volume>().profile.TryGet(out _dof);
        
        // _dof.focusDistance.value = 10;

        FocusOnFace();

        StartCoroutine(LoadCameraAngles());
    }

    // Update is called once per frame
    void Update()
    {
        //camera rotate around face mouse
        switch (UIManager.CurrentUI)
        {
            case UIType.None:
                _cam.transform.RotateAround(Model.Instance.face.position, Vector3.up, Input.GetAxis("Mouse X"));
                break;
            case UIType.Test:
                break;
            case UIType.Editor:
                break;
        }

        //change focus with mouse scroll
        if (Input.GetAxis("Mouse ScrollWheel") != 0f) // forward
        {
            _dof.focusDistance.value += Input.GetAxis("Mouse ScrollWheel") * GameData.Instance.focusSpeed;
        }

        //on click take screenshot
        if (Input.GetMouseButtonDown(0))
        {
            switch (UIManager.CurrentUI)
            {
                case UIType.None:
                    StartCoroutine(UIManager.Instance.TakeScreenshot());
                    break;
                case UIType.Test:
                    StartCoroutine(AngleTransition());
                    break;
                case UIType.Editor:
                    break;
            }
 
        }

        //Sphere at the focus distance
        // _sphere.position = _cam.transform.position + _cam.transform.forward * _dof.focusDistance.value;
    }
    
    public void AddAngle()
    {
        Angles.Add(new CameraAngle
        {
            Position = transform.position,
            Rotation = transform.eulerAngles,
            Name = "Angle " + MainCamera.Instance.Angles.Count
        });
    }

    void FocusOnFace()
    {
        _cam.transform.LookAt(Model.Instance.face);
        //calculate distance to face
        float distance = Vector3.Distance(_cam.transform.position, Model.Instance.face.position);
        _dof.focusDistance.value = distance;
    }
    
    private IEnumerator LoadCameraAngles()
    {
        cameraTree = JObject.Parse(File.ReadAllText(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "/CameraAngles.json"));
        foreach (var angle in cameraTree)
        {
            var position = angle.Value["position"];
            var rotation = angle.Value["rotation"];
            var name = angle.Key;
            Angles.Add(new CameraAngle
            {
                Position = new Vector3((float)position[0], (float)position[1], (float)position[2]),
                Rotation = new Vector3((float)rotation[0], (float)rotation[1], (float)rotation[2]),
                Name = name
            });
        }
        
        yield return null;
        // StartCoroutine(AngleTransition(angles[0], angles[1]));
    }

    public IEnumerator AngleTransition()
    {
        _priority += 10;
        //random
        CameraAngle to = Angles[Random.Range(0, Angles.Count)];
        print("Transitioning to " + to.Name);
        //create a new virtual camera
        var vcam = new GameObject("VirtualCamera").AddComponent<CinemachineVirtualCamera>();
        vcam.transform.position = to.Position;
        vcam.transform.rotation = Quaternion.Euler(to.Rotation);
        vcam.m_Lens.FieldOfView = 50;
        vcam.Priority = _priority;
        
        
        //transition to the new angle
        

        yield return null;
    }

}
