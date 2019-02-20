using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagSpawner : MonoBehaviour
{
    public GameObject blueH, blueV, redH, redV;
    public GameObject finishLine;
    public Vector2 firstPosition;
    public float minX, maxX;
    public int flagsToSpawn = 55;
    [Range(0, 1)]
    public float verticalChance;
    public bool red = false;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < flagsToSpawn; i ++)
        {
            float selection = Random.value;
            GameObject toSpawn = blueH;
            if(selection < verticalChance && i != 0)
            {
                if(red)
                {
                    toSpawn = redV;
                }
                else
                {
                    toSpawn = blueV;
                }
            }
            else
            {
                if(red)
                {
                    toSpawn = redH;
                }
                else
                {
                    toSpawn = blueH;
                }
            }

            GameObject temp = Instantiate(toSpawn, transform);
            temp.transform.position = i == 0 ? (Vector3)firstPosition : new Vector3(Random.Range(minX, maxX), firstPosition.y - 10 * i);

            red = !red;

            if(i == flagsToSpawn -1)
            {
                temp = Instantiate(finishLine, transform);
                temp.transform.position = new Vector3(0, firstPosition.y - 10 * i - 10);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
