
#region >>>> Usings

using System;
using UnityEngine;

#endregion

/// <summary>
/// This class displays various GUI elements and handles all user input.
/// </summary>
/// <remarks>
/// To simplify the implementation and keep things neat and organised we will expose
/// a number of delegates that other classes can attach handlers to.
/// This way whenever a user input action is recieved we will invoke a relevant delegate and 
/// if another class has attached a handler to that delegate, it will get called.
/// 
/// One of the benefits of using this approach is that we can have as many different handlers attached
/// to the same delegate as we want. So, essentially multiple scripts could react to the same event in the same frame.
/// </remarks>
public class script_rts_gui : MonoBehaviour
{
    private Camera _camera;

    #region >>>> Delegates

    public Action CallbackStartUpdates;
    public Action CallbackSuspendUpdates;

    #endregion

    #region >>>> UI Elements

    public Texture2D TextureControlsBackground;

    public GUIText TextGold;

    public int goldAmount;


    #endregion
    
    void Start()
    {
        // Initialize
        _camera = Camera.main;
    }
    
    public void addScore(int x)
    {
        goldAmount += x;
    }

    void OnGUI()
    {

        // Draw UI elements
        var contentColor = GUI.contentColor;
        var backgroundColor = GUI.backgroundColor;

        #region >>> Textures

        if (Event.current.type == EventType.Repaint)
        {
            // Top faded strip
            if (TextureControlsBackground != null)
            { GUI.DrawTexture(_camera.ViewportToScreenRect(new Rect(0F, 0.1F, 1F, -0.1F)), TextureControlsBackground); }

            // Bottom faded strip
            if (TextureControlsBackground != null)
            { GUI.DrawTexture(_camera.ViewportToScreenRect(new Rect(0F, 0.7F, 1F, 0.3F)), TextureControlsBackground); }
        }

        #endregion

        #region >>> Buttons

        //// Apply color to UI elements
        //GUI.contentColor = new Color(255, 255, 255, 0.7F);
        //GUI.backgroundColor = new Color(0, 255, 0, 0.7F);

        //// Start Button
        //if (GUI.Button(_camera.ViewportToScreenRect(new Rect(0.42F, 0.925F, 0.075F, 0.05F)), "Start"))
        //{ if (CallbackStartUpdates != null) { CallbackStartUpdates(); } }

        //GUI.backgroundColor = new Color(255, 0, 0, 0.7F);

        //// Stop Button
        //if (GUI.Button(_camera.ViewportToScreenRect(new Rect(0.505F, 0.925F, 0.075F, 0.05F)), "Stop"))
        //{ if (CallbackSuspendUpdates != null) { CallbackSuspendUpdates(); } }

        // Revert back to initial UI colors
        //GUI.contentColor = contentColor;
        //GUI.backgroundColor = backgroundColor;

        #endregion

        #region >>> Stats

        // Gold
        if (TextGold != null) { TextGold.text = goldAmount.ToString(); }

        //// Upkeep
        //if (TextUpkeep != null) { TextUpkeep.text = SharedData.UpkeepRate.ToString("0"); }

        //// Income
        //if (TextIncome != null) { TextIncome.text = SharedData.IncomeRate.ToString("0"); }

        //// Time
        //if (TextTime != null) { TextTime.text = SharedData.Time.ToString("HH:mm tt"); }

        #endregion
        
    }

}
