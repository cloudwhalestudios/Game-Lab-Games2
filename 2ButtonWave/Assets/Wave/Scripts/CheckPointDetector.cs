using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointDetector : MonoBehaviour
{
    public float CheckpointCycleTime;

    public List<GameObject> CurrentCheckpoints;
    public GameObject CurrentNewCheckpoint;
    public GameObject CurrentOldCheckpoint;
    public CheckPoint OldCheckPointScript;
    public Vector2[] CurrentCheckpointsPositions;
    public CheckPoint[] Check;

    public Vector2 CurrentSelectedCheckpointPosition;
    public float CurrentSelectedCheckpointPositionY;

    private bool CheckpointsAllCycled;
    private bool SelectionIsFrozen;

    public float CheckpointTriggerEnter;
    public GameObject[] testArray;

    private Player ThePlayer;

    void Start()
    {
        ThePlayer = FindObjectOfType<Player>();
        StartCoroutine(CycleCheckpoints());
    }

    void Update()
    {
        if (CheckpointsAllCycled == true && !SelectionIsFrozen)
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

            //add to list of current checkpoints
            CurrentNewCheckpoint = other.gameObject;
            CurrentCheckpoints.Add(CurrentNewCheckpoint);
            Check = new CheckPoint[CurrentCheckpoints.Count];
            for (int i = 0; i < CurrentCheckpoints.Count; i++)
            {
                Check[i] = CurrentCheckpoints[i].GetComponent<CheckPoint>();
            }
            

            //Here you have to log all checkpoints that enter player's collision box
            //Maybe list active collision checkpoints in trigger (not triggerEnter), then on trigger exit log them out.
        }

        if (other.gameObject.tag == "Player")
        {
            //Add something like PlayerHasArrived = true;
            return;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
            // Debug number for detecting amount of checkpoints within hitbox
            CheckpointTriggerEnter = CheckpointTriggerEnter - 1;
            print(CheckpointTriggerEnter);

            //Remove from list of current checkpoints
            CurrentOldCheckpoint = other.gameObject;
            OldCheckPointScript = CurrentOldCheckpoint.GetComponent<CheckPoint>();
            OldCheckPointScript.selected = false;
            CurrentCheckpoints.Remove(CurrentOldCheckpoint);
        }
    }

    IEnumerator CycleCheckpoints()
    {
        CheckpointsAllCycled = false;
        Check = new CheckPoint[CurrentCheckpoints.Count];
        CurrentCheckpointsPositions = new Vector2[CurrentCheckpoints.Count];
        for (int i = 0; i < CurrentCheckpoints.Count; i++)
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

    public void FreezeCurrentSelection()
    {
        if (!ThePlayer.hasArrivedAtCheckpoint)
        {
            SelectionIsFrozen = true;
            //Freeze the selection!!
            CheckpointsAllCycled = false;
            //Go to CurrentNewCheckpoint;  CurrentSelectedCheckpointPositionY;
            // Use this when player has arrived: CheckpointsAllCycled = true; 
        }
    }

    void DisableAllSelectedCheckpoints()
    {
        Check = new CheckPoint[CurrentCheckpoints.Count];
        for (int i = 0; i < CurrentCheckpoints.Count; i++)
        {
            Check[i] = CurrentCheckpoints[i].GetComponent<CheckPoint>();
            Check[i].selected = false;
        }
    }
}