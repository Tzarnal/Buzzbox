﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Buzzbox_Common
{
    class CardCollection
    {
        public List<Card> Cards;

        public CardCollection()
        {
            Cards = new List<Card>();
        }

        //Load a card collection from a supplied path and return it once deserialized.
        public static CardCollection Load(string path)
        {
            string data;

            try
            {
                data = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not load File: '{0}' due to {1}", path,  e.Message);
                throw;
            }

            try
            {
                return JsonConvert.DeserializeObject<CardCollection>(data);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not Deserialize File: '{0}' due to {1}", path, e.Message);
                throw;
            }
        }

        //Save This Card Collection to a supplied file path.
        public void Save(string path)
        {
            var data = JsonConvert.SerializeObject(this);

            try
            {
                File.WriteAllText(path, data);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to save to '{0}' due to {1}", path, e.Message );
                throw;
            }

        }
    }
}
