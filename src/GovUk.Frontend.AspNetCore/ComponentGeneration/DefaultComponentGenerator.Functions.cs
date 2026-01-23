using System.Globalization;
using System.Text;
using Fluid;
using Fluid.Values;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable IDE0060
internal partial class DefaultComponentGenerator
{
    private static class Functions
    {
        public static FluidValue Array(FunctionArguments args, TemplateContext context)
        {
            var result = new List<FluidValue>();

            foreach (var item in args.Values)
            {
                result.Add(item);
            }

            return FluidValue.Create(result, context.Options);
        }

        public static FluidValue Dict(FunctionArguments args, TemplateContext context)
        {
            var result = new Dictionary<string, FluidValue>();

            foreach (var name in args.Names)
            {
                var value = args[name]!;
                result.Add(name, value);
            }

            return FluidValue.Create(result, context.Options);
        }

        public static async ValueTask<FluidValue> GovukAttributesAsync(FunctionArguments args, TemplateContext context)
        {
#pragma warning disable CA1849
#pragma warning disable VSTHRD103
            // https://github.com/alphagov/govuk-frontend/blob/v5.8.0/packages/govuk-frontend/src/govuk/macros/attributes.njk

            var attrsArg = args.At(0);
            string attributesHtml;

            if (attrsArg.Type == FluidValues.String)
            {
                attributesHtml = attrsArg.ToStringValue();
            }
            else if (attrsArg.Type is FluidValues.Object && attrsArg.ToObjectValue() is AttributeCollection attributeCollection)
            {
                using var writer = new StringWriter();

                foreach (var attribute in attributeCollection.GetAttributes())
                {
                    if (attribute.Optional && attribute.Value is false or null)
                    {
                        continue;
                    }

                    writer.Write(' ');
                    writer.Write(_encoder.Encode(attribute.Name));

                    if (!attribute.Optional || attribute.Value is not true)
                    {
                        writer.Write('=');
                        writer.Write('"');
                        attribute.WriteTo(writer, _encoder);
                        writer.Write('"');
                    }
                }

                attributesHtml = writer.ToString();
            }
            else if (attrsArg.Type is FluidValues.Dictionary)
            {
                using var writer = new StringWriter();

                foreach (var attributeKvp in attrsArg.Enumerate(context))
                {
                    var attributeName = (await attributeKvp.GetIndexAsync(NumberValue.Create(0), context)).ToStringValue();

                    bool optional = false;

                    FluidValue attributeValue;
                    var valueFluidValue = await attributeKvp.GetIndexAsync(NumberValue.Create(1), context);

                    if (valueFluidValue.Type == FluidValues.Dictionary)
                    {
                        attributeValue = await valueFluidValue.GetValueAsync("value", context);
                        optional = (await valueFluidValue.GetValueAsync("optional", context)).ToBooleanValue();
                    }
                    else
                    {
                        attributeValue = valueFluidValue;
                    }

                    if (optional && !attributeValue.ToBooleanValue())
                    {
                        continue;
                    }

                    writer.Write(' ');
                    writer.Write(_encoder.Encode(attributeName));

                    if (!optional || !attributeValue.Equals(BooleanValue.True))
                    {
                        writer.Write('=');
                        writer.Write('"');
                        writer.Write(_encoder.Encode(attributeValue.ToStringValue()));
                        writer.Write('"');
                    }
                }

                attributesHtml = writer.ToString();
            }
            else
            {
                attributesHtml = attrsArg.Type == FluidValues.Nil
                    ? string.Empty
                    : throw new InvalidOperationException($"Cannot convert {attrsArg.Type} to attributes.");
            }

            return new StringValue(attributesHtml, encode: false);
#pragma warning restore VSTHRD103
#pragma warning restore CA1849
        }

        public static FluidValue GovukI18nAttributes(FunctionArguments args, TemplateContext context)
        {
            var key = args["key"].ToStringValue();

            if (args.HasNamed("messages"))
            {
                var sb = new StringBuilder();

                foreach (var pluralRuleArray in args["messages"].Enumerate(context))
                {
                    var pluralRuleParts = pluralRuleArray.Enumerate(context).ToArray();
                    var pluralRule = pluralRuleParts[0].ToStringValue();
                    var message = pluralRuleParts[1].ToStringValue();

                    sb.Append(CultureInfo.InvariantCulture, $" data-i18n.{key}.{pluralRule}=\"{_encoder.Encode(message)}\"");
                }

                return new StringValue(sb.ToString(), encode: false);
            }

            if (args.HasNamed("message"))
            {
                var message = args["message"];

                if (!message.IsNil())
                {
                    var attr = $" data-i18n.{key}=\"{_encoder.Encode(args["message"].ToStringValue())}\"";
                    return new StringValue(attr, encode: false);
                }
            }

            return NilValue.Instance;
        }

        public static FluidValue IfElse(FunctionArguments args, TemplateContext context)
        {
            return args.At(0).ToBooleanValue() ? args.At(1) : args.At(2);
        }

        public static FluidValue IsTruthy(FunctionArguments args, TemplateContext context)
        {
            return FluidValue.Create(args.At(0).ToBooleanValue(), context.Options);
        }

        public static FluidValue Not(FunctionArguments args, TemplateContext context)
        {
            return FluidValue.Create(!args.At(0).ToBooleanValue(), context.Options);
        }

        public static StringValue String(FunctionArguments args, TemplateContext context)
        {
            return StringValue.Create(args.At(0).ToStringValue());
        }
    }
}
