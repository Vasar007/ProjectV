using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Acolyte.Assertions;

namespace ProjectV.Configuration
{
    public sealed class ServiceConfigRegistry
    {
        private readonly Dictionary<string, XElement> _messageHandlerRegistry = 
            new Dictionary<string, XElement>();

        private readonly Dictionary<string, XElement> _inputRegistry =
            new Dictionary<string, XElement>();

        private readonly Dictionary<string, XElement> _crawlersRegistry =
            new Dictionary<string, XElement>();

        private readonly Dictionary<string, XElement> _appraisersRegistry =
            new Dictionary<string, XElement>();

        private readonly Dictionary<string, XElement> _outputRegistry =
            new Dictionary<string, XElement>();

        private readonly Dictionary<string, XElement> _repositoriesRegistry =
            new Dictionary<string, XElement>();


        public ServiceConfigRegistry()
        {
        }

        public void RegisterMessageHandlerParameter(string identifier, XElement element)
        {
            Register(_messageHandlerRegistry, identifier, element);
        }

        public void RegisterInputter(string identifier, XElement element)
        {
            Register(_inputRegistry, identifier, element);
        }

        public void RegisterCrawler(string identifier, XElement element)
        {
            Register(_crawlersRegistry, identifier, element);
        }

        public void RegisterAppraiser(string identifier, XElement element)
        {
            Register(_appraisersRegistry, identifier, element);
        }

        public void RegisterOutputter(string identifier, XElement element)
        {
            Register(_outputRegistry, identifier, element);
        }

        public void RegisterRepository(string identifier, XElement element)
        {
            Register(_repositoriesRegistry, identifier, element);
        }

        public XElement GetConfigForMessageHandlerParameter(string identifier)
        {
            return GetConfigForIdenrifier(_messageHandlerRegistry, identifier);
        }

        public XElement GetConfigForInputter(string identifier)
        {
            return GetConfigForIdenrifier(_inputRegistry, identifier);
        }

        public XElement GetConfigForCrawler(string identifier)
        {
            return GetConfigForIdenrifier(_crawlersRegistry, identifier);
        }

        public XElement GetConfigForAppraiser(string identifier)
        {
            return GetConfigForIdenrifier(_appraisersRegistry, identifier);
        }

        public XElement GetConfigForOutputter(string identifier)
        {
            return GetConfigForIdenrifier(_outputRegistry, identifier);
        }

        public XElement GetConfigForRepository(string identifier)
        {
            return GetConfigForIdenrifier(_repositoriesRegistry, identifier);
        }

        private static void Register(Dictionary<string, XElement> registry, string identifier,
            XElement element)
        {
            identifier.ThrowIfNullOrEmpty(nameof(identifier));
            element.ThrowIfNull(nameof(element));

            registry.Add(identifier, element);
        }

        private static XElement GetConfigForIdenrifier(Dictionary<string, XElement> registry,
            string identifier)
        {
            identifier.ThrowIfNullOrEmpty(nameof(identifier));

            if (!registry.TryGetValue(identifier, out XElement element))
            {
                throw new ArgumentException($"Identifier {identifier} was not registered.",
                                            nameof(identifier));
            }
            return element;
        }
    }
}
