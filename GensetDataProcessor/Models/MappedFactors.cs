namespace GensetDataProcessor.Models
{
    public class MappedFactors
    {
        public MappedFactors(ReferenceData data)
        {
            // Emission factors
            OffshoreWindEmissionFactor = 1;
            OnshoreWindEmissionFactor = 1;
            GasEmissionFactor = data.Factors.EmissionsFactor.Medium;
            CoalEmissionFactor = data.Factors.EmissionsFactor.High;

            // Value factors
            OffshoreWindValueFactor = data.Factors.ValueFactor.Low;
            OnshoreWindValueFactor = data.Factors.ValueFactor.High;
            GasValueFactor = data.Factors.ValueFactor.Medium;
            CoalValueFactor = data.Factors.ValueFactor.Medium;
        }

        public double OffshoreWindValueFactor { get; private set; }
        public double OffshoreWindEmissionFactor { get; private set; }
        public double OnshoreWindValueFactor { get; private set; }
        public double OnshoreWindEmissionFactor { get; private set; }
        public double GasValueFactor { get; private set; }
        public double GasEmissionFactor { get; private set; }
        public double CoalValueFactor { get; private set; }
        public double CoalEmissionFactor { get; private set; }
    }
}