using GensetDataProcessor.Models;

namespace GensetDataProcessor.CalculationEngines
{
    internal interface ICalculationEngine
    {
        GenerationOutput Calculate(GenerationReport generationReport);
    }
}