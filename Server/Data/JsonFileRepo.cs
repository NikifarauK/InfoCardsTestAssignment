using Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace Server.Data
{
    public class JsonFileRepo : IRepository<CardDto>
    {

        private readonly string _filePath;
        private string FilePath => _filePath;

        public JsonFileRepo(string filePath)
        {
            _filePath = filePath;
        }

        private List<CardDto> cards = null;
        public List<CardDto> Cards
        {
            get 
            {
                if (cards == null)
                    cards = LoadJsonFile(FilePath);
                return cards;
            }
            set { cards = value; }
        }

        private List<CardDto> LoadJsonFile(string filePath)
        {
            using(var reader = new StreamReader(filePath))
            {
                var str = reader.ReadToEnd();
                if(string.IsNullOrEmpty(str))
                    return new List<CardDto>();
                var result = JsonConvert.DeserializeObject<List<CardDto>>(str);
                return result;
            }
        }

        public IEnumerable<CardDto> GetAll() => Cards;

        public CardDto Get(int id) => Cards.FirstOrDefault(x => x.Id == id);


        public bool Insert(CardDto card)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(Insert));
            if(Cards.FirstOrDefault(x=> x.Id == card.Id) != null)
                return false;
            Cards.Add(card);
            SaveChanges();
            return true;
        }

        public bool Update(CardDto card)
        {
            if(card == null)
                throw new ArgumentNullException(nameof(Update));
            var index = Cards.IndexOf(Cards.FirstOrDefault(x => x.Id == card.Id));
            if (index == -1)
                return false;
            Cards[index] = card;
            SaveChanges();
            return true;
        }
        public bool Delete(int id)
        {
            var card = Cards.FirstOrDefault(x => x.Id == id);
            if (card == null)
                return false;
            Cards.Remove(card);
            SaveChanges();
            return true;
        }

        public void SaveChanges()
        {
            if(cards != null)
            {
                var str = JsonConvert.SerializeObject(Cards);
                using (var writer = new StreamWriter(FilePath))
                {
                    writer.Write(str);
                }
            }
        }
    }
}
