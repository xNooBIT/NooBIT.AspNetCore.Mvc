﻿using System;
using Microsoft.AspNetCore.Hosting;
using NooBIT.AspNetCore.Mvc.Http.Headers;
using NooBIT.AspNetCore.Mvc.Security.ContentSecurityPolicy;
using NooBIT.AspNetCore.Mvc.Security.FrameOptions;
using NooBIT.AspNetCore.Mvc.Security.ReferrerPolicy;
using NooBIT.AspNetCore.Mvc.Security.StrictTransportSecurity;
using NooBIT.AspNetCore.Mvc.Security.XssProtection;

namespace NooBIT.AspNetCore.Mvc.Http
{
    public class HeaderPolicyBuilder
    {
        private readonly HeaderPolicy _policy = new HeaderPolicy();

        public HeaderPolicyBuilder AddHeader(Header header)
        {
            if (_policy.SetHeaders.ContainsKey(header.Name))
                _policy.SetHeaders[header.Name] = header;
            else
                _policy.SetHeaders.Add(header.Name, header);
            return this;
        }

        public HeaderPolicyBuilder AddHeader(IHeaderBuilder builder)
        {
            var header = builder.Build();
            return AddHeader(header);
        }

        public HeaderPolicyBuilder RemoveHeader(Header header)
        {
            if (_policy.RemoveHeaders.Contains(header.Name))
                return this;

            _policy.RemoveHeaders.Add(header.Name);
            return this;
        }

        public HeaderPolicy Build()
        {
            return _policy;
        }

        public HeaderPolicyBuilder AddRecommendedSecurityHeaders(IHostingEnvironment environment)
        {
            RemoveServerHeader()
                .RemovePoweredByHeader()
                .AddContentTypeOptionsNoSniff()
                .AddContentSecurity(new ContentSecurityPolicyBuilder()
                    .Default())
                .AddXssProtection(new XssProtectionBuilder()
                    .Block())
                .AddFrameOptions(new FrameOptionsBuilder()
                    .UseSameOrigin())
                .AddReferrerPolicy(new ReferrerPolicyBuilder()
                    .UseStrictOriginWhenCrossOrigin());

            if (!environment.IsDevelopment())
            {
                AddStrictTransportSecurity(new StrictTransportSecurityBuilder()
                    .UseMaxAge((uint) TimeSpan.FromDays(365).TotalSeconds)
                    .WithIncludeSubDomains()
                    .WithPreload());
            }

            return this;
        }

        public HeaderPolicyBuilder RemoveServerHeader()
        {
            return RemoveHeader(Header.Server);
        }

        public HeaderPolicyBuilder RemovePoweredByHeader()
        {
            return RemoveHeader(Header.PoweredBy);
        }

        public HeaderPolicyBuilder AddStrictTransportSecurity(StrictTransportSecurityBuilder builder)
        {
            return AddHeader(builder);
        }

        public HeaderPolicyBuilder AddContentSecurity(ContentSecurityPolicyBuilder builder)
        {
            return AddHeader(builder);
        }

        public HeaderPolicyBuilder AddXssProtection(XssProtectionBuilder builder)
        {
            return AddHeader(builder);
        }

        public HeaderPolicyBuilder AddFrameOptions(FrameOptionsBuilder builder)
        {
            return AddHeader(builder);
        }

        public HeaderPolicyBuilder AddReferrerPolicy(ReferrerPolicyBuilder builder)
        {
            return AddHeader(builder);
        }

        public HeaderPolicyBuilder AddContentTypeOptionsNoSniff()
        {
            var header = Header.ContentTypeOptions;
            header.Value = ContentTypeOptionsHeader.NoSniff;
            return AddHeader(header);
        }
    }
}