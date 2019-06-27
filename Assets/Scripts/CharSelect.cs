using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharSelect : MonoBehaviour
{
    [SerializeField] private List<GameObject> options;
    [Range(1, 4)] [SerializeField] private int id = 1;
    private GameObject display = null;
    private GameObject previous = null;
    private int displayInt = 0;
    private bool changeDisp = false;
    [SerializeField] public bool selected = false;
    private float delay = 0.0f;
    private GameObject clip;

    [SerializeField] private Axes axes;
    public void SetID(int i)
    {
        id = i;
        axes = new Axes(id);
    }

    public int GetChar()
    {
        //Debug.Log("Display int found: " + displayInt.ToString());
        return displayInt;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetID(id);
        clip = GameObject.Find("Clip" + id.ToString());
        clip.GetComponent<SpriteRenderer>().enabled = false;
        display = Instantiate(options[displayInt], transform.position, Quaternion.identity);
        display.transform.parent = this.gameObject.transform;

    }

    // Update is called once per frame
    void Update()
    {
        if(delay > 0)
        {
            delay -= Time.deltaTime;
        }
        Inputs();
        if (changeDisp)
        {
            previous = display;
            display = Instantiate(options[displayInt], transform.position, Quaternion.identity);
            display.transform.parent = this.gameObject.transform;
            Destroy(previous);
            changeDisp = false;
        }

    }




    void Inputs()
    {
        if (delay <= 0  && !selected)
        {
            if (Input.GetAxisRaw(axes.movement) > 0.1)
            {
                displayInt++;
                if (displayInt > 3)
                {
                    displayInt = 0;
                }
                changeDisp = true;
                delay = 0.5f;
            }
            if (Input.GetAxisRaw(axes.movement) < -0.1)
            {
                displayInt--;
                if (displayInt < 0)
                {
                    displayInt = 3;
                }
                changeDisp = true;
                delay = 0.5f;
            }
        }
        if (Input.GetButtonDown(axes.jump)) //maybe check if is unique
        {
            //select current display
            selected = true;
            clip.GetComponent<SpriteRenderer>().enabled = true;
        }

        if (Input.GetButtonDown(axes.shoot))
        {
            //deselect
            selected = false;
            clip.GetComponent<SpriteRenderer>().enabled = false;
        }

    }


    
}
