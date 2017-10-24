using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class NumberCreater : MonoBehaviour
{
    public enum FontType
    {
        NumberFont0,
    }

    GameObject[] numberFonts = new GameObject[System.Enum.GetValues(typeof(FontType)).Length];


    // Use this for initialization
    void Awake()
    {
        LoadNumbersEnumString();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    // enumの文字列でファイル名指定(楽で直打ち無しだが、不意のエラーが怖い)
    void LoadNumbersEnumString()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(FontType)).Length; i++)
        {
            var fileName = "Prefabs/Numbers/" + ((FontType)i).ToString();
            numberFonts[i] = (GameObject)Resources.Load(fileName);
#if UNITY_EDITOR
            if (!numberFonts[i])
            {
                EditorUtility.DisplayDialog("エラー", fileName + "\r\n対応するエフェクトがResouce/Prefabs/Numbersに入っていない", "OK");
                continue;
            }
#endif
        }
    }

    public GameObject CreateNumberObject(float scale, float width, Vector3 position, FontType fontType = FontType.NumberFont0)
    {
        var newNumberObject = /*(RectTransform)*/Instantiate(numberFonts[(int)fontType]).transform;
        var newNumber = newNumberObject.GetComponent<Number>();
        newNumber.scale = scale;
        newNumber.width = width;
        newNumber.SetPosision(position);

        // InstantiateはStart関数が呼ばれない？ので自分で呼んでやる
        newNumberObject.GetComponent<ObjectPoller>().Pool();

        return newNumber.gameObject;
    }

    public Number CreateNumber(float scale, float width, Vector3 position, FontType fontType = FontType.NumberFont0)
    {
        return CreateNumberObject(scale, width, position, fontType).GetComponent<Number>();
    }
}
