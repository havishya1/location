using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationProject
{
    class ReadModel
    {
        private readonly string _fileName;

        public ReadModel(string file)
        {
            _fileName = file;
        }

        public IEnumerable<KeyValuePair<string, string>> ModelIterator()
        {
            StreamReader stream;
            try
            {
                stream = new StreamReader(_fileName);
            }
            catch (Exception)
            {
                Console.WriteLine(_fileName + "is not valid");
                throw;
            }
            string line;
            while ((line = stream.ReadLine()) != null)
            {
                string[] str = line.Split(new[]{' '});
                if (str.Length != 2)
                {
                    Console.WriteLine(line + " doesn't have 2 words");
                    throw new Exception();
                }
                else
                {
                    yield return new KeyValuePair<string, string> ( str[0], str[1] );   
                }
            }
        }
    }
}
