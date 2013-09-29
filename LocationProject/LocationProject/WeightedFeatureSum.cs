using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationProject
{
    class WeightedFeatureSum
    {
        private readonly List<string> _sentence;
        public WeightVector WeightVector { get; set; }

        public WeightedFeatureSum(WeightVector weightVector, List<string> sentence)
        {
            _sentence = sentence;
            WeightVector = weightVector;
        }

        public float GetFeatureValue(string t2, string t1, string t, int pos)
        {
            var features = new Features(t2, t1, t, _sentence, pos);
            float sum = 0;
            foreach (var feature in features.GetFeatures())
            {
                sum += WeightVector.Get(feature);
            }
            return sum;
        }

        internal float GetFeatureValue(string temp, int k)
        {
            string[] tags = temp.Split(new char[] {':'});
            if (tags.Length != 3)
            {
                throw new Exception(temp +" doesn't contain 3 tags");
            }
            return GetFeatureValue(tags[0], tags[1], tags[2], k);
        }
    }
}
