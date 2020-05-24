using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    [SerializeField]
    private StageManager stagePrefab = null;

    private void Awake()
    {
        StageManager newStage = Instantiate(stagePrefab);
    }

}
