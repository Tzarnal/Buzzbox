using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Buzzbox_Common;
using Newtonsoft.Json;


namespace Buzzbox_Stream
{
    class NameCollection
    {
        public Dictionary<string, Dictionary<string, List<string>>> Names;

        [JsonIgnore]
        public int ReplacementChance;

        private static readonly Random Rng = new Random();
        private ConsoleLog _consoleLog;

        public NameCollection()
        {            
            Names = new Dictionary<string, Dictionary<string, List<string>>>();
            _consoleLog = ConsoleLog.Instance;
        }

        public bool ReplaceName()
        {
            return Rng.Next(101) < ReplacementChance;
        }

        public string RandomName(string cardclass, string type)
        {
            cardclass = cardclass.ToLower();
            type = type.ToLower();
            List<string> nameList;

            if (string.IsNullOrWhiteSpace(type))
            {
                type = "MINION";
            }

            if (string.IsNullOrWhiteSpace(cardclass))
            {
                type = "NEUTRAL";
            }

            try
            {
                nameList = Names[cardclass][type];
            }
            catch (KeyNotFoundException e)
            {
                _consoleLog.VerboseWriteLine($"Could not find names for {cardclass}:{type}");                
                return $"None";
            }

            if (nameList.Count > 0)
            {
                return nameList.RandomItem();
            }
            
            return $"None";                        
        }

        //Load a namecollection from json file
        public static NameCollection Load(string path)
        {
            string data;

            try
            {
                data = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not load File: '{0}' due to {1}", path, e.Message);
                throw;
            }

            var nameCollection = new NameCollection();

            try
            {
                nameCollection = JsonConvert.DeserializeObject<NameCollection>(data);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not Deserialize File: '{0}' due to {1}", path, e.Message);
                throw;
            }

            return nameCollection;
        }
    }
}
