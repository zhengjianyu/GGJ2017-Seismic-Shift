
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

using System;
using System.Collections.Generic;

using UnityEngine;

#endregion

#region >>>> Value Controller

/// <summary>
/// Value Controller can be used to manage any number of constant and dynamic values over 
/// any given period of time.
/// It's best suited for implementation of timers, healthbars, spell castbars, 
/// damage / healing over-time mechanisms, stackable buffs / debuffs and so on.
/// 
/// If you need some values to change over time and notify other parts of your code
/// when they reach a given state, this is the class for you!
/// </summary>
public class ValueController : MonoBehaviour
{

    #region >>>> Members

    /// <summary>
    /// Delegate used to update dynamic values.
    /// </summary>        
    private DelegateValueControllerUpdate UpdateDynamicValues;

    /// <summary>
    /// Mode that determines how dynamic values are updated.
    /// </summary>
    private eValueControllerUpdateMode _updateMode = eValueControllerUpdateMode.OnUpdate;

    /// <summary>
    /// Collection of dynamic values.
    /// </summary>
    private Dictionary<int, ValueControllerDynamicValue> _dynamicValues;

    /// <summary>
    /// Collection of constant values.
    /// </summary>
    private Dictionary<int, float> _constantValues;

    /// <summary>
    /// Last key used to add a dynamic type.
    /// </summary>
    private int _lastDynamicValueKey;

    /// <summary>
    /// Last key used to add a constant type.
    /// </summary>
    private int _lastConstantValueKey;

    /// <summary>
    /// Flag used to check if the internal Update method should be called.
    /// See <see cref="StartUpdates()"/> and <see cref="StopUpdates()"/> for details.
    /// </summary>
    private bool _isSuspended;
    
    /// <summary>
    /// Returns False if the controller is currently suspended.
    /// </summary>
    public bool IsUpdating
    { get { return !_isSuspended; } }
    
    /// <summary>
    /// Flag used to check if the Update method should calculate the time delta immediately following
    /// the <see cref="StartUpdates()"/> call.    
    /// </summary>
    private bool _ignoreIdleTime;

    #endregion

    #region >>>> Constructors

    void Awake()
    {
        // Initialize
        _dynamicValues = new Dictionary<int, ValueControllerDynamicValue>();
        _constantValues = new Dictionary<int, float>();

        _lastDynamicValueKey = 0;
        _lastConstantValueKey = 0;
                        
        _isSuspended = true;
        _ignoreIdleTime = false;
    }

    void Start()
    { }

    #endregion

    #region >>>> Start / Suspend Updates

    /// <summary>
    /// Call this method to start updating all dynamic values maintained by the controller.
    /// </summary>        
    public void StartUpdates()
    { _isSuspended = false; }

    /// <summary>
    /// Call this method to start updating all dynamic values maintained by the controller.
    /// </summary>        
    /// <param name="ignoreIdleTime">
    /// Set to True if you need the controller to ignore the time difference since the last call 
    /// to <see cref="SuspendUpdates"/>.
    /// </param>
    /// <remarks>
    /// Since the Value Controller is using elapsed time to update all of its dynamic values we 
    /// need to have a way to resume the updates without 'fast forwarding' the values.
    /// </remarks>
    public void StartUpdates(bool ignoreIdleTime)
    {
        StartUpdates();

        _ignoreIdleTime = ignoreIdleTime;        
    }

    /// <summary>
    /// Call this method to stop updating all dynamic values maintained by the controller.
    /// This will effectively 'pause' the controller and keep all dynamic values at their present state.
    /// 
    /// To continue updating values call StartUpdates() method.
    /// </summary>        
    /// <remarks>
    /// You may want to call this method when the game is being paused.
    /// 
    /// However, keep in mind that if you are using Time.timeScale = 0 approach to do that 
    /// there is no need to suspend Value Controllers.
    /// Since all update calls are time dependent, the controller will 'auto suspend'
    /// if you set the time scale type to 0.
    /// </remarks>
    public void SuspendUpdates()
    { _isSuspended = true; }

    #endregion

    #region >>>> Constant Values

    /// <summary>
    /// Adds a constant value.
    /// </summary>
    /// <param name="type"></param>
    /// <returns>Key of the added value.</returns>
    public int AddConstantValue(float value)
    {
        // Update key
        _lastConstantValueKey++;

        // Add type to collection
        _constantValues.Add(_lastConstantValueKey, value);

        return _lastConstantValueKey;
    }

    /// <summary>
    /// Returns a constant value with the specified key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public float GetConstantValue(int key)
    {
        if (!_constantValues.ContainsKey(key))
        {
            Debug.LogError(String.Format("ValueController.GetConstantValue() : Key {0} not found in the collection.", key));
            return -1;
        }

        return _constantValues[key];
    }

    /// <summary>
    /// Changes a constant value with the specified key.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="type"></param>        
    public void ChangeConstantValue(int key, float value)
    {
        if (!_constantValues.ContainsKey(key))
        {
            Debug.LogError(String.Format("ValueController.ChangeConstantValue() : Key {0} not found in the collection.", key));
            return;
        }

        _constantValues[key] = value;
    }

    /// <summary>
    /// Checks if the controller has a constant value with the specified key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsConstantValue(int key)
    { return _constantValues.ContainsKey(key); }

    /// <summary>
    /// Removes a constant value with the specified key.
    /// </summary>        
    public void RemoveConstantValue(int key)
    {
        if (!_constantValues.ContainsKey(key))
        {
            Debug.LogError(String.Format("ValueController.GetConstantValue() : Key {0} not found in the collection.", key));
            return;
        }

        _constantValues.Remove(key);
    }

    /// <summary>
    /// Removes all constant values currently managed by this controller.
    /// </summary>
    public void RemoveAllConstantValues()
    {
        // Check
        if (_constantValues.Count == 0)
        { return; }

        var keys = _constantValues.ToListKeys();

        foreach (var key in keys)
        { RemoveConstantValue(key); }
    }

    /// <summary>
    /// Combines all constant values into a single constant value. 
    /// If the combined total is not equal to 0 a new constant value is added and its key is returned.
    /// All old constant values are removed.
    /// </summary>
    /// <returns>Key of the newly added value or -1 if no value was added.</returns>
    public int CombineConstantValues()
    { 
        var newKey = -1;

        // Check
        if (_constantValues.Count <= 1)
        { return newKey; }

        var values = _constantValues.ToListValues();
        var total = 0F;

        foreach (var value in values)
        { total += value; }

        RemoveAllConstantValues();

        if (total != 0)
        { newKey = AddConstantValue(total); }

        return newKey;
    }

    #endregion

    #region >>>> Dynamic Values

    /// <summary>
    /// [Simple] Instantiates a dynamic value that gradually increases or decreases over a fixed period of time.
    /// </summary>    
    /// <param name="value">Must be non-zero.</param>
    /// <param name="duration">Must be greater than zero.</param>
    /// <param name="updateMode">Value update direction.</param>
    /// <param name="disposalMode">Action to be taken once the type lapses.</param>
    /// <remarks>
    /// This is the simplest of all available dynamic values. It lets you easily create healing or damage over time effects.
    /// By using the AutomaticAddToConstants disposal mode you can pretty much automate your health bars.
    /// 
    /// Each time your player character is hit or healed you can add a new negative or positive dynamic value 
    /// with disposal mode set to AutomaticAddToConstants. This will tell the value controller to automatically
    /// turn your dynamic value into a constant value once it lapses.
    /// 
    /// This way the total of your constant values will reflect your player's health.
    /// </remarks>
    public int AddDynamicValue(float value, float duration,
        eValueControllerDynamicValueUpdateMode updateMode,
        eValueControllerDynamicValueDisposalMode disposalMode)
    {
        // Check            
        if (value == 0)
        { 
            Debug.LogError("Value must be non-zero.");
            return -1;
        }

        if (duration <= 0)
        { 
            Debug.LogError("Duration must be greater than zero.");
            return -1;
        }

        // Update key
        _lastDynamicValueKey++;

        // Add value to collection
        _dynamicValues.Add(_lastDynamicValueKey,
            new ValueControllerDynamicValue(_lastDynamicValueKey, value, duration, updateMode, disposalMode, OnValueLapsed));

        // Add Update delegate reference to trigger value updates when Update or FixedUpdate methods
        // of this class are called by the engine.
        UpdateDynamicValues += _dynamicValues[_lastDynamicValueKey].Update;

        return _lastDynamicValueKey;
    }

    /// <summary>
    /// [Simple Stackable] Instantiates a stackable dynamic value that gradually increases or decreases over a fixed period of time. 
    /// </summary>    
    /// <param name="value">Must be non-zero.</param>
    /// <param name="duration">Must be greater than zero.</param>
    /// <param name="stacks">Number of initial stacks for this value. Must be greater than zero.</param>
    /// <param name="updateMode">ValueFrom update direction.</param>
    /// <param name="disposalMode">Action to be taken once the value lapses.</param>    
    /// <remarks>
    /// Stacks can be used to force the value to 'restart' updating once a stack lapses. The updates will 
    /// continue as long as there is at least one stack remaining.
    /// 
    /// This is useful when you have a value effect that has a set duration but needs to be repeated several times.
    /// Adding stacks will restart the update process as many times as there are stacks.
    /// 
    /// There are situations when it is necessary to show the comulative duration of all stacks on the UI rather
    /// than simply the current remaining duration of a particular stack. 
    /// You have the option to get this total duration from the Value Controller when calling aggregation functions.
    /// 
    /// When a stack lapses the <see cref="DelegateValueControllerValueStackLapsed"/> callback method will be called. 
    /// The <see cref="DelegateValueControllerValueLapsed"/> method will only be invoked when the last stack lapses.
    /// </remarks>
    public int AddDynamicValue(float value, float duration, int stacks,
        eValueControllerDynamicValueUpdateMode updateMode,
        eValueControllerDynamicValueDisposalMode disposalMode)
    {
        // Check
        if (stacks <= 0)
        { 
            Debug.LogError("ValueController.AddDynamicValue() : Stacks value must be greater than 0.");
            return -1;
        }

        // Update key
        _lastDynamicValueKey++;

        // Add value to collection
        _dynamicValues.Add(_lastDynamicValueKey,
            new ValueControllerDynamicValue(_lastDynamicValueKey, value, duration, stacks,
                updateMode, disposalMode, OnValueLapsed, OnValueStackLapsed));

        // Add Update delegate reference to trigger value updates when Update or FixedUpdate methods
        // of this class are called by the engine.
        UpdateDynamicValues += _dynamicValues[_lastDynamicValueKey].Update;

        return _lastDynamicValueKey;
    }

    /// <summary>
    /// [Counter] Instantiates a counter dynamic value that gradually increases or decreases over a fixed interval
    /// until the target value is reached.
    /// </summary>    
    /// <param name="valueFrom">Initial value.</param>
    /// <param name="valueTo">Target value.</param>
    /// <param name="intervalValue">Step value used to change the initial countdown value toward the target value.</param>
    /// <param name="intervalDuration">The lenght of time (in seconds) each interval should take. Must be greater than zero.</param>        
    /// <param name="disposalMode">Action to be taken once the value lapses.</param>    
    /// <remarks>
    /// Using a counter value you can easily create...well countdowns or countups if you want. You can also make all kinds of
    /// time measuring devices. Timers, stopwatches and whatevers.
    /// 
    /// The best part is that the 'tick' of the said time keeping device can be anything and can take any lenght of time.
    /// So you can easily have a timer that ticks every 4.23456 seconds and the tick value could be 5490! 
    /// Wierd combo but you never know.
    /// 
    /// Suddenly that X gold per N seconds upkeep model for your fantasy RTS doent seem like such a hastle to implement does it?
    /// Simply have one dynamic value tick every N seconds with X interval value and that's all there is to it.
    /// Every tick a callback method will be invoked letting your handler class know that an interval lapsed 
    /// (queue for you to make that cool gold count bubble float above your townhall or something). 
    /// You can pull any other details you may need directly from the callback parameter.
    /// 
    /// 
    /// When an interval lapses the <see cref="DelegateValueControllerIntervalLapsed"/> callback method will be called. 
    /// The <see cref="DelegateValueControllerValueLapsed"/> method will only be invoked when the target value is reached.
    /// </remarks>
    public int AddDynamicValue(float valueFrom, float valueTo, float intervalValue, float intervalDuration,
        eValueControllerDynamicValueDisposalMode disposalMode)
    {
        // Update key
        _lastDynamicValueKey++;

        // Add value to collection
        _dynamicValues.Add(_lastDynamicValueKey,
            new ValueControllerDynamicValue(_lastDynamicValueKey, valueFrom, valueTo,
                intervalValue, intervalDuration, disposalMode, OnValueIntervalLapsed, OnValueLapsed));

        // Add Update delegate reference to trigger type updates when Update or FixedUpdate methods
        // of this class are called by the engine.
        UpdateDynamicValues += _dynamicValues[_lastDynamicValueKey].Update;

        return _lastDynamicValueKey;
    }
    
    /// <summary>
    /// [Timer] Instantiates a timer dynamic value that ticks once per specified interval.
    /// </summary>    
    /// <param name="intervalDuration">The lenght of time (in seconds) each interval should take. Must be greater than zero.</param>            
    /// <remarks>
    /// Timer is similar in concept to other dynamic values with the only difference being that it does not change a given value over time. 
    /// It simply ticks once per specified interval making it an excelent...well...timer!
    /// 
    /// When an interval lapses the <see cref="DelegateValueControllerIntervalLapsed"/> callback method will be called.     
    /// </remarks>
    public int AddDynamicValue(float intervalDuration)
    {
        // Update key
        _lastDynamicValueKey++;

        // Add value to collection
        _dynamicValues.Add(_lastDynamicValueKey,
            new ValueControllerDynamicValue(_lastDynamicValueKey, intervalDuration, OnValueIntervalLapsed));

        // Add Update delegate reference to trigger type updates when Update or FixedUpdate methods
        // of this class are called by the engine.
        UpdateDynamicValues += _dynamicValues[_lastDynamicValueKey].Update;

        return _lastDynamicValueKey;
    }

    /// <summary>
    /// [Continious] Instantiates a dynamic value that gradually increases or decreases over a fixed interval
    /// until maximum allowed positive / negative <see cref="Float"/> value is reached.
    /// </summary>    
    /// <param name="valueFrom">Initial value.</param>        
    /// <param name="intervalValue">Step value used to change the initial value. Must be non-zero.</param>
    /// <param name="intervalDuration">The lenght of time (in seconds) each interval should take. Must be greater than zero.</param>        
    /// <param name="disposalMode">Action to be taken once the value lapses.</param>    
    /// <remarks>
    /// This type of dynamic value is very similar to the counter value. The only difference being the fact that this one
    /// updates continiously until the maximum / minimum float value is reached.
    ///         
    /// One of the main applications of something like this would be to measure time. 
    /// 
    /// Usually when you need to see how much time has passed since event X you would record the Time.time value
    /// when the event occurs and then subtract that value from current Time.time whenever needed.
    /// This type of dynamic value simply expands on that concept and lets you configure how time is measured
    /// from the moment the event X occured. Without the need for any extra plumbing code. Moreover, you get the 
    /// added benefit of callbacks.
    /// 
    /// When an interval lapses the <see cref="DelegateValueControllerIntervalLapsed"/> callback method will be called. 
    /// The <see cref="DelegateValueControllerValueLapsed"/> method will only be invoked when the target value is reached.
    /// </remarks>    
    public int AddDynamicValue(float valueFrom, float intervalValue, float intervalDuration,
        eValueControllerDynamicValueDisposalMode disposalMode)
    {
        // Update key
        _lastDynamicValueKey++;

        // Add value to collection
        _dynamicValues.Add(_lastDynamicValueKey,
            new ValueControllerDynamicValue(_lastDynamicValueKey, valueFrom, intervalValue, intervalDuration,
                disposalMode, OnValueIntervalLapsed, OnValueLapsed));

        // Add Update delegate reference to trigger value updates when Update or FixedUpdate methods
        // of this class are called by the engine.
        UpdateDynamicValues += _dynamicValues[_lastDynamicValueKey].Update;

        return _lastDynamicValueKey;
    }
    
    /// <summary>
    /// [Continious Looped] Instantiates an 'infinite' dynamic value that gradually increases or decreases over a period of time 
    /// until the target value is reached. Value is then 'restarted' using initial values.
    /// </summary>        
    /// <param name="valueFrom">Initial value.</param>
    /// <param name="valueTo">Target value.</param>    
    /// <param name="duration">Must be greater than zero.</param>                            
    /// <remarks>
    /// This type of dynamic value is very similar to the counter value. The only difference being the fact that this one is infinite
    /// and doesn't have an interval.
    ///         
    /// We can easily make Day / Night cycles using this value. Your Sun's rotation can be updated using current value. 
    /// Move your sun(s) and moon(s) at any pace you want. Don't have a moving Sun? Not a problem. Just hook your global illumination 
    /// to the current value and that's it. Well almost it, you will need to mess with the numbers a bit but at least you won't need 
    /// to worry about anything else.    
    /// </remarks>
    public int AddDynamicValue(float valueFrom, float valueTo, float duration)
    {
        // Update key
        _lastDynamicValueKey++;

        // Add value to collection
        _dynamicValues.Add(_lastDynamicValueKey,
            new ValueControllerDynamicValue(_lastDynamicValueKey, valueFrom, valueTo, duration));

        // Add Update delegate reference to trigger value updates when Update or FixedUpdate methods
        // of this class are called by the engine.
        UpdateDynamicValues += _dynamicValues[_lastDynamicValueKey].Update;

        return _lastDynamicValueKey;
    }

    /// <summary>
    /// [Intervals Looped] Instantiates an 'infinite' dynamic value that gradually increases or decreases over a period of time with 
    /// fixed intervals until the target value is reached. Value is then 'restarted' using initial values.
    /// </summary>    
    /// <param name="valueFrom">Initial value.</param>
    /// <param name="valueTo">Target value.</param>
    /// <param name="intervalValue">Step value used to change the initial value. Must be non-zero.</param>
    /// <param name="intervalDuration">The lenght of time (in seconds) each interval should take. Must be greater than zero.</param>                            
    public int AddDynamicValue(float valueFrom, float valueTo, float intervalValue, float intervalDuration)
    {
        // Update key
        _lastDynamicValueKey++;

        // Add value to collection
        _dynamicValues.Add(_lastDynamicValueKey,
            new ValueControllerDynamicValue(_lastDynamicValueKey, valueFrom, valueTo, intervalValue, intervalDuration, OnValueIntervalLapsed));

        // Add Update delegate reference to trigger value updates when Update or FixedUpdate methods
        // of this class are called by the engine.
        UpdateDynamicValues += _dynamicValues[_lastDynamicValueKey].Update;

        return _lastDynamicValueKey;
    }


    /// <summary>
    /// Checks if the controller has a dynamic value with the specified key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsDynamicValue(int key)
    { return _dynamicValues.ContainsKey(key); }

    /// <summary>
    /// Retrieves a dynamic value with the specified key.
    /// </summary>
    /// <returns></returns>
    public ValueControllerDynamicValue GetDynamicValue(int key)
    {
        if (!_dynamicValues.ContainsKey(key))
        {
            Debug.LogError(String.Format("ValueController.GetDynamicValue() : Key {0} not found in the collection.", key));
            return null;
        }

        return _dynamicValues[key];
    }

    /// <summary>
    /// Removes a dynamic value with the specified key.
    /// </summary>        
    public void RemoveDynamicValue(int key)
    {
        if (!_dynamicValues.ContainsKey(key))
        {
            Debug.LogError(String.Format("ValueController.RemoveDynamicValue() : Key {0} not found in the collection.", key));
            return;
        }

        var value = _dynamicValues[key];

        UpdateDynamicValues -= value.Update;
        value.Dispose();
        value = null;

        _dynamicValues.Remove(key);
    }

    /// <summary>
    /// Removes all dynamic values currently managed by this controller.
    /// </summary>
    public void RemoveAllDynamicValues()
    {
        // Check
        if (_dynamicValues.Count == 0)
        { return; }

        var keys = _dynamicValues.ToListKeys();

        foreach (var key in keys)
        { RemoveDynamicValue(key); }
    }

    #endregion

    #region >>>> Updates

    void Update()
    {
        // Check
        if (_isSuspended || UpdateDynamicValues == null || _updateMode != eValueControllerUpdateMode.OnUpdate)
        { return; }

        // Get current time
        var time = Time.time;
                
        if (_ignoreIdleTime)
        {            
            ResetLastUpdateTime(time);
            _ignoreIdleTime = false;
        }

        // Update all Dynamic values            
        UpdateDynamicValues(time);
    }

    void FixedUpdate()
    {
        // Check
        if (_isSuspended || UpdateDynamicValues == null || _updateMode != eValueControllerUpdateMode.OnFixedUpdate)
        { return; }

        // Get current time
        var time = Time.time;

        if (_ignoreIdleTime)
        {
            ResetLastUpdateTime(time);
            _ignoreIdleTime = false;
        }

        // Update all Dynamic values            
        UpdateDynamicValues(time);
    }
        
    private void ResetLastUpdateTime(int key, float time)
    {
        // Check
        if (_dynamicValues == null)
        { return; }

        if (time < 0)
        {
            Debug.LogError(String.Format(@"ValueController.ResetLastUpdateTime() : Supplied Time value ({0}) is less than 0.", time));
            return;
        }

        if (!_dynamicValues.ContainsKey(key))
        {
            Debug.LogError(String.Format(@"ValueController.ResetLastUpdateTime() : Supplied key ({0}) was not found in the collection.", key));
            return;
        }

        var dv = _dynamicValues[key];
        dv.ChangeLastUpdateTime(time);
    }

    private void ResetLastUpdateTime(float time)
    {
        // Check
        if (time < 0)
        {
            Debug.LogError(String.Format(@"ValueController.ResetLastUpdateTime() : Supplied Time value ({0}) is less than 0.", time));
            return;
        }

        foreach (var dv in _dynamicValues.Values)
        { dv.ChangeLastUpdateTime(time); }
    }

    #endregion

    #region >>>> Counters & Totals

    /// <summary>
    /// Returns current total of all Constant values.
    /// </summary>
    /// <returns></returns>
    public float GetConstantValuesTotal()
    {
        float total = 0;

        foreach (var value in _constantValues.Values)
        { total += value; }

        return total;
    }

    /// <summary>
    /// Returns current total of all Dynamic values including all stacks.
    /// </summary>        
    public float GetDynamicValuesTotal()
    { return GetDynamicValuesTotal(true); }

    /// <summary>
    /// Returns current total of all Dynamic values.
    /// </summary>
    /// <param name="includeStacks">
    /// Set to True to get a commulitive total that includes all stacks.
    /// </param>
    /// <returns></returns>
    public float GetDynamicValuesTotal(bool includeStacks)
    {
        float total = 0;

        // Get current totals only
        if (!includeStacks)
        {
            foreach (var value in _dynamicValues.Values)
            { total += value.CurrentValue; }
        }
        // Get commulitive totals
        else
        {
            // Stack count is reduced by 1 to account for the fact that one of the stacks is current.
            foreach (var value in _dynamicValues.Values)
            { total += value.CurrentValue + (value.ValueFrom * (value.Stacks - 1)); }
        }

        return total;
    }

    /// <summary>
    /// Returns current grand total of Constant and Dynamic values, including all stacks.
    /// </summary>
    /// <returns></returns>
    public float GetAllValuesTotal()
    { return GetAllValuesTotal(true); }

    /// <summary>
    /// Returns current grand total of Constant and Dynamic values.
    /// </summary>
    /// <param name="includeStacks">
    /// Set to True to get a commulitive total that includes all stacks.
    /// </param>
    /// <returns></returns>
    public float GetAllValuesTotal(bool includeStacks)
    { return GetConstantValuesTotal() + GetDynamicValuesTotal(includeStacks); }

    #endregion

    #region >>>> Delegates

    /// <summary>
    /// Called each time a dynamic value managed by this controller lapses.
    /// </summary>
    public DelegateValueControllerValueLapsed CallbackValueLapsed;

    /// <summary>
    /// Internal handler for lapsed dynamic values.
    /// </summary>
    /// <param name="type"></param>
    private void OnValueLapsed(ValueControllerDynamicValue value)
    {
        // Check
        if (value == null)
        {
            Debug.LogError(@"ValueController.OnValueLapsed() : Value is null.");
            return;
        }

        // Remove delegate references
        UpdateDynamicValues -= value.Update;

        // Remove value
        switch (value.DisposalMode)
        {
            case eValueControllerDynamicValueDisposalMode.Manual:

                if (CallbackValueLapsed != null)
                {
                    // Notify external handlers
                    CallbackValueLapsed(value);
                }

                break;

            case eValueControllerDynamicValueDisposalMode.Automatic:

                if (!_dynamicValues.ContainsKey(value.Key))
                {
                    Debug.LogError(@"ValueController.OnValueLapsed() : Key not found in the collection.");
                    return;
                }

                RemoveDynamicValue(value.Key);
                break;

            case eValueControllerDynamicValueDisposalMode.AutomaticAddToConstants:

                if (!_dynamicValues.ContainsKey(value.Key))
                {
                    Debug.LogError(@"ValueController.OnValueLapsed() : Key not found in the collection.");
                    return;
                }

                // Remove dynamic value and add a constant in its place
                if (value.CurrentValue != 0)
                { AddConstantValue(value.CurrentValue); }

                RemoveDynamicValue(value.Key);
                break;
        }
    }

    /// <summary>
    /// Called each time a stack lapses on a dynamic value managed by this controller.
    /// </summary>
    public DelegateValueControllerStackLapsed CallcackValueStackLapsed;

    /// <summary>
    /// Internal handler for lapsed dynamic value stacks.
    /// </summary>
    /// <param name="type"></param>
    private void OnValueStackLapsed(ValueControllerDynamicValue value)
    {
        // Check
        if (value == null)
        {
            Debug.LogError(@"ValueController.OnValueStackLapsed() : Value is null.");
            return;
        }

        if (CallcackValueStackLapsed != null)
        {
            // Notify external handlers
            CallcackValueStackLapsed(value);
        }
    }

    /// <summary>
    /// Called each time an interval lapses on a dynamic value managed by this controller.
    /// </summary>
    public DelegateValueControllerIntervalLapsed CallbackValueIntervalLapsed;

    /// <summary>
    /// Internal handler for lapsed dynamic value intervals.
    /// </summary>
    /// <param name="type"></param>
    private void OnValueIntervalLapsed(ValueControllerDynamicValue value)
    {
        // Check
        if (value == null)
        {
            Debug.LogError(@"ValueController.OnValueIntervalLapsed() : Value is null.");
            return;
        }

        if (CallbackValueIntervalLapsed != null)
        {
            // Notify external handlers
            CallbackValueIntervalLapsed(value);
        }
    }

    #endregion

}

#endregion

#region >>>> Value Controller Dynamic Value

/// <summary>
/// This class represents a dynamic value type that can change over a given period of time.
/// </summary>
public class ValueControllerDynamicValue
{

    #region >>>> Private Types

    /// <summary>
    /// Determines value's behavior during Update calls.
    /// </summary>
    private enum eValueType
    {
        Simple,
        Counter,
        Timer,
        Continuous,
        ContiniousLooped,
        IntervalsLooped,
    }

    #endregion

    #region >>>> Members

    /// <summary>
    /// Holds value's disposed status.
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// Type of the value.
    /// </summary>
    private eValueType _type;

    /// <summary>
    /// Modifier used to apply correct sign to the delta value that is used to change 
    /// the main value during update calls.
    /// </summary>
    private float _valueDeltaModifier;

    /// <summary>
    /// Modifier used to correctly scale time delta values during update calls.
    /// </summary>
    private float _timeDeltaModifier;

    /// <summary>
    /// Time at the begining of the last Update cycle.
    /// </summary>
    private float _lastUpdateTime;

    /// <summary>
    /// Time delta value between the beginning of previous and current Update cycles.
    /// </summary>
    private float _updateTimeDelta;

    /// <summary>
    /// Mode using which the value is updated.
    /// </summary>
    private eValueControllerDynamicValueUpdateMode _updateMode;

    /// <summary>
    /// Method to call when the value lapses.
    /// </summary>
    public DelegateValueControllerValueLapsed CallbackValueLapsed;

    /// <summary>
    /// Method to call when value's stack lapses.
    /// </summary>
    public DelegateValueControllerStackLapsed CallbackValueStackLapsed;

    /// <summary>
    /// Method to call when the value's interval lapses.
    /// </summary>
    public DelegateValueControllerIntervalLapsed CallbackIntervalLapsed;

    /// <summary>
    /// Initial value set during initialization.
    /// </summary>
    /// <remarks>
    /// This property returns the value that was provided during intialization of this instance.
    /// 
    /// Meaning that, irrespective of value's update mode the returned value will always be equal to the one 
    /// specified in the call to the constructor.        
    /// </remarks>
    public float ValueFrom
    { get { return _valueFrom; } }
    private float _valueFrom;

    /// <summary>
    /// Target value set during initialization.
    /// </summary>
    /// <remarks>
    /// This property returns the value that was provided during intialization of this instance.
    /// 
    /// Meaning that, irrespective of value's update mode the returned value will always be equal to the one 
    /// specified in the call to the constructor.
    /// </remarks>
    public float ValueTo
    { get { return _valueTo; } }
    private float _valueTo;

    /// <summary>
    /// Difference between <see cref="ValueFrom"/> and <see cref="ValueTo"/> set during initialization.
    /// </summary>
    /// <remarks>
    /// This property returns the value that was provided during intialization of this instance.
    /// 
    /// Meaning that, irrespective of value's update mode the returned value will always be equal to the one 
    /// specified in the call to the constructor.        
    /// </remarks>
    public float Value
    { get { return _value; } }
    private float _value;

    /// <summary>
    /// Interval value.
    /// </summary>
    public float IntervalValue
    { get { return _intervalValue; } }
    private float _intervalValue;

    /// <summary>
    /// Interval duration.
    /// </summary>
    public float IntervalDuration
    { get { return _intervalDuration; } }
    private float _intervalDuration;

    /// <summary>
    /// Value set during previous Update cycle.
    /// </summary>
    public float LastValue
    { get { return _lastValue; } }
    private float _lastValue;

    /// <summary>
    /// Value set during current Update cycle.
    /// </summary>
    public float CurrentValue
    { get { return _currentValue; } }
    private float _currentValue;

    /// <summary>
    /// Variance between Last and Current value.
    /// </summary>
    public float DeltaValue
    { get { return _currentValue - _lastValue; } }
    private float _deltaValue;

    /// <summary>
    /// Initial duration (in seconds) for this value.
    /// </summary>
    public float Duration
    { get { return _duration; } }
    private float _duration;

    /// <summary>
    /// Time remaining (in seconds) before the value lapses.
    /// </summary>
    public float RemainingDuration
    { get { return _remainingDuration; } }
    private float _remainingDuration;

    /// <summary>
    /// Remaining number of stacks.
    /// </summary>
    public int Stacks
    { get { return _stacks; } }
    private int _stacks;

    /// <summary>
    /// Flag indicating if the value has lapsed and is ready for disposal.
    /// </summary>
    public bool Lapsed
    { get { return _lapsed; } }
    private bool _lapsed;

    /// <summary>
    /// Mode using which the value is disposed once it lapses.
    /// </summary>
    public eValueControllerDynamicValueDisposalMode DisposalMode
    { get { return _disposalMode; } }
    private eValueControllerDynamicValueDisposalMode _disposalMode = eValueControllerDynamicValueDisposalMode.AutomaticAddToConstants;

    /// <summary>
    /// Key that can be used to retrieve this value from the controller's internal collection.
    /// </summary>
    public int Key
    { get { return _key; } }
    private int _key;

    #endregion

    #region >>>> Constructors

    /// <summary>
    /// [Simple] Instantiates a dynamic value that gradually increases or decreases over a fixed period of time.
    /// </summary>
    /// <param name="key">ID of this instance.</param>
    /// <param name="value">Must be non-zero.</param>
    /// <param name="duration">Must be greater than zero.</param>
    /// <param name="updateMode">Value update direction.</param>
    /// <param name="disposalMode">Action to be taken once the type lapses.</param>
    /// <param name="callbackValueLapsed">Reference to the method that should be called once the value lapses.</param>
    public ValueControllerDynamicValue(int key, float value, float duration,
        eValueControllerDynamicValueUpdateMode updateMode,
        eValueControllerDynamicValueDisposalMode disposalMode,
        DelegateValueControllerValueLapsed callbackLapsed)
    {
        // Check            
        if (value == 0)
        { 
            Debug.LogError("Value must be non-zero.");
            return;
        }

        if (duration <= 0)
        { 
            Debug.LogError("Duration must be greater than zero.");
            return;
        }

        // Init
        _type = eValueType.Simple;

        _key = key;                
        _duration = duration;
        _remainingDuration = _duration;
        _stacks = 1;

        _updateMode = updateMode;
        _disposalMode = disposalMode;

        _disposed = false;
        _lapsed = false;

        _intervalValue = 0;
        _intervalDuration = 0;

        if (callbackLapsed != null)
        { CallbackValueLapsed += callbackLapsed; }
        else
        { CallbackValueLapsed = null; }

        CallbackValueStackLapsed = null;
        CallbackIntervalLapsed = null;


        // Calculate time delta modifier

        // The entire duration (regardless of its actual length in terms of seconds) is a single unit of time
        // that needs to pass before a value lapses.
        // So, in order to correctly change values over time we need to have a modifier (coefficient)
        // that can be used to normalize the time deltas.
        _timeDeltaModifier = 1 / duration;

        // Set initial values and get the value delta modifier
        switch (_updateMode)
        {
            case eValueControllerDynamicValueUpdateMode.FromZeroToValue:

                if (value > 0)
                { _valueDeltaModifier = 1; }
                else
                { _valueDeltaModifier = -1; }

                _valueFrom = 0;
                _lastValue = _valueFrom;
                _currentValue = _valueFrom;
                                
                _valueTo = value;                                
                break;

            case eValueControllerDynamicValueUpdateMode.FromValueToZero:
                
                if (value > 0)
                { _valueDeltaModifier = -1; }
                else
                { _valueDeltaModifier = 1; }

                _valueFrom = value;
                _lastValue = _valueFrom;
                _currentValue = _valueFrom;
                
                _valueTo = 0;
                break;
        }
                
        _value = Mathf.Abs(_valueFrom - _valueTo);
    }

    /// <summary>
    /// [Simple Stackable] Instantiates a stackable dynamic value that gradually increases or decreases over a fixed period of time. 
    /// </summary>
    /// <param name="key">ID of this instance.</param>
    /// <param name="value">Must be non-zero.</param>
    /// <param name="duration">Must be greater than zero.</param>
    /// <param name="stacks">Number of initial stacks for this value. Must be greater than zero.</param>
    /// <param name="updateMode">ValueFrom update direction.</param>
    /// <param name="disposalMode">Action to be taken once the value lapses.</param>
    /// <param name="callbackValueLapsed">Reference to the method that should be called once the value lapses.</param>
    /// <param name="callbackStackLapsed">Reference to the method that should be called once value's current stack lapses.</param>    
    public ValueControllerDynamicValue(int key, float value, float duration, int stacks,
        eValueControllerDynamicValueUpdateMode updateMode,
        eValueControllerDynamicValueDisposalMode disposalMode,
        DelegateValueControllerValueLapsed callbackValueLapsed,
        DelegateValueControllerStackLapsed callbackStackLapsed)
        : this(key, value, duration, updateMode, disposalMode, callbackValueLapsed)
    {
        // Check
        if (stacks <= 0)
        { 
            Debug.LogError("Stacks must be greater than zero.");
            return;
        }

        if (callbackStackLapsed != null)
        { CallbackValueStackLapsed += callbackStackLapsed; }
        else
        { CallbackValueStackLapsed = null; }

        _stacks = stacks;
    }

    /// <summary>
    /// [Counter] Instantiates a counter dynamic value that gradually increases or decreases over a fixed interval
    /// until the target value is reached.
    /// </summary>
    /// <param name="key">ID of this instance.</param>
    /// <param name="valueFrom">Initial value.</param>
    /// <param name="valueTo">Target value.</param>
    /// <param name="intervalValue">Step value used to change the initial countdown value toward the target value.</param>
    /// <param name="intervalDuration">The lenght of time (in seconds) each interval should take. Must be greater than zero.</param>        
    /// <param name="disposalMode">Action to be taken once the value lapses.</param>
    /// <param name="callbackValueLapsed">Reference to the method that should be called once the value lapses.</param>
    /// <param name="callbackIntervalLapsed">Reference to the method that should be called every time an interval lapses.</param>    
    public ValueControllerDynamicValue(int key, float valueFrom, float valueTo,
        float intervalValue, float intervalDuration,
        eValueControllerDynamicValueDisposalMode disposalMode,
        DelegateValueControllerIntervalLapsed callbackIntervalLapsed,
        DelegateValueControllerValueLapsed callbackValueLapsed)
    {
        // Check            
        if (valueFrom == valueTo)
        { 
            Debug.LogError("Value From must not be equal to Value To.");
            return;
        }

        if (intervalValue <= 0)
        { 
            Debug.LogError("Interval Value must be greater than zero.");
            return;
        }

        if (intervalDuration <= 0)
        { 
            Debug.LogError("Interval Duration must be greater than zero.");
            return;
        }

        // Init
        _type = eValueType.Counter;

        _key = key;
        _valueFrom = valueFrom;
        _valueTo = valueTo;
        _value = Mathf.Abs(_valueFrom - _valueTo);
        _stacks = 1;

        _disposalMode = disposalMode;
        _disposed = false;
        _lapsed = false;

        if (callbackValueLapsed != null)
        { CallbackValueLapsed += callbackValueLapsed; }
        else
        { CallbackValueLapsed = null; }

        if (callbackIntervalLapsed != null)
        { CallbackIntervalLapsed += callbackIntervalLapsed; }
        else
        { callbackIntervalLapsed = null; }

        CallbackValueStackLapsed = null;

        // Calculate time delta modifier
        _timeDeltaModifier = 1 / intervalDuration;

        // Set initial values and get the value delta modifier
        if (_valueFrom < _valueTo)
        { _valueDeltaModifier = 1; }
        else
        { _valueDeltaModifier = -1; }

        _lastValue = _valueFrom;
        _currentValue = _valueFrom;
        _intervalValue = intervalValue;
        _intervalDuration = intervalDuration;

        _remainingDuration = _intervalDuration;
    }

    /// <summary>
    /// [Timer] Instantiates a timer dynamic value that ticks once per specified interval.
    /// </summary>        
    /// <param name="key">ID of this instance.</param>    
    /// <param name="intervalDuration">The lenght of time (in seconds) each interval should take. Must be greater than zero.</param>            
    /// <param name="callbackIntervalLapsed">Reference to the method that should be called every time an interval lapses.</param>    
    public ValueControllerDynamicValue(int key, float intervalDuration,        
        DelegateValueControllerIntervalLapsed callbackIntervalLapsed)
    {
        // Check
        if (intervalDuration <= 0)
        {
            Debug.LogError("Interval Duration must be greater than zero.");
            return;
        }

        // Init
        _type = eValueType.Timer;

        _key = key;
        _valueFrom = 0;
        _valueTo = 0;
        _value = 0;
        _stacks = 1;

        _disposalMode = eValueControllerDynamicValueDisposalMode.Manual;
        _disposed = false;
        _lapsed = false;
        
        if (callbackIntervalLapsed != null)
        { CallbackIntervalLapsed += callbackIntervalLapsed; }
        else
        { callbackIntervalLapsed = null; }

        CallbackValueLapsed = null;
        CallbackValueStackLapsed = null;

        // Calculate time delta modifier
        _timeDeltaModifier = 1 / intervalDuration;

        // Set initial values and get the value delta modifier
        if (_valueFrom < _valueTo)
        { _valueDeltaModifier = 1; }
        else
        { _valueDeltaModifier = -1; }

        _lastValue = _valueFrom;
        _currentValue = _valueFrom;
        _intervalValue = 0;
        _intervalDuration = intervalDuration;

        _remainingDuration = _intervalDuration;
    }

    /// <summary>
    /// [Continuous] Instantiates a dynamic value that gradually increases or decreases over a fixed interval
    /// until maximum allowed positive / negative <see cref="Float"/> value is reached.
    /// </summary>
    /// <param name="key">ID of this instance.</param>
    /// <param name="valueFrom">Initial value.</param>        
    /// <param name="intervalValue">Step value used to change the initial value. Must be non-zero.</param>
    /// <param name="intervalDuration">The lenght of time (in seconds) each interval should take. Must be greater than zero.</param>        
    /// <param name="disposalMode">Action to be taken once the value lapses.</param>
    /// <param name="callbackValueLapsed">Reference to the method that should be called once the value lapses.</param>
    /// <param name="callbackIntervalLapsed">Reference to the method that should be called every time an interval lapses.</param>    
    public ValueControllerDynamicValue(int key, float valueFrom, float intervalValue, float intervalDuration,
        eValueControllerDynamicValueDisposalMode disposalMode,
        DelegateValueControllerIntervalLapsed callbackIntervalLapsed,
        DelegateValueControllerValueLapsed callbackValueLapsed)
        : this(key, valueFrom, intervalValue > 0 ? float.MaxValue : float.MinValue, intervalValue,
        intervalDuration, disposalMode, callbackIntervalLapsed, callbackValueLapsed)
    {
        // Init            
        _type = eValueType.Continuous;
    }

    /// <summary>
    /// [Continious Looped] Instantiates an 'infinite' dynamic value that gradually increases or decreases over a period of time
    /// until the target value is reached. Value is then 'restarted' using initial values.
    /// </summary>
    /// <param name="key">ID of this instance.</param>
    /// <param name="valueFrom">Initial value.</param>
    /// <param name="valueTo">Target value.</param>        
    /// <param name="duration">The lenght of time (in seconds) each interval should take. Must be greater than zero.</param>                                
    public ValueControllerDynamicValue(int key, float valueFrom, float valueTo, float duration)
    {
        // Check            
        if (valueFrom == valueTo)
        { 
            Debug.LogError("Value From must not be equal to Value To.");
            return;
        }

        if (duration <= 0)
        { 
            Debug.LogError("Duration must be greater than zero.");
            return;
        }

        // Init
        _type = eValueType.ContiniousLooped;

        _key = key;
        _valueFrom = valueFrom;
        _valueTo = valueTo;
        _value = Mathf.Abs(_valueFrom - _valueTo);
        _duration = duration;

        _stacks = 1;
        _intervalValue = 0;
        _intervalDuration = 0;
        _disposalMode = eValueControllerDynamicValueDisposalMode.Manual;
        _disposed = false;
        _lapsed = false;

        CallbackValueLapsed = null;
        CallbackIntervalLapsed = null;
        CallbackValueStackLapsed = null;

        // Calculate time delta modifier
        _timeDeltaModifier = 1 / duration;

        // Set initial values and get the value delta modifier
        if (_valueFrom < _valueTo)
        { _valueDeltaModifier = 1; }
        else
        { _valueDeltaModifier = -1; }

        _lastValue = _valueFrom;
        _currentValue = _valueFrom;
        _remainingDuration = _duration;
    }
    
    /// <summary>
    /// [Intervals Looped] Instantiates an 'infinite' dynamic value that gradually increases or decreases over a period of time with 
    /// fixed intervals until the target value is reached. Value is then 'restarted' using initial values.
    /// </summary>
    /// <param name="key">ID of this instance.</param>
    /// <param name="valueFrom">Initial value.</param>
    /// <param name="valueTo">Target value.</param>
    /// <param name="intervalValue">Step value used to change the initial value. Must be non-zero.</param>
    /// <param name="intervalDuration">The lenght of time (in seconds) each interval should take. Must be greater than zero.</param>                        
    /// <param name="callbackIntervalLapsed">Reference to the method that should be called every time an interval lapses.</param>        
    public ValueControllerDynamicValue(int key, float valueFrom, float valueTo, float intervalValue, float intervalDuration,
        DelegateValueControllerIntervalLapsed callbackIntervalLapsed)
        : this(key, valueFrom, valueTo, intervalValue, intervalDuration, eValueControllerDynamicValueDisposalMode.Manual,
        callbackIntervalLapsed, null)
    {
        // Init            
        _type = eValueType.IntervalsLooped;
    }

    #endregion

    #region >>>> Methods

    /// <summary>
    /// - THIS METHOD SHOULD NOT BE CALLED DIRECTLY FROM USER CODE -
    /// 
    /// Updates the value based on set parameters using current time. In majority of cases 
    /// supplied time must always be Time.time.
    /// </summary>
    public void Update(float currentTime)
    {
        // Skip update if the value has already lapsed
        if (_lapsed || _disposed) { return; }

        // Get time delta since last call to this method
        var timeDelta = _lastUpdateTime != 0 ? currentTime - _lastUpdateTime : 0;

        _lastUpdateTime = currentTime;

        // Check
        if (timeDelta < 0)
        {
            Debug.LogWarning(String.Format(@"ValueControllerDynamicValue.Update() : Supplied currentTime value ({0}) 
                    is less than the time value ({1}) supplied during last call to this method.",
                currentTime, _lastUpdateTime));
            return;
        }
        else if (timeDelta == 0)
        { return; }

        _lastValue = _currentValue;

        // Update value
        switch (_type)
        {
            #region >>> Simple

            case eValueType.Simple:

                // Get value

                // Here we need to apply two modifiers:
                // - Value delta modifier to get the correct sign 
                // - Time delta modifier in order to get correct value delta
                _currentValue += _value * (_valueDeltaModifier * (timeDelta * _timeDeltaModifier));

                // Get remaining duration
                _remainingDuration -= timeDelta;

                // If set duration time lapsed, we need to notify the controller
                if (_remainingDuration <= 0)
                {
                    // More than one stack left
                    if (_stacks > 1)
                    {
                        // Update stack count
                        RemoveStacks(1);

                        // Notify handlers                            
                        if (CallbackValueStackLapsed != null)
                        { CallbackValueStackLapsed(this); }

                        // Reset counters

                        // Since it is likely that there could be a negative _remainingDuration type (i.e. an overflow), 
                        // for instance due to a delayed Update call, we should account for that when
                        // dealing with stacked values.
                        // This way transation from one stack to the next will not affect the combined duration
                        // of all stacks.
                        _remainingDuration += _duration;

                        switch (_updateMode)
                        {
                            case eValueControllerDynamicValueUpdateMode.FromValueToZero:
                                _currentValue = _valueFrom;
                                break;

                            case eValueControllerDynamicValueUpdateMode.FromZeroToValue:
                                _currentValue = 0;
                                break;
                        }
                    }
                    // Last stack
                    else
                    {
                        // Reset counters
                        _stacks = 0;
                        _remainingDuration = 0;

                        switch (_updateMode)
                        {
                            case eValueControllerDynamicValueUpdateMode.FromValueToZero:
                                _currentValue = 0;
                                break;

                            case eValueControllerDynamicValueUpdateMode.FromZeroToValue:
                                _currentValue = _valueTo;                                
                                break;
                        }

                        // Notify handlers
                        _lapsed = true;

                        if (CallbackValueLapsed != null)
                        { CallbackValueLapsed(this); }
                    }
                }

                break;

            #endregion

            #region >>> Counter

            case eValueType.Counter:

                // Get remaining duration for current interval
                _remainingDuration -= timeDelta;

                // If set interval duration time lapsed, we need to update counter values and notify the controller
                if (_remainingDuration <= 0)
                {
                    // Notify handlers      
                    if (CallbackIntervalLapsed != null)
                    { CallbackIntervalLapsed(this); }

                    // Update counters
                    _currentValue += _intervalValue * _valueDeltaModifier;

                    // Since it is likely that there could be a negative _remainingDuration type (i.e. an overflow), 
                    // for instance due to a delayed Update call, we should account for that when
                    // dealing with interval values.
                    // This way transation from one interval to the next will not affect the combined duration
                    // of all intervals.
                    _remainingDuration += _intervalDuration;
                }

                // Check if the target value has been reached
                if ((_valueDeltaModifier > 0 && _currentValue >= _valueTo) || (_valueDeltaModifier < 0 && _currentValue <= _valueTo))
                {
                    if (CallbackValueLapsed != null)
                    { CallbackValueLapsed(this); }
                }

                break;

            #endregion

            #region >>> Timer

            case eValueType.Timer:

                // Get remaining duration for current interval
                _remainingDuration -= timeDelta;

                // If set interval duration time lapsed, we need to notify the controller
                if (_remainingDuration <= 0)
                {
                    // Notify handlers      
                    if (CallbackIntervalLapsed != null)
                    { CallbackIntervalLapsed(this); }
                                        
                    // Since it is likely that there could be a negative _remainingDuration type (i.e. an overflow), 
                    // for instance due to a delayed Update call, we should account for that when
                    // dealing with interval values.
                    // This way transation from one interval to the next will not affect the combined duration
                    // of all intervals.
                    _remainingDuration += _intervalDuration;
                }
                
                break;

            #endregion
                
            #region >>> Continious

            case eValueType.Continuous:

                // Get remaining duration for current interval
                _remainingDuration -= timeDelta;

                // If set interval duration time lapsed, we need to update counter values and notify the controller
                if (_remainingDuration <= 0)
                {
                    // Notify handlers      
                    if (CallbackIntervalLapsed != null)
                    { CallbackIntervalLapsed(this); }

                    // Update counters
                    _currentValue += (_currentValue + (_intervalValue * _valueDeltaModifier)).ClampToFloatLimits();

                    // Since it is likely that there could be a negative _remainingDuration type (i.e. an overflow), 
                    // for instance due to a delayed Update call, we should account for that when
                    // dealing with interval values.
                    // This way transation from one interval to the next will not affect the combined duration
                    // of all intervals.
                    _remainingDuration += _intervalDuration;
                }

                // Check if the target value has been reached
                if ((_valueDeltaModifier > 0 && _currentValue >= float.MaxValue) || (_valueDeltaModifier < 0 && _currentValue <= float.MinValue))
                {
                    if (CallbackValueLapsed != null)
                    { CallbackValueLapsed(this); }
                }

                break;

            #endregion

            #region >>> Loop Intervals

            case eValueType.IntervalsLooped:

                // Get remaining duration for current interval
                _remainingDuration -= timeDelta;

                // If set interval duration time lapsed, we need to update counter values and notify the controller
                if (_remainingDuration <= 0)
                {
                    // Notify handlers      
                    if (CallbackIntervalLapsed != null)
                    { CallbackIntervalLapsed(this); }

                    // Update counters
                    _currentValue += _intervalValue * _valueDeltaModifier;

                    // Since it is likely that there could be a negative _remainingDuration type (i.e. an overflow), 
                    // for instance due to a delayed Update call, we should account for that when
                    // dealing with interval values.
                    // This way transation from one interval to the next will not affect the combined duration
                    // of all intervals.
                    _remainingDuration += _intervalDuration;
                }

                // Check if the target value has been reached and reset counters 
                // taking into account possible overflow 
                if (_valueDeltaModifier > 0 && _currentValue >= _valueTo)
                {
                    _currentValue = _valueFrom + (_currentValue - _valueTo);
                }
                else if (_valueDeltaModifier < 0 && _currentValue <= _valueTo)
                {
                    _currentValue = _valueFrom + (_currentValue + _valueTo);
                }

                break;

            #endregion

            #region >>> Loop Continious

            case eValueType.ContiniousLooped:

                // Update counters
                _currentValue += _value * (_valueDeltaModifier * (timeDelta * _timeDeltaModifier));

                // Get remaining duration for current interval
                _remainingDuration -= timeDelta;

                // If set duration time lapsed, we need to update counter values
                if (_remainingDuration <= 0)
                {
                    // Since it is likely that there could be a negative _remainingDuration type (i.e. an overflow), 
                    // for instance due to a delayed Update call, we should account for that when
                    // dealing with interval values.
                    // This way transation from one interval to the next will not affect the combined duration
                    // of all intervals.
                    _remainingDuration += _duration;

                    // Check if the target value has been reached and reset counters 
                    // taking into account possible overflow 
                    if (_valueDeltaModifier > 0 && _currentValue > _valueTo)
                    { _currentValue = _valueFrom + (_currentValue - _valueTo); }
                    else if (_valueDeltaModifier < 0 && _currentValue < _valueTo)
                    { _currentValue = _valueFrom + (_currentValue + _valueTo); }
                    else
                    { _currentValue = _valueFrom; }
                }

                break;

            #endregion
        }

    }

    /// <summary>
    /// Adds specified number of stacks. 
    /// This method only affects stackable values.
    /// </summary>
    /// <param name="count">Number of stacks to add.</param>        
    public void AddStacks(int count)
    {
        // Update stack count
        _stacks = Mathf.Max(_stacks + Mathf.Abs(count), 0);
    }

    /// <summary>
    /// Removes specified number of stacks. 
    /// This method only affects stackable values.
    /// </summary>
    /// <param name="count">Number of stacks to remove.</param>        
    public void RemoveStacks(int count)
    {
        // Update stack count
        _stacks = Mathf.Max(_stacks - Mathf.Abs(count), 0);

        // If stack count is 0 then the type is considered lapsed.
        if (_stacks == 0)
        {
            // First notify the controller that all stacks have lapsed
            if (CallbackValueStackLapsed != null)
            { CallbackValueStackLapsed(this); }

            // Then notify the controller that the value has lapsed

            // Since in this case the type is 'forced' to lapse as opposed to 
            // expiring after a specified duration we will not update any internal counters.                
            _lapsed = true;

            if (CallbackValueLapsed != null)
            { CallbackValueLapsed(this); }
        }
    }

    /// <summary>   
    /// Changes value's CurrentValue.
    /// </summary>    
    /// <remarks>
    /// This is very useful when you want to 'fast forward' the value without triggering any callbacks or
    /// other internal update mechanisms.    
    /// </remarks>
    public void ChangeCurrentValue(float value)
    {
        _currentValue = value.ClampToFloatLimits(_valueFrom, _valueTo);
        _lastValue = _currentValue;
    }

    /// <summary>
    /// - THIS METHOD SHOULD NOT BE CALLED DIRECTLY FROM USER CODE -
    /// 
    /// Changes the <see cref="_lastUpdateTime"/> value used to calculate time delta values during updates.
    /// </summary>
    /// <param name="time">Must be greater than 0.</param>
    /// <remarks>
    /// Used by Start / Suspend Updates mechanism of the Value Controller.
    /// </remarks>
    public void ChangeLastUpdateTime(float time)
    {
        // Check
        if (time < 0)
        {
            Debug.LogError(String.Format(@"ValueControllerDynamicValue.ResetLastUpdateTime() : Supplied Time value ({0}) 
                    is less than 0.", time));
            return;
        }

        _lastUpdateTime = time;
    }
    
    /// <summary>
    /// Call this method when handling disposal of a lapsed value manually.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        { return; }

        CallbackValueLapsed = null;
        CallbackValueStackLapsed = null;
        CallbackIntervalLapsed = null;

        _disposed = true;
    }

    #endregion

}

#region >>>> Delegates

/// <summary>
/// 
/// </summary>
public delegate void DelegateValueControllerUpdate(float time);

/// <summary>
/// 
/// </summary>
public delegate void DelegateValueControllerValueLapsed(ValueControllerDynamicValue value);

/// <summary>
/// 
/// </summary>
public delegate void DelegateValueControllerStackLapsed(ValueControllerDynamicValue value);

/// <summary>
/// 
/// </summary>
public delegate void DelegateValueControllerIntervalLapsed(ValueControllerDynamicValue value);

#endregion

#endregion

#region >>>> Additional Types

    /// <summary>
    /// Determines when the Value Controller updates dynamic values
    /// </summary>
    public enum eValueControllerUpdateMode
    { 
        OnUpdate,
        OnFixedUpdate
    }

    /// <summary>
    /// Determines the update direction of a basic dynamic value.
    /// </summary>
    public enum eValueControllerDynamicValueUpdateMode
    {
        /// <summary>
        /// Value will increaze over time starting from 0.
        /// </summary>
        FromZeroToValue,

        /// <summary>
        /// Value will decrease over time until it reaches 0.
        /// </summary>
        FromValueToZero
    }

    /// <summary>
    /// Determines the update direction of a continious dynamic value.
    /// </summary>
    public enum eValueControllerDynamicValueContiniousUpdateMode
    {
        /// <summary>
        /// Value will increase over time until it reaches positive infinity.
        /// </summary>
        ToPositiveInfinity,

        /// <summary>
        /// Value will decrease over time until it reaches negative infinity.
        /// </summary>
        ToNegativeInfinity
        
        // To infinity...and beyond!        
        //
        // BUZZ WAS HERE        
    }

    /// <summary>
    /// Determines how dynamic values are treated after they lapse.
    /// </summary>
    public enum eValueControllerDynamicValueDisposalMode
    {       
        /// <summary>
        /// Value will remain in the internal collection util disposed by user code.
        /// </summary>
        Manual,

        /// <summary>
        /// Value will be automatically removed from the internal collection.
        /// </summary>
        Automatic,

        /// <summary>
        /// Value will be removed from the internal collection of dynamic values
        /// and a new constant value type will be added in its place.
        /// 
        /// Applies only to non-zero values.
        /// </summary>
        /// <remarks>
        /// This mode is very useful for overtime effects that end up with a non zero value.
        /// 
        /// For example you could apply some damage and then have it added to the constant collection 
        /// so that the total sum of all values is reduced by that amount of damage.
        /// </remarks>
        AutomaticAddToConstants
    }

#endregion
