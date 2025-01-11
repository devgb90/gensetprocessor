using GensetDataProcessor.Models;

namespace GensetDataProcessor.CalculationEngines
{
    /// <summary>
    /// Interface for calculation implementations
    /// </summary>
    public interface ICalculationEngine
    {
        /// <summary>
        /// Perform calculations and create output
        /// </summary>
        /// <param name="generationReport"></param>
        /// <returns></returns>
        GenerationOutput Calculate(GenerationInput generationReport);
    }
}