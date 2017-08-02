using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parsing_the_json
{
    class Program
    {
        static public Tuple<string, double> Predict(QueryLabel y)
        {
            Tuple<string, double> answer=new Tuple<string, double>("",0);
            for (int i = 0; i < y.intents.Count(); i++)
                if (y.intents[i].score > answer.Item2)
                    answer = Tuple.Create(y.intents[i].intent,y.intents[i].score);

           return answer;
        }
        static public void Log(string s)
        {
            Console.WriteLine(s);
        }
       static public double EvaluateIntent(JArray json, List<Tuple<string, string>> exampleList, string intentName)
        {
            //exanmple list => list of groundtruth
            int falsePos = 0;
            int falseNeg = 0;
            int truePos = 0;
            int trueNeg = 0;
            int counter = 0;
            foreach (var groundTruth in exampleList)
            {
                var y = json[counter++].ToObject<QueryLabel>();
                var predictedExample = Predict(y);

                bool groundPos = (groundTruth.Item2 == intentName);
                bool predPos = (predictedExample.Item1 == intentName);

                if (groundPos && predPos)
                    ++truePos;
                if (!groundPos && predPos)
                    ++falsePos;
                if (groundPos && !predPos)
                    ++falseNeg;
                if (!groundPos && !predPos)
                    ++trueNeg;
            }

            double precision = (double)truePos / (double)(truePos + falsePos);
            double recall = (double)truePos / (double)(truePos + falseNeg);
            double F1 = 2.0 * ((precision * recall) / (precision + recall));

            Log("Intent = " + intentName);
            Log("falsePos = " + falsePos);
            Log("falseNeg = " + falseNeg);
            Log("truePos = " + truePos);
            Log("trueNeg = " + trueNeg);
            Log("precision = " + precision);
            Log("recall = " + recall);
            Log("F1 = " + F1 * 100.0 + "%\n");

            return F1;
        }
        static void Main(string[] args)
        {
            string PathOfJson = @"C:\Users\t-abradw\Desktop\f-score75.txt",//path of web response
                PathOfGroundTruth = @"C:\Users\t-abradw\Desktop\test1.tsv";

            string[] Intents = { "send_email", "send_text", "make_call", "find_contact" };//the intints
            List<Tuple<string, string>> ListOfGroundTruth=new List<Tuple<string, string>>();
            //load the file to the memory => load the groundtruth to the tuple
            string[] lines; // Array to hold all lines of the file
            lines = System.IO.File.ReadAllLines(PathOfGroundTruth);
            for (int i = 0; i < lines.Count(); i++)
            {
                string []token = lines[i].Split('\t');
                ListOfGroundTruth.Add(Tuple.Create(token[0],token[1]));
             }
            ////
            using (StreamReader r = new StreamReader(PathOfJson))
            {
                string json = r.ReadToEnd();
                var x = JArray.Parse(json);//array of queries
                for (int i = 0; i < Intents.Count(); i++)
                    Console.WriteLine("the f1 score of "+ Intents[i]+"is " + EvaluateIntent(x, ListOfGroundTruth, Intents[i]).ToString());
            }
        }
    }
}
