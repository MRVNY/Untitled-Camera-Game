using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class MainCamera : MonoBehaviour
{
    private Camera _cam;
    private DepthOfField _dof;
    private Transform _sphere;
    // [SerializeField] private UnityEvent OnCapture;
    
    private List<Vector3> _camPoses = new List<Vector3>();
    private List<Vector3> _camRots = new List<Vector3>();

    public static MainCamera Instance;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        _sphere = GetComponentInChildren<MeshFilter>().transform;
        _cam = GetComponent<Camera>();
        //change volume focus distance
        GetComponentInChildren<Volume>().profile.TryGet(out _dof);
        // _dof.focusDistance.value = 10;
        
        FocusOnFace();
    }

    // Update is called once per frame
    void Update()
    {
        //camera rotate around face mouse
        _cam.transform.RotateAround(Model.Instance.face.position, Vector3.up, Input.GetAxis("Mouse X"));
        
        //Sphere at the focal point
        
        
        //change focus with mouse scroll
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            _dof.focusDistance.value -= 0.1f;
            // print(DOF.focusDistance.value);

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && _dof.focusDistance.value<10) // backwards
        {
            _dof.focusDistance.value += 0.1f;
            // print(DOF.focusDistance.value);
        }
        
        //on click take screenshot
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(UIManager.Instance.TakeScreenshot());
        }
    }
    
    void FocusOnFace()
    {
        _cam.transform.LookAt(Model.Instance.face);
        //calculate distance to face
        float distance = Vector3.Distance(_cam.transform.position, Model.Instance.face.position);
        _dof.focusDistance.value = distance;
        print(distance);
    }
    
    [CustomEditor(typeof(MainCamera))]
    public class GameEditor: Editor
    {
        public static GameEditor Instance;
        
        private void Awake()
        {
            Instance = this;
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            MainCamera myScript = (MainCamera)target;
            if (GUILayout.Button("Add Angle"))
            {
                AddAngle();
            }
            
            for(int i = 0; i < myScript._camPoses.Count; i++)
            {
                if (GUILayout.Button("Go to Angle " + i))
                {
                    myScript.transform.position = myScript._camPoses[i];
                    myScript.transform.eulerAngles = myScript._camRots[i];
                }
            }
        }
        
        public void AddAngle()
        {
            MainCamera myScript = (MainCamera)target;
            myScript._camPoses.Add(myScript.transform.position);
            myScript._camRots.Add(myScript.transform.eulerAngles);
        }
    }
}
