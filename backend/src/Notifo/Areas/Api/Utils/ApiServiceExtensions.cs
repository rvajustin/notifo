﻿// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.Extensions.Configuration;
using Notifo.Areas.Api.Controllers.Notifications;
using Notifo.Areas.Api.Utils;
using Notifo.Domain.Channels.Sms;
using Notifo.Domain.Channels.Web;
using Notifo.Domain.UserNotifications;
using Notifo.Identity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApiServiceExtensions
    {
        public static void AddMyApi(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<UrlOptions>(
                config.GetSection("url"));

            services.AddSingletonAs<NotificationHubAccessor>()
                .As<IStreamClient>();

            services.AddSingletonAs<UrlBuilder>()
                .As<IUserNotificationUrl>().As<ISmsUrl>();
        }
    }
}
