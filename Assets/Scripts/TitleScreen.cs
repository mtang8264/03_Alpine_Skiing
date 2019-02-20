using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TitleScreen : MonoBehaviour
{
    float fade = 0;

    public float fadeSpeed;
    public Text text;
    public float textOpacity;
    public Image image;
    public float imageOpacity;
    public TextMeshPro score;

    bool temp = true;

    // Start is called before the first frame update
    void Start()
    {
        score.text = "BEST TIME: " + GameManager.bestTime + "\nLEAST MISSES: " + GameManager.bestMisses;
    }

    // Update is called once per frame
    void Update()
    {
        if(temp)
        {
            temp = true;
            for (int i = 0; i < 3; i++)
            {
                MusicManager.instance.PlayLayer(i);
            }
        }

        if(Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene(1);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            fade += Time.deltaTime * fadeSpeed;
        }
        else
        {
            fade -= Time.deltaTime * fadeSpeed;
        }

        if(fade < 0)
        {
            fade = 0;
        }
        if(fade > 1)
        {
            fade = 1;
        }

        text.color = new Color(text.color.r, text.color.g, text.color.b, textOpacity * fade);
        image.color = new Color(image.color.r, image.color.g, image.color.b, imageOpacity * fade);
    }
}
