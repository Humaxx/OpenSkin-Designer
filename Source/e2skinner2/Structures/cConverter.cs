using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Collections;
using OpenSkinDesigner.Logic;
using System.Globalization;
using System.Xml;
using System.Windows.Forms;

namespace OpenSkinDesigner.Structures
{
    static public class cConverter
    {
        static Hashtable pTable = null;
        static List<cConverterSimple> pSimpleList;

        public static class strftime
        {

            public static String ToString(String format)
            {
                format = format.Replace("%a", "ddd");
                format = format.Replace("%A", "dddd");
                format = format.Replace("%b", "MMM");
                format = format.Replace("%B", "MMMM");
                format = format.Replace("%c", "ddd MMM d HH:mm:ss yyyy");

                //There's no way to specify the century in SimpleDateFormat.  We don't want to hard-code
                //20 since this could be wrong for the pre-2000 files.
                //format.Replace("%C", "20");
                format = format.Replace("%d", "dd");
                format = format.Replace("%D", "MM/dd/yy");
                format = format.Replace("%e", "dd"); //will show as '03' instead of ' 3'
                format = format.Replace("%F", "yyyy-MM-dd");
                format = format.Replace("%g", "yy");
                format = format.Replace("%G", "yyyy");
                format = format.Replace("%H", "HH");
                format = format.Replace("%h", "MMM");
                format = format.Replace("%I", "hh");
                format = format.Replace("%j", "DDD");
                format = format.Replace("%k", "HH"); //will show as '07' instead of ' 7'
                format = format.Replace("%l", "hh"); //will show as '07' instead of ' 7'
                format = format.Replace("%m", "MM");
                format = format.Replace("%M", "mm");
                format = format.Replace("%n", "\n");
                format = format.Replace("%p", "a");
                format = format.Replace("%P", "a");  //will show as pm instead of PM
                format = format.Replace("%r", "hh:mm:ss a");
                format = format.Replace("%R", "HH:mm");
                //There's no way to specify this with SimpleDateFormat
                //format.Replace("%s","seconds since ecpoch");
                format = format.Replace("%S", "ss");
                format = format.Replace("%t", "\t");
                format = format.Replace("%T", "HH:mm:ss");
                //There's no way to specify this with SimpleDateFormat
                //format.Replace("%u","day of week ( 1-7 )");

                //There's no way to specify this with SimpleDateFormat
                //format.Replace("%U","week in year with first sunday as first day...");

                format = format.Replace("%V", "ww"); //I'm not sure this is always exactly the same

                //There's no way to specify this with SimpleDateFormat
                //format.Replace("%W","week in year with first monday as first day...");

                //There's no way to specify this with SimpleDateFormat
                //format.Replace("%w","E");
                format = format.Replace("%X", "HH:mm:ss");
                format = format.Replace("%x", "MM/dd/yy");
                format = format.Replace("%y", "yy");
                format = format.Replace("%Y", "yyyy");
                format = format.Replace("%Z", "z");
                format = format.Replace("%z", "Z");
                format = format.Replace("%%", "%");

                return format;
            }
        }

        static public void init()
        {
            pTable = new Hashtable();
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load("xml/converter.xml");
                XmlNodeList screenNodes = xDoc.GetElementsByTagName("source");

                foreach (XmlNode nodeScreen in screenNodes)
                {
                    Hashtable pSourceTable = new Hashtable();
                    ArrayList sourceName = new ArrayList();

                    XmlNodeList previewNodes = nodeScreen.ChildNodes;
                    foreach (XmlNode nodePreview in previewNodes)
                    {
                        if (nodePreview.Name.Equals("name"))
                            sourceName.Add(nodePreview.InnerText);
                        else if (nodePreview.Name.Equals("entries"))
                        {
                            XmlNodeList entryNodes = nodePreview.ChildNodes;
                            foreach (XmlNode nodeEntry in entryNodes)
                            {
                                if (nodeEntry.NodeType == XmlNodeType.Comment)
                                    continue;
                                String valueName = nodeEntry.Attributes["name"].Value;
                                String valueString = nodeEntry.Attributes["value"].Value;
                                String typeString = nodeEntry.Attributes["type"].Value;
                                Object value = nodeEntry.Attributes["value"] == null ? "" : nodeEntry.Attributes["value"].Value;
                                switch (typeString)
                                {
                                    case "bool":
                                        value = Boolean.Parse(nodeEntry.Attributes["value"].Value);
                                        break;
                                    case "int":
                                        value = Int32.Parse(nodeEntry.Attributes["value"].Value);
                                        break;
                                    case "string":
                                    default:
                                        value = nodeEntry.Attributes["value"].Value;
                                        break;
                                }
                                if (valueName.Length > 0)
                                    pSourceTable.Add(valueName, value);
                            }
                        }
                    }

                    foreach (Object source in sourceName)
                        if (((String)source).Length > 0)
                            pTable.Add(((String)source), pSourceTable);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            try
            {
                // Loading converterSimple
                pSimpleList = cConverterSimple.ListAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                pSimpleList = null;
                MessageBox.Show(ex.ToString());
            }

        }

        static public void reset()
        {
            text = "";
        }

        static public String getText(String Source, String Type, String Paramter)
        {

            if(pSimpleList != null && Source != null)
            {
                // search in converterSimple
                cConverterSimple result = pSimpleList.Find(s => s.Source.ToLower().Equals(Source.ToLower()) &&
                            s.ConverterName.ToLower().Equals(Type.ToLower()) &&
                            s.ConverterParameter.ToLower().Equals(Paramter.ToLower()));

                if (result != null)
                {
                    return result.Value;
                }
            }

            switch (Type)
            {
                case "ServiceName":
                    new ServiceName(Paramter, Source);
                    break;
                case "ServiceTime":
                    new ServiceTime(Paramter, Source);
                    break;
                case "ServiceInfo":
                    new ServiceInfo(Paramter, Source);
                    break;
                case "FrontendInfo":
                    new FrontendInfo(Paramter, Source);
                    break;
                case "EventName":
                    new EventName(Paramter, Source);
                    break;
                case "EventTime":
                    new EventTime(Paramter, Source);
                    break;
                case "ServicePosition":
                    new ServicePosition(Paramter, Source);
                    break;
                case "TunerInfo":
                    new TunerInfo(Paramter, Source);
                    break;
                case "KravenFHDEventTime": //MOD
                    new EventTime(Paramter, Source);
                    break;
                case "ValueBitTest":
                    new ValueBitTest(Paramter, Source);
                    break;
                case "ValueRange":
                    new ValueRange(Paramter, Source);
                    break;
                case "ClockToText":
                    new ClockToText(Paramter, Source);
                    return text;
                case "RemainingToText":
                    new RemainingToText(Paramter, Source);
                    return text;
                case "ProgressToText":
                    new ProgressToText(Paramter, Source);
                    return text;
                case "ConditionalShowHide":
                    if (cProperties.getPropertyBool("skinned_conditional_all"))
                        return "MAGIC#TRUE";
                    else if (cProperties.getPropertyBool("skinned_conditional_random"))
                    {
                        Random random = new Random();
                        if (random.Next(0, 2) == 0)
                            return "MAGIC#FALSE";
                        else
                            return "MAGIC#TRUE";
                    }
                    else if (!cProperties.getPropertyBool("skinned_conditional_all") && !cProperties.getPropertyBool("skinned_conditional_default"))
                        return "MAGIC#FALSE";
                    else if (boolean)
                        return "MAGIC#TRUE";
                    else
                        return "MAGIC#FALSE";
            }
            return text;
        }

        /* Idea all converter are related from a parent,
         * this parent contains variables value boolean time text
         * if several converter are used they can access these values
         */

        static private String text = "";
        static private int value = -1;
        static private int time = -1;
        static private bool boolean = false;
        static private int range = 0;

        //static private String returnVal = "";

        public class ServiceName
        {
            public int NAME = 0;
            public int PROVIDER = 1;
            public int REFERENCE = 2;

            private int type = 0;

            public ServiceName(String type, String Source)
            {
                if (type.Equals("Provider"))
                    this.type = this.PROVIDER;
                else if (type.Equals("Reference"))
                    this.type = this.REFERENCE;
                else
                    this.type = this.NAME;

                text = getText(Source);
            }

            public String getText(String Source)
            {
                if (pTable.Contains(Source))
                {
                    if (this.type == this.NAME)
                        return (String)((Hashtable)pTable[Source])["service_name"];
                    else if (this.type == this.PROVIDER)
                        return (String)((Hashtable)pTable[Source])["provider"];
                    else if (this.type == this.REFERENCE)
                        return (String)((Hashtable)pTable[Source])["reference"];
                    else
                        return "???";
                }
                else
                    return "NoConverterFound";
                
            }
        }

        public class ServiceTime
        {
            public int STARTTIME = 0;
            public int ENDTIME = 1;
            public int DURATION = 2;

            private int type = 0;

            public ServiceTime(String type, String Source)
            {
                if (type.Equals("StartTime"))
                    this.type = this.STARTTIME;
                else if (type.Equals("EndTime"))
                    this.type = this.ENDTIME;
                else
                    this.type = this.DURATION;

                text = getText(Source);
                time = getTime(Source);
            }

            public int getTime(String Source)
            {
                if (pTable.Contains(Source))
                {
                    if (this.type == this.STARTTIME)
                        return (int)((Hashtable)pTable[Source])["event_starttime"];
                    else if (this.type == this.ENDTIME)
                        return (int)((Hashtable)pTable[Source])["event_endtime"];
                    else if (this.type == this.DURATION)
                        return (int)((Hashtable)pTable[Source])["event_duration"];
                    else
                        return -1;
                }
                else
                    return -10;
                
            }

            public String getText(String Source)
            {
                if (pTable.Contains(Source))
                {
                if (this.type == this.STARTTIME)
                    return ((Hashtable)pTable[Source])["event_starttime"].ToString();
                else if (this.type == this.ENDTIME)
                    return ((Hashtable)pTable[Source])["event_endtime"].ToString();
                else if (this.type == this.DURATION)
                    return ((Hashtable)pTable[Source])["event_duration"].ToString();
                else
                    return "???";
                }
                else
                    return "NoConverterFound";
                
            }
        }

        public class ServicePosition
        {
            public int LENGTH = 0;
            public int POSITION = 1;
            public int REMAINING = 2;
            public int GAUGE = 3;

            private int type = 0;

            bool negate = false;
            bool detailed = false;
            bool showHours = false;
            bool showNoSeconds = false;

            public ServicePosition(String type, String Source)
            {
                String[] argsS = type.Split(',');
                List<String> args = new List<String>(argsS.Length);
                args.AddRange(argsS);

                type = args[0];
                type.Remove(0);

                negate = args.Contains("Negate");
                detailed = args.Contains("Detailed");
                showHours = args.Contains("ShowHours");
                showNoSeconds = args.Contains("ShowNoSeconds");

                if (type.Equals("Length"))
                    this.type = this.LENGTH;
                else if (type.Equals("Position"))
                    this.type = this.POSITION;
                else if (type.Equals("Remaining"))
                    this.type = this.REMAINING;
                else if (type.Equals("Gauge"))
                    this.type = this.GAUGE;
                else
                    this.type = -1;

                text = getText(Source);
            }


            public String getText(String Source)
            {
                Int32 l = 0;
                String sign = "";
                if (this.type == this.LENGTH)
                    l = ((Int32)((Hashtable)pTable[Source])["type_length"]);
                else if (this.type == this.POSITION)
                    l = ((Int32)((Hashtable)pTable[Source])["type_position"]);
                else if (this.type == this.REMAINING)
                    l = (((Int32)((Hashtable)pTable[Source])["type_length"]) - ((Int32)((Hashtable)pTable[Source])["type_position"]));

                if (!detailed)
                    l /= 90000;

                if (negate)
                    l = -l;

                if (l > 0)
                    sign = "";
                else
                {
                    l = -l;
                    sign = "-";
                }

                if (!detailed)
                {
                    if (showHours)
                    {
                        if (showNoSeconds)
                            return sign + String.Format("{0}:{1:00}", l / 3600, l % 3600 / 60);
                        else
                            return sign + String.Format("{0}:{1:00}:{2:00}", l / 3600, l % 3600 / 60, l % 60);
                    }
                    else
                    {
                        if (showNoSeconds)
                            return sign + String.Format("{0}", l / 60);
                        else
                            return sign + String.Format("{0}:{1:00}", l / 60, l % 60);
                    }
                }
                else
                {
                    if (showHours)
                        return sign + String.Format("{0}:{1:00}:{2:00}:{3:000}", (l / 3600 / 90000), (l / 90000) % 3600 / 60, (l / 90000) % 60, (l % 90000) / 90);
                    else
                        return sign + String.Format("{0}:{1:00}:{2:000}", (l / 60 / 90000), (l / 90000) % 60, (l % 90000) / 90);
                }
            }
        }

        public class ServiceInfo
        {
            public int HAS_TELETEXT = 0;
            public int IS_MULTICHANNEL = 1;
            public int IS_CRYPTED = 2;
            public int IS_WIDESCREEN = 3;
            public int SUBSERVICES_AVAILABLE = 4;
            public int XRES = 5;
            public int YRES = 6;
            public int APID = 7;
            public int VPID = 8;
            public int PCRPID = 9;
            public int PMTPID = 10;
            public int TXTPID = 11;
            public int TSID = 12;
            public int ONID = 13;
            public int SID = 14;

            private int type = 0;

            public ServiceInfo(String type, String Source)
            {
                if (type.Equals("HasTelext"))
                    this.type = this.HAS_TELETEXT;
                else if (type.Equals("IsMultichannel"))
                    this.type = this.IS_MULTICHANNEL;
                else if (type.Equals("IsCrypted"))
                    this.type = this.IS_CRYPTED;
                else if (type.Equals("IsWidescreen"))
                    this.type = this.IS_WIDESCREEN;
                else if (type.Equals("SubservicesAvailable"))
                    this.type = this.SUBSERVICES_AVAILABLE;
                else if (type.Equals("VideoWidth"))
                    this.type = this.XRES;
                else if (type.Equals("VideoHeight"))
                    this.type = this.YRES;
                else if (type.Equals("VideoWidth"))
                    this.type = this.XRES;
                else if (type.Equals("AudioPid"))
                    this.type = this.APID;
                else if (type.Equals("VideoPid"))
                    this.type = this.VPID;
                else if (type.Equals("PcrPid"))
                    this.type = this.PCRPID;
                else if (type.Equals("PmtPid"))
                    this.type = this.PMTPID;
                else if (type.Equals("TxtPid"))
                    this.type = this.TXTPID;
                else if (type.Equals("TsId"))
                    this.type = this.TSID;
                else if (type.Equals("OnId"))
                    this.type = this.ONID;
                else if (type.Equals("Sid"))
                    this.type = this.SID;

                boolean = getBoolean(Source);
                text = getText(Source);
                value = getValue(Source);
            }

            public bool getBoolean(String Source)
            {
                if (this.type == this.HAS_TELETEXT)
                    return (bool)((Hashtable)pTable[Source])["hasteletext"];
                else if (this.type == this.IS_MULTICHANNEL)
                    return (bool)((Hashtable)pTable[Source])["ismultichannel"];
                else if (this.type == this.IS_CRYPTED)
                    return (bool)((Hashtable)pTable[Source])["iscrypted"];
                else if (this.type == this.IS_WIDESCREEN)
                    return (bool)((Hashtable)pTable[Source])["iswidescreen"];
                else if (this.type == this.SUBSERVICES_AVAILABLE)
                    return (bool)((Hashtable)pTable[Source])["subservicesavailable"];
                else
                    return false;
            }

            public String getText(String Source)
            {
                if (this.type == this.XRES)
                    return ((Hashtable)pTable[Source])["videowidth"].ToString();
                else if (this.type == this.YRES)
                    return ((Hashtable)pTable[Source])["videoheight"].ToString();
                else
                    return "";
            }

            public int getValue(String Source)
            {
                if (this.type == this.XRES)
                    return (int)((Hashtable)pTable[Source])["videowidth"];
                else if (this.type == this.YRES)
                    return (int)((Hashtable)pTable[Source])["videoheight"];
                else
                    return -1;
            }

        }

        public class FrontendInfo
        {
            public int BER = 0;
            public int SNR = 1;
            public int AGC = 2;
            public int LOCK = 3;
            public int SNRdB = 4;
            public int SLOT_NUMBER = 5;
            public int TUNER_TYPE = 6;

            private int type = 0;

            public FrontendInfo(String type, String Source)
            {
                if (type.Equals("BER"))
                    this.type = this.BER;
                else if (type.Equals("SNR"))
                    this.type = this.SNR;
                else if (type.Equals("AGC"))
                    this.type = this.AGC;
                else if (type.Equals("SNRdB"))
                    this.type = this.SNRdB;
                else if (type.Equals("NUMBER"))
                    this.type = this.SLOT_NUMBER;
                else if (type.Equals("TYPE"))
                    this.type = this.TUNER_TYPE;
                else
                    this.type = this.LOCK;

                boolean = getBoolean(Source);
                text = getText(Source);
                value = getValue(Source);
                range = 65536;
            }

            public bool getBoolean(String Source)
            {
                if (this.type == this.LOCK)
                    return (bool)((Hashtable)pTable[Source])["lock"];
                else
                    return ((int)((Hashtable)pTable[Source])["ber"]) > 0 ? true : false;
            }

            public String getText(String Source)
            {
                if (this.type == this.BER)
                    return ((int)((Hashtable)pTable[Source])["ber"]) == -1 ? "N/A" : ((int)((Hashtable)pTable[Source])["ber"]).ToString();
                else if (this.type == this.AGC)
                    return ((int)((Hashtable)pTable[Source])["agc"]) + " %";
                else if (this.type == this.SNR)
                    return ((int)((Hashtable)pTable[Source])["snr"]) + " %";
                else if (this.type == this.SNRdB)
                {
                    if (((int)((Hashtable)pTable[Source])["snrdb"]) != -1)
                        return String.Format("{0:###.00} db", ((int)((Hashtable)pTable[Source])["snrdb"]) / 100.0);
                    else
                        return ((int)((Hashtable)pTable[Source])["snr"]) + " %";
                }
                else if (this.type == this.TUNER_TYPE)
                {
                    switch ((int)((Hashtable)pTable[Source])["tuner_type"])
                    {
                        case 0:
                            return "DVB-S";
                        case 1:
                            return "DVB-C";
                        case 2:
                            return "DVB-T";
                        default:
                            return "Unknown";
                    }
                }
                else
                    return "";
            }

            public int getValue(String Source)
            {
                if (this.type == this.AGC)
                    return (int)((Hashtable)pTable[Source])["agc"];
                else if (this.type == this.SNR)
                    return (int)((Hashtable)pTable[Source])["snr"];
                else if (this.type == this.BER)
                    return (int)((Hashtable)pTable[Source])["ber"];
                else if (this.type == this.TUNER_TYPE)
                    return (int)((Hashtable)pTable[Source])["tuner_type"];
                else if (this.type == this.SLOT_NUMBER)
                    return (int)((Hashtable)pTable[Source])["slot_number"];
                else
                    return -1;
            }

        }

        public class EventName
        {
            public int NAME = 0;
            public int SHORT_DESCRIPTION = 1;
            public int EXTENDED_DESCRIPTION = 2;
            public int ID = 3;
            public int FULL_DESCRIPTION = 2;

            private int type = 0;

            public EventName(String type, String Source)
            {
                if (type.Equals("Description"))
                    this.type = this.SHORT_DESCRIPTION;
                else if (type.Equals("ShortDescription"))
                    this.type = this.SHORT_DESCRIPTION;
                else if (type.Equals("ExtendedDescription"))
                    this.type = this.EXTENDED_DESCRIPTION;
                else if (type.Equals("FullDescription"))
                    this.type = this.FULL_DESCRIPTION;
                else if (type.Equals("ID"))
                    this.type = this.ID;
                else
                    this.type = this.NAME;

                text = getText(Source);
            }

            public String getText(String Source)
            {
                if (pTable.Contains(Source))
                {
                    if (this.type == this.NAME)
                        return (String)((Hashtable)pTable[Source])["service_name"];
                    else if (this.type == this.SHORT_DESCRIPTION)
                        return (String)((Hashtable)pTable[Source])["short_description"];
                    else if (this.type == this.EXTENDED_DESCRIPTION)
                        return (String)((Hashtable)pTable[Source])["extended_description"];
                    else if (this.type == this.FULL_DESCRIPTION)
                        return (String)((Hashtable)pTable[Source])["full_description"];
                    else if (this.type == this.ID)
                        return (String)((Hashtable)pTable[Source])["id"];
                    else
                        return "???";
                }
                else
                    return "NoConverterFound";
                                
            }
        }

        public class EventTime
        {
            public int STARTTIME = 0;
            public int ENDTIME = 1;
            public int REMAINING = 2;
            public int PROGRESS = 3;
            public int DURATION = 4;

            private int type = 0;

            public EventTime(String type, String Source)
            {
                if (type.Equals("EndTime"))
                    this.type = this.ENDTIME;
                else if (type.Equals("Remaining"))
                    this.type = this.REMAINING;
                else if (type.Equals("StartTime"))
                    this.type = this.STARTTIME;
                else if (type.Equals("Duration"))
                    this.type = this.DURATION;
                else if (type.Equals("Progress"))
                    this.type = this.PROGRESS;
                else
                    this.type = this.STARTTIME;

                time = getTime(Source);
                value = getValue(Source);
                range = 1000;
            }

            private int getTime(String Source)
            {
                if (this.type == this.STARTTIME)
                {
                    return (int)((Hashtable)pTable[Source])["event_starttime"];
                }
                else if (this.type == this.ENDTIME)
                {
                    return (int)((Hashtable)pTable[Source])["event_endtime"];
                }
                else if (this.type == this.DURATION)
                {
                    return (int)((Hashtable)pTable[Source])["event_duration"];
                }
                else if (this.type == this.REMAINING)
                {
                    return (int)((Hashtable)pTable[Source])["event_remaining"];
                }
                else
                    return -1;
            }

            private int getValue(String Source)
            {
                if (this.type == this.PROGRESS)
                    return (int)((Hashtable)pTable[Source])["event_progress"];
                return -1;
            }
        }

        public class TunerInfo
        {
            public int FE_USE_MASK = 0;

            private int type = 0;

            public TunerInfo(String type, String Source)
            {
                if (type.Equals("TunerUseMask"))
                    this.type = this.FE_USE_MASK;

                boolean = getBoolean(Source);
                text = getText(Source);
                value = getValue(Source);

                //returnVal = text;
            }

            private bool getBoolean(String Source)
            {
                if (this.type == this.FE_USE_MASK)
                {
                    if ((int)((Hashtable)pTable[Source])["tunerusemask"] > 0)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }

            private String getText(String Source)
            {
                if (this.type == this.FE_USE_MASK)
                    return ((Hashtable)pTable[Source])["tunerusemask"].ToString();
                else
                    return "";
            }

            private int getValue(String Source)
            {
                if (this.type == this.FE_USE_MASK)
                    return (int)((Hashtable)pTable[Source])["tunerusemask"];
                return -1;
            }
        }


        public class ValueBitTest
        {
            private int testValue = 0;

            public ValueBitTest(String testValue, String Source)
            {
                this.testValue = int.Parse(testValue);

                boolean = getBoolean(Source);

                //returnVal = "";
            }

            public bool getBoolean(String Source)
            {
                //self.source.value & self.value and True or False
                if ((value & this.testValue) == this.testValue)
                    return true;
                else
                    return false;
            }
        }

        public class ValueRange
        {
            private int lower = 0;
            private int upper = 0;

            public ValueRange(String testValue, String Source)
            {
                String[] values = testValue.Split(new char[] { ',' }, 2);
                this.lower = int.Parse(values[0]);
                this.upper = int.Parse(values[1]);

                boolean = getBoolean(Source);

                //returnVal = "";
            }

            public bool getBoolean(String Source)
            {
                if (this.lower <= this.upper)
                    return (this.lower <= value && value <= this.upper);
                else
                    return (!(this.upper < value && value < this.lower));
            }
        }


        public class ClockToText
        {
            public int DEFAULT = 0;
            public int WITH_SECONDS = 1;
            public int IN_MINUTES = 2;
            public int DATE = 3;
            public int FORMAT = 4;
            public int AS_LENGTH = 5;
            public int SHORT_FULL_DATE = 6;

            private int type = 0;
            private String fmt_string = "";

            public ClockToText(String type, String Source)
            {
                if (type.Equals("WithSeconds"))
                    this.type = this.WITH_SECONDS;
                else if (type.Equals("InMinutes"))
                    this.type = this.IN_MINUTES;
                else if (type.Equals("Date"))
                    this.type = this.DATE;
                else if (type.Equals("ShortFullDate"))
                    this.type = this.SHORT_FULL_DATE;
                else if (type.Equals("AsLength"))
                    this.type = this.AS_LENGTH;
                else if (type.StartsWith("Format"))
                {
                    this.type = this.FORMAT;
                    this.fmt_string = type.Substring(7);
                }
                else
                    this.type = this.DEFAULT;

                text = getText(Source);

                time = -1; //reset time
            }

            public String getText(String Source)
            {

                //int time = (int)((Hashtable)pTable[Source])["time_sec"];
                if (time == -1)
                {
                    time = (int)((Hashtable)pTable[Source])["time_sec"];

                    // Current time to UNIX timestamp
                    if (time == -2)
                    {
                        // http://dotnet-snippets.de/dns/c-datum-in-unix-timestamp-wandeln-SID165.aspx
                        DateTime date1 = new DateTime(1970, 1, 1);  //Refernzdatum (festgelegt)
                        DateTime date2 = DateTime.Now;              //jetztiges Datum / Uhrzeit
                        TimeSpan ts = new TimeSpan(date2.Ticks - date1.Ticks);  // das Delta ermitteln
                        // Das Delta als gesammtzahl der sekunden ist der Timestamp
                        time = (Convert.ToInt32(ts.TotalSeconds));
                    }
                    else if (time == -1 || time == 0)
                        return "";
                }

                if (this.type == this.IN_MINUTES)
                    return String.Format("{0} min", (time / 60));
                else if (this.type == this.AS_LENGTH)
                    return String.Format("{0}:{01}", (time / 60), (time % 60));

                // UNIX timestamp to current time
                // http://dotnet-snippets.de/dns/unix-timestamp-in-datum-wandeln-SID164.aspx
                DateTime t = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
                // den Timestamp addieren           
                t = t.AddSeconds(time);

                if (this.type == this.WITH_SECONDS)
                    return String.Format("{0:00}:{1:00}:{2:00}", t.Hour, t.Minute, t.Second);
                else if (this.type == this.DEFAULT)
                    return String.Format("{0:t}", t);
                else if (this.type == this.DATE)
                {
                    return t.ToString(strftime.ToString("%A %B %d, %Y"));
                }
                else if (this.type == this.SHORT_FULL_DATE)
                {
                    return t.ToString(strftime.ToString("%a %b %d, %Y"));
                }
                else if (this.type == this.FORMAT)
                {
                    int spos = this.fmt_string.IndexOf('%');
                    if (spos > 0)
                    {
                        String s1 = this.fmt_string.Substring(0, spos);
                        String s2 = this.fmt_string.Substring(spos);
                        String s3 = t.ToString(strftime.ToString(s2));
                        return s1 + s3;
                    }
                    else
                    {
                        return t.ToString(strftime.ToString(this.fmt_string));
                    }
                }
                else
                    return "???";
            }
        }

        public class RemainingToText
        {
            public int DEFAULT = 0;
            public int WITH_SECONDS = 1;
            public int NO_SECONDS = 2;
            public int IN_SECONDS = 3;

            private int type = 0;

            public RemainingToText(String type, String Source)
            {
                if (type.Equals("WithSeconds"))
                    this.type = this.WITH_SECONDS;
                else if (type.Equals("NoSeconds"))
                    this.type = this.NO_SECONDS;
                else if (type.Equals("InSeconds"))
                    this.type = this.IN_SECONDS;

                else
                    this.type = this.DEFAULT;

                text = getText(Source);

                time = -1; //reset time
            }

            public String getText(String Source)
            {
                int remaining = time;
                int duration = time;

                if (this.type == this.WITH_SECONDS)
                {
                    if (remaining != -1)
                        return String.Format("{0}:{1:00}:{2:00}", remaining / 3600, (remaining / 60) - ((remaining / 3600) * 60), remaining % 60);
                    else
                        return String.Format("{0:00}:{1:00}:{2:00}", duration / 3600, (duration / 60) - ((duration / 3600) * 60), duration % 60);
                }
                else if (this.type == this.NO_SECONDS)
                {
                    if (remaining != -1)
                        return String.Format("+{0}:{1:00}", remaining / 3600, (remaining / 60) - ((remaining / 3600) * 60));
                    else
                        return String.Format("+{0}:{1:00}", duration / 3600, (duration / 60) - ((duration / 3600) * 60));
                }
                else if (this.type == this.IN_SECONDS)
                {
                    if (remaining != -1)
                        return remaining.ToString();
                    else
                        return duration.ToString();
                }
                else if (this.type == this.DEFAULT)
                {
                    if (remaining != -1)
                        return String.Format("+{0} min", remaining / 60);
                    else
                        return String.Format("+{0} min", duration / 60);
                }
                else
                    return "???";
            }
        }

        public class ProgressToText
        {
            public int DEFAULT = 0;
            public int IN_PERCENT = 1;

            private int type = 0;

            public ProgressToText(String type, String Source)
            {
                text = getText(Source);
            }

            public String getText(String Source)
            {
                int r = range;
                int v = value;

                if (this.type == this.IN_PERCENT)
                {
                    return String.Format("{0} %", v * 100 / r);
                }
                else
                    return String.Format("{0} / {1}", v, r);
            }
        }
    }
}
