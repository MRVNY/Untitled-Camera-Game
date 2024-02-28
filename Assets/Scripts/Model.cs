using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Model : MonoBehaviour
{
    public Transform face;
    public Image image;
    public GameObject canvas;
    public Volume volume;
    private DepthOfField DOF;
    
    // Start is called before the first frame update
    void Start()
    {
        //camera focus on face
        Camera.main.transform.LookAt(face);
        // Camera.main.aperture = 1.7f;
        // Camera.main.usePhysicalProperties = true;
        image.enabled = false;
        
        //change volume focus distance
        volume.profile.TryGet(out DOF);

        DOF.focusDistance.value = 10;

        // StartCoroutine(TakeScreenshot());
    }

    // Update is called once per frame
    void Update()
    {
        //camera rotate around face mouse
        Camera.main.transform.RotateAround(face.position, Vector3.up, Input.GetAxis("Mouse X"));
        
        //on click take screenshot
        if (Input.GetMouseButtonDown(0))
        {
            Coroutine taking = StartCoroutine(TakeScreenshot());
        }
        
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
    }
    
    IEnumerator TakeScreenshot()
    {
        canvas.SetActive(false);
        yield return new WaitForEndOfFrame();
        Texture2D photo = ScreenCapture.CaptureScreenshotAsTexture();

        yield return new WaitForEndOfFrame();
        canvas.SetActive(true);
        image.enabled = true;
        image.sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), Vector2.zero);
        
        yield return new WaitForSeconds(2);
        image.enabled = false;
    }
}
