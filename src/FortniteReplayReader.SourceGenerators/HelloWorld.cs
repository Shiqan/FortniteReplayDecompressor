using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Unreal.Core.Models.Enums;

namespace SourceGeneratorSamples
{
    [Generator]
    public class HelloWorldGenerator : ISourceGenerator
    {
        public void Execute(SourceGeneratorContext context)
        {
            var visitor = new GetAllSymbolsVisitor();
            visitor.Visit(context.Compilation.GlobalNamespace);

            // begin creating the source we'll inject into the users compilation
            var sourceBuilder = new StringBuilder(@"
using Unreal.Core.Attributes;
using Unreal.Core.Models.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;
using System;

namespace HelloWorldGenerated
{
    [NetFieldExportGroup(""/Script/FortniteGame.FortClientObservedStat"", minimalParseMode: ParseMode.Debug)]
    public partial class RandomClass : INetFieldExportGroup
    {
        public object Role { get; set; }
        public float? StormCNDamageVulnerabilityLevel2 { get; set; }

        public void Hello() {
            Console.WriteLine(""--------------- START"");
");

            foreach (var sym in visitor.Symbols)
            {
                sourceBuilder.Append(TestAdapters(sym));
                sourceBuilder.Append($@"Console.WriteLine(""\n"");");
            }

            sourceBuilder.Append($@"Console.WriteLine(""--------------- END"");");

            sourceBuilder.Append(@"
        }
    }    
}");

            context.AddSource("helloWorldGenerated", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));


            context.AddSource("NetFieldParserGenerated", SourceText.From(CreateNetFieldParser(visitor.Symbols), Encoding.UTF8));

            foreach (var sym in visitor.Symbols)
            {
                context.AddSource($"{sym.Name}AdapterGenerated", SourceText.From(AddReadFieldMethod(sym), Encoding.UTF8));
            }
        }

        public void Initialize(InitializationContext context)
        {
            //
        }

        private string CreateNetFieldParser(IEnumerable<INamedTypeSymbol> symbols)
        {
            StringBuilder source = new StringBuilder(@"
            using Unreal.Core.Models.Contracts;
            using Unreal.Core.Models;
            using System.Collections.Generic;
            ");

            var netFieldExportGroups = symbols.Where(symbol => symbol.GetAttributes().Any(attr => attr.AttributeClass.Name.Equals("NetFieldExportGroupAttribute")));

            foreach (var ns in GetUniqueNamespaces(netFieldExportGroups))
            {
                source.Append($"using {ns};");
            }

            source.Append($@"
            namespace Unreal.Core
            {{
                public class NetFieldParserGenerated : INetFieldParser
                {{
                    private HashSet<string> _playerControllers {{ get; set; }} = new HashSet<string>();
                    private HashSet<string> _netFieldGroups {{ get; set; }} = new HashSet<string>();
                    private HashSet<string> _classNetCaches {{ get; set; }} = new HashSet<string>();
                    
                    public bool IsPlayerController(string group)
                    {{
                        return _playerControllers.Contains(group);
                    }}

                    public bool WillReadClassNetCache(string group)
                    {{
                        return _classNetCaches.Contains(group);
                    }}

                    public bool WillReadType(string group)
                    {{
                        return _netFieldGroups.Contains(group);
                    }}

                    public INetFieldExportGroup CreateType(string path)
                    {{
                        switch (path)
                        {{
            ");

            foreach (var symbol in netFieldExportGroups)
            {
                source.Append(AddToCreateType(symbol));
            }

            source.Append(@"
                default:
                    return null;
                }
            }");

            source.Append(@"
                }
            }");

            // TODO add parse mode ?
            return source.ToString();
        }

        /// <summary>
        /// Adds switch case for object creation for each class marked with NetFieldExportGroup.
        /// </summary>
        /// <param name="classSymbol"></param>
        private string AddToCreateType(INamedTypeSymbol classSymbol)
        {
            // begin building the generated source
            var source = new StringBuilder();

            var attrs = classSymbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass.Name.Equals("NetFieldExportGroupAttribute"));
            if (attrs == null)
            {
                return "";
            }

            var path = (string)attrs.ConstructorArguments[0].Value;
            source.Append($@"
                case ""{path}"":
                    return new {classSymbol.Name}();
            ");
            return source.ToString();
        }

        /// <summary>
        /// Adds a ReadField(string field, INetBitReader reader) method to each (partial) class marked with NetFieldExportGroup.
        /// </summary>
        /// <param name="classSymbol"></param>
        private string AddReadFieldMethod(INamedTypeSymbol classSymbol)
        {
            var attrs = classSymbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass.Name.Equals("NetFieldExportGroupAttribute"));
            if (attrs == null)
            {
                return "";
            }

            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            // begin building the generated source
            StringBuilder source = new StringBuilder($@"
                using Unreal.Core.Models;
                using Unreal.Core.Models.Contracts;
                using Unreal.Core.Models.Enums;
            ");

            foreach (var ns in GetUniqueNamespaces(GetAllProperties(classSymbol)))
            {
                source.Append($"using {ns};");
            }

            source.Append($@"
            namespace {namespaceName}
            {{
                public class {classSymbol.Name}Adapter : NetFieldExportGroupAdapter<{classSymbol.Name}>
                {{
                    public override void ReadField(string field, INetBitReader netBitReader) 
                    {{
                        switch (field)
                        {{
            ");

            // create properties for each field 
            foreach (var propertySymbol in GetAllProperties(classSymbol))
            {
                ProcessProperties(source, propertySymbol);
            }

            source.Append(@"
                default:
                    // pass
                    break;
                }
            }");

            source.Append(@"
                }
            }");

            return source.ToString();
        }

        /// <summary>
        /// Adds a switch case for each property marked with NetFieldExportAttribute.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertySymbol"></param>
        private string ProcessProperties(StringBuilder source, IPropertySymbol propertySymbol)
        {
            var attrs = propertySymbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass.Name.Equals("NetFieldExportAttribute"));
            if (attrs is not null)
            {
                if (attrs.ConstructorArguments.Length < 2)
                {
                    return "";
                }

                var movement = propertySymbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass.Name.Equals("RepMovementAttribute"));

                var field = (string)attrs.ConstructorArguments[0].Value;
                var repLayout = (RepLayoutCmdType)attrs.ConstructorArguments[1].Value;

                var reader = repLayout switch
                {
                    RepLayoutCmdType.PropertyBool => "netBitReader.SerializePropertyBool();",
                    RepLayoutCmdType.PropertyNativeBool => "netBitReader.SerializePropertyNativeBool();",
                    RepLayoutCmdType.PropertyName => "netBitReader.SerializePropertyName();",
                    RepLayoutCmdType.PropertyFloat => "netBitReader.SerializePropertyFloat();",
                    RepLayoutCmdType.PropertyNetId => "netBitReader.SerializePropertyNetId();",
                    RepLayoutCmdType.PropertyPlane => "throw new NotImplementedException(\"Plane RepLayoutCmdType not implemented\");",
                    RepLayoutCmdType.PropertyObject => "netBitReader.SerializePropertyObject();",
                    RepLayoutCmdType.PropertyRotator => "netBitReader.SerializePropertyRotator();",
                    RepLayoutCmdType.PropertyString => "netBitReader.SerializePropertyString();",
                    RepLayoutCmdType.PropertyVector10 => "netBitReader.SerializePropertyVector10();",
                    RepLayoutCmdType.PropertyVector100 => "netBitReader.SerializePropertyVector100();",
                    RepLayoutCmdType.PropertyVectorNormal => "netBitReader.SerializePropertyVectorNormal();",
                    RepLayoutCmdType.PropertyVectorQ => "netBitReader.SerializePropertyQuantizedVector(VectorQuantization.RoundWholeNumber);",
                    RepLayoutCmdType.PropertyVector => "netBitReader.SerializePropertyVector();",
                    RepLayoutCmdType.PropertyVector2D => "netBitReader.SerializePropertyVector2D();",
                    RepLayoutCmdType.RepMovement => movement != null ? @$"netBitReader.SerializeRepMovement(
                            {movement.ConstructorArguments[0].ToCSharpString()}, 
                            {movement.ConstructorArguments[1].ToCSharpString()}, 
                            {movement.ConstructorArguments[2].ToCSharpString()});" : "netBitReader.SerializeRepMovement();",
                    RepLayoutCmdType.Enum => "netBitReader.SerializePropertyEnum();",
                    RepLayoutCmdType.PropertyByte => "netBitReader.ReadByte();",
                    RepLayoutCmdType.PropertyInt => "netBitReader.ReadInt32();",
                    RepLayoutCmdType.PropertyInt16 => "netBitReader.ReadInt16();",
                    RepLayoutCmdType.PropertyUInt16 => "netBitReader.SerializePropertyUInt16();",
                    RepLayoutCmdType.PropertyUInt32 => "netBitReader.ReadUInt32();",
                    RepLayoutCmdType.PropertyUInt64 => "netBitReader.ReadUInt64();",
                    //RepLayoutCmdType.Property => "(data as IProperty).Serialize(netBitReader); (data as IResolvable)?.Resolve(GuidCache);",
                    _ => "",
                };

                if (string.IsNullOrEmpty(reader))
                {
                    return "";
                }

                source.Append($@"
                    case ""{field}"":
                        Data.{propertySymbol.Name} = {reader};
                        break;
                ");

                return source.ToString();
            }

            return "";
        }

        private static string TestAdapters(INamedTypeSymbol classSymbol)
        {
            var attrs = classSymbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass.Name.Equals("NetFieldExportGroupAttribute"));
            if (attrs == null)
            {
                return "";
            }


            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            StringBuilder source = new StringBuilder();

            source.Append($@"Console.WriteLine(""{classSymbol.Name}"");");

            var path = (string)attrs.ConstructorArguments[0].Value;
            var parseMode = (ParseMode)attrs.ConstructorArguments[1].Value;


            source.Append($@"Console.WriteLine(""{path}"");");
            source.Append($@"Console.WriteLine(""{parseMode}"");");


            foreach (var ns in GetUniqueNamespaces(GetAllProperties(classSymbol)))
            {
                source.Append($@"Console.WriteLine(""{ns}"");");
            }

            foreach (var propertySymbol in GetAllProperties(classSymbol))
            {
                var exportAttrs = propertySymbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass.Name.Equals("NetFieldExportAttribute"));

                if (exportAttrs is not null)
                {
                    if (attrs.ConstructorArguments.Length < 2)
                    {
                        return "";
                    }

                    var field = (string)attrs.ConstructorArguments[0].Value;
                    var repLayout = (RepLayoutCmdType)attrs.ConstructorArguments[1].Value;

                    source.Append($@"Console.WriteLine(""{propertySymbol.Name}"");");
                    source.Append($@"Console.WriteLine(""{propertySymbol.Type.ToDisplayString()}"");");


                    var movement = propertySymbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass.Name.Equals("RepMovementAttribute"));
                    if (movement != null)
                    {
                        source.Append($@"Console.WriteLine(""{movement.ConstructorArguments[0].ToCSharpString()}"");");
                        source.Append($@"Console.WriteLine(""{movement.ConstructorArguments[1].ToCSharpString()}"");");
                        source.Append($@"Console.WriteLine(""{movement.ConstructorArguments[2].ToCSharpString()}"");");
                    }
                }
            }

            return source.ToString();
        }


        /// <summary>
        /// Get all unique namespaces.
        /// </summary>
        /// <param name="symbols"></param>
        private static IEnumerable<string> GetUniqueNamespaces(IEnumerable<ISymbol> symbols)
        {
            var namespaces = new HashSet<string>();
            foreach (var symbol in symbols)
            {
                namespaces.Add(symbol.ContainingNamespace.ToDisplayString());
            }
            return namespaces;
        }

        /// <summary>
        /// Get all properties of a class, including properties from base classes.
        /// </summary>
        /// <param name="symbol"></param>
        private static IEnumerable<IPropertySymbol> GetAllProperties(INamedTypeSymbol symbol)
        {
            foreach (var prop in symbol.GetMembers().OfType<IPropertySymbol>())
            {
                yield return prop;
            }

            var baseType = symbol.BaseType;
            if (baseType != null)
            {
                foreach (var prop in GetAllProperties(baseType))
                {
                    yield return prop;
                }
            }
        }

        /// <summary>
        /// SymbolVisitor to get all required classes for Fortnite replay parsing from a Compilation.
        /// </summary>
        class GetAllSymbolsVisitor : SymbolVisitor
        {
            public List<INamedTypeSymbol> Symbols { get; set; } = new List<INamedTypeSymbol>();

            public override void VisitNamespace(INamespaceSymbol symbol)
            {
                foreach (var s in symbol.GetMembers())
                {
                    s.Accept(this);
                }
            }

            public override void VisitNamedType(INamedTypeSymbol symbol)
            {
                if (symbol.GetAttributes().Any(i => i.AttributeClass.Name.Equals("NetFieldExportGroupAttribute")))
                {
                    Symbols.Add(symbol);
                }
            }
        }
    }
}