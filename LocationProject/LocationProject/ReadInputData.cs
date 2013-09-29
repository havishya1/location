﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationProject
{
    class ReadInputData
    {
        private readonly StreamReader _reader;
        public ReadInputData(string input)
        {
            _reader = new StreamReader(input);
            if (_reader == null)
                throw new Exception(input + "is invalid");
        }

        public IEnumerable<List<string>> GetSentence()
        {
            var sentence = new List<string>();
            string line = null;
            while ((line = _reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (string.IsNullOrEmpty(line))
                {
                    yield return sentence;
                    sentence.Clear();
                }
                else
                {
                    sentence.Add(line);
                }
            }
            if (sentence.Count > 0)
                yield return sentence;
        }

        internal void Reset()
        {
            _reader.BaseStream.Seek(0, SeekOrigin.Begin);
        }
    }
}
