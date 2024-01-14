using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using PlayFab;

public class CharacterSelection : MonoBehaviour {

    GameObject[] characters;
    int index;

    [SerializeField]
    private List<CharacterSkin> _skinList = new();


    public void toggleLeft() {
        characters[index].SetActive(false);
        if (index == 0) {
            index = transform.childCount - 1;
        } else {
            index--;
        }
        characters[index].SetActive(true);
    }

    public void toggleRight() 
    {
        characters[index].SetActive(false);
        if(index == transform.childCount-1){
            index = 0;
        }
        else{
            index++;
        }
        characters[index].SetActive(true);
    }

    public void selectCharacterAndStart(){
        PlayerPrefs.SetInt("SelectedCharacter", index);
        SceneManager.LoadScene("Main");
    }
    public int getIndex(){
        return index;
    }
    
    private void Start() 
    {
    }

    private void OnEnable()
    {
        UpdateCharacterList();
    }

    private void UpdateCharacterList()
    {
        // Clear transform children except default
        foreach (Transform child in transform)
        {
            if (child != transform.GetChild(0))
            {
                Destroy(child.gameObject);
            }
        }
        // Get inventory
        var userInv = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(userInv,
        r =>
        {
            List<ItemInstance> ii = r.Inventory;
            
            // Compare all inventory items with all available skins
            foreach (ItemInstance item in ii)
            {
                foreach (var skin in _skinList)
                {
                    // If player has the skin,
                    if (skin.itemID == item.ItemId)
                    {
                        // Instantiate in transform.parent
                        Instantiate(skin.skinObject, transform);
                    }
                }
            }

            index = PlayerPrefs.GetInt("SelectedCharacter");
            characters = new GameObject[transform.childCount];
            Debug.Log(transform.childCount);
            
            for (int i = 0; i < transform.childCount; i++) 
            {
                characters[i] = transform.GetChild(i).gameObject;
                characters[i].SetActive(false);
            }

            if ((index + 1) > transform.childCount) index = 0;

            if (characters[index]) 
            {
                characters[index].SetActive(true);

                if (SceneManager.GetActiveScene().name == "Main")
                {
                    characters[index].GetComponent<PlayerControl>().enabled = true;
                }
            }
        }, OnError);
    }

    private void OnError(PlayFabError e)
    {
        Debug.LogError(e.GenerateErrorReport());
    }
}

[System.Serializable]
public class CharacterSkin
{
    public string itemID;
    public GameObject skinObject;
}