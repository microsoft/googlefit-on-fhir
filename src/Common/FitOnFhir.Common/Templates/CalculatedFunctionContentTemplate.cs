﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Microsoft.Health.FitOnFhir.Common.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.Health.FitOnFhir.Common.Templates
{
    public class CalculatedFunctionContentTemplate : MappingTemplate
    {
        [JsonProperty(Required = Required.Always)]
        [JsonConverter(typeof(TemplateExpressionJsonConverter))]
        public virtual TemplateExpression TypeMatchExpression { get; set; }

        [JsonProperty(Required = Required.Always)]
        [JsonConverter(typeof(TemplateExpressionJsonConverter))]
        public virtual TemplateExpression DeviceIdExpression { get; set; }

        [JsonConverter(typeof(TemplateExpressionJsonConverter))]
        public virtual TemplateExpression PatientIdExpression { get; set; }

        [JsonConverter(typeof(TemplateExpressionJsonConverter))]
        public virtual TemplateExpression EncounterIdExpression { get; set; }

        [JsonProperty(Required = Required.Always)]
        [JsonConverter(typeof(TemplateExpressionJsonConverter))]
        public virtual TemplateExpression TimestampExpression { get; set; }

        [JsonConverter(typeof(TemplateExpressionJsonConverter))]
        public virtual TemplateExpression CorrelationIdExpression { get; set; }

#pragma warning disable CA2227
        public virtual IList<CalculatedFunctionValueExpression> Values { get; set; }
#pragma warning restore CA2227

        [JsonConverter(typeof(StringEnumConverter))]
        public virtual TemplateExpressionLanguage DefaultExpressionLanguage { get; set; } = TemplateExpressionLanguage.JsonPath;
    }
}
