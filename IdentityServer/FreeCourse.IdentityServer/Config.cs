// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace FreeCourse.IdentityServer
{
    public static class Config
    {
        // 38. derste eklendi.
        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("resource_catalog"){ Scopes = { "catalog_fullpermission" } },
                new ApiResource("resource_photo_stock"){ Scopes = { "photo_stock_fullpermission" } },
                new ApiResource("resource_basket"){ Scopes = { "basket_fullpermission" } },
                new ApiResource("resource_discount"){ Scopes = { "discount_fullpermission" } },
                new ApiResource("resource_order"){ Scopes = { "order_fullpermission" } },
                new ApiResource("resource_payment"){ Scopes = { "payment_fullpermission" } },
                new ApiResource("resource_gateway"){ Scopes = { "gateway_fullpermission" } },
                new ApiResource(IdentityServer4.IdentityServerConstants.LocalApi.ScopeName)
            };

        // 41. derste ici guncellendi.
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.Email(), // email claim nesnesi geliyor
                new IdentityResources.OpenId(), // sub claim nesnesi geliyor
                new IdentityResources.Profile(),
                new IdentityResource(){
                    Name = "roles",
                    DisplayName = "Roles",
                    Description = "Kullanici rolleri", 
                    UserClaims = new[]{ "role"} 
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("catalog_fullpermission", "Catalog API icin full erisim"),
                new ApiScope("photo_stock_fullpermission", "Photo Stock API icin full erisim"),
                new ApiScope("basket_fullpermission", "Basket API icin full erisim"),
                new ApiScope("discount_fullpermission", "Discount API icin full erisim"),
                new ApiScope("order_fullpermission", "Order API icin full erisim"),
                new ApiScope("payment_fullpermission", "FakePayment API icin full erisim"),
                new ApiScope("gateway_fullpermission", "Gateway API icin full erisim"),
                new ApiScope(IdentityServer4.IdentityServerConstants.LocalApi.ScopeName)
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientName = "Asp.Net Core Mvc",
                    ClientId = "WebMvcClient",
                    ClientSecrets = { new Secret("secret".Sha256())},
                    AllowedGrantTypes = { GrantType.ClientCredentials } ,
                    AllowedScopes = { 
                        "catalog_fullpermission",
                        "photo_stock_fullpermission",
                        "gateway_fullpermission",
                        IdentityServerConstants.LocalApi.ScopeName
                    }
                },
                new Client
                {
                    ClientName = "Asp.Net Core Mvc",
                    ClientId = "WebMvcClientForUser",
                    AllowOfflineAccess=true,
                    ClientSecrets = { new Secret("secret".Sha256())},
                    AllowedGrantTypes = {GrantType.ResourceOwnerPassword },
                    AllowedScopes = {
                        "basket_fullpermission",
                        //"discount_fullpermission",
                        "order_fullpermission",
                        //"payment_fullpermission",
                        "gateway_fullpermission",
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.LocalApi.ScopeName,
                        "roles"
                    },
                    AccessTokenLifetime =1*60*60,
                    RefreshTokenExpiration=TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime =(int)(DateTime.Now.AddDays(60)-DateTime.Now).TotalSeconds,
                    RefreshTokenUsage=TokenUsage.ReUse
                },
                new Client
                {
                    ClientName = "Token Exchange Client",
                    ClientId = "TokenExchangeClient",
                    ClientSecrets = { new Secret("secret".Sha256())},
                    AllowedGrantTypes = new []{ "urn:ietf:params:oauth:grant-type:token-exchange" } ,
                    AllowedScopes = {
                        "discount_fullpermission",
                        "payment_fullpermission",
                        IdentityServerConstants.StandardScopes.OpenId
                    }
                }
            };
    }
}