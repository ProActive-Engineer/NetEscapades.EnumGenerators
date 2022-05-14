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

    public const string Attribute = Header + @"

#if NETESCAPADES_ENUMGENERATORS_EMBED_ATTRIBUTES
namespace NetEscapades.EnumGenerators
{
    /// <summary>
    /// Add to enums to indicate that extension methods should be generated for the type
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Enum)]
    [System.Diagnostics.Conditional(""NETESCAPADES_ENUMGENERATORS_USAGES"")]
    public class EnumExtensionsAttribute : System.Attribute
    {
        /// <summary>
        /// The namespace to generate the extension class.
        /// If not provided the namespace of the enum will be used
        /// </summary>
        public string? ExtensionClassNamespace { get; set; }

        /// <summary>
        /// The name to use for the extension class.
        /// If not provided, the enum name with ""Extensions"" will be used.
        /// For example for an Enum called StatusCodes, the default name
        /// will be StatusCodesExtensions
        /// </summary>
        public string? ExtensionClassName { get; set; }
    }
}
#endif
";

    public static string GenerateExtensionClass(StringBuilder sb, EnumToGenerate enumToGenerate)
    {
        sb.Append(Header);
        if (!string.IsNullOrEmpty(enumToGenerate.Namespace))
        {
            sb.Append(@"
namespace ").Append(enumToGenerate.Namespace).Append(@"
{");
        }

        sb.Append(@"
    ").Append(enumToGenerate.IsPublic ? "public" : "internal").Append(@" static partial class ").Append(enumToGenerate.Name).Append(@"
    {
        /// <summary>
        /// The number of members in the enum.
        /// This is a non-distinct count of defined names.
        /// </summary>
        public const int Length = ").Append(enumToGenerate.Names.Count).Append(";").Append(@"

        public static string ToStringFast(this ").Append(enumToGenerate.FullyQualifiedName).Append(@" value)
            => value switch
            {");
        foreach (var member in enumToGenerate.Names)
        {
            sb.Append(@"
                ").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key)
                .Append(" => ");

            if (member.Value.DisplayName is null)
            {
                sb.Append("nameof(").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append("),");
            }
            else
            {
                sb.Append('"').Append(member.Value.DisplayName).Append(@""",");
            }
        }

        sb.Append(@"
                _ => value.ToString(),
            };");

        if (enumToGenerate.HasFlags)
        {
            sb.Append(@"

        public static bool HasFlag(this ").Append(enumToGenerate.FullyQualifiedName).Append(@" value, ").Append(enumToGenerate.FullyQualifiedName).Append(@" flag)
            => value switch
            {
                0  => flag.Equals(0),
                _ => (value & flag) != 0,
            };");
        }

        sb.Append(@"

       public static bool IsDefined(").Append(enumToGenerate.FullyQualifiedName).Append(@" value)
            => value switch
            {");
        foreach (var member in enumToGenerate.Names)
        {
            sb.Append(@"
                ").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key)
                .Append(" => true,");
        }

        sb.Append(@"
                _ => false,
            };");

        sb.Append(@"

        public static bool IsDefined(string name, bool allowMatchingMetadataAttribute)
        {");
        if (enumToGenerate.IsDisplayAttributeUsed)
        {
            sb.Append(@"
            var isDefinedInDisplayAttribute = false;
            if (allowMatchingMetadataAttribute)
            {
                isDefinedInDisplayAttribute = name switch
                {");
            foreach (var member in enumToGenerate.Names)
            {
                if (member.Value.DisplayName is not null && member.Value.IsDisplayNameTheFirstPresence)
                    {
                    sb.Append(@"
                    """).Append(member.Value.DisplayName).Append(@""" => true,");
                }
            }

            sb.Append(@"
                    _ => false,
                };
            }

            if (isDefinedInDisplayAttribute)
            {
                return true;
            }

            ");
        }
        sb.Append(@"
            return name switch
            {");
        foreach (var member in enumToGenerate.Names)
            {
             sb.Append(@"
                nameof(").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(@") => true,");
        }

        sb.Append(@"
                _ => false,
            };
        }");

        sb.Append(@"

        public static bool TryParse(
#if NETCOREAPP3_0_OR_GREATER
            [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
#endif
            string? name, 
            out ").Append(enumToGenerate.FullyQualifiedName).Append(@" value)
            => TryParse(name, out value, false, false);");
        sb.Append(@"

        public static bool TryParse(
#if NETCOREAPP3_0_OR_GREATER
            [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
#endif
            string? name, 
            out ").Append(enumToGenerate.FullyQualifiedName).Append(@" value,
            bool ignoreCase) 
            => TryParse(name, out value, ignoreCase, false);");
        sb.Append(@"

        public static bool TryParse(
#if NETCOREAPP3_0_OR_GREATER
            [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
#endif
            string? name, 
            out ").Append(enumToGenerate.FullyQualifiedName).Append(@" value, 
            bool ignoreCase, 
            bool allowMatchingMetadataAttribute)
        {");

        if (enumToGenerate.IsDisplayAttributeUsed)
        {
            sb.Append(@"
            if (allowMatchingMetadataAttribute)
            {
                if (ignoreCase)
                {
                    switch (name)
                    {");
            foreach (var member in enumToGenerate.Names)
            {
                if (member.Value.DisplayName is not null && member.Value.IsDisplayNameTheFirstPresence)
                {
                    sb.Append(@"
                        case string s when s.Equals(""").Append(member.Value.DisplayName).Append(@""", System.StringComparison.OrdinalIgnoreCase):
                            value = ").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(@";
                            return true;");
                }
            }

            sb.Append(@"
                        default:
                            break;
                    };
                }
                else
                {
                    switch (name)
                    {");
            foreach (var member in enumToGenerate.Names)
            {
                if (member.Value.DisplayName is not null && member.Value.IsDisplayNameTheFirstPresence)
                {
                    sb.Append(@"
                        case """).Append(member.Value.DisplayName).Append(@""":
                            value = ").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(@";
                            return true;");
                }
            }

            sb.Append(@"
                        default:
                            break;
                    };
                }
            }");
        }

        sb.Append(@"

            if (ignoreCase)
            {
                switch (name)
                {");
        foreach (var member in enumToGenerate.Names)
        {
            sb.Append(@"
                    case string s when s.Equals(nameof(").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(@"), System.StringComparison.OrdinalIgnoreCase):
                        value = ").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(@";
                        return true;");
        }

        sb.Append(@"
                    case string s when ").Append(enumToGenerate.UnderlyingType).Append(@".TryParse(name, out var val):
                        value = (").Append(enumToGenerate.FullyQualifiedName).Append(@")val;
                        return true;
                    default:
                        value = default;
                        return false;
                }
            }
            else
            {
                switch (name)
                {");
        foreach (var member in enumToGenerate.Names)
        {
            sb.Append(@"
                    case nameof(").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(@"):
                        value = ").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(@";
                        return true;");
        }

        sb.Append(@"
                    case string s when ").Append(enumToGenerate.UnderlyingType).Append(@".TryParse(name, out var val):
                        value = (").Append(enumToGenerate.FullyQualifiedName).Append(@")val;
                        return true;
                    default:
                        value = default;
                        return false;
                }
            }
        }");

        sb.Append(@"

        public static ").Append(enumToGenerate.FullyQualifiedName).Append(@"[] GetValues()
        {
            return new[]
            {");
        foreach (var member in enumToGenerate.Names)
        {
            sb.Append(@"
                ").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(',');
        }

        sb.Append(@"
            };
        }");

        sb.Append(@"

        public static string[] GetNames()
        {
            return new[]
            {");
        foreach (var member in enumToGenerate.Names)
        {
            sb.Append(@"
                nameof(").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append("),");
        }

        sb.Append(@"
            };
        }
    }");
        if (!string.IsNullOrEmpty(enumToGenerate.Namespace))
        {
            sb.Append(@"
}");
        }

        return sb.ToString();
    }
}