using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationProject
{
    class Program
    {
        static void Main(string[] args)
        {
            TrainingTest();
            Test1();

            //const string modelFile = "../../data/tag.model";
            //const string input = "../../data/gene.test";
            //const string outputFile = "../../data/gene_test.p2.out";

        }

        static void TrainingTest()
        {
            const string modelFile = "../../data/gene.key.model";
            const string input = "../../data/gene.key";
            var perceptron = new Perceptron(input, modelFile);
            perceptron.Train();
        }

        static void Test1()
        {
            const string input = "../../data/gene.dev";
            const string outputFile = "../../data/gene_dev.output3";
            const string modelFile = "../../data/gene.key.model";

            var testGLMViterbi = new TestGLMViterbi(modelFile, input, outputFile);
            testGLMViterbi.Setup();
        }

/*
        static void Test()
        {
            const string inputFile = "../../data/tag.model";
            //const string outputFile = "../../test.output1";
            var readModel = new ReadModel(inputFile);
            //var writeModel = new WriteModel(outputFile);
            var weightVector = new WeightVector();
            var tags = new List<string> {"I-GENE", "O"};

            PrintFeatureList(tags);

            foreach (var pair in readModel.ModelIterator())
            {
                weightVector.Add(pair);
                
            }
            //writeModel.WriteLine(line);
            //writeModel.Flush();
        }
*/

        static void PrintFeatureList(List<string> tags)
        {
            var featureTags = new Tags(tags);
            featureTags.Dump(3);
        }
    }
}
