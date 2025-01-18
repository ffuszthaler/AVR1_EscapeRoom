using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPatternManager : MonoBehaviour
{
    [SerializeField] private GameObject[] buttons; // Assign your buttons in the Inspector
    [SerializeField] private float sequenceTimeout = 5.0f; // Time to complete the sequence

    private int currentSequenceIndex = 0;
    private float timer;

    private void Start()
    {
        // PuzzleMaster.Instance.onPuzzleComplete += OnSequenceComplete;
        
        foreach (GameObject button in buttons)
        {
            var interactable = button.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
            if (interactable != null)
            {
                interactable.selectEntered.AddListener(OnButtonPressed);
            }
        }

        ResetSequence();
    }

    private void Update()
    {
        // Reset sequence if the timeout is exceeded
        if (currentSequenceIndex > 0)
        {
            timer += Time.deltaTime;
            if (timer > sequenceTimeout)
            {
                Debug.Log("Sequence timed out.");
                ResetSequence();
            }
        }
    }

    private void OnButtonPressed(UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs args)
    {
        GameObject pressedButton = args.interactableObject.transform.gameObject;

        if (pressedButton == buttons[currentSequenceIndex])
        {
            Debug.Log($"Button {currentSequenceIndex + 1} pressed correctly.");
            currentSequenceIndex++;
            timer = 0.0f; // Reset timer for sequence

            if (currentSequenceIndex >= buttons.Length)
            {
                Debug.Log("Correct sequence completed!");
                OnSequenceComplete();
                ResetSequence();
            }
        }
        else
        {
            Debug.Log("Wrong button pressed. Resetting sequence.");
            ResetSequence();
        }
    }

    private void ResetSequence()
    {
        currentSequenceIndex = 0;
        timer = 0.0f;
    }

    public void OnSequenceComplete()
    {
        // Handle what happens when the sequence is completed successfully
        Debug.Log("You can add custom logic here, such as unlocking a door or triggering an event.");
        
        PuzzleMaster.Instance.MarkPuzzleComplete(gameObject.tag);
    }

    private void OnDestroy()
    {
        foreach (GameObject button in buttons)
        {
            var interactable = button.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
            if (interactable != null)
            {
                interactable.selectEntered.RemoveListener(OnButtonPressed);
            }
        }
    }
}