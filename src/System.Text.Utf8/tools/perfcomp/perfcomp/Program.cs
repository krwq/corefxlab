using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace perfcomp
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string[]> output = new Dictionary<string, string[]>();

            for (int i = 0; i < args.Length; i++)
            {
                string path = args[i];

                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (XmlReader reader = XmlReader.Create(fs))
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.CDATA)
                            {
                                string resultsAsText = reader.Value;
                                string[] results = resultsAsText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string result in results)
                                {
                                    string[] parts = result.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (parts.Length != 3)
                                        throw new Exception("f'd up input");

                                    string[] moreParts = parts[2].Trim().Split(new string[] { " ", "(", ")", "Elapsed", "iterations" }, StringSplitOptions.RemoveEmptyEntries);
                                    if (moreParts.Length != 2)
                                        throw new Exception("f'd up input");
                                    string name = parts[0].Trim() + " : " + parts[1].Trim() + " : " + moreParts[1] + " iterations";
                                    if (name.Contains("ignore this"))
                                        continue;
                                    string value = moreParts[0];

                                    string[] partialResults;
                                    if (!output.TryGetValue(name, out partialResults))
                                    {
                                        partialResults = new string[args.Length];
                                    }

                                    partialResults[i] = value;
                                    output[name] = partialResults;
                                }
                            }
                        }
                    }
                }
            }

            foreach (var kp in output)
            {
                Console.WriteLine(kp.Key);
                Console.Write("\t" + string.Join("\t=>\t", kp.Value));
                if (args.Length == 2 && kp.Value[1] != "0ms")
                {
                    int old = int.Parse(kp.Value[0].Split(new string[] { "ms" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    int @new = int.Parse(kp.Value[1].Split(new string[] { "ms" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    Console.Write("\told/new={0,0}%", unchecked((int)(0.5 + 100.0 * (double)old / (double)@new)));
                }
                Console.WriteLine();
            }
        }
    }
}
