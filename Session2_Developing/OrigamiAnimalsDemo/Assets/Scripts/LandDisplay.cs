using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandDisplay : MonoBehaviour
{
  void Start()
  {
    MixedRealityCameraManager.Instance.OnDisplayDetected += OnDisplayDetected;
  }

  private void OnDisplayDetected(MixedRealityCameraManager.DisplayType displayType)
  {
    var land = GameObject.Find("land");
    land.SetActive(displayType == MixedRealityCameraManager.DisplayType.Opaque);
  }
}
