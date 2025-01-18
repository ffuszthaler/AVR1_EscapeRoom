using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleMaster : MonoBehaviour
{
    public static PuzzleMaster Instance;

    private void Awake()
    {
        Instance = this;
    }

    // UnityEvent for when all puzzles are completed
    public UnityEvent onAllPuzzlesComplete;

    // Array to store the tags of the puzzles (to differentiate them)
    private string[] puzzleTags = { "Puzzle1", "Puzzle2", "Puzzle3" };

    // Total number of puzzles
    [SerializeField] private int totalPuzzles = 3;
    
    // Track if each puzzle is completed
    [SerializeField] private bool[] puzzleCompletionStatus;

    private void Start()
    {
        // Initialize puzzleCompletionStatus array to false (all puzzles are not completed at the start)
        puzzleCompletionStatus = new bool[totalPuzzles];

        // Optionally initialize onAllPuzzlesComplete if it's not set
        if (onAllPuzzlesComplete == null)
        {
            onAllPuzzlesComplete = new UnityEvent();
        }
    }

    // Call this method when a puzzle is completed
    public void MarkPuzzleComplete(string puzzleTag)
    {
        // Find the puzzle index by its tag
        for (int i = 0; i < totalPuzzles; i++)
        {
            if (puzzleTags[i] == puzzleTag)
            {
                puzzleCompletionStatus[i] = true;
                break;
            }
        }

        // Check if all puzzles are complete
        CheckAllPuzzlesComplete();
    }

    // Check if all puzzles are complete
    private void CheckAllPuzzlesComplete()
    {
        foreach (bool isComplete in puzzleCompletionStatus)
        {
            if (!isComplete) return; // If any puzzle is incomplete, return
        }

        // If all puzzles are complete, trigger the event
        onAllPuzzlesComplete.Invoke();
    }

    public void AllPuzzlesComplete()
    {
        Debug.Log("### GAME FINISHED ###");
    }
}