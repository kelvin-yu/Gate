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
    public class AccessLevel
    {
        public string name { get; set; }

        public DateTime[] weekTimeStart { get; set; }
        public DateTime[] weekTimeEnd { get; set; }

        public string[] weekReader1Access { get; set; }
        public string[] weekReader2Access { get; set; }

        public bool usePassBack { get; set; }
        public int numberOfUses { get; set; }

        public bool useDateRange { get; set; }
        public DateTime dateStart { get; set; }
        public DateTime dateEnd { get; set; }

        public AccessLevel() { }

        public AccessLevel(string name, DateTime[] weekTimeStart, DateTime[] weekTimeEnd, string[] weekReader1Access, string[] weekReader2Access,
            bool usePassBack, int numberOfUses, bool useDateRange, DateTime dateStart, DateTime dateEnd)
        {
            this.name = name;
            this.weekTimeStart = weekTimeStart;
            this.weekTimeEnd = weekTimeEnd;
            this.weekReader1Access = weekReader1Access;
            this.weekReader2Access = weekReader2Access;
            this.usePassBack = usePassBack;
            this.numberOfUses = numberOfUses;
            this.useDateRange = useDateRange;
            this.dateStart = dateStart;
            this.dateEnd = dateEnd;
        }
            
    }
}