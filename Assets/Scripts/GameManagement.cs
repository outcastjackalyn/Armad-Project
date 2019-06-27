using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum GameState
{
    LOADING = 0, //initial program load
    LOBBY = 1, //character select screen
    MAPSTART = 2, //spawn players into world + countdown
    PLAY = 3, //game play
    MAPEND = 4, //winner calculated, loops back to mapstart on a different map
    GAMEEND = 5, //when x many games are played exit to score screen
    PAUSE = 6, // game play can be paused
    EXIT = 7, //program will close

}

public struct PlayerData
{
    public int controlID;
    public int characterID;
    public int score;
    //other data for multiple scenes for players


    public PlayerData(int i)
    {
        controlID = i;
        characterID = 0;
        score = 0;
    }

    public PlayerData(int i, int j, int k)
    {
        controlID = i;
        characterID = j;
        score = k;
    }

    /*
    public void IncrementScore()
    {
        this.score++;
    }

    public void SetCharID(int i)
    {
        this.characterID = i;
    }
    */
}



public class GameManagement : MonoBehaviour
{
    public List<PlayerData> currentPlayers = new List<PlayerData> ();
    public int alivePlayers = 0;
    [SerializeField] private List<Transform> spawnPoints;
    public List<GameObject> selectors;
    private int keyboardPlayerToggle = 1;
    public GameState gameState = GameState.LOADING;
    private bool changeLobby = false;
    private int controllers = 0;
    public float countdown = 0.0f;
    private float roundTimer = 0.0f;
    private float roundLength = 30.0f;
    private int mapID = 0;
    [SerializeField] private GameObject selector;
    [SerializeField] private GameObject spawner;




    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        //currentPlayers.Add(new PlayerData(1));
        checkPlayers();
        




       // gameState = GameState.LOBBY;
    }

    // Update is called once per frame
    void Update()
    {

        
        if (gameState == GameState.LOBBY)
        {
            lobbyHandling();

            if(readyCheck())
            {
                setCharacters();
                loadMap();
            }

        }

        if (gameState == GameState.LOADING)
        {
            if (Input.anyKey)
            {
                //change scene then change state
                SceneManager.LoadScene(1);
                gameState = GameState.LOBBY;
            }
        }
        if(countdown > 0.0f)
        {
            countdown -= Time.deltaTime;

        }
        else if (gameState == GameState.MAPSTART)
        {
            countdown = 0.0f;
            Debug.Log("Round start");
            gameState = GameState.PLAY;
        }

        if (gameState == GameState.PLAY)
        {
            roundTimer += Time.deltaTime;
            if(alivePlayers <= 1 //|| roundTimer > roundLength
                )
            {
                Debug.Log("Round end");
                gameState = GameState.MAPEND;
            }
        }

        if (gameState == GameState.MAPEND)
        {
            //game won
            if (false)
            {

            }
            else
            {
                //have 'outro' then change map after 
                loadMap();
            }
        }

    }

    void loadMap()
    {
        mapID = pickMap();
        SceneManager.LoadScene(mapID);
        gameState = GameState.MAPSTART;
        countdown = 2.9999f;
        StartCoroutine(spawnHandling());
    }


    void setCharacters()
    {
        for (int i = 0; i < currentPlayers.Count; i++)
        {
            int charID = selectors[currentPlayers[i].controlID - 1].GetComponent<CharSelect>().GetChar();
            PlayerData playerData = currentPlayers[i];
            currentPlayers[i] = new PlayerData(playerData.controlID, charID, playerData.score);
            //Debug.Log("character selected: " + currentPlayers[i].characterID.ToString());
        }

    }

    IEnumerator spawnHandling()
    {
        alivePlayers = currentPlayers.Count;
        yield return new WaitForSeconds(0.1f);
        spawnPoints.Clear();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("SpawnPoint"))
        {
            spawnPoints.Add(obj.transform);
        }
        int i = 0;
        foreach (PlayerData player in currentPlayers)
        {
            //instantiate spawners
            spawner.GetComponent<PlayerSpawner>().SetID(player.controlID);
            //set to zero while only one animated prefab exists
            spawner.GetComponent<PlayerSpawner>().SetChar(player.characterID); //player.characterID
            Instantiate(spawner, spawnPoints[i].position, Quaternion.identity);
            i++;
        }
        yield return true;
    }


    int pickMap()
    {
        //randomly select a new map
        return 2;
    }

    bool readyCheck()
    {
        if (selectors[0].GetComponent<CharSelect>().selected)
        {
            int total = selectors.Count;
            if (total >= 2)
            {
                int selected = 0;
                for (int i = 1; i <= total; i++)
                {
                    if (selectors[i - 1].GetComponent<CharSelect>().selected)
                    {
                        selected++;
                    }
                }
                if (selected == total)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void lobbyHandling()
    {
        if (Input.GetJoystickNames().Length != controllers)
        {
            Debug.Log("number of controllers changed!");
            checkPlayers();
        }

        if (Input.GetKeyDown("space")) // increase total max players beyond number of gamepads found, for keyboard players
        {
            keyboardPlayerToggle++;
            Debug.Log("number of keyboard players changed");
            if (keyboardPlayerToggle > 4)
            {
                keyboardPlayerToggle = 1;
            }
            checkPlayers();
        }
        if (changeLobby)
        {
            foreach (GameObject s in selectors)
            {
                Destroy(s);
            }
            selectors.Clear();
            for (int i = 1; i <= 4; i++) // load in the appropriate character selectors
            {
                GameObject obj = GameObject.Find("File" + i.ToString());
                if (i <= currentPlayers.Count)
                {
                    GameObject s = Instantiate(
                        selector, 
                        obj.transform.position, 
                        Quaternion.identity);
                    s.GetComponent<CharSelect>().SetID(i);
                    selectors.Add(s);
                    obj.GetComponent<SpriteRenderer>().enabled = true;
                }
                else
                {
                    obj.GetComponent<SpriteRenderer>().enabled = false;
                }
            }

            changeLobby = false;
        }

    }

    void checkPlayers()
    {
        currentPlayers.Clear();
        changeLobby = true;
        string[] names = Input.GetJoystickNames();
        controllers = names.Length;
        for (int i = 1; i <= 4; i++)
        {
          //  Debug.Log(names[0]);
            if (i <= controllers || i <= keyboardPlayerToggle)
            {
                currentPlayers.Add(new PlayerData(i));
            }
        }
      //  Debug.Log(currentPlayers.Count);
    }
}
