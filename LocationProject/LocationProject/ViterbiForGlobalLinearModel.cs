﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationProject
{
    class ViterbiForGlobalLinearModel
    {
        public WeightVector WeightVector { get; set; }
        public Tags Tags { get; set; }
        public List<Dictionary<string, float>> Pi { get; set; }
        public List<Dictionary<string, string>> Bp { get; set; }

        public ViterbiForGlobalLinearModel(WeightVector weightVector, Tags tags)
        {
            WeightVector = weightVector;
            Tags = tags;
            Pi = new List<Dictionary<string, float>>();
            Bp = new List<Dictionary<string, string>>();
        }

        public List<string> Decode(List<string> inputSentance)
        {
            var outputTags = new string[(inputSentance.Count)];
            var weightedFeatureSum  = new WeightedFeatureSum(WeightVector, inputSentance);
            var init = new Dictionary<string, float> {{"*:*", 0}};
            Pi.Add(init);
            var lastTwo = string.Empty;
            float lastTwoTagsValue = -0xFFFF;
            int k;
            
            for (k = 0; k < inputSentance.Count; k++ )
            {
                float max = - 0xFFFF;
                Pi.Add(new Dictionary<string, float>());
                Bp.Add(new Dictionary<string, string>());
                foreach (var tagStr in Tags.GetNGramTags(k == 0 ? 1 : 2))
                {
                    // follow algo from notes;
                    var tagsKey = tagStr;
                    float current;
                    if (k > 1)
                    {
                        var split = tagStr.Split(new char[] {':'});
                        foreach (var t in Tags.GetNGramTags(1))
                        {
                            var  newTemp = t+ ":"+ tagsKey;
                            Initialize(k - 1, t + ":" + split[0]);
                            var newWeight = weightedFeatureSum.GetFeatureValue(newTemp, k);
                            current = Pi[k - 1][t + ":" + split[0]] + newWeight;
                            if (current > max)
                            {
                                max = current;
                                outputTags[k] = split[1];
                            }
                            Initialize(k, tagsKey);
                            if (!(current > Pi[k][tagsKey])) continue;
                            Pi[k][tagsKey] = current;
                            Bp[k][tagsKey] = t;
                        }
                    }
                    else
                    {
                        if (k == 0)
                        {
                            tagsKey = "*:" + tagsKey;
                        }
                        var split = tagsKey.Split(new char[]{':'});
                        var newTemp = "*:" + tagsKey;
                        current = weightedFeatureSum.GetFeatureValue(newTemp, k);
                        Initialize(k, tagsKey);
                        if (current > Pi[k][tagsKey])
                        {
                            Pi[k][tagsKey] = current;
                            Bp[k][tagsKey] = "*";
                        }
                        if (current > max)
                        {
                            max = current;
                            outputTags[k] = split[1];
                        }
                    }
                    if (k != inputSentance.Count - 1) continue;
                    //var temp = tagsKey + ":STOP";
                    //current = Pi[k][tagsKey] + weightedFeatureSum.GetFeatureValue(temp, k + 1);
                    current = Pi[k][tagsKey];
                    if (!(current >= lastTwoTagsValue)) continue;
                    lastTwo = tagsKey;
                    lastTwoTagsValue = current;
                }
            }
            var n = inputSentance.Count - 1;
            var lastTwoSplit = lastTwo.Split(new char[] {':'});
            if (lastTwoSplit.Count() != 2)
            {
                throw new Exception("count mismatch for lastTwo tags"+ lastTwo);
            }
            outputTags[n-1] = lastTwoSplit[0];
            outputTags[n] = lastTwoSplit[1];

            //for (k = n - 2; k >= 0; k--)
            //{
            //    outputTags[k] = Bp[k + 2][outputTags[k + 1] + ":" + outputTags[k + 2]];
            //}
            return outputTags.ToList();
        }

        void Initialize(int k, string key)
        {
            if (!Pi[k].ContainsKey(key))
            {
                Pi[k].Add(key, -0xFFFF);
            }

            if (!Bp[k].ContainsKey(key))
            {
                Bp[k].Add(key, "");
            }
        }
    }
}
