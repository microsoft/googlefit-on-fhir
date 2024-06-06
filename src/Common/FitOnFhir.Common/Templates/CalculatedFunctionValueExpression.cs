// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Microsoft.Health.FitOnFhir.Common.Serialization;
using Newtonsoft.Json;

namespace Microsoft.Health.FitOnFhir.Common.Templates
{
    public class CalculatedFunctionValueExpression : LineAwareJsonObject
    {
        [JsonProperty(Required = Newtonsoft.Json.Required.Always)]
        public string ValueName { get; set; }

        [JsonProperty(Required = Newtonsoft.Json.Required.Always)]
        [JsonConverter(typeof(TemplateExpressionJsonConverter))]
        public TemplateExpression ValueExpression { get; set; }

        public bool Required { get; set; }
    }
}
