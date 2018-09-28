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
    public delegate void ItemStatsChanged(WordStats.Stats wsUkr, WordStats.Stats wsEng);

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

    public class Item : XmlItem
    {
        public string id { get; private set; }

        private string _rus = "";
        private string _eng = "";
        private string _ukr = "";

        public string rus { get { return _rus; } set { _rus = value; CheckModified(); } }
        public string eng { get { return _eng; } set { _eng = value; CheckModified(); } }
        public string ukr { get { return _ukr; } set { _ukr = value; CheckModified(); } }

        private string rus_org = "";
        private string eng_org = "";
        private string ukr_org = "";

        public bool noE { get; private set; }
        public bool noU { get; private set; }
        
        private List<XmlItem> cw_rus;
        private List<XmlItem> cw_eng;
        private List<XmlItem> cw_ukr;
        private List<XmlItem> cw_post;

        private bool modified = false;

        public WordStats wsUkr { get; private set; }
        public WordStats wsEng { get; private set; }

        ItemStateChanged itemStateChangedHandler;
        ItemStatsChanged itemStatsChangedHandler;

        public Item(ItemStateChanged itemStateChangedHandler, ItemStatsChanged itemStatsChangedHandler)
        {
            usable = true;
            this.itemStateChangedHandler = itemStateChangedHandler;
            this.itemStatsChangedHandler = itemStatsChangedHandler;
            wsUkr = new WordStats("");
            wsEng = new WordStats("");
        }

        public void CheckTranslation()
        {
            noE = eng.StartsWith("=") || Regex.Matches(eng, @"[а-яА-ЯыЫъЪьЬёЁ]").Count > 0;
            noU = ukr.StartsWith("=") || Regex.Matches(ukr, @"[ыЫёЁъЪ]").Count > 0;
        }

        private void CheckModified()
        {
            bool newlyModified = rus != rus_org || eng != eng_org || ukr != ukr_org;
            if (modified != newlyModified)
            {
                itemStateChangedHandler(newlyModified);
            }
            wsUkr.recalc(ukr != null ? ukr : "");
            wsEng.recalc(eng != null ? eng : "");
            itemStatsChangedHandler(wsUkr.diff(), wsEng.diff());
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

                            if (name == "rus")
                            {
                                rus = rus_org = fromXml(xmlReader.Value);
                                cw_rus = cw;
                            }
                            else if (name == "eng")
                            {
                                eng = eng_org = fromXml(xmlReader.Value);
                                if (eng == null) eng = eng_org = "===";
                                cw_eng = cw;
                                wsEng = new WordStats(eng);
                            }
                            else if (name == "ukr")
                            {
                                ukr = ukr_org = fromXml(xmlReader.Value);
                                if (ukr == null) ukr = ukr_org = "===";
                                cw_ukr = cw;
                                wsUkr = new WordStats(ukr);
                            }
                            else if (name == "text")
                            {
                                rus = rus_org = fromXml(xmlReader.Value);
                                cw_rus = cw;
                            }
                            else
                                break;

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

            foreach (XmlItem i in cw_rus)
                i.Store(xmlWriter);
            xmlWriter.WriteElementString("rus", toXml(_rus));
            rus_org = _rus;

            foreach (XmlItem i in cw_eng)
                i.Store(xmlWriter);
            xmlWriter.WriteElementString("eng", toXml(_eng));
            eng_org = _eng;

            foreach (XmlItem i in cw_ukr)
                i.Store(xmlWriter);
            xmlWriter.WriteElementString("ukr", toXml(_ukr));
            ukr_org = _ukr;

            foreach (XmlItem i in cw_post)
                i.Store(xmlWriter);

            xmlWriter.WriteEndElement();

            CheckModified();
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

    public class StringTable
    {
        internal List<XmlItem> header = new List<XmlItem>();
        internal List<XmlItem> items { get; private set; }
        internal List<XmlItem> footer = new List<XmlItem>();

        public bool headless { get; set; }
        public string path { get; set; }
        private int modifiedCount = 0;

        public WordStats.Stats wsUkr = new WordStats.Stats();
        public WordStats.Stats wsEng = new WordStats.Stats();

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
            writerSettings.Encoding = Encoding.GetEncoding(1251);
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
                StreamReader fileStream = new StreamReader(path, Encoding.GetEncoding(1251));
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
                            item = new Item(ItemStateChangedHandler, ItemStatsChangedHandler);
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

        internal void ItemStateChangedHandler(bool isChanged)
        {
            modifiedCount += (isChanged ? 1 : -1);
            itemStateChangedHandler(modifiedCount > 0);
        }

        internal void ItemStatsChangedHandler(WordStats.Stats wsUkrDiff, WordStats.Stats wsEngDiff)
        {
            wsUkr.recalc(wsUkrDiff);
            wsEng.recalc(wsEngDiff);
            itemStatsChangedHandler(wsUkr, wsEng);
        }
    }
}
