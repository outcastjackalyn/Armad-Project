using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Axes
{
    public string movement;
    public string jump;
    public string shoot;
    public string roll;

    public Axes(int i)
    {
        movement = "Move" + i.ToString();
        jump = "Jump" + i.ToString();
        shoot = "Shoot" + i.ToString();
        roll = "Roll" + i.ToString();
    }
}

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject character = null;
    public CharacterController2D controller = null;
    public Animator animator = null;
    public void SetCharacter (GameObject character) {
        this.character = character;
        this.controller = character.GetComponent<CharacterController2D>();
        //Debug.Log("set controller: " + controller);
       // Debug.Log(controller.transform.name);
        this.animator = character.GetComponent<Animator>();
        //Debug.Log("set anim: " + this.animator);
    }
    [Range(0, 4)] [SerializeField] private int id = 0;
    public void SetID (int i) {
        id = i;
        axes = new Axes(id);
    }
    [SerializeField] private float speed;
    [SerializeField] private bool jump = false;
    [SerializeField] private bool canShoot = false;
    [SerializeField] private bool reloading = false;
    [SerializeField] private bool roll = false;
    [SerializeField] private bool dash = false;
    private float maxSpeed = 15;
    [SerializeField] public Transform gunPoint;
    [SerializeField] private GameObject projectile;
    private Quaternion scatterAngle;
    public GameObject slick;
    private GameObject gameData;

    [SerializeField] private Axes axes;
     

    // Start is called before the first frame update
    void Start()
    {
      //  Debug.Log("controller found to be: " + controller);
        //set character with character select screen
        //character = GameObject.Find("Slick");
        //set axes too
     //   Debug.Log("id found to be : "+  id.ToString());
        if (id == 0)
        {
            SetID(1);
        }
        else
        {
            SetID(id);
        }
        /*
         * if (character == null)
        {
            SetCharacter(Instantiate(slick, transform.position, transform.rotation));
        }*/
        if(this.animator == null)
        {
            this.animator = this.character.GetComponent<Animator>();
            Debug.Log("backup// from: " + this.character);
            Debug.Log("backup// set anim: " + this.animator);
        }
        foreach (Transform t in this.character.GetComponentsInChildren(typeof(Transform)))
        {
            if (t.gameObject.name == "Front")
            {
                this.gunPoint = t;
            }
        }
        gameData = GameObject.Find("Game");
        //gunPoint = character.transform.GetChild(2);
    }

    // Update is called once per frame
    void Update()
    {
       /* Debug.Log("controller for: " + controller);
        Debug.Log("Character is: " + character);
        Debug.Log("anim for: " + this.animator);*/
        if(reloading || controller.isRolling)
        {
            canShoot = false;
        }
        else
        {
            canShoot = true;
        }
        if (character.GetComponent<Rigidbody2D>().velocity.magnitude < 0.2f)
        {
            if (roll && !dash)
            {
                roll = false;
            }
        }
        if (gameData.GetComponent<GameManagement>().gameState == GameState.PLAY)
        {
            Inputs();
        }
        if (Mathf.Abs(speed) > 0.01) //stop idling when moving
        {
            this.animator.SetBool("moving", true);
        }
        else
        {
            this.animator.SetBool("moving", false);
        }
        checkHit();
        
        this.animator.SetBool("rolling", controller.isRolling);
    }
    
    void checkHit()
    {
        if(!character.activeInHierarchy)
        {
            gameData.GetComponent<GameManagement>().alivePlayers--;
            Destroy(gameObject);
        }
    }

    void Inputs()
    {
        speed = Input.GetAxisRaw(axes.movement) * maxSpeed;
        if (Input.GetButtonDown(axes.jump))
        {
            jump = true;
            this.animator.SetBool("jumping", true);
            controller.land = false;
        }
        else if (controller.land)
        {
            this.animator.SetBool("jumping", false);
        }
        if (controller.grounded)
        {
            if (Input.GetButtonDown(axes.roll))
            {
                if(!roll)
                {
                    StartCoroutine(Dash());
                }
            }
        }
        if (Input.GetButtonDown(axes.shoot))
        {
            if (canShoot)
            {
                StartCoroutine(Shoot());
            }
        }
    }



    void FixedUpdate()
    {

        //Debug.Log("controller for: " + controller.transform.name);
        controller.Move(speed * Time.fixedDeltaTime, roll, jump, dash);
        if (jump)
        {
            jump = false;
        }
    }



    IEnumerator Dash()
    {
        if (reloading)
        {
            reloading = false;
        }
        dash = true;
        yield return new WaitForSeconds(0.1f);
        roll = true;
        dash = false;
        yield return true;
    }



    IEnumerator Shoot()
    {
        scatterAngle = Quaternion.Euler(0, 0, -Mathf.FloorToInt(Random.Range(2.1f, 5.9f)));
        reloading = true;
       /* Debug.Log("Shoot id: " + this.id);
        Debug.Log("Shoot origin: " + this.character);
        Debug.Log("Shoot point: " + this.gunPoint);*/

        projectile.GetComponent<Projectile>().SetOrigin(this.character);
        Quaternion rotation;
        rotation = this.gunPoint.rotation * scatterAngle;
        Instantiate(projectile, this.gunPoint.position, rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(projectile, this.gunPoint.position, this.gunPoint.rotation);
        yield return new WaitForSeconds(0.1f);
        scatterAngle = Quaternion.Euler(0, 0, -Mathf.FloorToInt(Random.Range(2.1f, 5.9f)));
        Quaternion inv = Quaternion.Inverse(scatterAngle);
        rotation = this.gunPoint.rotation * inv;
        Instantiate(projectile, this.gunPoint.position, rotation);

        yield return new WaitForSeconds(0.5f);
        //set can shoot back to true
        //reloading = false;
    }
}
