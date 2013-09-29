using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationProjectWithFeatureTemplate
{
    public class FeatureCache
    {
        public FeatureCache()
        {
            
        }

        public double GetFeatureK(string tags, int sententIndex, int featureIndex, int lineIndex)
        {
            var features = new Features("*", prevTag, tag, line, pos);
            foreach (var feature in features.GetFeatures())
            {
                if (WeightVector.GetFeatureToK(feature) == k)
                {
                    return Math.Exp(WeightVector.Get(k));
                }
            }
            return 0;
        }
    }
}
