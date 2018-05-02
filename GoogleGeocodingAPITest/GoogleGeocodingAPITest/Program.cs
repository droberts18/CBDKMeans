using Accord.MachineLearning;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace GoogleGeocodingAPITest
{
    class Program
    {
        static string formatForUrl(string location)
        {
            location = location.Replace(" ", "+");
            return location;
        }

        static void runKMeans(DistanceObj[] groups)
        {
            int numGroups = groups.Length;
            // Declaring and intializing array for K-Means
            double[][] observations = new double[numGroups][];

            for (int i = 0; i < observations.Length; i++)
            {
                observations[i] = new double[2];
                observations[i][0] = groups[i].coords[0];
                observations[i][1] = groups[i].coords[1];
            }

            KMeans km = new KMeans(7);

            KMeansClusterCollection clusters = km.Learn(observations);

            int[] labels = clusters.Decide(observations);

            for (int i = 0; i < labels.Length; i++)
            {
                Console.WriteLine(groups[i].address + ": " + labels[i]);
            }
        }

        static void outputCsv(DistanceObj[] groups)
        {
            string filePath = @"C:\Users\droberts18\Desktop\DisIsForYouRyan.csv";
            string delimiter = ",";

            StringBuilder sb = new StringBuilder();
            List<string> CsvRow = new List<string>();

            CsvRow.Add("Address");
            CsvRow.Add("X-Coordinate Relative to WU");
            CsvRow.Add("Y-Coordinate Relative to WU");
            sb.AppendLine(string.Join(delimiter, CsvRow));
            CsvRow.Clear();

            foreach(DistanceObj g in groups)
            {
                CsvRow.Add(g.address);
                CsvRow.Add(g.coords[0].ToString().Replace(",", " "));
                CsvRow.Add(g.coords[1].ToString().Replace(",", " "));
                sb.AppendLine(string.Join(delimiter, CsvRow));
                CsvRow.Clear();
            }

            File.WriteAllText(filePath, sb.ToString());
        }

        static void Main(string[] args)
        {
            // Creating Whitworth as home
            DistanceObj home = new DistanceObj("300 W. Hawthorne Rd., Spokane, WA");

            // Reading in address and creating destination objects
            string filename = @"C:\Users\droberts18\Desktop\CBDExampleAddresses.csv";
            // Getting number of groups by reading number of lines in csv file
            var numGroups = File.ReadLines(filename).Count();

            DistanceObj[] groups = new DistanceObj[numGroups];

            using (var reader = new StreamReader(filename))
            {
                int currentGroupsIndex = 0;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    string curAddress = line.Replace("\"", "");
                    groups[currentGroupsIndex] = new DistanceObj(curAddress, home.longitude, home.latitude);
                    currentGroupsIndex++;
                }
            }

            outputCsv(groups);
        }
    }
}
