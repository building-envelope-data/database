using System.Text.RegularExpressions;
using HotChocolate;
using HotChocolate.Types;

// TODO Maybe use an enumeration as runtime type instead of string (and fallback to english when the given one does not exist).namespace Database.GraphQl
namespace Database.GraphQl
{
    /// <summary>
    /// [BCP 47](https://tools.ietf.org/html/bcp47)
    /// compliant
    /// [Language Tag](https://tools.ietf.org/html/bcp47#section-2)
    /// string like `"de-AT"`, `"sr-Latn-RS"`, `"en-US"`, or `"en-GB"`, where the language
    /// part is essentially an
    /// [ISO 639 Language Code](https://www.iso.org/iso-639-language-codes.html),
    /// the script an
    /// [ISO 15924:2004 Script Code](https://www.iso.org/standard/29546.html),
    /// and the region an
    /// [ISO 3166-1 Country Code](https://www.iso.org/iso-3166-country-codes.html).
    /// 
    /// Note that the
    /// [Internet Assigned Numbers Authority (IANA)](https://www.iana.org)
    /// maintains the
    /// [Language Subtag Registry](https://www.iana.org/assignments/lang-subtags-templates/lang-subtags-templates.xhtml).
    /// </summary>
    public class LocaleType
        : RegexType
    {
        private const string _validationPattern =
            "^[a-zA-Z0-9]+(-[a-zA-Z0-9]+)?(-[a-zA-Z0-9]+)?$";

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleType"/> class.
        /// </summary>
        public LocaleType()
            : this(
                nameof(LocaleType).Remove(nameof(LocaleType).Length - "Type".Length),
                "BCP 47 compliant Language Tag string"
            )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleType"/> class.
        /// </summary>
        public LocaleType(
            NameString name,
            string? description = null,
            BindingBehavior bind = BindingBehavior.Explicit)
            : base(
                name,
                _validationPattern,
                description,
                RegexOptions.Compiled | RegexOptions.IgnoreCase,
                bind)
        {
        }
    }
}
