using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointDetector : MonoBehaviour
{
    public float CheckpointCycleTime;

    public GameObject[] CurrentCheckpoints;
    public Vector2[] CurrentCheckpointsPositions;
    public CheckPoint[] Check;

    public Vector2 CurrentSelectedCheckpointPosition;
    public float CurrentSelectedCheckpointPositionY;

    private bool CheckpointsAllCycled;

    public float CheckpointTriggerEnter;
    public GameObject[] testArray;

    void Start()
    {
        StartCoroutine(CycleCheckpoints());
    }

    void FindCheckpoints()
    {
        CurrentCheckpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        Check = new CheckPoint[CurrentCheckpoints.Length];
        for (int i = 0; i < CurrentCheckpoints.Length; i++)
        {
            Check[i] = CurrentCheckpoints[i].GetComponent<CheckPoint>();
        }
    }

    void Update()
    {
        FindCheckpoints();
        
        //Right Mouse Debugging
        if (Input.GetMouseButtonDown(1))
        {
            
        }
        if (CheckpointsAllCycled == true)
        {
            StartCoroutine(CycleCheckpoints());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
            CheckpointTriggerEnter = CheckpointTriggerEnter + 1;
            print(CheckpointTriggerEnter);
            //Here you have to log all checkpoints that enter player's collision box
            //Maybe list active collision checkpoints in trigger (not triggerEnter), then on trigger exit log them out.
        }

        if (other.gameObject.tag == "Player")
        {
            return;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
            //TheCheckPoint.selected = false;
        }
    }

    
    IEnumerator CycleCheckpoints()
    {
        CheckpointsAllCycled = false;
        CurrentCheckpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        Check = new CheckPoint[CurrentCheckpoints.Length];
        CurrentCheckpointsPositions = new Vector2[CurrentCheckpoints.Length];
        for (int i = 0; i < CurrentCheckpoints.Length; i++)
        {
            DisableAllSelectedCheckpoints();
            Check[i] = CurrentCheckpoints[i].GetComponent<CheckPoint>();
            Check[i].selected = true;
            CurrentCheckpointsPositions[i] = Check[i].transform.position;
            CurrentSelectedCheckpointPositionY = Check[i].transform.position.y;
            CurrentSelectedCheckpointPosition = CurrentCheckpointsPositions[i];
            yield return new WaitForSeconds(CheckpointCycleTime);
        }
        CheckpointsAllCycled = true;
        yield break;
    }

    void DisableAllSelectedCheckpoints()
    {
        CurrentCheckpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        Check = new CheckPoint[CurrentCheckpoints.Length];
        for (int i = 0; i < CurrentCheckpoints.Length; i++)
        {
            Check[i] = CurrentCheckpoints[i].GetComponent<CheckPoint>();
            Check[i].selected = false;
        }
    }
}