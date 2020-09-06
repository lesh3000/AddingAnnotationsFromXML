using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace DocuViewareREST.Models
{
    [XmlRoot(ElementName = "Wd")]
    public class Wd
    {
        [XmlElement(ElementName = "Txt")]
        public string Txt { get; set; }
        [XmlElement(ElementName = "Tp")]
        public string Tp { get; set; }
        [XmlElement(ElementName = "Lt")]
        public string Lt { get; set; }
        [XmlElement(ElementName = "Ht")]
        public string Ht { get; set; }
        [XmlElement(ElementName = "Wt")]
        public string Wt { get; set; }
    }

    [XmlRoot(ElementName = "Pg")]
    public class Pg
    {
        [XmlElement(ElementName = "Wd")]
        public List<Wd> Wd { get; set; }
        [XmlAttribute(AttributeName = "PgNo")]
        public string PgNo { get; set; }
    }

    [XmlRoot(ElementName = "Doc")]
    public class Doc
    {
        [XmlElement(ElementName = "Pg")]
        public Pg Pg { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "No")]
        public string No { get; set; }
    }

    [XmlRoot(ElementName = "Batch")]
    public class Batch
    {
        [XmlElement(ElementName = "Doc")]
        public Doc Doc { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }
    
}