﻿// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Notifo.Areas.Api.Controllers.Registration.Dto;
using Notifo.Domain;
using Notifo.Domain.Channels;
using Notifo.Domain.Channels.WebPush;
using Notifo.Domain.Subscriptions;
using Notifo.Domain.Users;
using Notifo.Pipeline;
using NSwag.Annotations;

namespace Notifo.Areas.Api.Controllers.Registration
{
    [OpenApiIgnore]
    public sealed class RegistrationController : BaseController
    {
        private readonly IUserStore userStore;
        private readonly ISubscriptionStore subscriptionStore;
        private readonly IWebPushService webPushService;

        public RegistrationController(IUserStore userStore, ISubscriptionStore subscriptionStore, IWebPushService webPushService)
        {
            this.userStore = userStore;
            this.subscriptionStore = subscriptionStore;
            this.webPushService = webPushService;
        }

        [HttpPost("api/web/register")]
        [AppPermission(Roles.WebManager, Roles.User)]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto request)
        {
            string? userId = null;
            string? userToken = null;

            if (request.CreateUser)
            {
                userId = Guid.NewGuid().ToString();

                var update = request.ToUpdate();

                var user = await userStore.UpsertAsync(App.Id, userId, update, HttpContext.RequestAborted);

                if (request.Topics?.Any() == true)
                {
                    var command = new Subscribe
                    {
                        TopicSettings = new NotificationSettings
                        {
                            [Providers.WebPush] = new NotificationSetting
                            {
                                Send = NotificationSend.Send
                            }
                        }
                    };

                    if (!string.IsNullOrEmpty(request.EmailAddress))
                    {
                        command.TopicSettings[Providers.Email] = new NotificationSetting
                        {
                            Send = NotificationSend.Send
                        };
                    }

                    foreach (var topic in request.Topics)
                    {
                        var topicId = new TopicId(topic);

                        await subscriptionStore.UpsertAsync(App.Id, userId, topicId, command, HttpContext.RequestAborted);
                    }
                }

                userToken = user.ApiKey;
            }

            var response = new RegisteredUserDto
            {
                PublicKey = webPushService.PublicKey,
                UserId = userId,
                UserToken = userToken
            };

            return Ok(response);
        }
    }
}
