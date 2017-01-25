
/// --- deVoid Studios - Copyright © 2011 - All rights reserved ---
/// 
/// Author  :   Alexander "Feature" Pavlovsky
/// Date    :   02 / 10 / 2011
/// Version :   1.00
/// 
/// Contact :   support@devoidstudios.com
/// 
/// ---

#region >>>> Usings

using UnityEngine;

using System.Collections;

#endregion

public class script_rts_startup : MonoBehaviour
{
    
    #region >>>> Day / Night Cycle

    /// <summary>
    /// Reference to the controller that will keep track of all Day / Night cycle dynamic values.
    /// </summary>
    private ValueController _controllerDayNight;

    /// <summary>
    /// Key to the dynamic value that stores Suns rotation angle.
    /// </summary>
    private int _keyDayNight_SunRotation;

    /// <summary>
    /// Reference to the dynamic value that stores Suns rotation angle.
    /// </summary>
    private ValueControllerDynamicValue _valueDayNight_SunRotation;

    /// <summary>
    /// Key to the dynamic value that stores time information.
    /// </summary>
    private int _keyDayNight_TimeOfDay;

    /// <summary>
    /// Reference to the dynamic value that stores time information.
    /// </summary>
    private ValueControllerDynamicValue _valueDayNight_TimeOfDay;

    /// <summary>
    /// Reference to the Sun / Moon prefab that we will rotate as time passes.
    /// </summary>
    private script_rts_daynight _dayNight;

    /// <summary>
    /// 
    /// </summary>
    private Transform _transformDayNight;

    /// <summary>
    /// This flag is used in checks that trigger day / night changes.
    /// </summary>
    private bool _isDay;

    #endregion

    #region >>>> Upkeep & Gold Reserves
    
    /// <summary>
    /// Reference to the controller that will keep track Upkeep related timers.
    /// </summary>
    private ValueController _controllerUpkeepTimers;

    /// <summary>
    /// Reference to the controller that will keep track of our Gold reserves.
    /// </summary>
    private ValueController _controllerGoldReserves;

    /// <summary>
    /// Key to the dynamic value that stores Upkeep timer.
    /// </summary>
    private int _keyUpkeep_Timer;

    /// <summary>
    /// Reference to the dynamic value that stores Upkeep timer.
    /// </summary>
    private ValueControllerDynamicValue _valueUpkeep_Timer;

    private float _dayTimeUpkeep = 342;
    private float _nightTimeUpkeep = 171;

    private float _dayTimeIncome = 468;
    private float _nightTimeIncome = 0;

    #endregion

    #region >>>> UI

    /// <summary>
    /// Reference to the controller that will display the GUI and recieve all user input from it.     
    /// </summary>
    private script_rts_gui _GUI;

    #endregion

    #region >>>> Village

    /// <summary>
    /// This property will keep references to all the houses in our little village so
    /// that we could call any of their methods without much hassle. 
    /// </summary>
    private script_rts_house[] _houses;

    #endregion
    
    void Awake()
    {

        // Inititialize various helpers and stuff

        // Day / Night
        _dayNight = (script_rts_daynight)GameObject.FindObjectOfType(typeof(script_rts_daynight));
        if (_dayNight) { _transformDayNight = _dayNight.transform; }


        // Village
        _houses = (script_rts_house[])GameObject.FindObjectsOfType(typeof(script_rts_house));


        // UI

        // To keep things somewhat organised we moved all UI related stuff out into a separate script so now we need to add it.
        _GUI = gameObject.GetComponent<script_rts_gui>();

        // Next we need to add references to the delegates we defined in the GUI class. These let our class react to input from the UI.        
        _GUI.CallbackStartUpdates += OnStartUpdates;
        _GUI.CallbackSuspendUpdates += OnSuspendUpdates;


        // Other members
        _isDay = true;
    }

    void Start ()
    {

        #region >>> Step 1 : Initialize Components
        
        // First we need to instantiate our Value Controllers. Since Value Controller is based on MonoBehaviour 
        // it needs to be added as a component of a GameObject.        
        // For the purposes of this tutorial we need a few controllers just to keep things well organised.

        // This controller will keep track of various Day / Night cycle values.
        _controllerDayNight = gameObject.AddComponent<ValueController>();

        // This controller will deal with the upkeep of our funky little town.
        _controllerUpkeepTimers = gameObject.AddComponent<ValueController>();

        // This controller will make sure the our town's council has enough money to...do whatever is it that they do with mnoney :)
        _controllerGoldReserves = gameObject.AddComponent<ValueController>();
        

        // Now we need to set initial Income and Upkeep values.
        SharedData.IncomeRate = _dayTimeIncome;
        SharedData.UpkeepRate = _dayTimeUpkeep;

        #endregion

        #region >>> Step 2 : Add values to controllers

        // At this point we are ready to add our values to the controllers
        // we instantiated in the previous step.
        //
        // Notice that the Add...() method is returning a value. This value is the
        // unique identifier (aka key) of each value stored by the controller.
        //
        // You don't have to store these individual keys unless you are planing to access
        // these newly added values later on.



        // Sun Rotation :
        // We are going to add a value that will start from 0 and gradually increase to 360 over 60 seconds.
        // This value will then reset and start over from 0.
        _keyDayNight_SunRotation = _controllerDayNight.AddDynamicValue(0, 360, 60);



        // Time of Day :
        // This value will keep our time of day. There are many ways we could have used to solve this
        // seemingly simple problem but why bother :)
        // It works pretty much the same way as the one that we defined for the sun's rotation
        // the only difference is the scale.
        
        _keyDayNight_TimeOfDay = _controllerDayNight.AddDynamicValue(0F, 1440F, 1F, 0.0416F);

        // The values we use here represent minutes. Why minutes? Well when we get to display the time we'll simply convert 
        // this value into a formated string using as custom extension method we've written just for this purpose. 
        // Have a look at UnityExtensions.ToDateTimeFrom....() for more info.
        //
        // This greatly simplifies the whole process of time keeping because we can have the value run at any speed we want
        // completely independent of actual game time. We can freeze or fastforward or slow time flow without having to mess
        // with Time.timeScale. 
        // Finally, and this is the best part, we can format the output string using in-built framework methods.
        // No need to re-invent the biped!
        // 
        // So, as per the line of code above :
        //    ValueFrom - 0 : Starting value of our timer
        //    ValueTo - 1440 : Number of minutes in a day
        //    IntervalValue - 1 : Number of minutes to count per every interval tick
        //    IntervalDuration - 0.0416 : Number of seconds between each tick (in this case in one second our value will advance ~24 minutes)
        //
        // Note: We chose to use minutes for our timer in this tutorial. However, you are free to use whichever
        // time component that suits your needs best. All you have to do is figure out the right values for the counter
        // and the Value Controller will take care of the rest.
                


        // Income / Upkeep :
        // This value will act as a timer for the Income / Upkeep cycle. On every tick of this timer (ie. every 3 seconds) we will update 
        // our gold reserves with the difference between our income and upkeep.
        _keyUpkeep_Timer = _controllerUpkeepTimers.AddDynamicValue(3F);

        #endregion 
        
        #region >>> Step 3 : Get references to added values

        // You dont have to do this in majority of cases since the Value Controller knows what
        // needs doing as far as the values are concerned.
        // 
        // However there are situations when having a reference to the value itself gives you 
        // finer control. In that case you can use this approach to get the references you need.

        _valueDayNight_SunRotation = _controllerDayNight.GetDynamicValue(_keyDayNight_SunRotation);
        _valueDayNight_TimeOfDay = _controllerDayNight.GetDynamicValue(_keyDayNight_TimeOfDay);
        _valueUpkeep_Timer = _controllerUpkeepTimers.GetDynamicValue(_keyUpkeep_Timer);

        #endregion 

        #region >>> Step 4a : Add handlers to controllers' events
        
        // Now that we have keys and actual references to our values we can add some event handlers.
        //        
        // This is one of the ways you can subscribe to (or handle) events raised by your values.
        // Here we will subscribe to controller wide events and then simply check 
        // which is the value that's raising that particular event.
        //
        // This approach is more suited to scenarios where you have lots of values runing
        // and you are not so concerned as to what their values actually are but you do need 
        // to know when they lapse etc.
        
        // In our case we don't need to hook up any such handlers. 
        // However, if you wanted to you could do it like so :
        //
        // _someController.CallbackValueLapsed += SomeHandlerMethod;
        //        
        // _someController.CallbackValueStackLapsed += SomeHandlerMethod;        
        //
        // _someController.CallbackIntervalLapsed += SomeHandlerMethod;

        #endregion

        #region >>> Step 4b : Add handlers to values' events
         
        // Here we will subscribe to the events we care about directly from the value.
        //
        // This is just another way of handling events. Either approach is fine, but in this particular case
        // you need to remember to unhook the handlers if the values whose events you are subscribing to
        // get disposed.

        // In case of the Time of Day value we care about the 'ticks'. Since we used a value that has intervals
        // we can simply subscribe a handler to that event which will essentially result in that method getting called
        // each time an interval lapses.
        _valueDayNight_TimeOfDay.CallbackIntervalLapsed += OnTimeOfDayTick;

        // This handler will let us know when it's time to update the gold reserves
        _valueUpkeep_Timer.CallbackIntervalLapsed += OnUpkeepTick;

        #endregion

        #region >>> Step 5 : Make various adjustments based on requirements

        // Here we will fiddle with the values a bit so that they better suit our needs.

        // Time of Day needs to be fast-forwarded to 7 a.m to make our Sun rise / set at the 'correct' hour.
        // At the same time this will make Sun's rotation correspond to the time of day.
        //
        // To acomplish this we will use a method that lets us change Current Value of dynamic values to whatever
        // we want without 'braking' the internal logic of the value itself.
        // The best part is that it will not trigger any events either so we don't have to worry about anything
        // braking in our initialization code or whatnot.
        //
        // 7 a.m in minutes is 420 so...
        _valueDayNight_TimeOfDay.ChangeCurrentValue(420F);

        #endregion

        #region >>> Step 6 : ...and now what?

        // We are all set and ready to go!
        //
        // If you haven't done so yet you could look over all the scripts that are part of this demo to get
        // a better understanding of how everything fits together.
        // 
        // Doesn't sound like much fun I know. Just go click the Play button!!1


        // PS: There are many other types of dynamic value you can construct. Have a look at the ValueController
        // class for detailed description of each supported value type.
        //
        // Don't forget to check out the other tutorials to see more intriguing perversions!
        
        #endregion

    }
    
    /// <summary>
    /// 
    /// </summary>
    private void OnSunset()
    {
        // Here we switch on the lights in all houses
        foreach (var h in _houses)
        { h.ToggleLights(true); }

        // ... and lightup that funky looking moon!
        _dayNight.ToggleDayNight(false);

        // Now we update our income / upkeep values just to make things interesting
        SharedData.IncomeRate = _nightTimeIncome;
        SharedData.UpkeepRate = _nightTimeUpkeep;
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnSunrise()
    {
        // Here we switch off the lights in all houses
        foreach (var h in _houses)
        { h.ToggleLights(false); }

        // ... and turn on the big lightbulb!
        _dayNight.ToggleDayNight(true);

        // More peeps awake...more mouths to feed
        SharedData.IncomeRate = _dayTimeIncome;
        SharedData.UpkeepRate = _dayTimeUpkeep;
    }

    /// <summary>
    /// This is a handler for a GUI delegate that tells our value controllers to start updating.
    /// </summary>
    private void OnStartUpdates()
    {
        // We start updates with a parameter that tells each controller to ignore the
        // the time that they have been idle.

        // Note: You do NOT need to do this if you are pausing your game by setting Time.timeScale to 0. However, in this case 
        // we are not using the time scale so it is necessary to do this.
        //         
        // One of the StartUpdates methods accepts a parameter. This parameter tells the value whether it should ignore the
        // time difference since it was last suspended. 
        // If you don't set it to True values maintained by the controllers would 'fast forward' once the updates are started again.
        // Some times this FF effect is what you want but most of the times it's not :)
        _controllerDayNight.StartUpdates(true);
        _controllerUpkeepTimers.StartUpdates(true);
        _controllerGoldReserves.StartUpdates(true);
    }

    /// <summary>
    /// This is a handler for a GUI delegate that tells our value controllers to stop updating.
    /// </summary>
    private void OnSuspendUpdates()
    {
        // Here we stop all dynamic values from updating by suspending the controllers.
        _controllerDayNight.SuspendUpdates();
        _controllerUpkeepTimers.SuspendUpdates();
        _controllerGoldReserves.SuspendUpdates();
    }

    /// <summary>
    /// This handler method will be called each time an interval of the Time Of Day dynamic value
    /// lapses.
    /// </summary>
    private void OnTimeOfDayTick(ValueControllerDynamicValue value)
    { 
        // Here we will update our Shared Data so that any code that relies on it
        // will get the latest time value.
                
        SharedData.Time = value.CurrentValue.ToDateTimeFromMinutes();
    }

    /// <summary>
    /// This handler method will be called each time an interval of the Upkeep Timer dynamic value
    /// lapses.
    /// </summary>
    private void OnUpkeepTick(ValueControllerDynamicValue value)
    {
        
        // To make things interesting and show off some other cool features of the ValueController
        // instead of simply updating the shared data with the difference of upkeep and income we
        // will do something a bit more elaborate.

        // Each time the value ticks we will add a new dynamic value to the Gold Reserves controller.
        // The value will be timed so it will have that hugely satisfying running counter effect.

        _controllerGoldReserves.AddDynamicValue(
            SharedData.IncomeRate - SharedData.UpkeepRate,
            1F,
            eValueControllerDynamicValueUpdateMode.FromZeroToValue,
            eValueControllerDynamicValueDisposalMode.AutomaticAddToConstants);

        // So, the parameters we specified are :
        //    Value : the value we want to add, i.e. the difference between our Income and Upkeep
        //    Duration : the length of time it will take for the value to fully update
        //    Update Mode : the direction the value will get updated. In our case we want the value to start out at 0
        //    Disposal Mode : what we want the controller to do with the value when its done updating. In our case
        //                    we want it to get added as a constant value on the controller.
        //                    This is more of a convenience feature, each time a value lapses (ie. is fully updated) the
        //                    the controller will add it as a constant value making it affect the grand total of all values
        //                    on that controller. 
        //                    You dont have to do this if you don't want to, or you could subscribe to CallbackValueLapsed and handle 
        //                    the disposal your own way.


        // And that's all we need to do here. Shared data is updated with the total value of our gold reserves 
        // in the main Update() method up above.
        
    }


    void Update()
    {

        // Here we will update our shared data with the current value of our gold reserves.
        // Since there could be many values in this controller we will call a Sum function that will 
        // simply give us their grand total.

        // It's a very convenient way of dealing with many similar problems in your own projects.
        // You may have a controller that could have hundreds or even thousadnds of both positive and negative values
        // being updated at arbitrary intervals in either direction.
        //
        // Calling one of the Get...Total() functions will provide you with a snapshot of the total
        // sum of all those values. Making your life cake**.
        SharedData.TotalGold = _controllerGoldReserves.GetAllValuesTotal();


        // As you may remember we told the controller to add fully updated values as constants (see OnUpkeepTick() method). 
        // As you can imagine after a few minutes of game time we will have a few hundred of these constant values laying around 
        // in our controller.
        // Furthermore, each time we ask the controller to give us the current values' total (like we did above) it would need to
        // go round and round needlessly sifting through all those values.

        // For that reason we added the following method
        _controllerGoldReserves.CombineConstantValues();
        
        // The name says it all. Since we don't have any particular use for these individual constant values we can simply combine 
        // them all together every now and then.

        // This way we will always have only a single constant value that will represent the combined total of all those dynamic 
        // upkeep / income difference values.
        // Doesn't change anything as far as our game logic goes but it's a big deal as far as performance is concerned.


        // ** Cake is a lie!        
    }

    /// <summary>
    /// In this method you can see how values stored in the dynamic value itself can be used to
    /// implement various 'interesting' features for your game.
    /// </summary>
    void LateUpdate()
    {

        #region >>> Rotate the Sun

        // This is a rather simplistic implementation of a Day / Night Cycle.
        // The rotation of the Sun/Moon prefab is achieved by simply querying the current
        // delta value of the dynamic value.

        // There is a lot of useful information you can get by referencing a dynamic value directly.
        // You would rarely do this under normal circumstances though. Most of the time you will use the
        // Value Controller to get summary totals of managed values but in those rare cases when you want 
        // to do something like this you have the option of keeping a direct reference to the dynamic
        // value itself.

        // We need to check if the controller is updating since we have the UI option to suspend it.
        if (_transformDayNight && _controllerDayNight.IsUpdating)
        { _transformDayNight.Rotate(_transformDayNight.forward, _valueDayNight_SunRotation.DeltaValue); }

        #endregion

        #region >>> Check for Sunset / Sunrise

        // Night
        if (_isDay && _valueDayNight_SunRotation.CurrentValue >= 180F)
        {
            // Trigger night change            
            OnSunset();

            // Update flag
            _isDay = false;
        }
        // Day
        else if (!_isDay && _valueDayNight_SunRotation.CurrentValue <= 180F)
        {
            // Trigger day change
            OnSunrise();

            // Update flag
            _isDay = true;
        }

        #endregion

    }

}
