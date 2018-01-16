using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Onots.AspNetCore.Identity.Anonymous
{
    public class AnonymousIdMiddleware
    {
        private RequestDelegate nextDelegate;
        private readonly CookieBuilder cookieBuilder;

        public AnonymousIdMiddleware(RequestDelegate nextDelegate, CookieBuilder cookieBuilder)
        {
            this.nextDelegate = nextDelegate;
            this.cookieBuilder = cookieBuilder;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            HandleRequest(httpContext);
            await nextDelegate.Invoke(httpContext);
        }

        public void HandleRequest(HttpContext httpContext)
        {
            string encodedValue;
            bool isAuthenticated = httpContext.User.Identity.IsAuthenticated;
            DateTime now = DateTime.UtcNow;

            // Handle secure cookies over an unsecured connection
            if (cookieBuilder.SecurePolicy == CookieSecurePolicy.Always && !httpContext.Request.IsHttps)
            {
                encodedValue = httpContext.Request.Cookies[cookieBuilder.Name];
                if (!string.IsNullOrWhiteSpace(encodedValue))
                {
                    httpContext.Response.Cookies.Delete(cookieBuilder.Name);
                }

                // Adds the feature to request collection
                httpContext.Features.Set<IAnonymousIdFeature>(new AnonymousIdFeature());

                return;
            }

            // Gets the value and anonymous Id data from the cookie, if available
            encodedValue = httpContext.Request.Cookies[cookieBuilder.Name];
            AnonymousIdData decodedValue = AnonymousIdEncoder.Decode(encodedValue);

            string anonymousId = null;

            if (decodedValue != null && !string.IsNullOrWhiteSpace(decodedValue.AnonymousId))
            {
                // Copy the existing value in Request header
                anonymousId = decodedValue.AnonymousId;

                // Adds the feature to request collection
                httpContext.Features.Set<IAnonymousIdFeature>(new AnonymousIdFeature()
                {
                    AnonymousId = anonymousId
                });
            }

            // User is already authenticated
            if (isAuthenticated)
            {
                return;
            }

            // Don't create a secure cookie in an unsecured connection
            if (cookieBuilder.SecurePolicy == CookieSecurePolicy.Always && !httpContext.Request.IsHttps)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(anonymousId))
            {
                // Creates a new identity
                anonymousId = Guid.NewGuid().ToString();

                // Adds the feature to request collection
                httpContext.Features.Set<IAnonymousIdFeature>(new AnonymousIdFeature()
                {
                    AnonymousId = anonymousId
                });
            }
            else
            {
                // Sliding expiration is not required for this request
                if (decodedValue != null 
                    && decodedValue.ExpireDate > now 
                    && decodedValue.ExpireDate - now > cookieBuilder.Expiration / 2)
                {
                    return;
                }
            }

            // Appends the new cookie
            CookieOptions options = cookieBuilder.Build(httpContext);
            AnonymousIdData data = new AnonymousIdData(anonymousId, options.Expires.Value.DateTime);
            encodedValue = AnonymousIdEncoder.Encode(data);
            httpContext.Response.Cookies.Append(cookieBuilder.Name, encodedValue, options);
        }

        public static void ClearAnonymousId(HttpContext httpContext, AnonymousIdCookieOptions cookieOptions)
        {
            if (!string.IsNullOrWhiteSpace(httpContext.Request.Cookies[cookieOptions.Name]))
            {
                httpContext.Response.Cookies.Delete(cookieOptions.Name);
            }
        }
    }
}