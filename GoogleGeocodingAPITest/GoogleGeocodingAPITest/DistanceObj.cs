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
    class DistanceObj
    {
        public string address { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double[] coords { get; set; }

        // used for creating home location
        public DistanceObj(string address)
        {
            this.address = address;
            retrieveLatAndLong();
            // origin
            coords = new double[] { 0, 0 };
        }

        // used for creating destinations
        public DistanceObj(string address, double homeLong, double homeLat)
        {
            this.address = address;
            retrieveLatAndLong();
            coords = new double[2];
            calculateCoords(homeLong, homeLat);
        }

        private void retrieveLatAndLong()
        {
            try
            {
                string geocodingApiUrl = ConfigurationManager.AppSettings["GoogleGeocodingApi"];
                string apiKey = ConfigurationManager.AppSettings["ApiKey"];

                string addressUrl = formatForUrl(address);

                geocodingApiUrl += addressUrl + "&key=" + apiKey;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(geocodingApiUrl);
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader sreader = new StreamReader(dataStream);
                string responseReader = sreader.ReadToEnd();
                response.Close();
                DataSet ds = new DataSet();
                ds.ReadXml(new XmlTextReader(new StringReader(responseReader)));

                latitude = Convert.ToDouble(ds.Tables["location"].Rows[0].ItemArray[0]);
                longitude = Convert.ToDouble(ds.Tables["location"].Rows[0].ItemArray[1]);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating location's latitude & longitude: " + ex.Message);
            }
        }

        private string formatForUrl(string rawAddress)
        {
            string urlAddress = rawAddress.Replace(" ", "+");
            return urlAddress;
        }

        public void calculateCoords(double homeLong, double homeLat)
        {
            // Dx - Wx
            double xDist = longitude - homeLong;

            //Dy - Wy
            double yDist = latitude - homeLat;

            // Calculating hypotenuse of triangle
            // Square root of xDist^2 + yDist^2
            double hypotenuse = Math.Sqrt(Math.Pow(xDist, 2) + Math.Pow(yDist, 2));

            // Calculating xCoord
            coords[0] = xDist / hypotenuse;

            // Calculating yCoord
            coords[1] = yDist / hypotenuse;
        }

        public void outputCoords()
        {
            Console.WriteLine("(" + coords[0] + ", " + coords[1] + ")\n");
        }
    }
}
