using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PhotoReview : UIPanel
{
    public static PhotoReview Instance;
    
    public Image photoFrame;
    
    // Start is called before the first frame update
    new void Start()
    {
        Instance = this;
        base.Start();

        photoFrame = transform.GetChild(0).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
