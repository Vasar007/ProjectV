﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Acolyte.Assertions;
using CsvHelper;
using CsvHelper.Configuration;
using FileHelpers;
using ProjectV.Logging;

namespace ProjectV.IO.Input.File
{
    /// <summary>
    /// Provides simple and common methods to read data from files.
    /// </summary>
    public sealed class SimpleFileReader : IFileReader
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<SimpleFileReader>();

        /// <summary>
        /// Name of the column with Thing name.
        /// </summary>
        private readonly string _thingNameHeader;


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        /// <param name="thingNameHeader">Name of the header with Thing names.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="thingNameHeader" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="thingNameHeader" /> presents empty strings or contains only whitespaces.
        /// </exception>
        public SimpleFileReader(
            string thingNameHeader = "Thing Name")
        {
            _thingNameHeader = thingNameHeader.ThrowIfNullOrWhiteSpace(nameof(thingNameHeader));
        }

        #region IFileReader Implementation

        /// <inheritdoc />
        /// <remarks>
        /// File must contain specified column: <see cref="_thingNameHeader" />.
        /// </remarks>
        public IEnumerable<string> ReadFile(string filename)
        {
            _logger.Info($"Reading file \"{filename}\".");

            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var result = new HashSet<string>();

            using var engine = new FileHelperAsyncEngine<InputFileData>();
            using (engine.BeginReadFile(filename))
            {
                foreach (InputFileData record in engine)
                {
                    if (result.Add(record.thingName))
                    {
                        yield return record.thingName;
                    }
                }
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// File must contain specified column: <see cref="_thingNameHeader" />.
        /// </remarks>
        /// <exception cref="InvalidDataException">CSV file doesn't contain header.</exception>
        public IEnumerable<string> ReadCsvFile(string filename)
        {
            _logger.Info($"Reading CSV file \"{filename}\".");

            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var result = new HashSet<string>();

            var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = true
            };

            using var reader = new StreamReader(filename);
            using var csv = new CsvReader(reader, csvConfig);

            if (!csv.Read() || !csv.ReadHeader())
            {
                throw new InvalidDataException("CSV file doesn't contain header!");
            }
            while (csv.Read())
            {
                string thingName = csv[_thingNameHeader];
                if (result.Add(thingName))
                {
                    yield return thingName;
                }
            }
        }

        #endregion
    }
}
