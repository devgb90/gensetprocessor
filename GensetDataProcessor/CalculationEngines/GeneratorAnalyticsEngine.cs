using GensetDataProcessor.Models;
using Microsoft.Extensions.Logging;

namespace GensetDataProcessor.CalculationEngines
{
    internal class GeneratorAnalyticsEngine : ICalculationEngine
    {
        private ILogger logger;
        private MappedFactors mappedFactors;

        public GeneratorAnalyticsEngine(ILogger<GeneratorAnalyticsEngine> logger, ReferenceDataProvider referenceDataProvider)
        {
            this.logger = logger;
            this.mappedFactors = referenceDataProvider.GetReferenceDataAsync().Result;
        }

        public GenerationOutput Calculate(GenerationReport generationReport)
        {
            logger.LogInformation("Starting calculations...");

            // Wind
            generationReport.Wind.WindGenerators.ForEach(x =>
            {
                var isOffshore = string.Equals(x.Location, "Offshore", StringComparison.OrdinalIgnoreCase);
                x.ValueFactor = isOffshore ? mappedFactors.OffshoreWindValueFactor : mappedFactors.OnshoreWindValueFactor;
                x.GeneratorType = isOffshore ? GeneratorType.WindOffshore : GeneratorType.WindOnshore;
                x.EmissionFactor = 0;
            });

            // Coal
            generationReport.Coal.CoalGenerators.ForEach(x =>
            {
                x.ValueFactor = mappedFactors.CoalValueFactor;
                x.EmissionFactor = mappedFactors.CoalEmissionFactor;
                x.GeneratorType = GeneratorType.Coal;
            });

            // Gas
            generationReport.Gas.GasGenerators.ForEach(x =>
            {
                x.ValueFactor = mappedFactors.GasValueFactor;
                x.EmissionFactor = mappedFactors.GasEmissionFactor;
                x.GeneratorType = GeneratorType.Gas;
            });
            var finalOutput = new GenerationOutput();
            var allGenerators = generationReport.Wind.WindGenerators
                                .Union(generationReport.Gas.GasGenerators)
                                .Union(generationReport.Coal.CoalGenerators);

            // Totals calculation
            foreach (var gen in allGenerators)
            {
                var output = new GeneratorOutput { Name = gen.Name, TotalGeneration = 0 };
                foreach (var day in gen.Generation.Days)
                {
                    output.TotalGeneration += (day.Energy * day.Price * gen.ValueFactor);
                    day.Emission = day.Energy * gen.EmissionsRating * gen.EmissionFactor;
                }
                finalOutput.Totals.Generators.Add(output);
            }

            // Daily emissions calculation
            finalOutput.MaxEmissionGenerators.EmissionDays = allGenerators
                            .SelectMany(t => t.Generation.Days, (generator, day) => new { day.Date, day.Emission, generator.Name })
                            .GroupBy(t => t.Date)
                            .Select(t => new EmissionDay
                            {
                                Date = t.Key,
                                Name = t.OrderByDescending(x => x.Emission).First().Name,
                                Emission = t.OrderByDescending(x => x.Emission).First().Emission
                            }).ToList();

            finalOutput.ActualHeatRates.ActualHeatRateList = allGenerators
                        .Where(t => t.GeneratorType == GeneratorType.Coal).ToList()
                        .Select(t => new ActualHeatRate
                        {
                            Name = t.Name,
                            HeatRate = t.TotalHeatInput / t.ActualNetGeneration
                        }).ToList();

            logger.LogInformation("Completed!");
            return finalOutput;
        }
    }
}