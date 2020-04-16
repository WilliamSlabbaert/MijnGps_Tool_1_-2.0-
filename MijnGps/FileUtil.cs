using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MijnGps
{

    class FileUtil
    {
        public static Dictionary<int, Graaf> grafenCache = new Dictionary<int, Graaf>();
        public static Dictionary<int, Gemeente> GemeenteCache = new Dictionary<int, Gemeente>();
        public static Dictionary<int, Straat> StreetCache = new Dictionary<int, Straat>();
        public static Dictionary<int, int> StreetGemeenteCache = new Dictionary<int, int>(); //juist
        public static List<int> vlaamseProcincies = new List<int>();
        public static List<int> GemeenteIDs = new List<int>();
        public static Boolean waitBuild = true;


        //Reading Directory
        public static List<String> readFileLines(String file)
        {
            List<String> list = new List<String>();
            using (var reader = new StreamReader(file))
            {
                while (!reader.EndOfStream)
                {
                    String line = reader.ReadLine();
                    list.Add(line);
                }
            }
            return list;
        }

        //reading
        public static void WRdataThread(List<String> lines)
        {
            Console.Write("Loading WRdata.....=> " + lines.Count + " ");
            lines.RemoveAt(0);
            foreach (String line in lines)
            {
                readWRdataLine(line);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("(DONE)");
            waitBuild = false;
            Console.WriteLine();
            Console.ResetColor();
        }
        public static void WRgemeenteIdsThread(List<String> lines)
        {
            Console.Write("Loading WRgemeenteIds.....=> " + lines.Count + " ");
            lines.RemoveAt(0);
            foreach (String line in lines)
            {
                readWRGemeenteIDLine(line);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("(DONE)");
            Console.WriteLine();
            Console.ResetColor();
        }
        public static void WRGemeentenaamThread(List<String> lines)
        {
            Console.Write("Loading WRgemeentenamen.....=> " + lines.Count + " ");
            lines.RemoveAt(0);
            foreach (String line in lines)
            {
                readWRGemeentenaamLine(line);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("(DONE)");
            Console.WriteLine();
            Console.ResetColor();
        }
        public static void ProvincieInfoThread(List<String> lines)
        {
            Console.Write("Loading ProvincieInfo.....=> " + lines.Count + " ");
            lines.RemoveAt(0);
            foreach (String line in lines)
            {
                readProvincieInfoLine(line);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("(DONE)");
            Console.WriteLine();
            Console.ResetColor();
        }
        public static void WRstraatnamenThread(List<String> lines)
        {
            Console.Write("Loading WRstraatnamen.....=> " + lines.Count + " ");
            lines.RemoveAt(0);
            foreach (String line in lines)
            {
                readWRstraatnamenLine(line);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("(DONE)");
            Console.WriteLine();
            Console.ResetColor();
        }
        public static void ProvincieIDsVlaanderenThread(List<String> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                readProvincieIDsVlaanderenLine(lines[i]);
            }
        }
        

        //decompose
        public static void readWRGemeenteIDLine(String line)
        {
            String[] data = line.Split(";");
            int ID = int.Parse(data[0]);
            if (!StreetGemeenteCache.ContainsKey(ID))
                StreetGemeenteCache.Add(ID, int.Parse(data[1]));
        }
        public static void readWRdataLine(String lines)
        {
            List<String> cache1 = new List<String>();
            List<String> cache2 = new List<String>();
            List<String> data = new List<String>();

            data = lines.Split(";").ToList();
            cache1 = (data[1].Replace("(", "").Replace(")", "").Replace("LINESTRING ", "")).Split(",").ToList();

            List<Punt> points = new List<Punt>();
            foreach (String pline in cache1)
            {
                String temp = pline;
                if (pline.StartsWith(" "))
                {
                    temp = pline.Substring(1);
                    cache2.Add(temp);
                }
            }
            foreach (String da in cache2)
            {
                Double x = Double.Parse(da.Split(" ")[0].Replace(".", ","));
                Double y = Double.Parse(da.Split(" ")[1].Replace(".", ","));
                Punt punt = new Punt(x, y);
                points.Add(punt);
            }

            Knoop start = new Knoop(int.Parse(data[4]), points.First());
            Knoop end = new Knoop(int.Parse(data[5]), points.Last());

            Segment segment = new Segment(int.Parse(data[0]), start, end, points);

            int straat1 = int.Parse(data[6]);
            int straat2 = int.Parse(data[7]);

            if (straat1 != -9)
                addGraaf(straat1, segment);
            if (straat2 != -9)
                addGraaf(straat2, segment);
        }
        public static void readWRGemeentenaamLine(String line)
        {
            String[] data = line.Split(";");
            int ID = int.Parse(data[1]);
            if (!GemeenteCache.ContainsKey(ID))
            {
                GemeenteCache.Add(ID, new Gemeente(ID, data[3]));
            }
        }
        public static void readProvincieInfoLine(String line)
        {
            String[] data = line.Split(";");

            if (data[2].ToLower() == "nl" && vlaamseProcincies.Contains(Int32.Parse(data[1])))
            {
                int ID = int.Parse(data[1]);

                if (!Program.Provincies.ContainsKey(ID))
                {
                    Program.Provincies.Add(ID, new Provincie(ID, data[3]));
                }
                
                int GemeenteID = int.Parse(data[0]);
                Program.Provincies[ID].addGemeente(GemeenteCache[GemeenteID]);
            }
            
        }
        public static void readWRstraatnamenLine(String line)
        {
            String[] data = line.Split(";");
            
            int ID = int.Parse(data[0]);
            if (!StreetCache.ContainsKey(ID))
            {
                StreetCache.Add(ID, new Straat(ID, data[1].Trim()));
            }
        }
        public static void addGraaf(int ID, Segment segment)
        {
            if (!grafenCache.ContainsKey(ID))
                grafenCache.Add(ID, new Graaf(ID));
            grafenCache[ID].addSegment(segment);
        }
        public static void BuildStreetsThread()
        {

            while (waitBuild)
                Thread.Sleep(25);

            Console.Write("Building streets.....=> " + StreetCache.Count + " ");

            //foreach (Straat street in StreetCache.Values)
            //{
            //    if (StreetGemeenteCache.ContainsKey(street.ID))
            //    {
            //        if (StreetGemeenteCache.ContainsKey(street.ID))
            //            GemeenteCache[StreetGemeenteCache[street.ID]].addStreet(StreetCache[street.ID]);
            //        if (grafenCache.ContainsKey(street.ID))
            //            StreetCache[street.ID].Graaf = grafenCache[street.ID];
            //    }

            //}

            foreach (int ids in StreetGemeenteCache.Keys)
            {
                if (grafenCache.ContainsKey(ids))
                    StreetCache[ids].graaf = grafenCache[ids];
                if(StreetCache.ContainsKey(ids))
                    GemeenteCache[StreetGemeenteCache[ids]].addStreet(StreetCache[ids]);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("(DONE)");
            Console.WriteLine();
            Console.ResetColor();

            //foreach (int ids in StreetGemeenteCache.Keys)
            //{
            //    if (GemeenteCache[StreetGemeenteCache[ids]].ID.Equals(ids))
            //        GemeenteCache[StreetGemeenteCache[ids]].addIds(StreetGemeenteCache[ids]);
            //    if (grafenCache.ContainsKey(ids))
            //        StreetCache[ids].Graaf = grafenCache[ids];
            //}
        }
        public static void readProvincieIDsVlaanderenLine(String line)
        {
            String[] data = line.Split(",");
            foreach (String s in data)
            {
                vlaamseProcincies.Add(int.Parse(s));
            }
        }
    }
}


