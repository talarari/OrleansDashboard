﻿using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Orleans.Runtime.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace OrleansDashboard
{
    public static class ExtensionMethods
    {
        public static async Task ReturnFile(this IOwinContext context, string name, string contentType)
        {
            context.Response.ContentType = contentType;
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream($"OrleansDashboard.{name}"))
            using (var reader = new StreamReader(stream))
            {
                var content = await reader.ReadToEndAsync();
                await context.Response.WriteAsync(content);
            }
        }

        public static Task ReturnJson(this IOwinContext context, object value)
        {
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(
                JsonConvert.SerializeObject(value,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
        }

        public static Task ReturnUnauthorised(this IOwinContext context)
        {
            context.Response.StatusCode = 401;
            context.Response.ReasonPhrase = "Unauthorized";
            context.Response.Headers.Add("WWW-Authenticate", new string[] { "Basic realm=\"OrleansDashboard\"" });
            return Task.FromResult(0);
        }

        public static void RegisterDashboard(this GlobalConfiguration config, int port = 8080, string username = null, string password = null)
        {
            var settings = new Dictionary<string, string>();
            settings.Add("Port", port.ToString());
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                settings.Add("Username", username);
                settings.Add("Password", password);
            }
            config.RegisterBootstrapProvider<Dashboard>("Dashboard", settings);
        }

    }
}
