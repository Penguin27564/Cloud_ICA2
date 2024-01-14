using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

[System.Serializable]
public class Boundary{
    public float xMin, xMax, zMin, zMax;
}

public class PlayerControl : MonoBehaviour, IDamageable {

    private Rigidbody playerRb;
    private AudioSource playerWeapon;
    public float tiltMultiplier;
    public Boundary boundary;

    public GameObject shot;
    public Transform shotSpawn;
    public Transform shotSpawn2;

    private float nextFire;
    private CharacterSelection characterSelection;

    // Stats
    public float _fireRate;
    public float _speed;
    private int _maxHealth, _currentHealth = 1;

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;

        if (_currentHealth <= 0)
        {
            GameController.Instance.gameIsOver();
            Destroy(gameObject);
        }
    }

    private void Start() {
        GameObject cSelectionObject = GameObject.FindWithTag("CharacterSelection");
        if (cSelectionObject != null) {
            characterSelection = cSelectionObject.GetComponent<CharacterSelection>();
        }


        playerRb = GetComponent<Rigidbody>();
        playerWeapon = GetComponent<AudioSource>();
        _currentHealth = _maxHealth;
    }

    private void Update() {
        if(Input.GetButton("Jump") && Time.time > nextFire){
            nextFire = Time.time + (1 / _fireRate);
            if(characterSelection.getIndex() == 1){
                Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
                Instantiate(shot, shotSpawn2.position, shotSpawn2.rotation);
            }
            else{
                Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
            }
            playerWeapon.Play();
        }

    }

    private void FixedUpdate() {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        playerRb.velocity = new Vector3(moveHorizontal * _speed, 0.0f, moveVertical * _speed);

        playerRb.position = new Vector3(
            Mathf.Clamp(playerRb.position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(playerRb.position.z, boundary.zMin, boundary.zMax)
        );

        playerRb.rotation = Quaternion.Euler(0.0f, 0.0f, -playerRb.velocity.x * tiltMultiplier);
    }

    private void UpdatePlayerStats(int maxHealth, float speed, float fireRate)
    {
        _maxHealth = maxHealth;
        _speed = speed;
        _fireRate = fireRate;
    }

    private void Awake()
    {
        PFDataMgr pFDataMgr = PFDataMgr.Instance;
        GetUserData();

    }

    public void GetUserData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest() 
        {
            // Playfab ID = myPlayFabID,
            // Keys = null
        },
        result => 
        {
            Debug.Log("Got user data");
            // if (result.Data == null || !result.Data.ContainsKey("XP")) Debug.Log("No XP");
            // else
            // {
            //     Debug.Log("XP: " + result.Data["XP"].Value);
            //     _XPDisplay.text = "XP: " + result.Data["XP"].Value;
            // }

            if (result.Data == null) return;

            // If player has Health stat
            if (result.Data.ContainsKey("Health"))
            {
                // Update stored health stat
                _maxHealth = int.Parse(result.Data["Health"].Value);
                _currentHealth = _maxHealth;
            }
            else
            {
                // Set health to default value
                Debug.LogError("No health found");
                _maxHealth = 1;
                _currentHealth = _maxHealth;
            }

            if (result.Data.ContainsKey("Speed"))
            {
                _speed = float.Parse(result.Data["Speed"].Value);
            }
            else
            {
                Debug.LogError("No speed found");
                _speed = 1;
            }

            if (result.Data.ContainsKey("FireRate"))
            {
                _fireRate = float.Parse(result.Data["FireRate"].Value);
            }
            else
            {
                Debug.LogError("No fire rate found");
                _fireRate = 1;
            }
            
            Debug.Log("PLAYER STATS (HEALTH, SPEED, FIRERATE)" + _maxHealth + ", " + _speed + ", "+ _fireRate);
        },
        error =>  
        {
            Debug.Log("Got error retrieving user data: ");
            Debug.Log(error.GenerateErrorReport());
        }
        );
    }
}
