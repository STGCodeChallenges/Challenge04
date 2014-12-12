using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace STGCodeChallenge4
{
    public class CodeDom
    {
        private List<CodeCompileUnit> listCompileUnits = new List<CodeCompileUnit>();
        private List<CodeNamespace> listNamespaces = new List<CodeNamespace>();
        private System.Collections.Specialized.StringCollection listReferencedAssemblies =
            new System.Collections.Specialized.StringCollection() { "System.dll" };
        private CodeTypeDeclaration lastClassAdded { get; set; }

        public enum Language { CSharp, VP };

        private Language _language;

        public CodeDom()
            : this(Language.CSharp)
        {
        }

        private CodeDom(Language language)
        {
            _language = language;
        }

        public static CodeDomProvider Provider(Language provider)
        {
            var providerOptions = new Dictionary<string, string>(); providerOptions.Add("CompilerVersion", "v3.5");

            switch (provider)
            {
                case Language.VP:
                    return new Microsoft.VisualBasic.VBCodeProvider(providerOptions);

                case Language.CSharp:
                default:
                    return new Microsoft.CSharp.CSharpCodeProvider(providerOptions);
            }
        }

        public CodeNamespace AddNamespace(string namespaceName)
        {
            CodeNamespace codeNamespace = new CodeNamespace(namespaceName);
            listNamespaces.Add(codeNamespace);

            return codeNamespace;
        }

        public void AddUsing(string usingRef)
        {
            listNamespaces.Last().Imports.Add(new CodeNamespaceImport(usingRef));
        }

        public CodeDom AddReference(string referencedAssembly)
        {
            listReferencedAssemblies.Add(referencedAssembly);

            return this;
        }

        public void AddClass(CodeTypeDeclaration newClass)
        {
            lastClassAdded = newClass;
            listNamespaces.Last().Types.Add(lastClassAdded);
        }

        public CodeTypeDeclaration Class(string className)
        {
            return new CodeTypeDeclaration(className);
        }

        public void AddMethod(CodeSnippetTypeMember newMethod)
        {
            lastClassAdded.Members.Add(newMethod);
        }

        public CodeSnippetTypeMember Method(string returnType, string methodName, string paramList, string methodBody)
        {
            return Method(string.Format("public static {0} {1}({2}) {{ {3} }} ", returnType, methodName, paramList, methodBody));
        }

        public CodeSnippetTypeMember Method(string methodName, string paramList, string methodBody)
        {
            return Method("void", methodName, paramList, methodBody);
        }

        public CodeSnippetTypeMember Method(string methodName, string methodBody)
        {
            return Method("void", methodName, "", methodBody);
        }

        public CodeSnippetTypeMember Method(string methodBody)
        {
            return new CodeSnippetTypeMember(methodBody);
        }

        public CodeCompileUnit CompileUnit
        {
            get
            {
                // Create a new CodeCompileUnit to contain 
                // the program graph.
                CodeCompileUnit compileUnit = new CodeCompileUnit();

                foreach (var ns in listNamespaces)
                    compileUnit.Namespaces.Add(ns);

                return compileUnit;
            }
        }

        public Assembly Compile()
        {
            return Compile(null);
        }

        public Assembly Compile(string assemblyPath)
        {
            CompilerParameters options = new CompilerParameters();
            options.IncludeDebugInformation = false;
            options.GenerateExecutable = false;
            options.GenerateInMemory = (assemblyPath == null);
            foreach (string refAsm in listReferencedAssemblies)
                options.ReferencedAssemblies.Add(refAsm);
            if (assemblyPath != null)
                options.OutputAssembly = assemblyPath.Replace('\\', '/');

            CodeDomProvider codeProvider = Provider(_language);

            CompilerResults results =
               codeProvider.CompileAssemblyFromDom(options, CompileUnit);
            codeProvider.Dispose();

            if (results.Errors.Count == 0)
                return results.CompiledAssembly;

            // Process compilation errors
            System.Diagnostics.Trace.WriteLine("Compilation Errors:");
            foreach (string outpt in results.Output)
                System.Diagnostics.Trace.WriteLine(outpt);
            foreach (CompilerError err in results.Errors)
                System.Diagnostics.Trace.WriteLine(err.ToString());

            return null;
        }

        public string GenerateCode()
        {
            StringBuilder sb = new StringBuilder();
            TextWriter tw = new IndentedTextWriter(new StringWriter(sb));

            CodeDomProvider codeProvider = Provider(_language);
            codeProvider.GenerateCodeFromCompileUnit(CompileUnit, tw, new CodeGeneratorOptions());
            codeProvider.Dispose();

            tw.Close();

            return sb.ToString();
        }
    }
}
