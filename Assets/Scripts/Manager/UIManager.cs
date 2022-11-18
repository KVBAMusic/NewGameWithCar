using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text position;
    [SerializeField] private TMP_Text lap;

    private CarBrain player;
    // Start is called before the first frame update
    void Start()
    {   // will have to be changed once multiplayer is implemented
        var cars = FindObjectsOfType<CarBrain>();
        foreach (var c in cars)
        {
            if (!c.isAI)
            {
                player = c;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!(player is null))
        {
            position.text = player.Position.Position.ToString();
            lap.text = player.Position.lap.ToString();
        }
    }
}
