﻿using System.Collections.Generic;
using System.IO;
using Zippy.Chirp;
using Zippy.Chirp.Engines;

namespace Console.Chirp 
{
    class Program
    {
        static void Main(string[] args)
        {
            string findPath = string.Empty;
            if (args.Length > 0)
            {
                findPath = args[0];
            }

            List<TransformEngine> listTrasformEngine = new List<TransformEngine>();

            listTrasformEngine.Add(new YuiCssEngine());
            listTrasformEngine.Add(new YuiJsEngine());
            listTrasformEngine.Add(new ClosureCompilerEngine());
            listTrasformEngine.Add(new LessEngine());
            listTrasformEngine.Add(new CoffeeScriptEngine());
            listTrasformEngine.Add(new UglifyEngine());
            listTrasformEngine.Add(new MsJsEngine());
            listTrasformEngine.Add(new MsCssEngine());
            //// listTrasformEngine.Add(new ConfigEngine());
            listTrasformEngine.Add(new ViewEngine());
            //// listTrasformEngine.Add(new T4Engine());

            foreach (var directory in Directory.GetDirectories(findPath, "*", SearchOption.AllDirectories))
            {
                var settings = new Settings(directory);
                foreach (TransformEngine transformEngine in listTrasformEngine)
                {
                    transformEngine.Settings = settings;
                    foreach (string extension in transformEngine.Extensions)
                    {
                        foreach (
                            string filename in
                                Directory.GetFiles(directory, "*" + extension, SearchOption.TopDirectoryOnly))
                        {
                            if (filename.Contains(".min."))
                            {
                                continue;
                            }

                            string text = System.IO.File.ReadAllText(filename);
                            string minFileName = Utilities.GetBaseFileName(filename, extension) +
                                                 transformEngine.GetOutputExtension(filename);
                            text = transformEngine.Transform(filename, text, null);
                            System.IO.File.WriteAllText(minFileName, text);
                            System.Console.WriteLine(string.Format("{0} -- {1}", transformEngine.GetType().Name, filename));
                        }
                    }
                }
            }

            // config file
            var configEngine = new ConfigEngine();

            foreach (var directory in Directory.GetDirectories(findPath, "*", SearchOption.AllDirectories))
            {
                var settings = new Settings(directory);
                foreach (string filename in Directory.GetFiles(findPath, "*" + new Settings(directory).ChirpConfigFile, SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        System.Console.WriteLine(string.Format("ConfigEngine -- {0}", filename));
                        configEngine.Run(filename, null);
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        System.Console.WriteLine(string.Format("File not found in config file={0}", filename));
                    }
                }
            }
        }
    }
}
