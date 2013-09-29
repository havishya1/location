﻿using System;
using System.Collections.Generic;

namespace LocationProjectWithFeatureTemplate
{
    class WeightVector
    {
        public readonly Dictionary<int, float> WDictionary;
        public Dictionary<string, int> FeatureKDictionary;

        public WeightVector()
        {
            WDictionary = new Dictionary<int, float>();
            FeatureKDictionary = new Dictionary<string, int>();
            throw new Exception("did you forget ???");
        }

        public WeightVector(Dictionary<string, int> inputFeatureTemp)
        {
            WDictionary = new Dictionary<int, float>();
            FeatureKDictionary = inputFeatureTemp;
        }
        
        public void Add(KeyValuePair<string, string> input)
        {
            if (FeatureKDictionary.ContainsKey(input.Key))
            {
                var k = FeatureKDictionary[input.Key];
                WDictionary.Add(k, float.Parse(input.Value));
            }
        }

        public float Get(string tag)
        {
            if (FeatureKDictionary.ContainsKey(tag))
            {
                return Get(FeatureKDictionary[tag]);
            }
            return 0;
        }

        public float Get(int k)
        {
            if (WDictionary.ContainsKey(k))
            {
                return WDictionary[k];
            }
            return 0;
        }

        public void AddToKey(string key, float value)
        {
            if (FeatureKDictionary.ContainsKey(key))
            {
                AddToKey(FeatureKDictionary[key], value);
            }
        }

        public void AddToKey(int key, float value)
        {
            if (!WDictionary.ContainsKey(key))
            {
                WDictionary.Add(key, 0);
            }
            WDictionary[key] += value;
        }



        //private float Abs(float value)
        //{
        //    if (value < 0)
        //    {
        //        value = -1 * value;
        //    }
        //    return value;
        //}
        //public void NormalizeAllWeights(int normalize = 10)
        //{
        //    var tagDictionary = new Dictionary<string, float>();
        //    foreach (var keyValuePair in WDictionary)
        //    {
        //        var tag = keyValuePair.Key.Substring(0, 
        //            keyValuePair.Key.IndexOf("TAG:", System.StringComparison.Ordinal));
        //        if (tagDictionary.ContainsKey(tag))
        //        {
        //            tagDictionary[tag] += Abs(keyValuePair.Value);
        //        }
        //        else
        //        {
        //            tagDictionary.Add(tag, Abs(keyValuePair.Value));
        //        }
        //    }


        //    List<string> keyList = WDictionary.Keys.ToList();
        //    foreach (var key in keyList)
        //    {
        //        var tag = key.Substring(0,
        //            key.IndexOf("TAG:", StringComparison.Ordinal));

        //        WDictionary[key] = (WDictionary[key] * normalize) /
        //            tagDictionary[tag];
        //    }
        //}
    }
}