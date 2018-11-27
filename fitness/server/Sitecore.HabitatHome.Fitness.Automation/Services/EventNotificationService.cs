﻿using Newtonsoft.Json.Linq;
using Sitecore.XConnect;
using System;
using System.Net;

namespace Sitecore.HabitatHome.Fitness.Automation.Services
{
    public class EventNotificationService : IEventNotificationService
    {
        public string FirebaseMessagingApiUri{ get; set; }

        public string FirebaseMessagingApiKey { get; set; }

        public string PublicHostName { get; set; }

        public void SendInitialEventNotification(Contact contact, string token)
        {
            var uri = new Uri(FirebaseMessagingApiUri);
            using (var client = new WebClient())
            {
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("Authorization", $"key={FirebaseMessagingApiKey}");

                dynamic data = new JObject();
                data.notification = new JObject();
                data.notification.title = $"Hey {GetCurrentContactName(contact)}!";
                data.notification.body = $"Thanks for registering for the event, it will be outstanding.";
                data.notification.click_action = PublicHostName;
                data.notification.icon = $"{PublicHostName}/favicon-32x32.png";
                data.to = token;

                try
                {
                    var result = client.UploadString(uri, data.ToString());
                }

                catch (Exception ex)
                {
                    // TODO: inject logger here.
                }
            }
        }

        private string GetCurrentContactName(Contact contact)
        {
            // TODO: get contact first name - this errors out now
            //var facets = contact.GetFacet<PersonalInformation>(PersonalInformation.DefaultFacetKey);
            return "Visitor";
        }
    }
}