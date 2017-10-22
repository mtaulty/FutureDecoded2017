using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorStartup : MonoBehaviour
{
  void Update()
  {
    if (!this.switchOnAnchorStore && MixedRealityCameraManager.IsInitialized)
    {
      this.switchOnAnchorStore = true;

      if (MixedRealityCameraManager.Instance.CurrentDisplayType == MixedRealityCameraManager.DisplayType.Transparent)
      {
        this.GetComponent<WorldAnchorManager>().enabled = true;
      }
    }
    if (WorldAnchorManager.IsInitialized)
    {
      var anchorStore = WorldAnchorManager.Instance.AnchorStore;

      if (!this.triedInitialise && (anchorStore != null))
      {
        this.triedInitialise = true;

        // Get all the anchor Ids that have been persisted and 
        // reloaded.
        var anchorIds = anchorStore.GetAllIds();

        // Anchor Ids will have been defaulted to the name of the object
        // that was attached to them - names like "rabbit(clone)".
        foreach (var anchoredObjectName in anchorIds)
        {
          // Try and get to the real name.
          var shortName = anchoredObjectName.Split('(')[0];

          // Try and load the prefab.
          var prefabObject = Resources.Load("origami/" + shortName);

          // Try and instantiate a copy of that object.
          var newObject = (GameObject)GameObject.Instantiate(prefabObject);
          newObject.name = anchoredObjectName;

          // And try to re-attach the anchor.
          var worldAnchor = anchorStore.Load(
            anchoredObjectName, newObject);
        }
      }
    }
  }
  bool triedInitialise = false;
  bool switchOnAnchorStore = false;
}
