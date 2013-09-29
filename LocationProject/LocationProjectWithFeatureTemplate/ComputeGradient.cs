using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationProjectWithFeatureTemplate
{
    class ComputeGradient
    {
        private readonly List<List<string>> _inputSentence;
        private readonly List<List<string>> _outputTagsList;
        private readonly List<string> _tagList;
        private readonly double _lambda;
        private List<ForwardBackwordAlgo> forwardBackwordAlgos;
        private WeightVector _weightVector;

        public ComputeGradient(List<List<string>> inputSentence, List<List<string>> tagsList,
            List<string> tagList, double lambda)
        {
            _inputSentence = inputSentence;
            _outputTagsList = tagsList;
            _tagList = tagList;
            _lambda = lambda;
            forwardBackwordAlgos = new List<ForwardBackwordAlgo>();
            _weightVector = new WeightVector();
        }

        private void SetForwardBackwordAlgo(WeightVector weightVector)
        {
            if (_inputSentence.Count != _outputTagsList.Count)
            {
                throw new Exception("counts dont match " + _inputSentence.Count + "with " + _outputTagsList.Count);
            }
            int counter = 0;
            forwardBackwordAlgos.Clear();
            foreach (var sentence in _inputSentence)
            {
                var outputTags = _outputTagsList[counter];
                forwardBackwordAlgos.Add(new ForwardBackwordAlgo(sentence, weightVector, outputTags));
                counter++;
            }
        }

        public WeightVector RunIterations(WeightVector weightVector, int iterationCount)
        {
            for (int iter = 0; iter < iterationCount; iter++)
            {
                var newWeightVector = new WeightVector(weightVector.FeatureKDictionary);
                SetForwardBackwordAlgo(weightVector);
                for (var k = 0; k < weightVector.FeatureKDictionary.Count; k++)
                {
                    var wk = Compute(k, weightVector);
                    wk = weightVector.Get(k) + _lambda*wk;
                    newWeightVector.SetKey(k, wk);
                }
                weightVector = newWeightVector;
            }
            _weightVector = weightVector;
            return weightVector;
        }

        public void Dump(string outputFile, Dictionary<int, string> dictKtoFeature)
        {
            Console.WriteLine("training is complete");
            var output = new WriteModel(outputFile);
            foreach (var weight in _weightVector.WDictionary)
            {
                output.WriteLine(string.Format("{0} {1} {2}", weight.Key,
                    dictKtoFeature[weight.Key], weight.Value));
            }
            output.Flush();
        }

        public double Compute(int k, WeightVector weightVector)
        {
            double output = 0;
            double secondTerm = 0;
            int i = 0;
            var weightedFeaturesum = new WeightedFeatureSum(weightVector, null, true);

            if (_inputSentence.Count != _outputTagsList.Count)
            {
                throw new Exception("counts dont match "+ _inputSentence.Count + "with "+ _outputTagsList.Count);
            }
            var ngramTags = new Tags(_tagList);

            // first term.
            foreach (var sentence in _inputSentence)
            {
                var outputTags = _outputTagsList[i];
                output += weightedFeaturesum.GetAllFeatureK(outputTags, k, sentence);

                // second term.
                for (var j = 0; j < outputTags.Count; j++)
                {
                    double sum = 0;
                    foreach (var ngramTag in ngramTags.GetNGramTags(2))
                    {
                        string[] split = ngramTag.Split(new[] {':'});
                        sum += (forwardBackwordAlgos[i].GetQ(j, split[0], split[1]) *
                            weightedFeaturesum.GetFeatureK(split[0], split[1], j, k, sentence));
                    }
                    secondTerm += sum;
                }
                i++;
            }

            output = output - secondTerm - (_lambda*weightVector.Get(k));
            return output;
        }

    }
}

