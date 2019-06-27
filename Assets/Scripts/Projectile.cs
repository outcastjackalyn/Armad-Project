using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField] private float speed = .1f;
    private Rigidbody2D rigidbody2D;
    [SerializeField] private GameObject origin;
    [SerializeField] private GameObject impact;
    public void SetOrigin(GameObject gameObject) { origin = gameObject; }
    private float length;
    private LineRenderer lineRenderer;
    private Vector3 end;
    private bool ended = false;
    private float lengthScale = .2f;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.velocity = transform.right * speed;
    }



    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.gameObject != origin && collider2D.tag != "Bullet")
        {
            if (collider2D.tag != "Bounds")
            {
                Instantiate(impact, transform.position, Quaternion.Inverse(transform.rotation));
            }
            if (collider2D.tag == "Player")
            {
                //find associated player and destroy it too
                collider2D.gameObject.SetActive(false);
                //damage enemy here

            }
            //Destroy(gameObject);
            ended = true;
            end = transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        if(transform.position - transform.right * lengthScale == end)
        {
            Destroy(gameObject);
        }
        lineRenderer.SetPosition(0, ended ? end : transform.position);
        lineRenderer.SetPosition(1, transform.position - transform.right * lengthScale); //check start is less dist less than lengthscale

    }
}
