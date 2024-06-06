// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace Microsoft.Health.FitOnFhir.Common.Telemetry
{
    public interface IExceptionTelemetryProcessor
    {
        /// <summary>
        /// Evaluates if the exception is handleable, i.e., can be continued upon.
        /// The associated exception metric is logged.
        /// </summary>
        /// <param name="ex">Exception that is to be evaluated as handleable or not.</param>
        /// <param name="telemetryLogger">Telemetry logger used to log the exception/metric.</param>
        /// <returns>Returns true if the exception is handleable, i.e., can be continued upon. False otherwise.</returns>
        bool HandleException(Exception ex, ITelemetryLogger telemetryLogger);

        /// <summary>
        /// Logs the exception metric for the supplied exception.
        /// If the exception is of type ITelemetryFormattable, then the associated error metric is logged, if not the passed in exceptionMetric is logged.
        /// </summary>
        /// <param name="ex">Exception for which the metric is to be logged.</param>
        /// <param name="telemetryLogger">Telemetry logger used to log the exception/metric.</param>
        /// <param name="exceptionMetric">Exception metric that is to be logged if the passed in exception is not of type ITelemetryFormattable.</param>
        void LogExceptionMetric(Exception ex, ITelemetryLogger telemetryLogger, Metric exceptionMetric = null);
    }
}
