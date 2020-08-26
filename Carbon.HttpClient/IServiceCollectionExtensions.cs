﻿using Microsoft.AspNetCore.HeaderPropagation;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Carbon.HttpClients
{
    /// <summary>
    /// Represents Service collections extensions of Carbon.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds http client with header propogation
        /// </summary>
        /// <seealso cref = "HeaderPropagationOptions" />
        public static void AddHttpClientWithHeaderPropagation(this IServiceCollection services, Action<HeaderPropagationOptions> configureHeaderPropagation)
        {
            services.AddHeaderPropagation(o =>
            {
                o.Headers.Add("x-request-id", "6161613434343");
            });

            services.AddHttpClient<WebapiClient>().AddHeaderPropagation(o => o.Headers.Add("x-request-id", "6161613434343"));
        }
    }
}
