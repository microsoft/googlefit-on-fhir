﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using EnsureThat;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.Health.FitOnFhir.Common.Templates
{
    public class TemplateExpression : LineAwareJsonObject
    {
        public TemplateExpression()
        {
        }

        public TemplateExpression(string value, TemplateExpressionLanguage language)
        {
            Value = EnsureArg.IsNotNullOrWhiteSpace(value, nameof(value));
            Language = language;
        }

        [JsonProperty(Required = Required.Always)]
        public virtual string Value { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TemplateExpressionLanguage? Language { get; set; }
    }
}
