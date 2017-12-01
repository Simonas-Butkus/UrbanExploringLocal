using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;
using System.Threading;
using System.Net;

namespace UrbanExploringLocalToDB
{
    class ReadLocal
    {
        ObjectInfo obj;
        string path = @"C:\Users\Simonas\Desktop\Apleisti";
        string subPath;
        List<string> Photos;
        List<string> Photos64;
        List<string> Streets;
        List<string> Streets64;

        public void GetFolders()
        {


            var directories = Directory.GetDirectories(path);
            foreach(var directory in directories)
            {
                if (Path.GetFileName(directory) != "Degsnio Dvaras")
                    continue;
                Photos = new List<string>();
                Photos64 = new List<string>();
                Streets = new List<string>();
                Streets64 = new List<string>();

                Debug.WriteLine(directory);
                obj = new ObjectInfo();
                subPath = directory;
                GetFiles(subPath);
                obj.Name = Path.GetFileName(directory);
                AddPhotos();

                new WriteToDb(obj);
            }
        }


        public void GetFiles(string subPath)
        {
            string[] files = Directory.GetFiles(subPath);

            foreach(var file in files)
            { 
                ProcessFile(file);
            }
        }


        private void  ProcessFile(string filePath)
        {

            string fileName = Path.GetFileName(filePath);



            StreamReader sr;
            switch (fileName)
            {
                case "info.txt":
                    sr = new StreamReader(filePath);
                    obj.Notes = sr.ReadToEnd();
                    break;
                case "istorija.txt":
                    sr = new StreamReader(filePath);
                    obj.Info = sr.ReadToEnd();
                    break;
                case "planas.txt":
                    //ProcessPlan(filePath); ToDo
                    break;
                case "maps.url":
                    using(WebClient client = new WebClient())
                    {
                        obj.MapsURL = client.DownloadString(filePath);
                    }
                    break;


            }

            if (Regex.IsMatch(fileName, @"(^maps.+)")){

                Byte[] bytes = File.ReadAllBytes(filePath);
                obj.MapsPhoto = Convert.ToBase64String(bytes);
            }

            if(Regex.IsMatch(fileName, @"(^ topo.+)"))
            {
                Byte[] bytes = File.ReadAllBytes(filePath);
                obj.TopoPhoto = Convert.ToBase64String(bytes);

            }

            if(Regex.IsMatch(fileName, @"(^satellite.+)"))
            {
                Byte[] bytes = File.ReadAllBytes(filePath);
                obj.SatellitePhoto = Convert.ToBase64String(bytes);

            }

            if(Regex.IsMatch(fileName, @"(^photo.+)"))
            {
                Photos.Add(filePath);
            }

            if(Regex.IsMatch(fileName, @"(^street.+)"))
            {
                Streets.Add(filePath);
            }

        }

        private void AddPhotos()
        {
            if (Photos.Count() > 0)
            {
                foreach (var photo in Photos)
                {

                    Byte[] bytes = File.ReadAllBytes(photo);
                    Photos64.Add(Convert.ToBase64String(bytes));
                }

                obj.Photos = Photos64;
            }


            if (Streets.Count() > 0)
            {
                foreach (var photo in Streets)
                {
                    Byte[] bytes = File.ReadAllBytes(photo);
                    Streets64.Add(Convert.ToBase64String(bytes));
                }

                obj.Streetview = Streets64;
            }

        }


        private void ProcessPlan(string filePath)
        {
            StreamReader sr = new StreamReader(filePath);
            List<string> lines = new List<string>();
            string line;
            while ((line = sr.ReadLine()) != null)
                lines.Add(line);
            DataTable table = new DataTable();
            var matches = Regex.Matches(lines[0], @"([^\t\n]+)");

            foreach (Match match in matches)
            {
                var word = match.Value;
                table.Columns.Add(word);
            }

            lines.RemoveAt(0);
            foreach (var Line in lines)
            { 

                List<string> row = new List<string>();
                var Matches = Regex.Matches(Line, @"[^\t\n]+");
                foreach (Match match in Matches)
                {


                    var word = match.Value;
                    if (!Regex.IsMatch(Matches[0].ToString(), @"\d"))
                    {

                        continue;
                    }
                    else
                    {

                        row.Add(match.ToString());

                    }
                }
                table.Rows.Add();


            }
            DebugTable(table);

            obj.Plan = table;
        }

        public void DebugTable(DataTable table)
        {
            Debug.WriteLine("--- DebugTable(" + table.TableName + ") ---");
            int zeilen = table.Rows.Count;
            int spalten = table.Columns.Count;

            // Header
            for (int i = 0; i < table.Columns.Count; i++)
            {
                string s = table.Columns[i].ToString();
                Debug.Write(String.Format("{0,-20} | ", s));
            }
            Debug.Write(Environment.NewLine);
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Debug.Write("---------------------|-");
            }
            Debug.Write(Environment.NewLine);

            // Data
            for (int i = 0; i < zeilen; i++)
            {
                DataRow row = table.Rows[i];
                //Debug.WriteLine("{0} {1} ", row[0], row[1]);
                for (int j = 0; j < spalten; j++)
                {
                    string s = row[j].ToString();
                    if (s.Length > 20) s = s.Substring(0, 17) + "...";
                    Debug.Write(String.Format("{0,-20} | ", s));
                }
                Debug.Write(Environment.NewLine);
            }
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Debug.Write("---------------------|-");
            }
            Debug.Write(Environment.NewLine);
        }
    }

}
