using Microsoft.AspNetCore.Http;
using System;

namespace Onots.AspNetCore.Identity.Anonymous
{
    public class AnonymousIdCookieOptionsBuilder
    {
        private const string DEFAULT_COOKIE_NAME = ".ASPXANONYMOUS";
        private const string DEFAULT_COOKIE_PATH = "/";
        private const int DEFAULT_COOKIE_TIMEOUT_SECONDS = 100000;
        private const int MINIMUM_COOKIE_TIMEOUT_SECONDS = 1;
        private const int MAXIMUM_COOKIE_TIMEOUT_SECONDS = 60 * 60 * 24 * 365 * 2; // 2 years?
        private const CookieSecurePolicy DEFAULT_SECURE_POLICY = CookieSecurePolicy.SameAsRequest;
        
        private string cookieName;
        private string cookiePath;
        private int? cookieTimeout;
        private string cookieDomain;
        private CookieSecurePolicy? cookieSecurePolicy;

        public AnonymousIdCookieOptionsBuilder SetCustomCookieName(string cookieName)
        {
            this.cookieName = cookieName;
            return this;
        }

        public AnonymousIdCookieOptionsBuilder SetCustomCookiePath(string cookiePath)
        {
            this.cookiePath = cookiePath;
            return this;
        }

        public AnonymousIdCookieOptionsBuilder SetCustomCookieExpiration(int cookieTimeoutInSeconds)
        {
            this.cookieTimeout = Math.Min(Math.Max(MINIMUM_COOKIE_TIMEOUT_SECONDS, cookieTimeoutInSeconds), MAXIMUM_COOKIE_TIMEOUT_SECONDS);
            return this;
        }

        public AnonymousIdCookieOptionsBuilder SetCustomCookieDomain(string cookieDomain)
        {
            this.cookieDomain = cookieDomain;
            return this;
        }

        public AnonymousIdCookieOptionsBuilder SetCustomCookieSecurePolicy(CookieSecurePolicy cookieSecurePolicy)
        {
            this.cookieSecurePolicy = cookieSecurePolicy;
            return this;
        }

        public CookieBuilder Build()
        {
            CookieBuilder builder = new CookieBuilder()
            {
                Name = cookieName ?? DEFAULT_COOKIE_NAME,
                Path = cookiePath ?? DEFAULT_COOKIE_PATH,
                Expiration = TimeSpan.FromSeconds(cookieTimeout ?? DEFAULT_COOKIE_TIMEOUT_SECONDS),
                //Expires = DateTime.UtcNow.AddSeconds(cookieTimeout ?? DEFAULT_COOKIE_TIMEOUT_SECONDS),
                SecurePolicy = cookieSecurePolicy ?? DEFAULT_SECURE_POLICY
            };
            //AnonymousIdCookieOptions options = new AnonymousIdCookieOptions
            //{
            //    Name = cookieName ?? DEFAULT_COOKIE_NAME,
            //    Path = cookiePath ?? DEFAULT_COOKIE_PATH,
            //    Timeout = cookieTimeout ?? DEFAULT_COOKIE_TIMEOUT_SECONDS,
            //    Expires = DateTime.UtcNow.AddSeconds(cookieTimeout ?? DEFAULT_COOKIE_TIMEOUT),
            //    Secure = cookieSecurePolicy ?? DEFAULT_SECURE_POLICY
            //};

            if (!string.IsNullOrWhiteSpace(cookieDomain))
            {
                builder.Domain = cookieDomain;
            }

            return builder;
        }
    }
}