using System;
using System.Collections.Generic;
using System.IO;

namespace LocationProjectWithFeatureTemplate
{
    class MapFeaturesToK
    {
        private readonly string _inputFile;
        private readonly List<string> _tagList;
        public Dictionary<string, int> DictFeaturesToK;
        public Dictionary<int, string> DictKToFeatures;
        public int FeatureCount;
        private readonly WriteModel _writeModel;
        private readonly Tags _tags;

        public MapFeaturesToK(string inputFile, string outputFile, List<string> tagList)
        {
            _writeModel = new WriteModel(outputFile);

            _inputFile = inputFile;
            _tagList = tagList;
            DictFeaturesToK = new Dictionary<string, int>();
            DictKToFeatures = new Dictionary<int, string>();
            FeatureCount = 0;
            _tags = new Tags(tagList);
        }

        public void StartMapping()
        {
            var inputData = new ReadInputData(_inputFile);
            foreach (var line in inputData.GetSentence())
            {
                var inputTags = new List<string>(line.Count);
                for (var j = 0; j < line.Count; j++)
                {
                    var split = line[j].Split(new char[] {' '});
                    line[j] = split[0];
                    inputTags.Add(split[1]);
                }
                GenerateMappingForSentence(line);

            }
            inputData.Reset();
            foreach (var pair in DictFeaturesToK)
            {
                _writeModel.WriteLine(string.Format("{0}\t{1}", pair.Key, pair.Value));
            }
            _writeModel.Flush();
        }

        private void GenerateMappingForSentence(List<string> inputSentance)
        {
            for (int k = 0; k < inputSentance.Count; k++)
            {
                foreach (var tagStr in _tags.GetNGramTags(k == 0 ? 1 : 2))
                {
                    if (k > 1)
                    {
                        foreach (var t in _tags.GetNGramTags(1))
                        {
                            var newTemp = t + ":" + tagStr;
                            GenerateFeatures(newTemp, inputSentance, k);
                        }
                    }
                    else
                    {
                        var newTemp = tagStr;
                        if (k == 0)
                        {
                            newTemp = "*:" + newTemp;
                        }
                        newTemp = "*:" + newTemp;
                        GenerateFeatures(newTemp, inputSentance, k);
                    }
                }
            }
        }

        private void GenerateFeatures(string temp, List<string> inputSentance, int pos)
        {
            var tags = temp.Split(new char[] { ':' });
            if (tags.Length != 3)
            {
                throw new Exception(temp + " doesn't contain 3 tags");
            }
            var features = new Features(tags[0], tags[1], tags[2], inputSentance, pos);
            foreach (var feature in features.GetFeatures())
            {
                if (DictFeaturesToK.ContainsKey(feature))
                    continue;
                DictFeaturesToK.Add(feature, FeatureCount++);
                DictKToFeatures.Add(FeatureCount-1, feature);
            }
        }

        public int GetK(string feature)
        {
            if (DictFeaturesToK.ContainsKey(feature))
            {
                return DictFeaturesToK[feature];
            }
            return -1;
        }
    }
}
