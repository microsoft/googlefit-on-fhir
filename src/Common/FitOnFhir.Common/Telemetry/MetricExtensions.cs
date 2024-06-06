// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Diagnostics;
using EnsureThat;

namespace Microsoft.Health.FitOnFhir.Common.Telemetry
{
    public static class MetricExtensions
    {
        public static Metric AddDimension(this Metric metric, string dimensionName, string dimensionValue)
        {
            EnsureArg.IsNotNull(metric, nameof(metric));

            if (string.IsNullOrWhiteSpace(dimensionName) || string.IsNullOrWhiteSpace(dimensionValue))
            {
                return metric;
            }

            try
            {
                metric.Dimensions.Add(dimensionName, dimensionValue);
            }
            catch (ArgumentException ex)
            {
                Trace.TraceError($"An error occurred while adding dimension {dimensionName} to metric {metric.Name}. {ex}");
            }

            return metric;
        }

        public static Metric ToErrorMetric(this string metricName, string operation, string errorType, string errorSeverity, string errorSource = "", string errorName = "")
        {
            EnsureArg.IsNotNullOrWhiteSpace(metricName, nameof(metricName));
            EnsureArg.IsNotNullOrWhiteSpace(operation, nameof(operation));
            EnsureArg.IsNotNullOrWhiteSpace(errorType, nameof(errorType));
            EnsureArg.IsNotNullOrWhiteSpace(errorSeverity, nameof(errorSeverity));

            return new Metric(
                metricName,
                new Dictionary<string, object>
                {
                    { DimensionNames.Name, string.IsNullOrWhiteSpace(errorName) ? metricName : errorName },
                    { DimensionNames.Category, Category.Errors },
                    { DimensionNames.TotalErrors, 1 },
                    { DimensionNames.ErrorType, errorType },
                    { DimensionNames.ErrorSeverity, errorSeverity },
                    { DimensionNames.Operation, operation },
                })
                .AddDimension(DimensionNames.ErrorSource, errorSource);
        }
    }
}
