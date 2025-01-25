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

    [SerializeField] private bool isTriggered = false;

    private void OnEnable()
    {
        LoadConfiguration();

        // subscribe to events for all socket interactors
        foreach (var socket in socketInteractors)
        {
            socket.selectEntered.AddListener(OnSelectEntered);
            socket.selectExited.AddListener(OnSelectExited);
        }
    }

    private void OnDisable()
    {
        // unsubscribe from events for all socket interactors
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

            if (interactable != null &&
                socketToInteractableMapping.ContainsKeyValuePair(socket.name, interactable.name))
            {
                Debug.Log($"Correct item '{interactable.name}' socketed in '{socket.name}'.");

                // correct item added to socket sound
                if (isTriggered == false)
                {
                    AkSoundEngine.PostEvent("Play_Interaction", gameObject);
                    isTriggered = true;
                }

                // check if all interactables are correctly socketed
                if (AreAllSocketsCorrect())
                {
                    Debug.Log("All interactables are correctly socketed!");
                    OnAllSocketsCorrect();
                }

                isTriggered = false;
            }
            else
            {
                Debug.LogWarning($"Incorrect item '{interactable?.name}' attempted to be socketed in '{socket.name}'.");

                // wrong item added to socket sound
                if (isTriggered == false)
                {
                    AkSoundEngine.PostEvent("Play_Interaction", gameObject);
                    isTriggered = true;
                }

                if (lockWrongSockets)
                {
                    // force release the incorrect interactable
                    socket.interactionManager.SelectExit(socket, args.interactableObject);
                }

                isTriggered = false;
            }
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        XRSocketInteractor socket = args.interactorObject as XRSocketInteractor;

        if (socket != null && socketInteractors.Contains(socket))
        {
            Debug.Log($"Item removed from '{socket.name}'.");
            // isTriggered = false;

            // item removed from socket sound
            if (isTriggered == false)
            {
                AkSoundEngine.PostEvent("Play_Interaction", gameObject);
                isTriggered = true;
            }

            isTriggered = false;
        }
    }


    private bool AreAllSocketsCorrect()
    {
        foreach (var socket in socketInteractors)
        {
            // check the selected interactable in each socket
            if (socket.GetOldestInteractableSelected() is XRGrabInteractable interactable)
            {
                if (!socketToInteractableMapping.ContainsKeyValuePair(socket.name, interactable.name))
                {
                    return false;
                }
            }
            else
            {
                // if a socket is empty, not all sockets are correct
                return false;
            }
        }

        return true; // all sockets have the correct interactables
    }

    private void OnAllSocketsCorrect()
    {
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

        // return true if resource is found, otherwise false
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