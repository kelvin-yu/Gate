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
using System.Threading;


namespace Gate
{
    public static class SerializeTools
    {
        public static List<Card> deserializeCardList()
        {
            int tries = 3;
            do
            {
                try
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
                catch(Exception e)
                {
                    Console.WriteLine(e.Source + " " + e.Message);
                    Thread.Sleep(1000);
                    tries--;
                }
            }
            while (tries > 0);
            return null;
        }

        public static List<AccessLevel> deserializeAccessLevelList()
        {
            int tries = 3;
            do
            {
                try
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
                catch (Exception e)
                {
                    Console.WriteLine(e.Source + " " + e.Message);
                    Thread.Sleep(1000);
                    tries--;
                }
            }
            while (tries > 0);
            return null;
        }
        
        public static List<Transaction> deserializeTransactionList()
        {
            int tries = 3;
            do
            {
                try
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
                catch (Exception e)
                {
                    Console.WriteLine(e.Source + " " + e.Message);
                    Thread.Sleep(1000);
                    tries--;
                }
            }
            while (tries > 0);
            return null;
        }

        public static void serializeCardList(List<Card> cardList)
        {
            int tries = 3;
            bool succeeded = false;
            do
            {
                try
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
                    succeeded = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Source + " " + e.Message);
                    Thread.Sleep(1000);
                    if (tries == 0)
                        throw e;
                    tries--;
                }
            }
            while (!succeeded && tries > 0);
        }

        public static void serializeAccessLevelList(List<AccessLevel> accessLevelList)
        {
            int tries = 3;
            bool succeeded = false;
            do
            {
                try
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
                    succeeded = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Source + " " + e.Message);
                    Thread.Sleep(1000);
                    if (tries == 0)
                        throw e;
                    tries--;
                }
            }
            while (!succeeded && tries > 0);
        }

        public static void serializeTransaction(List<Transaction> transactionList)
        {
            int tries = 3;
            bool succeeded = false;
            do
            {
                try
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
                    succeeded = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Source + " " + e.Message);
                    Thread.Sleep(1000);
                    if (tries == 0)
                        throw e;
                    tries--;
                }
            }
            while (!succeeded && tries > 0);
        }

        public static void deleteAllXml()
        {
            deleteAccessAndCardXml();
            deleteTransactionXml();
        }

        public static void deleteCardXml()
        {
            int tries = 3;
            bool succeeded = false;
            do
            {
                try
                {
                    string documentsPath = Android.OS.Environment.ExternalStorageDirectory + "/Gate";
                    var cardPath = Path.Combine(documentsPath, "card.xml");
                    File.Delete(cardPath);
                    succeeded = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Source + " " + e.Message);
                    Thread.Sleep(1000);
                    if (tries == 0)
                        throw e;
                    tries--;
                }
            }
            while (!succeeded && tries > 0);
        }

        public static void deleteAccessAndCardXml()
        {
            int tries = 3;
            bool succeeded = false;
            do
            {
                try
                {
                    string documentsPath = Android.OS.Environment.ExternalStorageDirectory + "/Gate";
                    var accessLevelPath = Path.Combine(documentsPath, "accesslevel.xml");
                    File.Delete(accessLevelPath);
                    var cardPath = Path.Combine(documentsPath, "card.xml");
                    File.Delete(cardPath);
                    succeeded = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Source + " " + e.Message);
                    Thread.Sleep(1000);
                    if (tries == 0)
                        throw e;
                    tries--;
                }
            }
            while (!succeeded && tries > 0);
        }

        public static void deleteTransactionXml()
        {
            int tries = 3;
            bool succeeded = false;
            do
            {
                try
                {
                    string documentsPath = Android.OS.Environment.ExternalStorageDirectory + "/Gate";
                    var transactionPath = Path.Combine(documentsPath, "transaction.xml");
                    File.Delete(transactionPath);
                    succeeded = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Source + " " + e.Message);
                    Thread.Sleep(1000);
                    if(tries == 0)
                        throw e;
                    tries--;
                }
            }
            while (!succeeded && tries > 0);
        }

        public static List<Card> sortCard(List<Card> sort, string sortby)
        {
            switch (sortby)
            {
                case "Oldest to Newest":
                    sort = sort.OrderBy(o => o.dateAdded).ToList();
                    break;
                case "Newest to Oldest":
                    sort = sort.OrderByDescending(o => o.dateAdded).ToList();
                    break;
                case "Name":
                    sort = sort.OrderBy(o => o.name).ToList();
                    break;
                case "Access Level":
                    sort = sort.OrderBy(o => o.accessLevel).ToList();
                    break;
                case "Card Code":
                    sort = sort.OrderBy(o => o.cardCode).ToList();
                    break;
            }
            return sort;
        }

        public static List<AccessLevel> sortAccess(List<AccessLevel> sort, string sortby)
        {
            switch (sortby)
            {
                case "Name":
                    sort = sort.OrderBy(o => o.name).ToList();
                    break;
            }
            return sort;
        }

        public static List<Transaction> sortTransaction(List<Transaction> sort, string sortby)
        {
            switch (sortby)
            {
                case "Oldest to Newest":
                    sort = sort.OrderBy(o => o.dateTime).ToList();
                    break;
                case "Newest to Oldest":
                    sort = sort.OrderByDescending(o => o.dateTime).ToList();
                    break;
                case "Card Code":
                    sort = sort.OrderBy(o => o.cardCode).ToList();
                    break;
            }
            return sort;
        }
    }
}