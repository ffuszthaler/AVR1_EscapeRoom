using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dev.Ffuszthaler.DictionaryKVP;
using Newtonsoft.Json;
using Unity.XR.CoreUtils.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SocketManager : MonoBehaviour
{
    public UnityEvent completePuzzle;

    [SerializeField] private List<XRSocketInteractor> socketInteractors; // List of child sockets to validate

    [SerializeField] private string configurationFilePath = "Assets/Configs/socket_config.json";

    [SerializeField] private SerializableDictionary<string, string> socketToInteractableMapping =
        new SerializableDictionary<string, string>();

    [SerializeField] private bool lockWrongSockets = true;

    private void OnEnable()
    {
        LoadConfiguration();

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
        AkSoundEngine.PostEvent("Play_Interaction", gameObject);

        if (socket != null && socketInteractors.Contains(socket))
        {
            XRGrabInteractable interactable = args.interactableObject as XRGrabInteractable;

            if (interactable != null &&
                socketToInteractableMapping.ContainsKeyValuePair(socket.name, interactable.name))
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

                if (lockWrongSockets)
                {
                    // Force release the incorrect interactable
                    socket.interactionManager.SelectExit(socket, args.interactableObject);
                }
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


    private bool AreAllSocketsCorrect()
    {
        foreach (var socket in socketInteractors)
        {
            // Check the selected interactable in each socket
            if (socket.GetOldestInteractableSelected() is XRGrabInteractable interactable)
            {
                if (!socketToInteractableMapping.ContainsKeyValuePair(socket.name, interactable.name))
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
        // Debug.Log("Puzzle solved! Triggering completion events...");

        // PuzzleMaster.Instance.MarkPuzzleComplete(gameObject.tag);
        completePuzzle.Invoke();
    }

    private string LoadJsonFromResources(string path)
    {
        string filePath = path.Replace(".json", "");
        TextAsset file = Resources.Load<TextAsset>(filePath);
        return file.text;
    }
    
    private bool FileExistsInResources(string path)
    {
        string filePath = path.Replace(".json", "");
        TextAsset file = Resources.Load<TextAsset>(filePath);
    
        // Return true if resource is found, otherwise false
        return file != null;
    }
    
    private void LoadConfiguration()
    {
        if (FileExistsInResources(configurationFilePath))
        {
            string json = LoadJsonFromResources(configurationFilePath);
            socketToInteractableMapping.Clear();
            socketToInteractableMapping = JsonConvert.DeserializeObject<SerializableDictionary<string, string>>(json);
            Debug.Log("Configuration loaded successfully.");
            Debug.LogWarning("Path: " + configurationFilePath);

            foreach (var kvp in socketToInteractableMapping)
            {
                Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
            }
        }
        else
        {
            Debug.LogWarning("Configuration file not found. Using default settings.");
            Debug.LogWarning("Path: " + configurationFilePath);
        }
    }

    private void SaveConfiguration()
    {
        string directory = Path.GetDirectoryName(configurationFilePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string json = JsonUtility.ToJson(socketToInteractableMapping, true);
        File.WriteAllText(configurationFilePath, json);
        Debug.Log("Configuration saved successfully.");
    }
}