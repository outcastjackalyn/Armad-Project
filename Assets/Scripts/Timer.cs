using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{


    [SerializeField] private List<Sprite> sprites;
    public int value;
    private GameObject gameData;
    // Start is called before the first frame update
    void Start()
    {
        gameData = GameObject.Find("Game");
    }

    // Update is called once per frame
    void Update()
    {
        if(gameData.GetComponent<GameManagement>().countdown > 0.0f)
        {
            value = Mathf.FloorToInt(gameData.GetComponent<GameManagement>().countdown);
            this.GetComponent<SpriteRenderer>().enabled = true;
            this.GetComponent<SpriteRenderer>().sprite = sprites[value];
        } 
        else
        {
            this.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
