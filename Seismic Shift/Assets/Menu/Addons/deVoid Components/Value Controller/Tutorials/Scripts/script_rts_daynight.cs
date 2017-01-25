
#region >>>> Usings

using UnityEngine;

using System.Collections;

#endregion


public class script_rts_daynight : MonoBehaviour
{
    
    public GameObject[] Suns;
    public GameObject[] Moons;
        
    public void ToggleDayNight(bool value)
    {

        // Here we simply activate one collection and deactivate the other.

        foreach (var s in Suns)
        { s.SetActiveRecursively(value); }

        foreach (var m in Moons)
        { m.SetActiveRecursively(!value); }        
    }

}
