using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveChecker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("NotFirstTime");
        if (SaveLoader.isSaveFile(new DinamicData(), "gameState.nre"))
        {
            StaticDataProvider.loadGame();
            SceneManager.LoadScene(1);
        }
    }
}
