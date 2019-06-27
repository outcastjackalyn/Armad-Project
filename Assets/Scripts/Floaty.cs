using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floaty : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private GameObject host;
    [SerializeField] private Transform topTransform;
    private float clampedX;
    private float clampedY;
    private float screenTop = 2.0f;
    private float screenEdges = 3.4f;
    private float displace = 0.16f;
    private float height = 0.1f;


    public void SetHost(GameObject obj)
    {
        this.host = obj;
        foreach (Transform t in host.GetComponentsInChildren(typeof(Transform)))
        {
            if (t.gameObject.name == "Top")
            {
                this.topTransform = t;
            }
        }
       // Debug.Log(this.host);
       // Debug.Log("host set");
    }


    public void SetSprite(int i)
    {
        this.GetComponent<SpriteRenderer>().sprite = sprites[i - 1];
        this.GetComponent<SpriteRenderer>().sortingOrder = 20;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(host);
        Vector3 pos = topTransform.position;
        //clampedX = pos.x > screenEdges ? screenEdges : pos.x < -1  * screenEdges ? -1 * screenEdges : pos.x;
        clampedY = pos.y + displace > screenTop - height ? screenTop - height : pos.y + displace;
        this.transform.position = new Vector3(pos.x, clampedY, pos.z);

    }
}
