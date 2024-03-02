using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MainCamera : MonoBehaviour
{
    private Camera cam;
    private DepthOfField DOF;
    [SerializeField] private UnityEvent OnCapture;

    public static MainCamera Instance;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        
        cam = GetComponent<Camera>();
        //change volume focus distance
        GetComponentInChildren<Volume>().profile.TryGet(out DOF);
        DOF.focusDistance.value = 10;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        //camera rotate around face mouse
        cam.transform.RotateAround(Model.Instance.face.position, Vector3.up, Input.GetAxis("Mouse X"));
        
        //change focus with mouse scroll
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            DOF.focusDistance.value -= 0.1f;
            // print(DOF.focusDistance.value);

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && DOF.focusDistance.value<10) // backwards
        {
            DOF.focusDistance.value += 0.1f;
            // print(DOF.focusDistance.value);
        }
        
        //on click take screenshot
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(UIManager.Instance.TakeScreenshot());
        }
    }
}
