using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualNovel : UIPanel
{ 
    public static VisualNovel Instance;
    // Start is called before the first frame update
    new void Start()
    {
        Instance = this;
        
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
