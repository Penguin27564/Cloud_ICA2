using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_ChangeScene : MonoBehaviour
{
    [SerializeField]
    private string _sceneToChange;

    public void ChangeScene()
    {
        SceneManager.LoadScene(_sceneToChange);
    }
}
