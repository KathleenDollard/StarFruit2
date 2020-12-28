using Microsoft.CodeAnalysis;
using StarFruit2.Common;
using System.Linq;
using System.Xml.Linq;

namespace StarFruit2
{
    public class DescriptionFromXmlComments : IDescriptionProvider
    {
        static DescriptionFromXmlComments()
        {
            Provider = new DescriptionFromXmlComments();
        }
        public static IDescriptionProvider Provider { get; private set; }


        public static string? XmlComments(ISymbol symbol)
        {
            var xml = symbol.GetDocumentationCommentXml();
            if (string.IsNullOrWhiteSpace(xml) || xml!.StartsWith("<!-- Bad"))
            {
                return null;
            }
            var xDoc = XElement.Parse(xml);
            // Assume correct structure or quit
            return (xDoc.FirstNode as XElement)?.Value.Trim();
        }

        public static string? XmlComments(IParameterSymbol symbol)
        {
            // for now, assume correct structure
            var methodSymbol = symbol.ContainingSymbol as IMethodSymbol;
            Assert.NotNull(methodSymbol);
            var xml = methodSymbol.GetDocumentationCommentXml();
            if (string.IsNullOrWhiteSpace(xml))
            {
                return null;
            }
            var xDoc = XElement.Parse(xml);
            var element = xDoc.Elements("param")
                              .Where<XElement>(n => n.Attributes("name").First<XAttribute>().Value == symbol.Name)
                              .FirstOrDefault();
            if (element is null)
            {
                return null;
            }
            return element.Value.Trim();
        }

        public string? GetDescription<T>(T source, string route = null)
        {
            return source switch
            {
                IParameterSymbol t => XmlComments(t),
                ISymbol t => XmlComments(t),
                _ => null
            };
        }
    }
}
