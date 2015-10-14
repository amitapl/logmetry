using Microsoft.CSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Story.Core.Utils;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Story.Core
{
    public interface IStoryRulesetProvider
    {
        IRuleset<IStory, IStoryHandler> GetRuleset();
    }

    public class BasicStoryRulesetProvider : IStoryRulesetProvider
    {
        private readonly IRuleset<IStory, IStoryHandler> ruleset;

        public BasicStoryRulesetProvider(IRuleset<IStory, IStoryHandler> ruleset)
        {
            this.ruleset = ruleset;
        }

        public IRuleset<IStory, IStoryHandler> GetRuleset()
        {
            return this.ruleset;
        }
    }

    public class RulesetSerialization
    {
        private readonly Dictionary<string, Func<IRule<IStory, IStoryHandler>>> ruleFactoryMapping;
        private readonly Dictionary<string, Func<IStoryHandler>> storyHandlerFactoryMapping;

        public RulesetSerialization()
        {
            this.ruleFactoryMapping = new Dictionary<string, Func<IRule<IStory, IStoryHandler>>>();
            this.storyHandlerFactoryMapping = new Dictionary<string, Func<IStoryHandler>>();

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    var nameAttribute = t.GetCustomAttribute<NameAttribute>();
                    if (nameAttribute != null)
                    {
                        if (t.GetInterfaces().Any(i => i == typeof(IRule<IStory, IStoryHandler>)))
                        {
                            ruleFactoryMapping.Add(nameAttribute.Name, () => Activator.CreateInstance(t) as IRule<IStory, IStoryHandler>);
                        }

                        if (t.GetInterfaces().Any(i => i == typeof(IStoryHandler)))
                        {
                            storyHandlerFactoryMapping.Add(nameAttribute.Name, () => Activator.CreateInstance(t) as IStoryHandler);
                        }
                    }
                }
            }
        }
    }

    public class NameAttribute : Attribute
    {
        public NameAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }

    public class JsonRuleConverter : JsonCreationConverter<IRule<IStory, IStoryHandler>>
    {
        private readonly Dictionary<string, Func<IRule<IStory, IStoryHandler>>> ruleFactoryMapping;

        public JsonRuleConverter(Dictionary<string, Func<IRule<IStory, IStoryHandler>>> ruleFactoryMapping)
        {
            this.ruleFactoryMapping = ruleFactoryMapping;
        }

        protected override IRule<IStory, IStoryHandler> Create(Type objectType, JObject jsonObject)
        {
            var typeName = jsonObject["ruleType"].ToString();
            Func<IRule<IStory, IStoryHandler>> ruleCreateFunc;
            this.ruleFactoryMapping.TryGetValue(typeName, out ruleCreateFunc);
            if (ruleCreateFunc != null)
            {
                return ruleCreateFunc();
            }

            return null;
        }
    }

    public class JsonStoryHandlerConverter : JsonCreationConverter<IStoryHandler>
    {
        private readonly Dictionary<string, Func<IStoryHandler>> storyHandlerFactoryMapping;

        public JsonStoryHandlerConverter(Dictionary<string, Func<IStoryHandler>> storyHandlerFactoryMapping)
        {
            this.storyHandlerFactoryMapping = storyHandlerFactoryMapping;
        }

        protected override IStoryHandler Create(Type objectType, JObject jsonObject)
        {
            var handlerName = jsonObject["handlerType"].ToString();
            Func<IStoryHandler> storyHandlerCreateFunc;
            this.storyHandlerFactoryMapping.TryGetValue(handlerName, out storyHandlerCreateFunc);
            if (storyHandlerCreateFunc != null)
            {
                return storyHandlerCreateFunc();
            }

            return null;
        }
    }

    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        protected abstract T Create(Type objectType, JObject jsonObject);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType,
          object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var target = Create(objectType, jsonObject);
            serializer.Populate(jsonObject.CreateReader(), target);
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value,
       JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class FileBasedStoryRulesetProvider : IStoryRulesetProvider, IDisposable
    {
        private FileWatcher fileWatcher;
        private IRuleset<IStory, IStoryHandler> ruleset;

        public FileBasedStoryRulesetProvider(string path)
        {
            this.fileWatcher = new FileWatcher(path, OnFileChanged);
        }

        private void OnFileChanged(string fileContent)
        {
            /*
            [
                {
                    'ruleType': 'type',
                    properties...
                }
            ]
            */
            // Create a new instance of the C# compiler
            var compiler = new CSharpCodeProvider();

            // Create some parameters for the compiler
            var parms = new CompilerParameters()
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                TreatWarningsAsErrors = false
            };

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    parms.ReferencedAssemblies.Add(assembly.Location);
                    foreach (AssemblyName assemblyName in assembly.GetReferencedAssemblies())
                    {
                        parms.ReferencedAssemblies.Add(Assembly.Load(assemblyName).Location);
                    }
                }
                catch
                {
                }
            }

            // Try to compile the string into an assembly
            var results = compiler.CompileAssemblyFromSource(parms, fileContent);

            // If there weren't any errors get an instance of "MyClass" and invoke
            // the "Message" method on it
            if (results.Errors.Count == 0)
            {
                var myClass = results.CompiledAssembly.CreateInstance("MyClass");
                ruleset = myClass as IRuleset<IStory, IStoryHandler>;
            }
        }

        public IRuleset<IStory, IStoryHandler> GetRuleset()
        {
            return this.ruleset;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.fileWatcher != null)
                {
                    this.fileWatcher.Dispose();
                    this.fileWatcher = null;
                }
            }
        }
    }
}
