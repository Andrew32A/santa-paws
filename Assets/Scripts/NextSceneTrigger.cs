using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextSceneTrigger : MonoBehaviour
{
    [Header("Level Loader")]
    public LevelLoader levelLoader;

    void Start()
    {
        levelLoader.NextLevel();
    }
}
