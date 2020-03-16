/**
 * @license
 * Internet Systems Consortium license
 *
 * Copyright (c) 2020 Maksym Sadovnychyy (MAKS-IT)
 * Website: https://maks-it.com
 * Email: commercial@maks-it.com
 *
 * Permission to use, copy, modify, and/or distribute this software for any purpose
 * with or without fee is hereby granted, provided that the above copyright notice
 * and this permission notice appear in all copies.
 *
 * THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH
 * REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND
 * FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT,
 * INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS
 * OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER
 * TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF
 * THIS SOFTWARE.
 */

/*
 * https://www.newtonsoft.com/json/help/html/CustomJsonConverter.htm
 */

using System;
using System.Management.Automation;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PSCmdlets {
    [Cmdlet(VerbsData.Convert, "ToJson")]
    [OutputType(typeof(string))]
    public class ConvertToJson : Cmdlet {
        [Parameter(Position = 1, Mandatory = true, ValueFromPipeline = true)]
        public PSObject psObj { get; set; }

        
        protected override void BeginProcessing() {
            base.BeginProcessing();
        }

        protected override void ProcessRecord() {
            try {
                string json = JsonConvert.SerializeObject(psObj, Formatting.Indented, new PSObjectJsonConverter(typeof(PSObject)));
                WriteObject(json);
            }
            catch (Exception ex)
            {
                WriteObject("ERR: " + ex.Message.ToString());
            }
        }
    }

    public class PSObjectJsonConverter : JsonConverter {

        private readonly Type[] _types;

        public PSObjectJsonConverter(params Type[] types) {
            _types = types;
        }

        public override void WriteJson(JsonWriter jsonWriter, object value, JsonSerializer jsonSerializer)
        {
            PSObject psObj = (PSObject)value;
            jsonWriter.WriteStartObject();
            DefaultContractResolver contractResolver = (DefaultContractResolver)jsonSerializer.ContractResolver;
            NamingStrategy namingStrategy = (contractResolver == null ? null : contractResolver.NamingStrategy) ?? new DefaultNamingStrategy();

            psObj.Properties.Where(p => p.IsGettable).ToList().ForEach(p => {
                    jsonWriter.WritePropertyName(namingStrategy.GetPropertyName(p.Name, false));
                    jsonSerializer.Serialize(jsonWriter, p.Value);
                }
            );

            jsonWriter.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        public override bool CanRead {
            get { return false; }
        }

        public override bool CanConvert(Type objectType) {
            return _types.Any(t => t == objectType);
        }
    }
}