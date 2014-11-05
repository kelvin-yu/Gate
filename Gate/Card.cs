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

namespace Gate
{
    
    public class Card
    {
        public string name { get; set; }
        public int cardCode { get; set; }
        public string accessLevel { get; set; }
        public DateTime dateAdded { get; set; }

        public Card() { }

        public Card(string name, int cardCode, string accessLevel, DateTime dateAdded)
        {
            this.name = name;
            this.cardCode = cardCode;
            this.accessLevel = accessLevel;
            this.dateAdded = dateAdded;
        }
    }
}