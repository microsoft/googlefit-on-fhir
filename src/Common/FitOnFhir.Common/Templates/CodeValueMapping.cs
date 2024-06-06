﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace Microsoft.Health.FitOnFhir.Common.Templates
{
    public class CodeValueMapping
    {
#pragma warning disable CA2227
        [JsonProperty(Required = Required.Always)]
        public IList<FhirCode> Codes { get; set; }
#pragma warning restore CA2227

        public FhirValueType Value { get; set; }
    }
}
