// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace Microsoft.Health.FitOnFhir.Common.Exceptions
{
    public class ThirdPartyLoggedFormattableException : TelemetryFormattableException
    {
        public ThirdPartyLoggedFormattableException()
        {
            this.SetLogForwarding(true);
        }

        public ThirdPartyLoggedFormattableException(string message)
            : base(message)
        {
            this.SetLogForwarding(true);
        }

        public ThirdPartyLoggedFormattableException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.SetLogForwarding(true);
        }

        public ThirdPartyLoggedFormattableException(
            string message,
            Exception innerException,
            string name,
            string operation)
            : base(message, innerException, name, operation)
        {
            this.SetLogForwarding(true);
        }
    }
}
