using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using StartingMultiTenantLib.Const;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StartingMultiTenantLib
{
    public class TenantResolveStrategy : IMultiTenantStrategy
    {
        private readonly string regex;

        public TenantResolveStrategy(string template) {
            if (template == SMTConsts.TenantToken) {
                template = template.Replace(SMTConsts.TenantToken, @"(?<identifier>.+)");
            } else {
                // Check for valid template.
                // Template cannot be null or whitespace.
                if (string.IsNullOrWhiteSpace(template)) {
                    throw new MultiTenantException("Template cannot be null or whitespace.");
                }
                // Wildcard "*" must be only occur once in template.
                if (Regex.Match(template, @"\*(?=.*\*)").Success) {
                    throw new MultiTenantException("Wildcard \"*\" must be only occur once in template.");
                }
                // Wildcard "*" must be only token in template segment.
                if (Regex.Match(template, @"\*[^\.]|[^\.]\*").Success) {
                    throw new MultiTenantException("\"*\" wildcard must be only token in template segment.");
                }
                // Wildcard "?" must be only token in template segment.
                if (Regex.Match(template, @"\?[^\.]|[^\.]\?").Success) {
                    throw new MultiTenantException("\"?\" wildcard must be only token in template segment.");
                }

                template = template.Trim().Replace(".", @"\.");
                string wildcardSegmentsPattern = @"(\.[^\.]+)*";
                string singleSegmentPattern = @"[^\.]+";
                if (template.Substring(template.Length - 3, 3) == @"\.*") {
                    template = template.Substring(0, template.Length - 3) + wildcardSegmentsPattern;
                }

                wildcardSegmentsPattern = @"([^\.]+\.)*";
                template = template.Replace(@"*\.", wildcardSegmentsPattern);
                template = template.Replace("?", singleSegmentPattern);
                template = template.Replace(SMTConsts.TenantToken, @"(?<identifier>[^\.]+)");
            }

            this.regex = $"^{template}$";
        }
        public async Task<string?> GetIdentifierAsync(object context) {
            if (!(context is HttpContext httpContext))
                throw new MultiTenantException(null,
                    new ArgumentException($"\"{nameof(context)}\" type must be of type HttpContext", nameof(context)));

            string? identifier = null;
            var resolveResult= await resolveFromHeader(httpContext.Request);
            if (!resolveResult.Item1) {
                resolveResult = await resolveFromHost(httpContext.Request);

                if (!resolveResult.Item1) {
                    return null;
                }
            }

            identifier = resolveResult.Item3;
            var contextTenantDomain = httpContext.RequestServices.GetRequiredService<ContextTenantDomain>();
            contextTenantDomain.TenantDomain = resolveResult.Item2;

            return identifier;
        }

        private async Task<Tuple<bool,string,string>> resolveFromHeader(HttpRequest httpRequest) {
            string tenantIdentifier = string.Empty;
            string tenantDomain = string.Empty;
            if (!httpRequest.Headers.ContainsKey(SMTConsts.TenantIdentifierHeaderKey) || string.IsNullOrEmpty((tenantIdentifier= httpRequest.Headers[SMTConsts.TenantIdentifierHeaderKey]))) {
                return Tuple.Create<bool, string, string>(false, null, null);
            }

            if (!httpRequest.Headers.ContainsKey(SMTConsts.TenantDomainHeaderKey) || string.IsNullOrEmpty((tenantDomain = httpRequest.Headers[SMTConsts.TenantDomainHeaderKey]))) {
                return Tuple.Create<bool, string, string>(false, null, null);
            }

            return Tuple.Create(true, tenantDomain,tenantIdentifier);
        }

        private async Task<Tuple<bool, string, string>> resolveFromHost(HttpRequest httpRequest) {
            var host = httpRequest.Host;
            if (host.HasValue == false)
                return Tuple.Create<bool, string, string>(false, null, null);

            if (System.Net.IPAddress.TryParse(host.Host, out _)) {
                return Tuple.Create<bool, string, string>(false, null, null);
            }

            string tenantIdentifier = string.Empty;
            string tenantDomain = string.Empty;

            var match = Regex.Match(host.Host, regex,
                RegexOptions.ExplicitCapture,
                TimeSpan.FromMilliseconds(100));

            if (match.Success) {
                tenantIdentifier = match.Groups["identifier"].Value;
                if (tenantIdentifier.Length == match.Value.Length) {
                    return Tuple.Create<bool, string, string>(false, null, null);
                }
                tenantDomain = match.Value.Substring(tenantIdentifier.Length + 1);
            }

            return Tuple.Create(true, tenantDomain, tenantIdentifier);
        }
    }
}
