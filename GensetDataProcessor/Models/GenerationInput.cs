using System.Xml.Serialization;

namespace GensetDataProcessor.Models
{
    [XmlRoot("GenerationReport")]
    public class GenerationInput
    {
        public Wind Wind { get; set; }
        public Gas Gas { get; set; }
        public Coal Coal { get; set; }
    }

    public class Wind
    {
        [XmlElement("WindGenerator")]
        public List<Generator> WindGenerators { get; set; }
    }

    public class Gas
    {
        [XmlElement("GasGenerator")]
        public List<Generator> GasGenerators { get; set; }
    }

    public class Coal
    {
        [XmlElement("CoalGenerator")]
        public List<Generator> CoalGenerators { get; set; }
    }

    public class Generation
    {
        [XmlElement("Day")]
        public List<Day> Days { get; set; }
    }

    public class Day
    {
        public DateTime Date { get; set; }
        public double Energy { get; set; }
        public double Price { get; set; }
        public double Emission { get; set; }
    }

    public class Generator
    {
        public string Name { get; set; }
        public Generation Generation { get; set; }
        public double TotalHeatInput { get; set; }
        public double ActualNetGeneration { get; set; }
        public double EmissionsRating { get; set; }
        public string Location { get; set; }
        public double ValueFactor { get; set; }
        public double EmissionFactor { get; set; }
        public GeneratorType GeneratorType { get; set; }
    }

    public enum GeneratorType
    {
        WindOffshore,
        WindOnshore,
        Coal,
        Gas
    }
}