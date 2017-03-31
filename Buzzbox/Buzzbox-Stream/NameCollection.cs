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

        public NameCollection()
        {            
            Names = new Dictionary<string, Dictionary<string, List<string>>>();
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

            try
            {
                nameList = Names[cardclass][type];
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine($"Could not find names for {cardclass}:{type}");
                return $"No Valid Item In Namecollection for {cardclass}:{type}";
            }

            if (nameList.Count > 0)
            {
                return nameList.RandomItem();
            }
            
            return $"No Valid Item In Namecollection for {cardclass}:{type}";                        
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
