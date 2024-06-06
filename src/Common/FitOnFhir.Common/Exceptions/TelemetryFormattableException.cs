// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using EnsureThat;
using Microsoft.Health.FitOnFhir.Common.Interfaces;
using Microsoft.Health.FitOnFhir.Common.Telemetry;

namespace Microsoft.Health.FitOnFhir.Common.Exceptions
{
    public class TelemetryFormattableException :
        Exception,
        ITelemetryFormattable
    {
        private readonly string _name;
        private readonly string _operation = "Unknown";

        public TelemetryFormattableException()
        {
        }

        public TelemetryFormattableException(string message)
            : base(message)
        {
        }

        public TelemetryFormattableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public TelemetryFormattableException(
            string message,
            Exception innerException,
            string name,
            string operation)
            : base(message, innerException)
        {
            _name = EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));
            _operation = EnsureArg.IsNotNullOrWhiteSpace(operation, nameof(operation));
        }

        public virtual string ErrType => ErrorType.GeneralError;

        public virtual string ErrSeverity => ErrorSeverity.Warning;

        public virtual string ErrSource => nameof(ErrorSource.Undefined);

        public virtual string ErrName => _name;

        public virtual string Operation => _operation;

        public Metric ToMetric => ErrName.ToErrorMetric(Operation, ErrType, ErrSeverity, ErrSource);
    }
}
