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
            _weightVector = null;
        }

        public void Dump(string outputFile, Dictionary<int, string> dictKtoFeature)
        {
            Console.WriteLine(DateTime.Now+" training is complete");
            var output = new WriteModel(outputFile);
            var sortedDictionary = from pair in _weightVector.WDictionary
                orderby Math.Abs(pair.Value) descending
                select pair;
            foreach (var weight in sortedDictionary)
            {
                output.WriteLine(string.Format("{0} {1} {2}", weight.Key,
                    dictKtoFeature[weight.Key], weight.Value));
            }
            output.Flush();
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
                if (sentence.Count != outputTags.Count)
                {
                    throw new Exception("counts dont match " + sentence.Count + "with " + outputTags.Count);
                }
                forwardBackwordAlgos.Add(new ForwardBackwordAlgo(sentence, weightVector, outputTags));
                counter++;
            }
        }

        public WeightVector RunIterations(WeightVector weightVector, int iterationCount)
        {
            for (int iter = 0; iter < iterationCount; iter++)
            {
                Console.WriteLine(DateTime.Now + " running iteration " + iter);
                var newWeightVector = new WeightVector(weightVector.FeatureKDictionary);
                SetForwardBackwordAlgo(weightVector);
                //for (var k = 0; k < weightVector.FeatureKDictionary.Count; k++)
                for (var k = weightVector.FeatureKDictionary.Count-1; k >= 0; k--)
                {
                    if (k%100 == 0)
                    {
                        Console.WriteLine(DateTime.Now + " running iteration for k " + k);
                    }
                    var wk = Compute(k, weightVector);
                    wk = weightVector.Get(k) + _lambda*wk;
                    newWeightVector.SetKey(k, wk);
                }
                weightVector = newWeightVector;
            }
            _weightVector = weightVector;
            return weightVector;
        }

        private double Compute(int k, WeightVector weightVector)
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

                if (sentence.Count != outputTags.Count)
                {
                    throw new Exception("compute counts dont match " + sentence.Count + "with " + outputTags.Count);
                }

                output += CalculateGradient(weightedFeaturesum, sentence, outputTags, k, ngramTags, i);

                //output += weightedFeaturesum.GetAllFeatureK(outputTags, k, sentence);

                //// second term.
                //for (var j = 0; j < outputTags.Count; j++)
                //{
                //    double sum = 0;
                //    foreach (var ngramTag in ngramTags.GetNGramTags(2))
                //    {
                //        string[] split = ngramTag.Split(new[] {':'});
                //        sum += (forwardBackwordAlgos[i].GetQ(j, split[0], split[1]) *
                //            weightedFeaturesum.GetFeatureK(split[0], split[1], j, k, sentence));
                //    }
                //    secondTerm += sum;
                //}
                i++;
            }

            output = output - secondTerm - (_lambda*weightVector.Get(k));
            return output;
        }

        private double CalculateGradient(WeightedFeatureSum weightedFeatureSum,
            List<string> sentence, List<string> outputTags,
            int k, Tags ngramTags, int i)
        {
            double output = 0;
            double secondTerm = 0;
            output += weightedFeatureSum.GetAllFeatureK(outputTags, k, sentence);

            // second term.
            for (var j = 0; j < outputTags.Count; j++)
            {
                //double sum = 0;
                secondTerm += GetSecondTerm(sentence, ngramTags, weightedFeatureSum, i, j, k);
                //foreach (var ngramTag in ngramTags.GetNGramTags(2))
                //{
                //    string[] split = ngramTag.Split(new[] { ':' });
                //    sum += (forwardBackwordAlgos[i].GetQ(j, split[0], split[1]) *
                //        weightedFeatureSum.GetFeatureK(split[0], split[1], j, k, sentence));
                //}
                //secondTerm += sum;
            }
            return output - secondTerm;
        }

        private double GetSecondTerm(List<string> sentence, Tags ngramTags, 
            WeightedFeatureSum weightedFeatureSum, int i, int j, int k)
        {
            double sum = 0;
            foreach (var ngramTag in ngramTags.GetNGramTags(2))
            {
                string[] split = ngramTag.Split(new[] { ':' });
                sum += (forwardBackwordAlgos[i].GetQ(j, split[0], split[1]) *
                    weightedFeatureSum.GetFeatureK(split[0], split[1], j, k, sentence));
            }
            return sum;
        }
    }
}

