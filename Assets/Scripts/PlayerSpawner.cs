using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> characters;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject floaty;
    [Range(1, 4)] [SerializeField] private int id = 2;
    [Range(0, 3)] [SerializeField] private int charID = 0;

    public void SetChar(int i)
    {
        charID = i;
    }

    public void SetID(int i)
    {
        id = i;
    }
    // Start is called before the first frame update
    void Start()
    {
        GameObject character = characters[charID]; //set from gamedata/charselect
        GameObject current = Instantiate(character, transform.position, Quaternion.identity);
        player.GetComponent<Player>().SetID(id);
       // Debug.Log("set id: " + id.ToString());
        player.GetComponent<Player>().SetCharacter(current);
        //Debug.Log("Spawning: " + current.name);
        // Debug.Log(player.GetComponent<Player>().controller);
        // Debug.Log(player.GetComponent<Player>().animator);
        floaty.GetComponent<Floaty>().SetHost(current);
        floaty.GetComponent<Floaty>().SetSprite(id);
        Instantiate(player, transform.position, Quaternion.identity);
        Instantiate(floaty, transform.position, Quaternion.identity);
        //current.transform.parent = player.transform;
        Destroy(gameObject);

    }

    // Update is called once per frame
    void Update()
    {
    }
}
