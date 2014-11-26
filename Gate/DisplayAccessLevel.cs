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
using Android.Preferences;

using Gate.WebReference;
using System.Threading;


namespace Gate
{
    [Activity(Label = "Display Access Level")]
    public class DisplayAccessLevel : Activity
    {
        EditText name, numberOfUses;
        TextView sunStart, monStart, tuesStart, wedStart, thursStart, friStart, satStart, sunEnd, monEnd, tuesEnd, wedEnd, thursEnd, friEnd, satEnd;
        CheckBox r1Sun, r1Mon, r1Tues, r1Wed, r1Thurs, r1Fri, r1Sat, r2Sun, r2Mon, r2Tues, r2Wed, r2Thurs, r2Fri, r2Sat;
        CheckBox usePassBack, useDateRange;
        TextView dateStart, dateEnd, dateStartView, dateEndView;
        Button doneButton, cancelButton;

        List<AccessLevel> accessLevelList;
        List<string> accessNameList = new List<string>();
        List<Card> cardList;
        int levelIndex;

        const int TIME_DIALOG_ID_SUNS = 0;
        const int TIME_DIALOG_ID_MONS = 1;
        const int TIME_DIALOG_ID_TUESS = 2;
        const int TIME_DIALOG_ID_WEDS = 3;
        const int TIME_DIALOG_ID_THURS = 4;
        const int TIME_DIALOG_ID_FRIS = 5;
        const int TIME_DIALOG_ID_SATS = 6;
        const int TIME_DIALOG_ID_SUNE = 7;
        const int TIME_DIALOG_ID_MONE = 8;
        const int TIME_DIALOG_ID_TUESE = 9;
        const int TIME_DIALOG_ID_WEDE = 10;
        const int TIME_DIALOG_ID_THURE = 11;
        const int TIME_DIALOG_ID_FRIE = 12;
        const int TIME_DIALOG_ID_SATE = 13;
        const int DATE_DIALOG_ID_START = 14;
        const int DATE_DIALOG_ID_END = 15;
        int hour = 0, minute = 0, year = 0, month = 0, day = 0;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AddAccessLevel);
            Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);

            accessLevelList = SerializeTools.deserializeAccessLevelList();
            cardList = SerializeTools.deserializeCardList();

            InitiateViews();
            InitiateListeners();

            foreach (AccessLevel access in accessLevelList)
                accessNameList.Add(access.name.ToLower());
        }

        private void InitiateViews()
        {
            doneButton = FindViewById<Button>(Resource.Id.doneAccessButton);
            cancelButton = FindViewById<Button>(Resource.Id.cancelAccessButton);
            doneButton.Text = "Update";

            name = FindViewById<EditText>(Resource.Id.accessNameField);
            numberOfUses = FindViewById<EditText>(Resource.Id.numberOfUsesField);

            sunStart = FindViewById<TextView>(Resource.Id.startTimeSunday);
            monStart = FindViewById<TextView>(Resource.Id.startTimeMonday);
            tuesStart = FindViewById<TextView>(Resource.Id.startTimeTuesday);
            wedStart = FindViewById<TextView>(Resource.Id.startTimeWednesday);
            thursStart = FindViewById<TextView>(Resource.Id.startTimeThursday);
            friStart = FindViewById<TextView>(Resource.Id.startTimeFriday);
            satStart = FindViewById<TextView>(Resource.Id.startTimeSaturday);

            sunEnd = FindViewById<TextView>(Resource.Id.endTimeSunday);
            monEnd = FindViewById<TextView>(Resource.Id.endTimeMonday);
            tuesEnd = FindViewById<TextView>(Resource.Id.endTimeTuesday);
            wedEnd = FindViewById<TextView>(Resource.Id.endTimeWednesday);
            thursEnd = FindViewById<TextView>(Resource.Id.endTimeThursday);
            friEnd = FindViewById<TextView>(Resource.Id.endTimeFriday);
            satEnd = FindViewById<TextView>(Resource.Id.endTimeSaturday);

            r1Sun = FindViewById<CheckBox>(Resource.Id.r1Sun);
            r1Mon = FindViewById<CheckBox>(Resource.Id.r1Mon);
            r1Tues = FindViewById<CheckBox>(Resource.Id.r1Tues);
            r1Wed = FindViewById<CheckBox>(Resource.Id.r1Wed);
            r1Thurs = FindViewById<CheckBox>(Resource.Id.r1Thurs);
            r1Fri = FindViewById<CheckBox>(Resource.Id.r1Fri);
            r1Sat = FindViewById<CheckBox>(Resource.Id.r1Sat);

            r2Sun = FindViewById<CheckBox>(Resource.Id.r2Sun);
            r2Mon = FindViewById<CheckBox>(Resource.Id.r2Mon);
            r2Tues = FindViewById<CheckBox>(Resource.Id.r2Tues);
            r2Wed = FindViewById<CheckBox>(Resource.Id.r2Wed);
            r2Thurs = FindViewById<CheckBox>(Resource.Id.r2Thurs);
            r2Fri = FindViewById<CheckBox>(Resource.Id.r2Fri);
            r2Sat = FindViewById<CheckBox>(Resource.Id.r2Sat);

            usePassBack = FindViewById<CheckBox>(Resource.Id.usePassBack);
            useDateRange = FindViewById<CheckBox>(Resource.Id.useDateRange);

            dateStartView = FindViewById<TextView>(Resource.Id.dateStartView);
            dateEndView = FindViewById<TextView>(Resource.Id.dateEndView);
            dateStart = FindViewById<TextView>(Resource.Id.dateStart);
            dateEnd = FindViewById<TextView>(Resource.Id.dateEnd);
            /////////////////////////////////////////////////////
            dateStart.Text = DateTime.Now.ToString("M/d/yyyy");
            dateEnd.Text = DateTime.Now.ToString("M/d/yyyy");

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            levelIndex = prefs.GetInt("accesslevelclick", -1);
            AccessLevel currentAccessLevel = accessLevelList[levelIndex];

            name.Text = currentAccessLevel.name;

            sunStart.Text = currentAccessLevel.weekTimeStart[0].ToString("HH:mm");
            monStart.Text = currentAccessLevel.weekTimeStart[1].ToString("HH:mm");
            tuesStart.Text = currentAccessLevel.weekTimeStart[2].ToString("HH:mm");
            wedStart.Text = currentAccessLevel.weekTimeStart[3].ToString("HH:mm");
            thursStart.Text = currentAccessLevel.weekTimeStart[4].ToString("HH:mm");
            friStart.Text = currentAccessLevel.weekTimeStart[5].ToString("HH:mm");
            satStart.Text = currentAccessLevel.weekTimeStart[6].ToString("HH:mm");

            sunEnd.Text = currentAccessLevel.weekTimeEnd[0].ToString("HH:mm");
            monEnd.Text = currentAccessLevel.weekTimeEnd[1].ToString("HH:mm");
            tuesEnd.Text = currentAccessLevel.weekTimeEnd[2].ToString("HH:mm");
            wedEnd.Text = currentAccessLevel.weekTimeEnd[3].ToString("HH:mm");
            thursEnd.Text = currentAccessLevel.weekTimeEnd[4].ToString("HH:mm");
            friEnd.Text = currentAccessLevel.weekTimeEnd[5].ToString("HH:mm");
            satEnd.Text = currentAccessLevel.weekTimeEnd[6].ToString("HH:mm");

            r1Sun.Checked = currentAccessLevel.weekReader1Access[0] == "Y";
            r1Mon.Checked = currentAccessLevel.weekReader1Access[1] == "Y";
            r1Tues.Checked = currentAccessLevel.weekReader1Access[2] == "Y";
            r1Wed.Checked = currentAccessLevel.weekReader1Access[3] == "Y";
            r1Thurs.Checked = currentAccessLevel.weekReader1Access[4] == "Y";
            r1Fri.Checked = currentAccessLevel.weekReader1Access[5] == "Y";
            r1Sat.Checked = currentAccessLevel.weekReader1Access[6] == "Y";

            r2Sun.Checked = currentAccessLevel.weekReader2Access[0] == "Y";
            r2Mon.Checked = currentAccessLevel.weekReader2Access[1] == "Y";
            r2Tues.Checked = currentAccessLevel.weekReader2Access[2] == "Y";
            r2Wed.Checked = currentAccessLevel.weekReader2Access[3] == "Y";
            r2Thurs.Checked = currentAccessLevel.weekReader2Access[4] == "Y";
            r2Fri.Checked = currentAccessLevel.weekReader2Access[5] == "Y";
            r2Sat.Checked = currentAccessLevel.weekReader2Access[6] == "Y";

            numberOfUses.Text = currentAccessLevel.numberOfUses.ToString();
            usePassBack.Checked = currentAccessLevel.usePassBack;
            useDateRange.Checked = currentAccessLevel.useDateRange;

            dateStart.Text = currentAccessLevel.dateStart.ToString("M/d/yyyy");
            dateEnd.Text = currentAccessLevel.dateEnd.ToString("M/d/yyyy");
        }
        private void InitiateListeners()
        {
            sunStart.Click += delegate { ShowDialog(TIME_DIALOG_ID_SUNS); };
            monStart.Click += delegate { ShowDialog(TIME_DIALOG_ID_MONS); };
            tuesStart.Click += delegate { ShowDialog(TIME_DIALOG_ID_TUESS); };
            wedStart.Click += delegate { ShowDialog(TIME_DIALOG_ID_WEDS); };
            thursStart.Click += delegate { ShowDialog(TIME_DIALOG_ID_THURS); };
            friStart.Click += delegate { ShowDialog(TIME_DIALOG_ID_FRIS); };
            satStart.Click += delegate { ShowDialog(TIME_DIALOG_ID_SATS); };

            sunEnd.Click += delegate { ShowDialog(TIME_DIALOG_ID_SUNE); };
            monEnd.Click += delegate { ShowDialog(TIME_DIALOG_ID_MONE); };
            tuesEnd.Click += delegate { ShowDialog(TIME_DIALOG_ID_TUESE); };
            wedEnd.Click += delegate { ShowDialog(TIME_DIALOG_ID_WEDE); };
            thursEnd.Click += delegate { ShowDialog(TIME_DIALOG_ID_THURE); };
            friEnd.Click += delegate { ShowDialog(TIME_DIALOG_ID_FRIE); };
            satEnd.Click += delegate { ShowDialog(TIME_DIALOG_ID_SATE); };

            dateStart.Click += delegate { ShowDialog(DATE_DIALOG_ID_START); };
            dateEnd.Click += delegate { ShowDialog(DATE_DIALOG_ID_END); };

            useDateRange.CheckedChange += delegate
            {
                if (useDateRange.Checked)
                {
                    dateStart.Enabled = true;
                    dateEnd.Enabled = true;
                    dateStartView.Enabled = true;
                    dateEndView.Enabled = true;
                }
                else
                {
                    dateStart.Enabled = false;
                    dateEnd.Enabled = false;
                    dateStartView.Enabled = false;
                    dateEndView.Enabled = false;
                }
            };

            doneButton.Click += delegate
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                if (!name.Text.Equals(String.Empty) && !numberOfUses.Text.Equals(String.Empty) && (!accessNameList.Contains(name.Text) || name.Text.Equals(accessLevelList[levelIndex].name)))
                {
                    if (ReaderServices.isConnectable(prefs.GetString("reader_ip", "192.168.2.180")))
                    {
                        bool SQLStatus = true;
                        try
                        {
                            Global.cs.Hello();
                        }
                        catch
                        {
                            SQLStatus = false;
                            CreateOkDialog("No Connection", "No connection to database!");
                        }
                        if (SQLStatus)
                        {
                            UpdateAccessLevel();
                            Finish();
                        }
                    }
                    else
                        CreateOkDialog("No Connection", "No connection to reader!");
                }
                else if (name.Text.Equals(string.Empty))
                    CreateOkDialog("Name", "You must set a name!");
                else if (numberOfUses.Text.Equals(String.Empty))
                    CreateOkDialog("Number of Uses", "You must set a number of uses!");
                else
                    CreateOkDialog("Access Level Name", "Access level already exists!");

            };
            cancelButton.Click += delegate { Finish(); };
        }
        public void UpdateAccessLevel()
        {
            DateTime[] startTime = new DateTime[7] {
                DateTime.ParseExact(sunStart.Text, "HH:mm", System.Globalization.CultureInfo.InvariantCulture), 
                DateTime.ParseExact(monStart.Text, "HH:mm", System.Globalization.CultureInfo.InvariantCulture), 
                DateTime.ParseExact(tuesStart.Text, "HH:mm", System.Globalization.CultureInfo.InvariantCulture), 
                DateTime.ParseExact(wedStart.Text, "HH:mm", System.Globalization.CultureInfo.InvariantCulture), 
                DateTime.ParseExact(thursStart.Text, "HH:mm", System.Globalization.CultureInfo.InvariantCulture), 
                DateTime.ParseExact(friStart.Text, "HH:mm", System.Globalization.CultureInfo.InvariantCulture), 
                DateTime.ParseExact(satStart.Text, "HH:mm", System.Globalization.CultureInfo.InvariantCulture)
            };

            DateTime[] endTime = new DateTime[7] {
                DateTime.ParseExact(sunEnd.Text, "HH:mm", System.Globalization.CultureInfo.InvariantCulture), 
                DateTime.ParseExact(monEnd.Text, "HH:mm", System.Globalization.CultureInfo.InvariantCulture), 
                DateTime.ParseExact(tuesEnd.Text, "HH:mm", System.Globalization.CultureInfo.InvariantCulture), 
                DateTime.ParseExact(wedEnd.Text, "HH:mm", System.Globalization.CultureInfo.InvariantCulture), 
                DateTime.ParseExact(thursEnd.Text, "HH:mm", System.Globalization.CultureInfo.InvariantCulture), 
                DateTime.ParseExact(friEnd.Text, "HH:mm", System.Globalization.CultureInfo.InvariantCulture), 
                DateTime.ParseExact(satEnd.Text, "HH:mm", System.Globalization.CultureInfo.InvariantCulture)
            };

            string[] r1 = new string[7] {r1Sun.Checked ? "Y" : "N",
                r1Mon.Checked ? "Y" : "N",
                r1Tues.Checked ? "Y" : "N",
                r1Wed.Checked ? "Y" : "N",
                r1Thurs.Checked ? "Y" : "N",
                r1Fri.Checked ? "Y" : "N",
                r1Sat.Checked ? "Y" : "N",};

            string[] r2 = new string[7] {r2Sun.Checked ? "Y" : "N",
                r2Mon.Checked ? "Y" : "N",
                r2Tues.Checked ? "Y" : "N",
                r2Wed.Checked ? "Y" : "N",
                r2Thurs.Checked ? "Y" : "N",
                r2Fri.Checked ? "Y" : "N",
                r2Sat.Checked ? "Y" : "N",};

            DateTime dStart = DateTime.ParseExact(dateStart.Text, "M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime dEnd = DateTime.ParseExact(dateEnd.Text, "M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            if (!name.Text.Equals(accessLevelList[levelIndex].name)) //if the name has been modified, modify for all cards
            {
                foreach (Card card in cardList)
                {
                    if (card.accessLevel.Equals(accessLevelList[levelIndex].name))
                    {
                        card.accessLevel = name.Text;
                        bool result, resultSpecified;
                        Global.cs.ModifyValues("cards", "access_level", card.accessLevel, accessLevelList[levelIndex].name, out result, out resultSpecified);
                    }
                }
                SerializeTools.serializeCardList(cardList);
            }
            string oldName = accessLevelList[levelIndex].name;
            //Add to local
            accessLevelList.RemoveAt(levelIndex);
            accessLevelList.Add(new AccessLevel(name.Text, startTime, endTime, r1, r2, usePassBack.Checked, Convert.ToInt16(numberOfUses.Text), useDateRange.Checked, dStart, dEnd));
            SerializeTools.serializeAccessLevelList(accessLevelList);
            //Send to reader
            ReaderServices.sendAccessLevel(accessLevelList, this);
            //Add to SQL
            bool result2, resultSpecified2;
            Global.cs.DeleteAccess(oldName, out result2, out resultSpecified2);
            Global.cs.AddOneAccessSQL(new AccessLevel(name.Text, startTime, endTime, r1, r2, usePassBack.Checked, Convert.ToInt16(numberOfUses.Text), useDateRange.Checked, dStart, dEnd), out result2, out resultSpecified2);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add(0, 0, 0, "Delete Access Level");
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 0:
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    AlertDialog alertDialog = builder.Create();
                    alertDialog.SetTitle("Confirm");
                    alertDialog.SetIcon(Android.Resource.Drawable.IcDialogAlert);
                    alertDialog.SetMessage("Deleting this access level with delete all cards associated with this access level!");
                    alertDialog.SetButton("OK", (s, ev) =>
                    {
                        bool result, resultSpecified;
                        for (int i = 0; i < cardList.Count; i++) // Deleting all cards associated with access level
                        {
                            if (cardList[i].accessLevel == accessLevelList[levelIndex].name)
                            {
                                Global.cs.DeleteCard(cardList[i].name, out result, out resultSpecified);
                                cardList.RemoveAt(i--);
                            }
                        }
                        //Writing to local
                        SerializeTools.serializeCardList(cardList);
                        SerializeTools.serializeAccessLevelList(accessLevelList);
                        //to reader
                        ReaderServices.sendCard(cardList, this);
                        ReaderServices.sendAccessLevel(accessLevelList, this);
                        //to SQL
                        Global.cs.DeleteAccess(accessLevelList[levelIndex].name, out result, out resultSpecified);
                        accessLevelList.RemoveAt(levelIndex);

                        Finish();
                    });
                    alertDialog.SetButton2("Cancel", (s, ev) =>
                    {
                    });
                    alertDialog.Show();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public void CreateOkDialog(string title, string message)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            AlertDialog alertDialog = builder.Create();
            alertDialog.SetTitle(title);
            alertDialog.SetIcon(Android.Resource.Drawable.IcDialogAlert);
            alertDialog.SetMessage(message);
            alertDialog.SetButton("OK", (s, ev) =>
            {
            });
            alertDialog.Show();
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back)
            {
                Finish();
            }
            return base.OnKeyDown(keyCode, e);
        }

        protected override Dialog OnCreateDialog(int id)
        {
            if (id == TIME_DIALOG_ID_SUNS)
                return new TimePickerDialog(this, TimePickerCallbackSUNS, hour, minute, true);
            if (id == TIME_DIALOG_ID_MONS)
                return new TimePickerDialog(this, TimePickerCallbackMONS, hour, minute, true);
            if (id == TIME_DIALOG_ID_TUESS)
                return new TimePickerDialog(this, TimePickerCallbackTUESS, hour, minute, true);
            if (id == TIME_DIALOG_ID_WEDS)
                return new TimePickerDialog(this, TimePickerCallbackWEDS, hour, minute, true);
            if (id == TIME_DIALOG_ID_THURS)
                return new TimePickerDialog(this, TimePickerCallbackTHURS, hour, minute, true);
            if (id == TIME_DIALOG_ID_FRIS)
                return new TimePickerDialog(this, TimePickerCallbackFRIS, hour, minute, true);
            if (id == TIME_DIALOG_ID_SATS)
                return new TimePickerDialog(this, TimePickerCallbackSATS, hour, minute, true);

            if (id == TIME_DIALOG_ID_SUNE)
                return new TimePickerDialog(this, TimePickerCallbackSUNE, hour, minute, true);
            if (id == TIME_DIALOG_ID_MONE)
                return new TimePickerDialog(this, TimePickerCallbackMONE, hour, minute, true);
            if (id == TIME_DIALOG_ID_TUESE)
                return new TimePickerDialog(this, TimePickerCallbackTUESE, hour, minute, true);
            if (id == TIME_DIALOG_ID_WEDE)
                return new TimePickerDialog(this, TimePickerCallbackWEDE, hour, minute, true);
            if (id == TIME_DIALOG_ID_THURE)
                return new TimePickerDialog(this, TimePickerCallbackTHURE, hour, minute, true);
            if (id == TIME_DIALOG_ID_FRIE)
                return new TimePickerDialog(this, TimePickerCallbackFRIE, hour, minute, true);
            if (id == TIME_DIALOG_ID_SATE)
                return new TimePickerDialog(this, TimePickerCallbackSATE, hour, minute, true);

            if (id == DATE_DIALOG_ID_START)
            {
                var datePicker = new DatePickerDialog(this, DatePickerCallBackStart, year, month, day);
                datePicker.UpdateDate(DateTime.Now);
                return datePicker;
            }
            if (id == DATE_DIALOG_ID_END)
            {
                var datePicker = new DatePickerDialog(this, DatePickerCallBackEnd, year, month, day);
                datePicker.UpdateDate(DateTime.Now);
                return datePicker;
            }

            return null;
        }
        private void DatePickerCallBackStart(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            dateStart.Text = e.Date.ToString("M/d/yyyy");
        }
        private void DatePickerCallBackEnd(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            dateEnd.Text = e.Date.ToString("M/d/yyyy");
        }

        private string timeReturn(string time, int hour, int minute)
        {
            string shour = Convert.ToString(hour);
            string sminute = Convert.ToString(minute);
            if (shour.Length == 1)
                shour = "0" + shour;
            if (sminute.Length == 1)
                sminute = "0" + sminute;

            return shour + ":" + sminute;
        }
        //START
        private void TimePickerCallbackSUNS(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            string time = "";
            time = timeReturn(time, e.HourOfDay, e.Minute);
            sunStart.Text = time;
        }
        private void TimePickerCallbackMONS(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            string time = "";
            time = timeReturn(time, e.HourOfDay, e.Minute);
            monStart.Text = time;
        }
        private void TimePickerCallbackTUESS(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            string time = "";
            time = timeReturn(time, e.HourOfDay, e.Minute);
            tuesStart.Text = time;
        }
        private void TimePickerCallbackWEDS(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            string time = "";
            time = timeReturn(time, e.HourOfDay, e.Minute);
            wedStart.Text = time;
        }
        private void TimePickerCallbackTHURS(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            string time = "";
            time = timeReturn(time, e.HourOfDay, e.Minute);
            thursStart.Text = time;
        }
        private void TimePickerCallbackFRIS(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            string time = "";
            time = timeReturn(time, e.HourOfDay, e.Minute);
            friStart.Text = time;
        }
        private void TimePickerCallbackSATS(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            string time = "";
            time = timeReturn(time, e.HourOfDay, e.Minute);
            satStart.Text = time;
        }
        //END
        private void TimePickerCallbackSUNE(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            string time = "";
            time = timeReturn(time, e.HourOfDay, e.Minute);
            sunEnd.Text = time;
        }
        private void TimePickerCallbackMONE(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            string time = "";
            time = timeReturn(time, e.HourOfDay, e.Minute);
            monEnd.Text = time;
        }
        private void TimePickerCallbackTUESE(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            string time = "";
            time = timeReturn(time, e.HourOfDay, e.Minute);
            tuesEnd.Text = time;
        }
        private void TimePickerCallbackWEDE(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            string time = "";
            time = timeReturn(time, e.HourOfDay, e.Minute);
            wedEnd.Text = time;
        }
        private void TimePickerCallbackTHURE(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            string time = "";
            time = timeReturn(time, e.HourOfDay, e.Minute);
            thursEnd.Text = time;
        }
        private void TimePickerCallbackFRIE(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            string time = "";
            time = timeReturn(time, e.HourOfDay, e.Minute);
            friEnd.Text = time;
        }
        private void TimePickerCallbackSATE(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            string time = "";
            time = timeReturn(time, e.HourOfDay, e.Minute);
            satEnd.Text = time;
        }
    }
}