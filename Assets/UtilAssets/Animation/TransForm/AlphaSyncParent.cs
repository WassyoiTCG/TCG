using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaSyncParent : MonoBehaviour {

    Image myImage;
    Image parentImage;

	// Use this for initialization
	void Awake()
    {
        myImage = GetComponent<Image>();
        parentImage = transform.parent.GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    // 親の画像のαと同期する
    	if(myImage && parentImage)
        {
            var newColor = myImage.color;
            newColor.a = parentImage.color.a;
            myImage.color = newColor;
        }
	}
}
