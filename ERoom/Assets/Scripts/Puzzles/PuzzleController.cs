using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleController : MonoBehaviour
{
    [SerializeField] int totalPuzzles = 3;
    
    [SerializeField] private bool[] puzzleCompletionStatus;
    
    [SerializeField] private string[] puzzleTags = { "Puzzle1", "Puzzle2", "Puzzle3" };
    
    // UnityEvent for when all puzzles are completed
    // public UnityEvent onAllPuzzlesComplete;

    public void CompletePuzzle(string puzzleTag)
    {
        // Find the puzzle index by its tag
        for (int i = 0; i < totalPuzzles; i++)
        {
            if (puzzleTags[i] == puzzleTag)
            {
                puzzleCompletionStatus[i] = true;
                Debug.Log(puzzleTag + " is complete");
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
        AllPuzzlesComplete();
    }

    public void AllPuzzlesComplete()
    {
        Debug.Log("### GAME FINISHED ###");
    }
}
