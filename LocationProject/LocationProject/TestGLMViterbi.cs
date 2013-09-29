using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationProject
{
    class TestGLMViterbi
    {
        private readonly string _outputTestFile;
        public string InputModelFile { get; set; }
        public string InputTestFile { get; set; }
        private WeightVector _weightVector;
        private Tags _tags;
        private ViterbiForGlobalLinearModel _viterbiForGlobalLinearModel;

        public TestGLMViterbi(string inputModelFile , string inputTestFile, string outputTestFile)
        {
            _outputTestFile = outputTestFile;
            InputModelFile = inputModelFile;
            InputTestFile = inputTestFile;
        }

        public void Setup()
        {
            var readModel = new ReadModel(InputModelFile);
            _weightVector = new WeightVector();

            foreach (var pair in readModel.ModelIterator())
            {
                _weightVector.Add(pair);
            }

            _tags = new Tags(new List<string> { "I-GENE", "O" });

            _viterbiForGlobalLinearModel = new ViterbiForGlobalLinearModel(_weightVector, _tags);

            // read input file in a class and per line iterator.
            var inputData = new ReadInputData(InputTestFile);
            var writeModel = new WriteModel(_outputTestFile);
            foreach (var line in inputData.GetSentence())
            {
                var outputTags = _viterbiForGlobalLinearModel.Decode(line);
                writeModel.WriteDataWithTag(line, outputTags);
            }
            writeModel.Flush();
        }
    }
}
