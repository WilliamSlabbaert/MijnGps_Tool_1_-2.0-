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
    class Program
    {
        public static Dictionary<int, Provincie> Provincies = new Dictionary<int, Provincie>();
        public static List<Punt> points = new List<Punt>();
        public static Dictionary<int, Gemeente> Cities = new Dictionary<int, Gemeente>();

        public static String WRdata = @"C:\Users\willi\Desktop\WRdata-master\WRdata.csv";
        public static String WRGemeenteID = @"C:\Users\willi\Desktop\WRdata-master\WRGemeenteID.csv";
        public static String WRGemeentenaam = @"C:\Users\willi\Desktop\WRdata-master\WRGemeentenaam.csv";
        public static String WRstraatnamen = @"C:\Users\willi\Desktop\WRdata-master\WRstraatnamen.csv";
        public static String ProvincieInfo = @"C:\Users\willi\Desktop\WRdata-master\ProvincieInfo.csv";
        public static String ProvincieID = @"C:\Users\willi\Desktop\WRdata-master\ProvincieIDsVlaanderen.csv";

        //public static List<String> WRdatalines = FileUtil.readFileLines(WRdata);
        //public static List<String> WRGemeenteIDlines = FileUtil.readFileLines(WRGemeenteID);
        //public static List<String> WRGemeentenaamlines = FileUtil.readFileLines(File.ReadAllLines(WRGemeentenaam));
        //public static List<String> WRstraatnamenlines = FileUtil.readFileLines(File.ReadAllLines(WRstraatnamen));
        //public static List<String> ProvincieInfolines = FileUtil.readFileLines(File.ReadAllLines(ProvincieInfo));

        static void Main(string[] args)
        {

            //Load WRdata
            Task ThreadWRdatalineslines = Task.Factory.StartNew(() => FileUtil.WRdataThread(FileUtil.readFileLines(WRdata)));
            ThreadWRdatalineslines.Wait();
            //Load ProvincieID
            Task ThreadProvincieID = Task.Factory.StartNew(() => FileUtil.ProvincieIDsVlaanderenThread(FileUtil.readFileLines(ProvincieID)));
            ThreadProvincieID.Wait();
            //Load WRstraatnamenlines
            Task ThreadWRstraatnamenlines = Task.Factory.StartNew(() => FileUtil.WRstraatnamenThread(FileUtil.readFileLines(WRstraatnamen)));
            //Load WRGemeenteNamen
            Task ThreadWRGemeentenaamlines = Task.Factory.StartNew(() => FileUtil.WRGemeentenaamThread(FileUtil.readFileLines(WRGemeentenaam)));
            //Load WRGemeenteIDS
            Task ThreadWRGemeenteIDlines = Task.Factory.StartNew(() => FileUtil.WRgemeenteIdsThread(FileUtil.readFileLines(WRGemeenteID)));

            Task.WaitAll(ThreadWRGemeenteIDlines, ThreadWRGemeentenaamlines, ThreadWRstraatnamenlines);

            //Load ProvincieInfolines
            Task ThreadProvincieInfolines = Task.Factory.StartNew(() => FileUtil.ProvincieInfoThread(FileUtil.readFileLines(ProvincieInfo)));

            Thread.Sleep(25);
            Task FinishingStreets = Task.Factory.StartNew(() => FileUtil.BuildStreetsThread());
            Task.WaitAll(ThreadProvincieInfolines, FinishingStreets);

            Task Export = Task.Factory.StartNew(() => ExportDataFiles());
            Export.Wait();


            Task printThread = Task.Factory.StartNew(() => Print());
            printThread.Wait();
        }
        public static void Print()
        {

            int GemCount = 0;
            int StrtCount = 0;
            foreach (Provincie pro in Provincies.Values)
            {
                //Console.WriteLine(pro);
                GemCount += pro.gemeentes.Count;
                foreach (Gemeente gem in pro.gemeentes)
                {
                    StrtCount += gem.straten.Count;
                }
            }


            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Total Steden => " + GemCount);
            Console.WriteLine("Total Straten => " + StrtCount);
            Console.WriteLine("Total Vlaamse Provincies => " + Provincies.Count);
            Console.WriteLine();
            foreach (Provincie pro in Provincies.Values)
            {
                Console.WriteLine(pro);
            }
            //List<double> lengte = new List<double> { };
            //foreach (Gemeente gem in FileUtil.GemeenteCache.Values)
            //{
            //    Console.WriteLine(gem.Name);

            //    foreach (Straat strt in gem.straten)
            //    {
            //        //Console.WriteLine(strt.graaf.getLength().Max());

            //        if (strt.graaf != null)
            //        {
            //            lengte.Add(strt.graaf.getLength());
            //            //        Console.WriteLine(strt.ID + " |name>" + strt.Name.ToString());
            //            //        Console.WriteLine("|Lengte>" + strt.graaf.getLength());
            //            //        List<Segment> li = new List<Segment> { };
            //            //        foreach (Knoop i in strt.graaf.map.Keys)
            //            //        {
            //            //            foreach (Segment s in strt.graaf.map[i])
            //            //            {
            //            //                if (!li.Contains(s))
            //            //                {
            //            //                    li.Add(s);
            //            //                }
            //            //            }
            //            //        }
            //            //        foreach (Segment seg in li)
            //            //        {
            //            //            Console.WriteLine(seg);
            //            //        }

            //            //        Console.WriteLine();
            //        }
            //    }
            //    Console.WriteLine(lengte.Max());
            //}
            Console.Write("Press ENTER to continue...");
            Console.ReadLine();
        }
        public static void ExportDataFiles()
        {
            string fileName = @"C:\Users\willi\Desktop\Rapport.txt";
            string fileName2 = @"C:\Users\willi\Desktop\Rapport2.txt";
            string DatafileName = @"C:\Users\willi\Desktop\DataRapportProvincie.csv";
            string DatafileName2 = @"C:\Users\willi\Desktop\DataRapportGemeente.csv";
            string DatafileName3 = @"C:\Users\willi\Desktop\DataRapportStraten.csv";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            if (File.Exists(DatafileName))
            {
                File.Delete(DatafileName);
            }
            if (File.Exists(DatafileName2))
            {
                File.Delete(DatafileName2);
            }
            if (File.Exists(DatafileName3))
            {
                File.Delete(DatafileName3);
            }

            int GemCount = 0;
            int StrtCount = 0;
            foreach (Provincie pro in Provincies.Values)
            {
                //Console.WriteLine(pro);
                GemCount += pro.gemeentes.Count;
                foreach (Gemeente gem in pro.gemeentes)
                {
                    StrtCount += gem.straten.Count;
                }
            }

            using (StreamWriter outputFile = new StreamWriter(fileName))
            {
                Console.WriteLine("Loading Data...");
                outputFile.WriteLine("Total Totale Steden => " + GemCount);
                outputFile.WriteLine("Total Totale Straten => " + StrtCount);
                outputFile.WriteLine("Total Vlaamse Provincies => " + Provincies.Count);
                foreach (Provincie pro in Provincies.Values)
                {
                    outputFile.WriteLine(pro);
                }

            }
            using (StreamWriter outputFile = new StreamWriter(fileName2))
            {
                outputFile.WriteLine("GemeenteName;MAX;MIN");
                foreach (Provincie pro in Provincies.Values)
                {
                    foreach (Gemeente gem in pro.gemeentes)
                    {
                        outputFile.Write(gem.Name.ToString());
                        List<double> lengte = new List<double> { };
                        foreach (Straat str in gem.straten)
                        {
                            if (str.graaf != null)
                            {
                                int ids = 0;
                                if (str.ID != null)
                                {
                                    ids = str.ID;
                                }
                                lengte.Add(str.graaf.getLength());
                            }
                        }
                        lengte.Sort();
                        lengte.RemoveAll(i => i == 0);
                        if (lengte.Count != 0)
                        {
                            outputFile.Write(";" + lengte[lengte.Count - 1]);
                            outputFile.Write(";" + lengte[0]);
                        }
                        else
                        {
                            outputFile.Write(";" + -0);
                            outputFile.Write(";" + -0);
                        }
                        outputFile.WriteLine();
                    }
                }
            }
            using (StreamWriter outputFile = new StreamWriter(DatafileName3))
            {

                FileUtil.StreetCache.Remove(FileUtil.StreetCache.Keys.First());
                outputFile.WriteLine("StreetID;StreetName;Lenght;Segment");
                foreach (Provincie pro in Provincies.Values)
                {
                    foreach (Gemeente gem in pro.gemeentes)
                    {
                        foreach (Straat strt in gem.straten)
                        {
                            if (strt.graaf != null)
                            {
                                outputFile.Write(strt.ID + ";" + strt.Name.ToString() + ";" + strt.graaf.getLength() + ";");
                                List<Segment> li = new List<Segment> { };
                                foreach (Knoop i in strt.graaf.map.Keys)
                                {
                                    foreach (Segment s in strt.graaf.map[i])
                                    {
                                        if (!li.Contains(s))
                                        {
                                            li.Add(s);
                                        }
                                    }
                                }
                                foreach (Segment seg in li)
                                {
                                    outputFile.Write(seg + ",");
                                }

                                outputFile.WriteLine();
                            }
                        }
                    }
                }
            }
            using (StreamWriter outputFile = new StreamWriter(DatafileName))
            {
                outputFile.WriteLine("ProvincieID" + ";" + "ProvincieName" + ";" + "GemeenteID");
                foreach (Provincie pro in Provincies.Values)
                {
                    outputFile.Write(pro.ID + ";" + pro.Name.ToString() + ";");
                    foreach (Gemeente gem in pro.gemeentes)
                    {
                        outputFile.Write(gem.ID + ",");
                    }
                    outputFile.WriteLine();
                }
            }
            using (StreamWriter outputFile = new StreamWriter(DatafileName2))
            {
                outputFile.WriteLine("GemeenteID" + ";" + "GemeenteName" + ";" + "StreetID;MAX;MIN");
                foreach (Provincie pro in Provincies.Values)
                {
                    foreach (Gemeente gem in FileUtil.GemeenteCache.Values)
                    {
                        outputFile.Write(gem.ID + ";" + gem.Name.ToString() + ";");
                        List<double> lengte = new List<double> { };
                        foreach (Straat str in gem.straten)
                        {
                            if (str.graaf != null)
                            {
                                int ids = 0;
                                if (str.ID != null)
                                {
                                    ids = str.ID;
                                }
                                outputFile.Write(ids + ",");
                                lengte.Add(str.graaf.getLength());
                            }
                        }
                        lengte.Sort();
                        lengte.RemoveAll(i => i == 0);
                        if (lengte.Count != 0)
                        {
                            outputFile.Write(";" + lengte[lengte.Count - 1]);
                            outputFile.Write(";" + lengte[0]);
                        }
                        else
                        {
                            outputFile.Write(";" + -0);
                            outputFile.Write(";" + -0);
                        }
                        
                        outputFile.WriteLine();
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Done");
            Console.ResetColor();
            Console.WriteLine();
        }
    }

}
