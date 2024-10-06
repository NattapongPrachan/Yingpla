using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;
namespace YGG{
public static class Utils{
        public static string GetTextFromFile(string filePath){
            var streamReader = new StreamReader(filePath);
            return streamReader.ReadToEnd();
        }
        public static void TrySetDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
        public static bool GetFileExist(string path)
        {
            return File.Exists(path);
        }
        public static async void WriteText(string filePath,string context)
        {
            Debug.Log("write text"+filePath);
            FileStream fileStream = new FileStream(filePath,FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.ReadWrite);
            StreamWriter writer = new StreamWriter(fileStream);
            fileStream.SetLength(0);
            await writer.WriteAsync(context);
            await writer.FlushAsync();
            writer.Close();
        }
        public static string[] GetTextFilesInDirectory(string path)
        {
            return Directory.GetFiles(path, "*.txt", SearchOption.TopDirectoryOnly);
        }
        public static void ClearScrollviewContent(Transform content){
            Debug.Log("CleareContent "+content.childCount);
            for (var i = 0; i < content.childCount; i++)
            {
                var obj = content.GetChild(i).gameObject;
                UnityEngine.GameObject.Destroy((GameObject)obj);
            }

        }
        public static string ConvertToDatetime(string timestamp){
            System.DateTime date = new System.DateTime();
            date.AddSeconds(System.Convert.ToUInt32(timestamp)).ToLocalTime();
            return date.ToLongDateString();
        }

        public static void AddEventTrigger(GameObject obj,EventTriggerType type,UnityAction<BaseEventData> action){
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener(action);
            trigger.triggers.Add(entry);
        }
        public static Dictionary<TKey, TValue> CloneDictionaryCloningValues<TKey, TValue>(Dictionary<TKey, TValue> original) where TValue : ICloneable
        {
            Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(original.Count,
                                                                    original.Comparer);
            foreach (KeyValuePair<TKey, TValue> entry in original)
            {
                ret.Add(entry.Key, (TValue) entry.Value.Clone());
            }
            return ret;
        }
        public static Texture2D PaintWaveformSpectrum(AudioClip audio, float saturation, int width, int height, Color col) {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float[] samples = new float[audio.samples];
            float[] waveform = new float[width];
            audio.GetData(samples, 0);
            int packSize = ( audio.samples / width ) + 1;
            Debug.Log("pack size "+packSize);
            Debug.Log("--> "+(packSize/audio.length));
            int s = 0;
            for (int i = 0; i < audio.samples; i += packSize) {
                waveform[s] = Mathf.Abs(samples[i]);
                s++;
            }
        
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    tex.SetPixel(x, y, Color.black);
                }
            }
        
            for (int x = 0; x < waveform.Length; x++) {
                for (int y = 0; y <= waveform[x] * ((float)height * .75f); y++) {
                    tex.SetPixel(x, ( height / 2 ) + y, col);
                    tex.SetPixel(x, ( height / 2 ) - y, col);
                }
            }
            tex.Apply();
    
        return tex;
    }
        public static Vector3 Get1PersonDirection(Transform targetTransform,Vector3 inputDir)
        {
            return targetTransform.right * inputDir.x + targetTransform.forward*inputDir.y;
        }
        public static Vector3 Get3PersonDirection(Transform cameraTransform,Vector3 inputDir)
        {
            var moveDirection = cameraTransform.transform.forward *  inputDir.z;
            moveDirection += cameraTransform.transform.right * inputDir.x;
            return moveDirection;
        }
    }
}
