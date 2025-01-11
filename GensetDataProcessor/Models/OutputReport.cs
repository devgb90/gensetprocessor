using System.Xml.Serialization;

namespace GensetDataProcessor.Models
{
    [XmlRoot("GenerationOutput")]
    public class GenerationOutput
    {
        public Totals Totals { get; set; } = new();
        public MaxEmissionGenerators MaxEmissionGenerators { get; set; } = new();
        public ActualHeatRates ActualHeatRates { get; set; } = new();
    }

    public class Totals
    {
        [XmlElement("Generator")]
        public List<GeneratorOutput> Generators { get; set; } = new();
    }

    public class GeneratorOutput
    {
        public string Name { get; set; }

        [XmlElement("Total")]
        public double TotalGeneration { get; set; }
    }

    public class MaxEmissionGenerators
    {
        [XmlElement("Day")] public List<EmissionDay> EmissionDays { get; set; } = new();
    }

    public class EmissionDay
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public double Emission { get; set; }
    }

    public class ActualHeatRates
    {
        [XmlElement("ActualHeatRate")] public List<ActualHeatRate> ActualHeatRateList { get; set; }
    }

    public class ActualHeatRate
    {
        public string Name { get; set; }
        public double HeatRate { get; set; }
    }
}