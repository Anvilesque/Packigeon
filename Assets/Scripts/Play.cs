using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{
    private Button playButton;
    // Start is called before the first frame update
    void Start()
    {
        playButton = GetComponent<Button>();
        playButton.onClick.AddListener(GoToLevel);
    }
    private void GoToLevel()
    {
        SceneManager.LoadScene("Level 1");
    }
}
