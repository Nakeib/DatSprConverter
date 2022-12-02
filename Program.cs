using System;
using System.Diagnostics;
using System.IO;

namespace OpenTibiaUnity
{
    static class Program
    {
        public const int SEGMENT_DIMENTION = 512;
        public const int BITMAP_SIZE = SEGMENT_DIMENTION * SEGMENT_DIMENTION;
        
        static void ConvertClientVersion(int fromVersion, int toVersion, bool useAlpha) {
            string datFile = fromVersion.ToString() + "/Tibia.dat";
            string sprFile = fromVersion.ToString() + "/Tibia.spr";
            if (!File.Exists(datFile) || !File.Exists(sprFile)) {
                Console.WriteLine("Tibia.dat or Tibia.spr doesn't exist");
                Environment.Exit(0);
                return;
            }

            Directory.CreateDirectory(toVersion.ToString());
            string newDatFile = toVersion.ToString() + "/Tibia.dat";
            string newSprFile = toVersion.ToString() + "/Tibia.spr";
            
            var datParser = new Core.Assets.ContentData(File.ReadAllBytes(datFile), fromVersion);

            byte[] result = datParser.ConvertTo(toVersion);
            File.WriteAllBytes(newDatFile, result);

            var sprParser = new Core.Assets.ContentSprites(File.ReadAllBytes(sprFile), fromVersion, useAlpha);

            result = sprParser.ConvertTo(toVersion);
            File.WriteAllBytes(newSprFile, result);

            Console.WriteLine("Convertion Successfull to " + toVersion + ".");
        }

        static void Main(string[] args) {
            int clientVersion = -1;
            int buildVersion = -1;
            int convertTo = -1;
            int mappingFrom = -1;
            int mappingTo = -1;
            bool useAlpha = false;
            foreach (var arg in args) {
                if (arg.StartsWith("--version=")) {
                    int.TryParse(arg.Substring(10), out clientVersion);
                } else if (arg.StartsWith("--build-version=")) {
                    int.TryParse(arg.Substring(16), out buildVersion);
                } else if (arg.StartsWith("--alpha=")) {
                    var boolstr = arg.Substring(8).ToLower();
                    useAlpha = boolstr == "y" || boolstr == "yes" || boolstr == "true" || boolstr == "1";
                } else if (arg.StartsWith("--convert-to=")) {
                    convertTo = int.Parse(arg.Substring(13));
                } else if (arg.StartsWith("--mappingFrom=")) {
                    mappingFrom = int.Parse(arg.Substring(14));
                } else if (arg.StartsWith("--mappingTo=")) {
                    mappingTo = int.Parse(arg.Substring(12));
                } else {
                    Console.WriteLine("Unknown Attribute: " + arg);
                    return;
                }
            }

            if (mappingFrom == -1)
            {
                if (clientVersion == -1)
                {
                    Console.WriteLine("Invalid client version.");
                    return;
                }

                if (clientVersion >= 1100 && buildVersion == -1)
                {
                    Console.WriteLine("Invalid build version.");
                    return;
                }

                if (clientVersion >= 1100)
                    Console.WriteLine("Loading version: {0}.{1}", clientVersion, buildVersion);
                else
                    Console.WriteLine("Loading version: {0}", clientVersion);
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();

            if (mappingFrom != -1 && mappingTo != -1)
            {
                if (mappingFrom >= 1100 || mappingTo >= 1100)
                {
                    Console.WriteLine("Wrong client version for mapping");
                    return;
                }

                Core.Converter.LegacyConverter fromConverter = null;
                Core.Converter.LegacyConverter toConverter = null;

                fromConverter = new Core.Converter.LegacyConverter(mappingFrom, useAlpha);
                toConverter = new Core.Converter.LegacyConverter(mappingTo, useAlpha);

                fromConverter.BeginMapping();
                toConverter.BeginMapping();

                StreamWriter writer = new StreamWriter("mapping_output.txt");
                foreach (var thing in fromConverter.m_contentData.ThingTypeDictionaries[(int)Core.Assets.ThingCategory.Item])
                {
                    uint id = thing.Key;
                    Core.Assets.ThingType thingType = thing.Value;

                    bool hasMapping = false;
                    string mapping = id.ToString() + " to ";
                    foreach (var thingTo in toConverter.m_contentData.ThingTypeDictionaries[(int)Core.Assets.ThingCategory.Item])
                    {
                        uint idTo = thingTo.Key;
                        Core.Assets.ThingType thingTypeTo = thingTo.Value;

                        if (thingType.CompareWith(thingTypeTo, fromConverter.m_contentSprites, toConverter.m_contentSprites))
                        {
                            mapping += idTo.ToString() + " ";
                            hasMapping = true;
                        }
                    }
                    if (hasMapping)
                    {
                        writer.WriteLine(mapping);
                        Console.WriteLine(mapping);
                    }
                }

                Console.WriteLine("All done!");
            }
            else if (convertTo != -1) {
                Console.WriteLine("Converting to: " + convertTo);
                ConvertClientVersion(clientVersion, convertTo, useAlpha);
            }
            else {
                Core.Converter.IConverter converter = null;

                if (clientVersion < 1100)
                    converter = new Core.Converter.LegacyConverter(clientVersion, useAlpha);
                else
                    converter = new Core.Converter.ProtobufConverter(clientVersion, buildVersion);

                var task = converter.BeginProcessing();
                task.Wait();
            }
            
            watch.Stop();
            
            double seconds = watch.ElapsedMilliseconds / (double)1000;
            Console.WriteLine("Time elapsed: " + seconds + " seconds.");
        }
    }
}
