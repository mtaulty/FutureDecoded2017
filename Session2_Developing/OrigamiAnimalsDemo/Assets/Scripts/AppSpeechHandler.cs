using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppSpeechHandler : MonoBehaviour
{
  public void OnStart()
  {
    // Find all of the cloned objects..
    var objs = GameObject.FindGameObjectsWithTag("FullResModel");
    StartCoroutine(PlayAudioSourcesQueued(objs));
  }
  IEnumerator PlayAudioSourcesQueued(IEnumerable<GameObject> objs)
  {
    foreach (var obj in objs)
    {
      if (!_isPlaying)
      {
        _isPlaying = true;
        yield break;
      }

      var src = obj.GetComponent<AudioSource>();
      src.Play();
      yield return new WaitForSeconds(src.clip.length);
    }
  }
  public void OnStop()
  {
    this._isPlaying = false;
  }
  bool _isPlaying;
}