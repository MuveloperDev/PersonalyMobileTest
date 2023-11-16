using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoveScene : MonoBehaviour
{
    [SerializeField] private Button _gameStartButton;
    void Start()
    {
        _gameStartButton = GetComponent<Button>();
        _gameStartButton.onClick.AddListener(() => {
            SceneManager.LoadScene("MobileInGameTestScene");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
