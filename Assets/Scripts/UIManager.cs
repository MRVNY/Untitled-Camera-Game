using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [SerializeField] private UIPanel photoReview;
    [SerializeField] private UIPanel visualNovel;
    
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        
        SwitchUI(UIType.VisualNovel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public IEnumerator TakeScreenshot()
    {
        SwitchUI(UIType.None);
        
        yield return new WaitForEndOfFrame();
        Texture2D photo = ScreenCapture.CaptureScreenshotAsTexture();
        yield return new WaitForEndOfFrame();
        
        SwitchUI(UIType.PhotoReview);
        PhotoReview.Instance.photoFrame.sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), Vector2.zero);
        
        yield return new WaitForSeconds(2);
        SwitchUI(UIType.None);
    }

    public void SwitchUI(UIType type)
    {
        photoReview.gameObject.SetActive(false);
        visualNovel.gameObject.SetActive(false);
        
        switch (type)
        {
            case UIType.PhotoReview:
                photoReview.gameObject.SetActive(true);
                break;
            case UIType.VisualNovel:
                visualNovel.gameObject.SetActive(true);
                break;
            case UIType.None:
                break;
        }
    }
}

public enum UIType
{
    None,
    PhotoReview,
    VisualNovel
}
