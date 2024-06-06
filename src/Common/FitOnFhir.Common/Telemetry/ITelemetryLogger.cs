// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace Microsoft.Health.FitOnFhir.Common.Telemetry
{
    public interface ITelemetryLogger
    {
        void LogMetric(Metric metric, double metricValue);

        void LogError(Exception ex);

        void LogTrace(string message);
    }
}
