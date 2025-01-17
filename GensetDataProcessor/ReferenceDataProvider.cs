﻿using GensetDataProcessor.Models;
using GensetDataProcessor.Parsers;
using Microsoft.Extensions.Configuration;

namespace GensetDataProcessor
{
    /// <summary>
    /// Provides reference data
    /// </summary>
    public class ReferenceDataProvider
    {
        private readonly IInputParser inputParser;
        private readonly IConfiguration configuration;
        private MappedFactors? mappedFactors = null;

        public ReferenceDataProvider(IConfiguration configuration, IInputParser parser)
        {
            this.inputParser = parser;
            this.configuration = configuration;
        }

        public async Task<MappedFactors> GetReferenceDataAsync()
        {
            // Read and parse file only when Mapped Factors are not yet calculated
            if (this.mappedFactors == null)
            {
                var refDataFilePath = this.configuration.GetValue<string>("ReferenceDataFilePath");
                if (string.IsNullOrEmpty(refDataFilePath) || !File.Exists(refDataFilePath))
                {
                    throw new ArgumentException("Reference data file path is invalid. Please check configuration");
                }
                this.mappedFactors = new MappedFactors(await this.inputParser.ParseAsync<ReferenceData>(refDataFilePath));
            }
            return mappedFactors;
        }
    }
}