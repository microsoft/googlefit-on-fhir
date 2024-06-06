// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using EnsureThat;
using Microsoft.Health.FitOnFhir.Common.Interfaces;

namespace Microsoft.Health.FitOnFhir.Common.Telemetry
{
    public class ExceptionTelemetryProcessor : IExceptionTelemetryProcessor
    {
        private readonly HashSet<Type> _handledExceptions;

        public ExceptionTelemetryProcessor()
        {
            _handledExceptions = new HashSet<Type>();
        }

        public ExceptionTelemetryProcessor(params Type[] handledExceptionTypes)
        {
            _handledExceptions = new HashSet<Type>(handledExceptionTypes);
        }

        public virtual bool HandleException(
            Exception ex,
            ITelemetryLogger telemetryLogger)
        {
            return HandleException(ex, telemetryLogger, handledExceptionMetric: null);
        }

        public virtual void LogExceptionMetric(
            Exception ex,
            ITelemetryLogger telemetryLogger,
            Metric exceptionMetric = null)
        {
            EnsureArg.IsNotNull(telemetryLogger, nameof(telemetryLogger));

            Metric metric = ex is ITelemetryFormattable tel ? tel.ToMetric : exceptionMetric;
            if (metric != null)
            {
                telemetryLogger.LogMetric(metric, metricValue: 1);
            }
        }

        /// <summary>
        /// Evaluates if the exception is handleable, i.e., can be continued upon.
        /// The associated exception metric is logged.
        /// </summary>
        /// <param name="ex">Exception that is to be evaluated as handleable or not.</param>
        /// <param name="telemetryLogger">Telemetry logger used to log the exception/metric.</param>
        /// <param name="handledExceptionMetric">Exception metric that is to be logged if the passed in exception is handled and not of type ITelemetryFormattable.</param>
        /// <returns>Returns true if the exception is handleable, i.e., can be continued upon. False otherwise.</returns>
        protected bool HandleException(
            Exception ex,
            ITelemetryLogger telemetryLogger,
            Metric handledExceptionMetric = null)
        {
            EnsureArg.IsNotNull(ex, nameof(ex));
            EnsureArg.IsNotNull(telemetryLogger, nameof(telemetryLogger));

            var exType = ex.GetType();
            var lookupType = exType.IsGenericType ? exType.GetGenericTypeDefinition() : exType;

            if (_handledExceptions.Contains(lookupType))
            {
                telemetryLogger.LogError(ex);
                LogExceptionMetric(ex, telemetryLogger, handledExceptionMetric);
                return true;
            }

            return false;
        }
    }
}
