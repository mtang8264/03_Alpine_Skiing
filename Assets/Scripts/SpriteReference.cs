using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteReference : MonoBehaviour
{
    public Reference[] references;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Sprite GetSprite(string name)
    {
        for (int i = 0; i < references.Length; i++)
        {
            if(references[i].name.Equals(name))
            {
                return references[i].sprite;
            }
        }

        return null;
    }
}

[System.Serializable]
public class Reference
{
    public string name;
    public Sprite sprite;
}