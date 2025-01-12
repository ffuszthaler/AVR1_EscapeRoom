using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SocketManager : MonoBehaviour
{
    [SerializeField] private List<XRSocketInteractor> socketInteractors; // List of child sockets to validate

    [SerializeField] private Dictionary<string, string> socketToInteractableMapping = new Dictionary<string, string>
    {
        // Define socket-to-interactable mappings (SocketName -> InteractableName)
        { "I2_Socket", "I2_Object" },
        { "J3_Socket", "J3_Object" },
        { "K4_Socket", "K4_Object" }
    };

    private void OnEnable()
    {
        // Subscribe to events for all socket interactors
        foreach (var socket in socketInteractors)
        {
            socket.selectEntered.AddListener(OnSelectEntered);
            socket.selectExited.AddListener(OnSelectExited);
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from events for all socket interactors
        foreach (var socket in socketInteractors)
        {
            socket.selectEntered.RemoveListener(OnSelectEntered);
            socket.selectExited.RemoveListener(OnSelectExited);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        XRSocketInteractor socket = args.interactorObject as XRSocketInteractor;

        if (socket != null && socketInteractors.Contains(socket))
        {
            XRGrabInteractable interactable = args.interactableObject as XRGrabInteractable;

            if (interactable != null && IsValidInteraction(socket.name, interactable.name))
            {
                Debug.Log($"Correct item '{interactable.name}' socketed in '{socket.name}'.");

                // Check if all interactables are correctly socketed
                if (AreAllSocketsCorrect())
                {
                    Debug.Log("All interactables are correctly socketed!");
                    OnAllSocketsCorrect();
                }
            }
            else
            {
                Debug.LogWarning($"Incorrect item '{interactable?.name}' attempted to be socketed in '{socket.name}'.");
                // Force release the incorrect interactable
                socket.interactionManager.SelectExit(socket, args.interactableObject);
            }
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        XRSocketInteractor socket = args.interactorObject as XRSocketInteractor;

        if (socket != null && socketInteractors.Contains(socket))
        {
            Debug.Log($"Item removed from '{socket.name}'.");
        }
    }

    private bool IsValidInteraction(string socketName, string interactableName)
    {
        // Check if the socket name exists in the mapping and matches the interactable name
        return socketToInteractableMapping.TryGetValue(socketName, out string validInteractable) &&
               validInteractable == interactableName;
    }

    private bool AreAllSocketsCorrect()
    {
        foreach (var socket in socketInteractors)
        {
            // Check the selected interactable in each socket
            if (socket.GetOldestInteractableSelected() is XRGrabInteractable interactable)
            {
                if (!IsValidInteraction(socket.name, interactable.name))
                {
                    return false;
                }
            }
            else
            {
                // If a socket is empty, not all sockets are correct
                return false;
            }
        }

        return true; // All sockets have the correct interactables
    }

    private void OnAllSocketsCorrect()
    {
        // Logic for when all sockets are correctly filled
        Debug.Log("Puzzle solved! Triggering completion events...");
    }
}