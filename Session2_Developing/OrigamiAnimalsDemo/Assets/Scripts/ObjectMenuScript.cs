using HoloToolkit.Unity.Buttons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMenuScript : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
    foreach (var button in this.gameObject.GetComponentsInChildren<Button>())
    {
      button.OnButtonPressed += OnButtonPressed;
    }
  }

  void OnButtonPressed(GameObject obj)
  {
    // What's the name of the button being pressed?
    var name = obj.transform.GetChild(0).name;
    Debug.Log("OnButtonPressed - " + name);

    // Get the root of that name (fox, owl, etc)
    var shortName = name.Split('_')[0];

    // Find the prefab for the full scale object.
    var resource = Resources.Load("origami/" + shortName);

    // Instantiate it
    var copiedObject = (GameObject)GameObject.Instantiate(resource);

    // Add a collider
    copiedObject.AddComponent<BoxCollider>();

    // Add a white box around it
    copiedObject.AddComponent<BoxAdorner>();

    // Add tap to place.
    var tapToPlace = copiedObject.AddComponent<OneShotTapToPlace>();
    tapToPlace.AllowMeshVisualizationControl = true;

    // Remove these behaviours when we have placed the object.
    tapToPlace.Placed += (s, e) =>
    {
      Destroy(copiedObject.GetComponent<BoxAdorner>());
      Destroy(copiedObject.GetComponent<OneShotTapToPlace>());
      copiedObject.AddComponent<ScaleRotateAdorner>();
    };
  }
}
