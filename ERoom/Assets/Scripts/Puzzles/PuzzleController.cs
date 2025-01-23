using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PuzzleController : MonoBehaviour
{
    [SerializeField] int totalPuzzles = 3;
    
    [SerializeField] private bool[] puzzleCompletionStatus;
    
    [SerializeField] private string[] puzzleTags = { "Puzzle1", "Puzzle2", "Puzzle3" };
    
    private void Start()
    {
        // Initialize puzzleCompletionStatus array to false (all puzzles are not completed at the start)
        puzzleCompletionStatus = new bool[totalPuzzles];
    }

    public void CompletePuzzle(string puzzleTag)
    {
        // Find the puzzle index by its tag
        for (int i = 0; i < totalPuzzles; i++)
        {
            if (puzzleTags[i] == puzzleTag)
            {
                puzzleCompletionStatus[i] = true;
                
                // sound effect for individual puzzle completion
                AkSoundEngine.PostEvent("Play_Success", gameObject);
                
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
        // sound effect for all puzzles finished
        Debug.Log("### GAME FINISHED ###");
        
        // change scene to winmenu
        SceneManager.LoadScene("WinMenu");
    }
}
