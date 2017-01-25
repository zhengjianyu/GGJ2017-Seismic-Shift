
#region >>>> Usings

using UnityEngine;

using System.Collections;

#endregion

/// <summary>
/// 
/// </summary>
public class script_rts_house : MonoBehaviour
{

    /// <summary>
    /// 
    /// </summary>
    public bool LightsOn
    { get { return _lightsOn; } }
    private bool _lightsOn;
    
    /// <summary>
    /// 
    /// </summary>
    public GameObject[] Windows;

    void Awake() 
    {
        _lightsOn = true;
    }
        

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void ToggleLights(bool value)
    {
        foreach (var w in Windows)
        { w.SetActiveRecursively(value); }

        _lightsOn = value;
    }

}
