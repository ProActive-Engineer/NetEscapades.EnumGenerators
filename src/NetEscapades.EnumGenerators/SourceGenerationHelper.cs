using System.Text;

namespace NetEscapades.EnumGenerators;

public static class SourceGenerationHelper
{
    private const string Header = @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the NetEscapades.EnumGenerators source generator
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#nullable enable";

    public static string GenerateExtensionClass(StringBuilder sb, EnumToGenerate enumToGenerate)
    {
        sb.Append(Header);

        if (!string.IsNullOrEmpty(enumToGenerate.Namespace))
        {
            sb.Append(@"
namespace ").Append(enumToGenerate.Namespace);
        }

        sb.Append(@"
{
    ").Append(enumToGenerate.IsPublic ? "public" : "internal").Append(@" static partial class ").Append(enumToGenerate.Name).Append(@"
    {
        public static bool IsDefined(this ").Append(enumToGenerate.FullyQualifiedName).Append(@" value)
            => value switch
            {");
        foreach (var member in enumToGenerate.Values)
        {
            sb.Append(@"
                ").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key)
                .Append(" => true,");
        }
        sb.Append(@"
                _ => false,
            };

        public static string ToStringFast(this ").Append(enumToGenerate.FullyQualifiedName).Append(@" value)
            => value switch
            {");
        foreach (var member in enumToGenerate.Values)
        {
            sb.Append(@"
                ").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key)
                .Append(" => nameof(")
                .Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append("),");
        }

        sb.Append(@"
                _ => value.ToString(),
            };

        public static bool TryParse(string name, bool ignoreCase, out ").Append(enumToGenerate.FullyQualifiedName).Append(@" value)
            => ignoreCase ? TryParseIgnoreCase(name, out value) : TryParse(name, out value);

        private static bool TryParseIgnoreCase(string name, out ").Append(enumToGenerate.FullyQualifiedName).Append(@" value)
        {
            switch (name)
            {");
        foreach (var member in enumToGenerate.Values)
        {
            sb.Append(@"
                case { } s when s.Equals(nameof(").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(@"), System.StringComparison.OrdinalIgnoreCase):
                    value = ").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(@";
                    return true;");
        }

        sb.Append(@"
                default:
                    value = default;
                    return false;
            }
        }

        public static bool TryParse(string name, out ").Append(enumToGenerate.FullyQualifiedName).Append(@" value)
        {
            switch (name)
            {");
        foreach (var member in enumToGenerate.Values)
        {
            sb.Append(@"
                case nameof(").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(@"):
                    value = ").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(@";
                    return true;");
        }

        sb.Append(@"
                default:
                    value = default;
                    return false;
            }
        }

        public static ").Append(enumToGenerate.FullyQualifiedName).Append(@"[] GetValues()
        {
            return new[]
            {");
        foreach (var member in enumToGenerate.Values)
        {
            sb.Append(@"
                ").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(',');
        }
        sb.Append(@"
            };
        }

        public static string[] GetNames()
        {
            return new[]
            {");
        foreach (var member in enumToGenerate.Values)
        {
            sb.Append(@"
                nameof(").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append("),");
        }
        sb.Append(@"
            };
        }
    }
}");

        return sb.ToString();
    }
}