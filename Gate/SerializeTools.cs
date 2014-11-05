using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using System.Xml.Serialization;

using Gate.WebReference;


namespace Gate
{
    public static class SerializeTools
    {
        public static List<Card> deserializeCardList()
        {
            string documentsPath = Android.OS.Environment.ExternalStorageDirectory + "/Gate";
            var cardPath = Path.Combine(documentsPath, "card.xml");
            if (File.Exists(cardPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Card>));
                StreamReader reader = new StreamReader(cardPath);
                List<Card> cardList = (List<Card>)serializer.Deserialize(reader);
                reader.Close();
                return cardList;
            }
            return new List<Card>();
        }

        public static List<AccessLevel> deserializeAccessLevelList()
        {
            string documentsPath = Android.OS.Environment.ExternalStorageDirectory + "/Gate";
            var accessLevelPath = Path.Combine(documentsPath, "accesslevel.xml");
            if (File.Exists(accessLevelPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<AccessLevel>));
                StreamReader reader = new StreamReader(accessLevelPath);
                List<AccessLevel> accessLevelList = (List<AccessLevel>)serializer.Deserialize(reader);
                reader.Close();
                return accessLevelList;
            }
            return new List<AccessLevel>();
        }

        public static List<Transaction> deserializeTransactionList()
        {
            string documentsPath = Android.OS.Environment.ExternalStorageDirectory + "/Gate";
            var transactionPath = Path.Combine(documentsPath, "transaction.xml");
            if (File.Exists(transactionPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Transaction>));
                StreamReader reader = new StreamReader(transactionPath);
                List<Transaction> transactionList = (List<Transaction>)serializer.Deserialize(reader);
                reader.Close();
                return transactionList;
            }
            return new List<Transaction>();
        }

        public static void serializeCardList(List<Card> cardList)
        {
            string documentsPath = Android.OS.Environment.ExternalStorageDirectory + "/Gate";

            if (!Directory.Exists(documentsPath))
            {
                Directory.CreateDirectory(documentsPath);
            }
            var cardPath = Path.Combine(documentsPath, "card.xml");

            XmlSerializer writer = new XmlSerializer(typeof(List<Card>));
            using (var file = File.Open(cardPath, FileMode.Create, FileAccess.ReadWrite))
            {
                StreamWriter strm = new StreamWriter(file);
                writer.Serialize(strm, cardList);
                strm.Close();
            }
        }

        public static void serializeAccessLevelList(List<AccessLevel> accessLevelList)
        {
            string documentsPath = Android.OS.Environment.ExternalStorageDirectory + "/Gate";

            if (!Directory.Exists(documentsPath))
            {
                Directory.CreateDirectory(documentsPath);
            }
            var accessLevelPath = Path.Combine(documentsPath, "accesslevel.xml");

            XmlSerializer writer = new XmlSerializer(typeof(List<AccessLevel>));
            using (var file = File.Open(accessLevelPath, FileMode.Create, FileAccess.ReadWrite))
            {
                StreamWriter strm = new StreamWriter(file);
                writer.Serialize(strm, accessLevelList);
                strm.Close();
            }
        }

        public static void serializeTransaction(List<Transaction> transactionList)
        {
            string documentsPath = Android.OS.Environment.ExternalStorageDirectory + "/Gate";

            if (!Directory.Exists(documentsPath))
            {
                Directory.CreateDirectory(documentsPath);
            }
            var transactionPath = Path.Combine(documentsPath, "transaction.xml");

            XmlSerializer writer = new XmlSerializer(typeof(List<Transaction>));
            using (var file = File.Open(transactionPath, FileMode.Create, FileAccess.ReadWrite))
            {
                StreamWriter strm = new StreamWriter(file);
                writer.Serialize(strm, transactionList);
                strm.Close();
            }
        }
    }
}