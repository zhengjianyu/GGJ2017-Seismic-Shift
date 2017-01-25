
/// --- deVoid Studios - Copyright © 2011 - All rights reserved ---
/// 
/// Author  :   Alexander "Feature" Pavlovsky
/// Date    :   02 / 10 / 2011
/// 
/// Contact :   support@devoidstudios.com
/// 
/// ---

#region >>>> Usings

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion


/// <summary>
/// Some useful extension methods for all kinds of stuff.
/// </summary>
public static partial class UnityExtensions
{

    #region >>>> Collections

    /// <summary>
    /// Returns a typed list of dictionary's keys.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static List<TKey> ToListKeys<TKey, TValue>(this Dictionary<TKey, TValue> dict)
    {
        var keys = new List<TKey> { };

        // Check
        if (dict == null)
        { return keys; }

        foreach (var item in dict)
        { keys.Add(item.Key); }

        return keys;
    }

    /// <summary>
    /// Returns a typed list of dictionary's keys.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static List<TKey> ToListKeys<TKey, TValue>(this SortedDictionary<TKey, TValue> dict)
    {
        var keys = new List<TKey> { };

        // Check
        if (dict == null)
        { return keys; }

        foreach (var item in dict)
        { keys.Add(item.Key); }

        return keys;
    }

    /// <summary>
    /// Returns a typed list of dictionary's values.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static List<TValue> ToListValues<TKey, TValue>(this Dictionary<TKey, TValue> dict)
    {

        var values = new List<TValue> { };

        // Check
        if (dict == null)
        { return values; }

        foreach (var item in dict)
        { values.Add(item.Value); }

        return values;
    }

    /// <summary>
    /// Returns a typed list of dictionary's values.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static List<TValue> ToListValues<TKey, TValue>(this SortedDictionary<TKey, TValue> dict)
    {

        var values = new List<TValue> { };

        // Check
        if (dict == null)
        { return values; }

        foreach (var item in dict)
        { values.Add(item.Value); }

        return values;
    }
    
    /// <summary>
    /// Returns a typed array of dictionary's values.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static TValue[] ToArrayValues<TKey, TValue>(this Dictionary<TKey, TValue> dict)
    {
        var values = new List<TValue> { };

        // Check
        if (dict == null)
        { return values.ToArray(); }

        foreach (var item in dict)
        { values.Add(item.Value); }

        return values.ToArray();
    }

    /// <summary>
    /// Returns a typed array of dictionary's values.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static TValue[] ToArrayValues<TKey, TValue>(this SortedDictionary<TKey, TValue> dict)
    {
        var values = new List<TValue> { };

        // Check
        if (dict == null)
        { return values.ToArray(); }

        foreach (var item in dict)
        { values.Add(item.Value); }

        return values.ToArray();
    }

    #endregion

    #region >>>> Floats

    /// <summary>
    /// Clamps input to the maximum / minimum float value. This is useful when you are expecting an 
    /// overflow and don't want to do all those .IsInfinity checks.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static float ClampToFloatLimits(this float input)
    {
        if (float.IsNegativeInfinity(input))
        { return float.MinValue; }

        if (float.IsPositiveInfinity(input))
        { return float.MaxValue; }

        return input;
    }

    /// <summary>
    /// Clamps input to the specified maximum / minimum values.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static float ClampToFloatLimits(this float input, float minimum, float maximum)
    {        

        // Check
        if (minimum > maximum)
        { 
            Debug.LogWarning(string.Format("ClampToFloatLimits : Minimum value {0} is greater than Maximum value {1}. Swaping values.", minimum, maximum));

            var temp = minimum;
            minimum = maximum;
            maximum = temp;
        }
        
        if (input < minimum)
        { return minimum; }

        if (input > maximum)
        { return maximum; }

        return input;
    }

    #endregion

    #region >>>> Rectangles

    /// <summary>
    /// Returns dimensions of a rectangle positioned inside a parent rectangle.
    /// </summary>
    /// <param name="rect">Normalized rectangle representing the desired area within the parent rectangle.</param>
    /// <param name="parentRect">Rectangle used to calculate the dimensions of the output rectangle.</param>
    /// <remarks>
    /// This method provides a quick way of getting absolute size of rectangle (i.e pixels) based on its normalized dimensions.    
    /// </remarks>
    public static Rect InsideRectFromNormalized(this Rect rect, Rect parentRect)
    {
        var left = (int)(parentRect.width * rect.xMin);
        var top = (int)(parentRect.height * rect.yMin);
        var width = (int)(parentRect.width * rect.width);
        var height = (int)(parentRect.height * rect.height);

        return new Rect(left, top, width, height);
    }

    #endregion

    #region >>>> Screen / Viewport

    /// <summary>
    /// Transforms a rectangle from viewport space into screen space.
    /// </summary>
    /// <param name="camera">Camera to be used in the conversion.</param>
    /// <param name="rect">Normalized area of the camera's viewport to be converted.</param>
    /// <returns></returns>
    public static Rect ViewportToScreenRect(this Camera camera, Rect rect)
    {
        // Check
        if (camera == null)
        { return new Rect(); }

        var left = camera.ViewportToScreenPoint(new Vector3(rect.xMin, 0)).x;
        var top = camera.ViewportToScreenPoint(new Vector3(0, rect.yMin)).y;
        var width = camera.ViewportToScreenPoint(new Vector3(rect.xMax, 0)).x - left;
        var height = camera.ViewportToScreenPoint(new Vector3(0, rect.yMax)).y - top;

        return new Rect(left, top, width, height);
    }

    #endregion

    #region >>>> WWW

    #region >>>> Typed Methods

    /// <summary>
    /// Use this extension method to download a resource from a given URL.
    /// It will attach a temporary wwwDowloader component to the target game object that will be destroyed once the download is complete.    
    /// </summary>    
    /// <param name="gameObject">Parent GameObject used to 'host' a temporary instance of the wwwDownloader class.</param>
    /// <param name="url">URL of the resource to be downloaded.</param>
    /// <param name="callback">Method to be called once the download is complete.</param>    
    public static void DownloadAssetBundle(this GameObject gameObject, string url, Action<AssetBundle> callback)
    {
        var downloader = (wwwDownloader)gameObject.AddComponent<wwwDownloader>();

        downloader.DownloadAssetBundle(url,
            (param) =>
            {
                // Destroy the downloader component
                GameObject.Destroy(downloader);

                // Return downloaded data to the caller
                callback(param);
            });
    }

    /// <summary>
    /// Use this extension method to download a resource from a given URL list.
    /// It will attach a temporary wwwDowloader component to the target game object that will be destroyed once the download is complete.    
    /// </summary>    
    /// <param name="gameObject">Parent GameObject used to 'host' a temporary instance of the wwwDownloader class.</param>
    /// <param name="urls">URLs of the resources to be downloaded.</param>
    /// <param name="callback">Method to be called once the download is complete for all resources.</param>
    /// <remarks>
    /// If the supplied parameters are invalid, value ruturned by the callback method will be <see cref="null"/>.
    /// </remarks>
    public static void DownloadAssetBundle(this GameObject gameObject, string[] urls, Action<AssetBundle[]> callback)
    {
        // Check
        if (callback == null)
        { Debug.LogError("DownloadAssetBundle : callback is null."); }

        if (gameObject == null)
        { callback(null); }

        // Init
        var downloads = new SortedDictionary<int, AssetBundle> { };

        #region >>> Actions

        // This method will add every download to the collection using the right indexing key.
        Action<int, AssetBundle> downloadComplete = (key, download) =>
        {
            downloads.Add(key, download);

            // Check if we have all the downloads we were expecting
            if (downloads.Count == urls.Length)
            {
                // Return downloaded data to the caller
                callback(downloads.ToArrayValues());
            }
        };

        #endregion

        // Lookup list used to identify and correctly sort each download in the output collection
        var indices = new Dictionary<string, int> { };

        // Download all resources
        foreach (var url in urls)
        {

            // Add an indexing key for this url
            indices.Add(url, indices.Count);

            DownloadAssetBundle(gameObject, url,
                (param) =>
                {
                    // Get the index of this url
                    var index = 0;
                    indices.TryGetValue(url, out index);

                    // Add the download to the collection
                    downloadComplete(index, param);
                });
        }
    }

    /// <summary>
    /// Use this extension method to download a resource from a given URL.
    /// It will attach a temporary wwwDowloader component to the target game object that will be destroyed once the download is complete.    
    /// </summary>    
    /// <param name="gameObject">Parent GameObject used to 'host' a temporary instance of the wwwDownloader class.</param>
    /// <param name="url">URL of the resource to be downloaded.</param>
    /// <param name="callback">Method to be called once the download is complete.</param>    
    public static void DownloadTexture2D(this GameObject gameObject, string url, Action<Texture2D> callback)
    {
        var downloader = (wwwDownloader)gameObject.AddComponent<wwwDownloader>();

        downloader.DownloadTexture2D(url,
            (param) =>
            {
                // Destroy the downloader component
                GameObject.Destroy(downloader);

                // Return downloaded data to the caller
                callback(param);
            });
    }

    /// <summary>
    /// Use this extension method to download a resource from a given URL list.
    /// It will attach a temporary wwwDowloader component to the target game object that will be destroyed once the download is complete.    
    /// </summary>    
    /// <param name="gameObject">Parent GameObject used to 'host' a temporary instance of the wwwDownloader class.</param>
    /// <param name="urls">URLs of the resources to be downloaded.</param>
    /// <param name="callback">Method to be called once the download is complete for all resources.</param>
    /// <remarks>
    /// If the supplied parameters are invalid, value ruturned by the callback method will be <see cref="null"/>.
    /// </remarks>
    public static void DownloadTexture2D(this GameObject gameObject, string[] urls, Action<Texture2D[]> callback)
    {
        // Check
        if (callback == null)
        { Debug.LogError("DownloadTexture2D : callback is null."); }

        if (gameObject == null)
        { callback(null); }

        // Init
        var downloads = new SortedDictionary<int, Texture2D> { };

        #region >>> Actions

        // This method will add every download to the collection using the right indexing key.
        Action<int, Texture2D> downloadComplete = (key, download) =>
        {
            downloads.Add(key, download);

            // Check if we have all the downloads we were expecting
            if (downloads.Count == urls.Length)
            {
                // Return downloaded data to the caller
                callback(downloads.ToArrayValues());
            }
        };

        #endregion

        // Lookup list used to identify and correctly sort each download in the output collection
        var indices = new Dictionary<string, int> { };

        // Download all resources
        foreach (var url in urls)
        {

            // Add an indexing key for this url
            indices.Add(url, indices.Count);

            DownloadTexture2D(gameObject, url,
                (param) =>
                {
                    // Get the index of this url
                    var index = 0;
                    indices.TryGetValue(url, out index);

                    // Add the download to the collection
                    downloadComplete(index, param);
                });
        }
    }

    /// <summary>
    /// Use this extension method to download a resource from a given URL.
    /// It will attach a temporary wwwDowloader component to the target game object that will be destroyed once the download is complete.    
    /// </summary>    
    /// <param name="gameObject">Parent GameObject used to 'host' a temporary instance of the wwwDownloader class.</param>
    /// <param name="url">URL of the resource to be downloaded.</param>
    /// <param name="callback">Method to be called once the download is complete.</param>    
    public static void DownloadMovieTexture(this GameObject gameObject, string url, Action<MovieTexture> callback)
    {
        var downloader = (wwwDownloader)gameObject.AddComponent<wwwDownloader>();

        downloader.DownloadMovieTexture(url,
            (param) =>
            {
                // Destroy the downloader component
                GameObject.Destroy(downloader);

                // Return downloaded data to the caller
                callback(param);
            });
    }
    
    /// <summary>
    /// Use this extension method to download a resource from a given URL list.
    /// It will attach a temporary wwwDowloader component to the target game object that will be destroyed once the download is complete.    
    /// </summary>    
    /// <param name="gameObject">Parent GameObject used to 'host' a temporary instance of the wwwDownloader class.</param>
    /// <param name="urls">URLs of the resources to be downloaded.</param>
    /// <param name="callback">Method to be called once the download is complete for all resources.</param>
    /// <remarks>
    /// If the supplied parameters are invalid, value ruturned by the callback method will be <see cref="null"/>.
    /// </remarks>
    public static void DownloadMovieTexture(this GameObject gameObject, string[] urls, Action<MovieTexture[]> callback)
    {
        // Check
        if (callback == null)
        { Debug.LogError("DownloadMovieTexture : callback is null."); }

        if (gameObject == null)
        { callback(null); }

        // Init
        var downloads = new SortedDictionary<int, MovieTexture> { };

        #region >>> Actions

        // This method will add every download to the collection using the right indexing key.
        Action<int, MovieTexture> downloadComplete = (key, download) =>
        {
            downloads.Add(key, download);

            // Check if we have all the downloads we were expecting
            if (downloads.Count == urls.Length)
            {
                // Return downloaded data to the caller
                callback(downloads.ToArrayValues());
            }
        };

        #endregion

        // Lookup list used to identify and correctly sort each download in the output collection
        var indices = new Dictionary<string, int> { };

        // Download all resources
        foreach (var url in urls)
        {

            // Add an indexing key for this url
            indices.Add(url, indices.Count);

            DownloadMovieTexture(gameObject, url,
                (param) =>
                {
                    // Get the index of this url
                    var index = 0;
                    indices.TryGetValue(url, out index);

                    // Add the download to the collection
                    downloadComplete(index, param);
                });
        }
    }

    /// <summary>
    /// Use this extension method to download a resource from a given URL.
    /// It will attach a temporary wwwDowloader component to the target game object that will be destroyed once the download is complete.    
    /// </summary>    
    /// <param name="gameObject">Parent GameObject used to 'host' a temporary instance of the wwwDownloader class.</param>
    /// <param name="url">URL of the resource to be downloaded.</param>
    /// <param name="callback">Method to be called once the download is complete.</param>    
    public static void DownloadAudioClip(this GameObject gameObject, string url, Action<AudioClip> callback)
    {
        var downloader = (wwwDownloader)gameObject.AddComponent<wwwDownloader>();

        downloader.DownloadAudioClip(url,
            (param) =>
            {
                // Destroy the downloader component
                GameObject.Destroy(downloader);

                // Return downloaded data to the caller
                callback(param);
            });
    }
    
    /// <summary>
    /// Use this extension method to download a resource from a given URL list.
    /// It will attach a temporary wwwDowloader component to the target game object that will be destroyed once the download is complete.    
    /// </summary>    
    /// <param name="gameObject">Parent GameObject used to 'host' a temporary instance of the wwwDownloader class.</param>
    /// <param name="urls">URLs of the resources to be downloaded.</param>
    /// <param name="callback">Method to be called once the download is complete for all resources.</param>
    /// <remarks>
    /// If the supplied parameters are invalid, value ruturned by the callback method will be <see cref="null"/>.
    /// </remarks>
    public static void DownloadAudioClip(this GameObject gameObject, string[] urls, Action<AudioClip[]> callback)
    {
        // Check
        if (callback == null)
        { Debug.LogError("DownloadAudioClip : callback is null."); }

        if (gameObject == null)
        { callback(null); }

        // Init
        var downloads = new SortedDictionary<int, AudioClip> { };

        #region >>> Actions

        // This method will add every download to the collection using the right indexing key.
        Action<int, AudioClip> downloadComplete = (key, download) =>
        {
            downloads.Add(key, download);

            // Check if we have all the downloads we were expecting
            if (downloads.Count == urls.Length)
            {
                // Return downloaded data to the caller
                callback(downloads.ToArrayValues());
            }
        };

        #endregion

        // Lookup list used to identify and correctly sort each download in the output collection
        var indices = new Dictionary<string, int> { };

        // Download all resources
        foreach (var url in urls)
        {

            // Add an indexing key for this url
            indices.Add(url, indices.Count);

            DownloadAudioClip(gameObject, url,
                (param) =>
                {
                    // Get the index of this url
                    var index = 0;
                    indices.TryGetValue(url, out index);

                    // Add the download to the collection
                    downloadComplete(index, param);
                });
        }
    }
    
    /// <summary>
    /// Use this extension method to download a resource from a given URL.
    /// It will attach a temporary wwwDowloader component to the target game object that will be destroyed once the download is complete.    
    /// </summary>    
    /// <param name="gameObject">Parent GameObject used to 'host' a temporary instance of the wwwDownloader class.</param>
    /// <param name="url">URL of the resource to be downloaded.</param>
    /// <param name="callback">Method to be called once the download is complete.</param>    
    public static void DownloadBytes(this GameObject gameObject, string url, Action<byte[]> callback)
    {
        var downloader = (wwwDownloader)gameObject.AddComponent<wwwDownloader>();

        downloader.DownloadBytes(url,
            (param) =>
            {
                // Destroy the downloader component
                GameObject.Destroy(downloader);

                // Return downloaded data to the caller
                callback(param);
            });
    }

    /// <summary>
    /// Use this extension method to download a resource from a given URL list.
    /// It will attach a temporary wwwDowloader component to the target game object that will be destroyed once the download is complete.    
    /// </summary>    
    /// <param name="gameObject">Parent GameObject used to 'host' a temporary instance of the wwwDownloader class.</param>
    /// <param name="urls">URLs of the resources to be downloaded.</param>
    /// <param name="callback">Method to be called once the download is complete for all resources.</param>
    /// <remarks>
    /// If the supplied parameters are invalid, value ruturned by the callback method will be <see cref="null"/>.
    /// </remarks>
    public static void DownloadBytes(this GameObject gameObject, string[] urls, Action<byte[][]> callback)
    {
        // Check
        if (callback == null)
        { Debug.LogError("DownloadBytes : callback is null."); }

        if (gameObject == null)
        { callback(null); }

        // Init
        var downloads = new SortedDictionary<int, byte[]> { };

        #region >>> Actions

        // This method will add every download to the collection using the right indexing key.
        Action<int, byte[]> downloadComplete = (key, download) =>
        {
            downloads.Add(key, download);

            // Check if we have all the downloads we were expecting
            if (downloads.Count == urls.Length)
            {
                // Return downloaded data to the caller
                callback(downloads.ToArrayValues());
            }
        };

        #endregion

        // Lookup list used to identify and correctly sort each download in the output collection
        var indices = new Dictionary<string, int> { };

        // Download all resources
        foreach (var url in urls)
        {

            // Add an indexing key for this url
            indices.Add(url, indices.Count);

            DownloadBytes(gameObject, url,
                (param) =>
                {
                    // Get the index of this url
                    var index = 0;
                    indices.TryGetValue(url, out index);

                    // Add the download to the collection
                    downloadComplete(index, param);
                });
        }
    }
    
    /// <summary>
    /// Use this extension method to download a resource from a given URL.
    /// It will attach a temporary wwwDowloader component to the target game object that will be destroyed once the download is complete.    
    /// </summary>    
    /// <param name="gameObject">Parent GameObject used to 'host' a temporary instance of the wwwDownloader class.</param>
    /// <param name="url">URL of the resource to be downloaded.</param>
    /// <param name="callback">Method to be called once the download is complete.</param>    
    public static void DownloadText(this GameObject gameObject, string url, Action<string> callback)
    {
        var downloader = (wwwDownloader)gameObject.AddComponent<wwwDownloader>();

        downloader.DownloadText(url,
            (param) =>
            {
                // Destroy the downloader component
                GameObject.Destroy(downloader);

                // Return downloaded data to the caller
                callback(param);
            });
    }
    
    /// <summary>
    /// Use this extension method to download a resource from a given URL list.
    /// It will attach a temporary wwwDowloader component to the target game object that will be destroyed once the download is complete.    
    /// </summary>    
    /// <param name="gameObject">Parent GameObject used to 'host' a temporary instance of the wwwDownloader class.</param>
    /// <param name="urls">URLs of the resources to be downloaded.</param>
    /// <param name="callback">Method to be called once the download is complete for all resources.</param>
    /// <remarks>
    /// If the supplied parameters are invalid, value ruturned by the callback method will be <see cref="null"/>.
    /// </remarks>
    public static void DownloadText(this GameObject gameObject, string[] urls, Action<string[]> callback)
    {
        // Check
        if (callback == null)
        { Debug.LogError("DownloadText : callback is null."); }

        if (gameObject == null)
        { callback(null); }

        // Init
        var downloads = new SortedDictionary<int, string> { };

        #region >>> Actions

        // This method will add every download to the collection using the right indexing key.
        Action<int, string> downloadComplete = (key, download) =>
        {
            downloads.Add(key, download);

            // Check if we have all the downloads we were expecting
            if (downloads.Count == urls.Length)
            {
                // Return downloaded data to the caller
                callback(downloads.ToArrayValues());
            }
        };

        #endregion

        // Lookup list used to identify and correctly sort each download in the output collection
        var indices = new Dictionary<string, int> { };

        // Download all resources
        foreach (var url in urls)
        {

            // Add an indexing key for this url
            indices.Add(url, indices.Count);

            DownloadText(gameObject, url,
                (param) =>
                {
                    // Get the index of this url
                    var index = 0;
                    indices.TryGetValue(url, out index);

                    // Add the download to the collection
                    downloadComplete(index, param);
                });
        }
    }

    #endregion

    #region >>>> Generic Methods

    /// <summary>
    /// Use this extension method to download resources from a given URL.
    /// It will attach a temporary wwwDowloader component to the target game object that will be destroyed once the download is complete.    
    /// </summary>
    /// <typeparam name="T">
    /// Supported Types :
    /// 
    /// AssetBundle - to download WWW.assetBundle
    /// AudioClip - to download WWW.audioClip
    /// Object - to download WWW.bytes (once the download is complete cast returned value to a byte array)
    /// Texture - to download WWW.texture
    /// MovieTexture - to download WWW.movie
    /// String - to download WWW.text
    /// 
    /// </typeparam>
    /// <param name="gameObject">Parent GameObject used to 'host' a temporary instance of the wwwDownloader class.</param>
    /// <param name="url">URL of the resource to be downloaded.</param>
    /// <param name="callback">Method to be called once the download is complete.</param>    
    public static void Download<T>(this GameObject gameObject, string url, Action<T> callback)
    {
        var downloader = (wwwDownloader)gameObject.AddComponent<wwwDownloader>();

        downloader.Download<T>(url,
            (param) =>
            {
                // Destroy the downloader component
                GameObject.Destroy(downloader);

                // Return downloaded data to the caller
                callback(param);
            });
    }

    #endregion

    #endregion

    #region >>>> Dates / Time

    /// <summary>
    /// 
    /// </summary>    
    /// <returns></returns>
    public static DateTime ToDateTimeFromHours(this float hours)
    {
        return ToDateTimeFromHours((double)hours);
    }

    /// <summary>
    /// 
    /// </summary>    
    /// <returns></returns>
    public static DateTime ToDateTimeFromMinutes(this float minutes)
    {
        return ToDateTimeFromMinutes((double)minutes);
    }

    /// <summary>
    /// 
    /// </summary>    
    /// <returns></returns>
    public static DateTime ToDateTimeFromSeconds(this float seconds)
    {
        return ToDateTimeFromSeconds((double)seconds);
    }

    /// <summary>
    /// 
    /// </summary>    
    /// <returns></returns>
    public static DateTime ToDateTimeFromMilliSeconds(this float milliseconds)
    {
        return ToDateTimeFromMilliSeconds((double)milliseconds);
    }

    /// <summary>
    /// 
    /// </summary>    
    /// <returns></returns>
    public static DateTime ToDateTimeFromHours(this double hours)
    {
        var date = DateTime.Today;
        var time = TimeSpan.FromHours(hours);

        return new DateTime(date.Year, date.Month, date.Day, (int)time.Hours, (int)time.Minutes, (int)time.Seconds, (int)time.Milliseconds);
    }

    /// <summary>
    /// 
    /// </summary>    
    /// <returns></returns>
    public static DateTime ToDateTimeFromMinutes(this double minutes)
    {
        var date = DateTime.Today;
        var time = TimeSpan.FromMinutes(minutes);

        return new DateTime(date.Year, date.Month, date.Day, (int)time.Hours, (int)time.Minutes, (int)time.Seconds, (int)time.Milliseconds);
    }

    /// <summary>
    /// 
    /// </summary>    
    /// <returns></returns>
    public static DateTime ToDateTimeFromSeconds(this double seconds)
    {
        var date = DateTime.Today;
        var time = TimeSpan.FromSeconds(seconds);

        return new DateTime(date.Year, date.Month, date.Day, (int)time.Hours, (int)time.Minutes, (int)time.Seconds, (int)time.Milliseconds);
    }

    /// <summary>
    /// 
    /// </summary>    
    /// <returns></returns>
    public static DateTime ToDateTimeFromMilliSeconds(this double milliseconds)
    {
        var date = DateTime.Today;
        var time = TimeSpan.FromMilliseconds(milliseconds);

        return new DateTime(date.Year, date.Month, date.Day, (int)time.Hours, (int)time.Minutes, (int)time.Seconds, (int)time.Milliseconds);
    }

    #endregion

}


#region >>>> Additional Utility Types

/// <summary>
/// Utility class used by the various Dowload...() extension methods to download resources.
/// </summary>
public class wwwDownloader : MonoBehaviour
{

    #region >>>> Typed Methods

    /// <summary>
    /// Call this method to start downloading a resource.
    /// </summary>        
    public void DownloadAssetBundle(string url, Action<AssetBundle> callback)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError(@"DownloadAssetBundle : url is null or empty.");

            // Return a default value of whichever type was supplied to this method
            callback(null);
        }

        // Start the downloader
        StartCoroutine(DownloadAssetBundleWorker(url, callback));
    }

    /// <summary>
    /// Worker for <see cref="DownloadAssetBundle"/>.
    /// </summary>    
    /// <param name="url"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator DownloadAssetBundleWorker(string url, Action<AssetBundle> callback)
    {
        // Download the asset
        var www = new WWW(url);

        // Wait for download to complete            
        yield return www;

        // Return downloaded resource
        callback(www.assetBundle);
    }
    
    /// <summary>
    /// Call this method to start downloading a resource.
    /// </summary>        
    public void DownloadAudioClip(string url, Action<AudioClip> callback)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError(@"DownloadAudioClip : url is null or empty.");

            // Return a default value of whichever type was supplied to this method
            callback(null);
        }

        // Start the downloader
        StartCoroutine(DownloadAudioClipWorker(url, callback));
    }

    /// <summary>
    /// Worker for <see cref="DownloadAudioClip"/>.
    /// </summary>    
    /// <param name="url"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator DownloadAudioClipWorker(string url, Action<AudioClip> callback)
    {
        // Download the asset
        var www = new WWW(url);

        // Wait for download to complete            
        yield return www;

        // Return downloaded resource
        callback(www.audioClip);
    }
    
    /// <summary>
    /// Call this method to start downloading a resource.
    /// </summary>        
    public void DownloadBytes(string url, Action<byte[]> callback)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError(@"DownloadBytes : url is null or empty.");

            // Return a default value of whichever type was supplied to this method
            callback(null);
        }

        // Start the downloader
        StartCoroutine(DownloadBytesWorker(url, callback));
    }

    /// <summary>
    /// Worker for <see cref="DownloadBytes"/>.
    /// </summary>    
    /// <param name="url"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator DownloadBytesWorker(string url, Action<byte[]> callback)
    {
        // Download the asset
        var www = new WWW(url);

        // Wait for download to complete            
        yield return www;

        // Return downloaded resource
        callback(www.bytes);
    }
    
    /// <summary>
    /// Call this method to start downloading a resource.
    /// </summary>        
    public void DownloadMovieTexture(string url, Action<MovieTexture> callback)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError(@"DownloadMovieTexture : url is null or empty.");

            // Return a default value of whichever type was supplied to this method
            callback(null);
        }

        // Start the downloader
        StartCoroutine(DownloadMovieTextureWorker(url, callback));
    }

    /// <summary>
    /// Worker for <see cref="DownloadMovieTexture"/>.
    /// </summary>    
    /// <param name="url"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator DownloadMovieTextureWorker(string url, Action<MovieTexture> callback)
    {
        // Download the asset
        var www = new WWW(url);

        // Wait for download to complete            
        yield return www;

        // Return downloaded resource
        callback(www.movie);
    }
    
    /// <summary>
    /// Call this method to start downloading a resource.
    /// </summary>        
    public void DownloadTexture2D(string url, Action<Texture2D> callback)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError(@"DownloadTexture2D : url is null or empty.");

            // Return a default value of whichever type was supplied to this method
            callback(null);
        }

        // Start the downloader
        StartCoroutine(DownloadTexture2DWorker(url, callback));
    }

    /// <summary>
    /// Worker for <see cref="DownloadTexture2D"/>.
    /// </summary>    
    /// <param name="url"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator DownloadTexture2DWorker(string url, Action<Texture2D> callback)
    {
        // Download the asset
        var www = new WWW(url);

        // Wait for download to complete            
        yield return www;

        // Return downloaded resource
        callback(www.texture);
    }
    
    /// <summary>
    /// Call this method to start downloading a resource.
    /// </summary>        
    public void DownloadText(string url, Action<string> callback)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError(@"DownloadText : url is null or empty.");

            // Return a default value of whichever type was supplied to this method
            callback(null);
        }

        // Start the downloader
        StartCoroutine(DownloadTextWorker(url, callback));
    }

    /// <summary>
    /// Worker for <see cref="DownloadText"/>.
    /// </summary>    
    /// <param name="url"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator DownloadTextWorker(string url, Action<string> callback)
    {
        // Download the asset
        var www = new WWW(url);

        // Wait for download to complete            
        yield return www;

        // Return downloaded resource
        callback(www.text);
    }
    
    #endregion 

    #region >>>> Generic Methods
    
    /// <summary>
    /// Call this method to start downloading a resource.
    /// </summary>        
    public void Download<T>(string url, Action<T> callback)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError(@"Download : url is null or empty.");

            // Return a default value of whichever type was supplied to this method
            callback(default(T));
        }

        // Start the downloader
        StartCoroutine(DownloadResource<T>(url, callback));
    }

    /// <summary>
    /// Worker for <see cref="Download<T>"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator DownloadResource<T>(string url, Action<T> callback)
    {
        object download;

        // Get name of the asset type we are going to download
        var assetType = typeof(T).ToString();

        // Download the asset
        var www = new WWW(url);

        // Wait for download to complete            
        yield return www;

        // Retrieve correct asset type from the downloader based on the type's name
        switch (assetType)
        {
            case "UnityEngine.AssetBundle":
                download = www.assetBundle;
                break;

            case "UnityEngine.AudioClip":
                download = www.audioClip;
                break;

            case "System.Object":
                download = www.bytes;
                break;

            case "UnityEngine.MovieTexture":
                download = www.movie;
                break;

            case "UnityEngine.Texture2D":
                download = www.texture;
                break;

            case "System.String":
                download = www.text;
                break;

            default:
                download = default(T);
                break;
        }

        // Return downloaded asset
        if (download != null)
        { callback((T)download); }
        else
        { callback(default(T)); }
    }

    #endregion
        
}

#endregion
