using GensetDataProcessor.Models;
using Microsoft.Extensions.Logging;

namespace GensetDataProcessor.CalculationEngines
{
    /// <summary>
    /// Generator analytics calculator as per specification in the coding challenge
    /// </summary>
    public class GeneratorAnalyticsEngine : ICalculationEngine
    {
        private readonly ILogger logger;
        private readonly MappedFactors mappedFactors;

        public GeneratorAnalyticsEngine(ILogger<GeneratorAnalyticsEngine> logger, ReferenceDataProvider referenceDataProvider)
        {
            this.logger = logger;
            this.mappedFactors = referenceDataProvider.GetReferenceDataAsync().Result;
        }

        public GenerationOutput Calculate(GenerationInput generationReport)
        {
            logger.LogInformation("Starting calculations...");
            var finalOutput = new GenerationOutput();

            // Set value factor and emission factor for all generators as per its type
            // Also set (enum)GeneratorType to be able to find Coal generators for Actual Heat Rate calculations. Could be avoided, but added for the sake of clarity

            // Wind
            generationReport.Wind.WindGenerators.ForEach(x =>
            {
                var isOffshore = string.Equals(x.Location, "Offshore", StringComparison.OrdinalIgnoreCase);
                x.ValueFactor = isOffshore ? mappedFactors.OffshoreWindValueFactor : mappedFactors.OnshoreWindValueFactor;
                x.EmissionFactor = 0;
                x.GeneratorType = isOffshore ? GeneratorType.WindOffshore : GeneratorType.WindOnshore;
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

            // Flatten all lists of generators in one list
            var allGenerators = generationReport.Wind.WindGenerators
                                .Union(generationReport.Gas.GasGenerators)
                                .Union(generationReport.Coal.CoalGenerators);

            // Totals calculation
            foreach (var gen in allGenerators)
            {
                var output = new GeneratorOutput { Name = gen.Name, TotalGeneration = 0 };
                foreach (var day in gen.Generation.Days)
                {
                    // Calculate generation
                    output.TotalGeneration += (day.Energy * day.Price * gen.ValueFactor);

                    // Calculate emission and store it in the generator object itself for daily emissions calculation
                    day.Emission = day.Energy * gen.EmissionsRating * gen.EmissionFactor;
                }
                finalOutput.Totals.Generators.Add(output);
            }

            // Daily emissions calculation
            finalOutput.MaxEmissionGenerators.EmissionDays = allGenerators
                            // Flatten all days and select date, emission and generator name
                            .SelectMany(t => t.Generation.Days, (generator, day) => new { day.Date, day.Emission, generator.Name })
                            // Group by date
                            .GroupBy(t => t.Date)
                            // Create one entry per group with the generator info where emission is max
                            .Select(t => new EmissionDay
                            {
                                Date = t.Key,
                                Name = t.MaxBy(t => t.Emission)?.Name!,
                                Emission = t.Max(t => t.Emission)
                            }).ToList();
            // Actual heat rate calculation
            finalOutput.ActualHeatRates.ActualHeatRateList = allGenerators
                        // Filter generator of only coal type
                        .Where(t => t.GeneratorType == GeneratorType.Coal).ToList()
                        // calculate actual heat rate for all generators
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