using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace StalTran
{
    public delegate void ItemStateChanged(bool isModified);
    public delegate void ItemStatsChanged(string lang, WordStats.Stats stats);

    public abstract class XmlItem
    {
        public bool usable { get; set; } = false;

        public abstract void Load(XmlReader xmlReader);
        public abstract void Store(XmlWriter xmlWriter);
    }

    public class XmlDeclarationItem : XmlItem
    {
        private string value;
        public override void Load(XmlReader xmlReader) { value = xmlReader.Value; }
        public override void Store(XmlWriter xmlWriter) { xmlWriter.WriteProcessingInstruction("xml", value); }
    }

    public class CommentItem : XmlItem
    {
        private string value;
        public override void Load(XmlReader xmlReader) { value = xmlReader.Value; }
        public override void Store(XmlWriter xmlWriter) { xmlWriter.WriteComment(value); }
    }

    public class WhitespaceItem : XmlItem
    {
        private string value;
        public override void Load(XmlReader xmlReader) { value = xmlReader.Value; }
        public override void Store(XmlWriter xmlWriter) { xmlWriter.WriteWhitespace(value); }
    }

    public class TextItem : XmlItem
    {
        private string value;
        public override void Load(XmlReader xmlReader) { value = xmlReader.Value; }
        public override void Store(XmlWriter xmlWriter) { xmlWriter.WriteString(value); }
    }

    /*public class RootItem : XmlItem
    {
        private string value;
        public override void Load(XmlReader xmlReader) { value = xmlReader.Value; }
        public override void Store(XmlWriter xmlWriter) { xmlWriter.WriteWhitespace(value); }
    }*/

    public class WordStats
    {
        public struct Stats
        {
            public int letters { get; private set; }
            public int whites { get; private set; }
            public int digits { get; private set; }
            public int puncts { get; private set; }

            public Stats(Stats prev, Stats curr)
            {
                letters = curr.letters - prev.letters;
                whites = curr.whites - prev.whites;
                digits = curr.digits - prev.digits;
                puncts = curr.puncts - prev.puncts;
            }

            public void recalc(string s)
            {
                letters = s.Count(char.IsLetter);
                digits = s.Count(char.IsDigit);
                whites = s.Count(char.IsWhiteSpace);
                puncts = s.Count(char.IsPunctuation);
            }

            public void recalc(Stats diff)
            {
                letters += diff.letters;
                whites += diff.whites;
                digits += diff.digits;
                puncts += diff.puncts;
            }
        }

        public Stats prev = new Stats();
        public Stats curr = new Stats();

        public WordStats(string s)
        {
            curr.recalc(s);
            prev = curr;
        }

        public void recalc(string s)
        {
            prev = curr;
            curr.recalc(s);
        }

        public Stats diff()
        {
            return new Stats(prev, curr);
        }
    }

    public class LangItem
    {
        public LangItem(string str, bool from_file)
        {
            orig = curr = str;
            translationRequired = false;
            fromFile = from_file;
            modified = false;
            stats = new WordStats(str);
        }

        public LangItem(LangItem item)
        {
            orig = item.orig;
            curr = item.curr;
            translationRequired = item.translationRequired;
            modified = item.modified;
            fromFile = false;
            stats = new WordStats(curr);
        }

        public string orig { get; set; }
        public string curr { get; set; }
        public bool translationRequired { get; set; }
        public bool modified { get; set; }
        public bool fromFile { get; set; }
        public WordStats stats { get; set; }

        public bool mustBeStored()
        {
            return fromFile || curr.StartsWith("===") || curr != orig;
        }
    }

    public class LangComparer : IComparer<string>
    {
        public int Compare(string a, string b)
        {
            if (a.CompareTo(b) != 0)
            {
                int idxA = CalcIndex(a);
                int idxB = CalcIndex(b);
                if (idxA == 3 && idxB == 3)
                    return a.CompareTo(b);
                else
                    return idxA.CompareTo(idxB);
            }
            else
                return 0;
        }

        public static int CalcIndex(string s)
        {
            if (s == "rus") return 0;
            if (s == "eng") return 1;
            if (s == "ukr") return 2;
            return 3;
        }
    }

    public interface INotificationReceiver
    {
        void translationRequired(string lang, int incdec);
        void statsChanged(string lang, WordStats.Stats stats);
        void stateChanged(bool isChanged);
    }

    public class Item : XmlItem
    {
        public string id { get; private set; }

        //! Actual values
        private SortedDictionary<string, LangItem> _texts = new SortedDictionary<string, LangItem>(new LangComparer());

        //! Sets new text value under language
        public void set(string lang, string text)
        {
            //! Update text
            LangItem item = _texts[lang];
            item.curr = text;
            item.modified = item.orig != item.curr;
            CheckModified(item.modified);

            //! Update stats
            item.stats.recalc(text);
            notificationReceiver.statsChanged(lang, item.stats.diff());
        }

        //! Gets text value for language. If it doesn't exist, creates it out of rus text
        public string get(string lang)
        {
            return getItem(lang).curr;
        }

        public LangItem getItem(string lang)
        {
            LangItem item;
            if (_texts.TryGetValue(lang, out item))
                return item;

            //! Not there yet
            item = new LangItem(_texts["rus"]);
            _texts[lang] = item;
            return item;
        }
        public bool translationRequired(string lang) { return getItem(lang).translationRequired; }

        private Dictionary<string, List<XmlItem>> cw_lang = new Dictionary<string, List<XmlItem>>();
        private List<XmlItem> cw_post;
        private INotificationReceiver notificationReceiver;
        private bool modified = false;

        public Item(INotificationReceiver notificationReceiver)
        {
            usable = true;
            this.notificationReceiver = notificationReceiver;
        }

        public void CheckTranslation()
        {
            foreach(KeyValuePair<string, LangItem> kv in _texts)
            {
                bool prevState = kv.Value.translationRequired;
                if (kv.Key == "ukr")
                    kv.Value.translationRequired = kv.Value.curr.StartsWith("=") || Regex.Matches(kv.Value.curr, @"[ыЫёЁъЪэЭ]").Count > 0;
                else
                    kv.Value.translationRequired = kv.Value.curr.StartsWith("=") || Regex.Matches(kv.Value.curr, @"[а-яА-ЯыЫъЪьЬёЁ]").Count > 0;

                if (prevState != kv.Value.translationRequired)
                    notificationReceiver.translationRequired(kv.Key, prevState ? -1 : 1);
            }
        }

        private void CheckModified(bool newlyModified)
        {
            if (modified != newlyModified)
            {
                notificationReceiver.stateChanged(newlyModified);
            }
            modified = newlyModified;
        }

        public override void Load(XmlReader xmlReader)
        {
            //! Read root node
            xmlReader.Read();
            id = xmlReader.GetAttribute("id");

            List<XmlItem> cw = new List<XmlItem>();
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Comment:
                        {
                            XmlItem item = new CommentItem();
                            item.Load(xmlReader);
                            cw.Add(item);
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        {
                            XmlItem item = new WhitespaceItem();
                            item.Load(xmlReader);
                            cw.Add(item);
                        }
                        break;
                    case XmlNodeType.Element:
                        {
                            string name = xmlReader.Name;
                            xmlReader.Read();//! Move to content

                            string value = fromXml(xmlReader.Value);
                            if (name != "rus" && (value == null || value == "==="))
                            {
                                value = "===" + _texts["rus"].orig;
                                _texts[name] = new LangItem(value, false);
                            }
                            else
                            {
                                _texts[name] = new LangItem(value, true);
                            }
                            cw_lang[name] = cw;
                            cw = new List<XmlItem>();
                        }
                        break;
                    default:
                        break;
                };
            }
            cw_post = cw;
            CheckTranslation();
        }

        public override void Store(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("string");
            xmlWriter.WriteAttributeString("id", id);

            foreach (KeyValuePair<string, LangItem> kv in _texts)
            {
                //! Store if:
                //!  1. rus or 
                //!  2. differs from rus and not === only or
                //!  3. was orifinally in file (do not erase what was there)
                if (kv.Value.mustBeStored())
                {
                    List<XmlItem> cw = null;
                    if (!cw_lang.TryGetValue(kv.Key, out cw))
                        cw = cw_lang["rus"];
                    foreach (XmlItem i in cw)
                        i.Store(xmlWriter);
                    xmlWriter.WriteElementString(kv.Key, toXml(kv.Value.curr));
                }
                kv.Value.orig = kv.Value.curr;
                kv.Value.fromFile = true;
            }

            foreach (XmlItem i in cw_post)
                i.Store(xmlWriter);

            xmlWriter.WriteEndElement();

            CheckModified(false);
        }

        private string fromXml(string src)
        {
            return src.Replace("" + (char)160, "~");
        }

        private string toXml(string src)
        {
            return src.Replace("~", "" + (char)160);
        }
    }

    public class StringTable : INotificationReceiver
    {
        internal List<XmlItem> header = new List<XmlItem>();
        internal List<XmlItem> items { get; private set; }
        internal List<XmlItem> footer = new List<XmlItem>();

        public bool headless { get; set; }
        public string path { get; set; }
        private int modifiedCount = 0;

        private Dictionary<string, WordStats.Stats> fileStats = new Dictionary<string, WordStats.Stats>();
        private Dictionary<string, int> translationRequired = new Dictionary<string, int>();
        private ItemStateChanged itemStateChangedHandler;
        private ItemStatsChanged itemStatsChangedHandler;

        public StringTable(string path, ItemStateChanged itemStateChangedHandler, ItemStatsChanged itemStatsChangedHandler)
        {
            this.path = path;
            this.itemStateChangedHandler = itemStateChangedHandler;
            this.itemStatsChangedHandler = itemStatsChangedHandler;
        }

        public void Store()
        {
            if (items.Count == 0)
                return;

            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.Encoding = new UTF8Encoding(false);
            writerSettings.Indent = true;
            writerSettings.IndentChars = "\t";
            writerSettings.ConformanceLevel = headless ? ConformanceLevel.Fragment : ConformanceLevel.Document;

            XmlWriter writer = XmlWriter.Create(path, writerSettings);

            foreach (XmlItem i in header)
                i.Store(writer);

            if (!headless)
                writer.WriteStartElement("string_table");

            foreach (XmlItem i in items)
                i.Store(writer);

            if (!headless)
                writer.WriteEndElement();

            foreach (XmlItem i in footer)
                i.Store(writer);

            writer.Close();
        }

        public static StringTable Load(string path, ItemStateChanged itemStateChangedHandler, ItemStatsChanged itemStatsChangedHandler)
        {
            try
            {
                StreamReader fileStream = new StreamReader(path, Encoding.UTF8);
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                XmlReader xmlReader = XmlReader.Create(fileStream, settings);
                StringTable result = new StringTable(path, itemStateChangedHandler, itemStatsChangedHandler);

                result.Load(xmlReader);
                fileStream.Close();
                return result;
            }
            catch (FileNotFoundException)
            {
                return new StringTable(path, itemStateChangedHandler, itemStatsChangedHandler);
            }
        }

        public bool HasModifications()
        {
            return modifiedCount > 0;
        }

        public WordStats.Stats getFileStats(string lang)
        {
            WordStats.Stats stats;
            if (fileStats.TryGetValue(lang, out stats))
                return stats;
            stats = new WordStats.Stats();
            fileStats[lang] = stats;
            return stats;
        }

        public int getTranslationRequired(string lang)
        {
            if (translationRequired.ContainsKey(lang))
                return translationRequired[lang];
            translationRequired[lang] = 0;
            return 0;
        }

        private void Load(XmlReader xmlReader)
        {
            items = new List<XmlItem>();
            List<XmlItem> active = header;
            headless = true;

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.XmlDeclaration:
                        {
                            XmlItem item = new XmlDeclarationItem();
                            item.Load(xmlReader);
                            active.Add(item);
                        }
                        break;
                    case XmlNodeType.Comment:
                        {
                            XmlItem item = new CommentItem();
                            item.Load(xmlReader);
                            active.Add(item);
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        {
                            XmlItem item = new WhitespaceItem();
                            item.Load(xmlReader);
                            active.Add(item);
                        }
                        break;
                    case XmlNodeType.Element:
                        {
                            if (xmlReader.Name == "string_table")
                            {
                                headless = false;
                                LoadItems(xmlReader.ReadSubtree(), true);
                            }
                            else
                            {
                                LoadItems(xmlReader, false);
                            }
                            active = footer;
                        }
                        break;
                    default:
                        break;
                };
            }
        }

        private void LoadItems(XmlReader xmlReader, bool isStringTable)
        {
            bool skipFirstRead = !isStringTable;

            if (isStringTable)
                xmlReader.Read();//! Skip root node
                
            while (skipFirstRead || xmlReader.Read())
            {
                skipFirstRead = false;
                XmlItem item = null;
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Comment:
                        {
                            item = new CommentItem();
                            item.Load(xmlReader);
                        }
                        break;
                    case XmlNodeType.Element:
                        {
                            item = new Item(this);
                            item.Load(xmlReader.ReadSubtree());
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        {
                            item = new WhitespaceItem();
                            item.Load(xmlReader);
                        }
                        break;
                    case XmlNodeType.EndElement:
                        continue;
                    case XmlNodeType.Text:
                        {
                            item = new TextItem();
                            item.Load(xmlReader);
                        }
                        break;
                    default:
                        throw new XmlException("Unsupported xml element " + xmlReader.NodeType.ToString());
                };
                
                items.Add(item);
            }
        }

        void INotificationReceiver.stateChanged(bool isChanged)
        {
            modifiedCount += (isChanged ? 1 : -1);
            itemStateChangedHandler(modifiedCount > 0);
        }

        void INotificationReceiver.statsChanged(string lang, WordStats.Stats diff)
        {
            WordStats.Stats stats;
            if (!fileStats.TryGetValue(lang, out stats))
            {
                stats = new WordStats.Stats();
                fileStats[lang] = stats;
            }
            stats.recalc(diff);
            itemStatsChangedHandler(lang, stats);
        }

        void INotificationReceiver.translationRequired(string lang, int incdec)
        {
            if (!translationRequired.ContainsKey(lang))
                translationRequired[lang] = 0;
            
            int currValue = translationRequired[lang];
            currValue += incdec;
            if (currValue < 0)
                currValue = 0;

            translationRequired[lang] = currValue;
        }
    }
}
