namespace WhatIsOn.Infrastructure.Persistence.Seed;

/// <summary>
/// Stable identifiers for the development seed. Centralized here so seeders
/// (Users → Speakers → Events → Layouts) can reference each other without
/// passing dictionaries around, and so manual API testing can rely on
/// predictable URLs.
/// </summary>
internal static class SeedData
{
    internal static class Users
    {
        public static readonly Guid Organizer = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid Regular = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public static readonly Guid Vip = Guid.Parse("33333333-3333-3333-3333-333333333333");
        public static readonly Guid OtherOrganizer = Guid.Parse("55555555-5555-5555-5555-555555555555");
    }

    internal static class Speakers
    {
        public static readonly Guid JaneDoe = Guid.Parse("44444444-4444-4444-4444-444444444444");
        public static readonly Guid JohnSmith = Guid.Parse("44444444-4444-4444-4444-000000000001");
        public static readonly Guid AishaKhan = Guid.Parse("44444444-4444-4444-4444-000000000002");
        public static readonly Guid MarcusLee = Guid.Parse("44444444-4444-4444-4444-000000000003");
        public static readonly Guid ElenaCosta = Guid.Parse("44444444-4444-4444-4444-000000000004");
        public static readonly Guid TomasBrandt = Guid.Parse("44444444-4444-4444-4444-000000000005");
    }

    internal static class Events
    {
        public static readonly Guid AwsSummit = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        public static readonly Guid VipRoundtable = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        public static readonly Guid FrontendConf = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
        public static readonly Guid PastSummit = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
    }

    internal static class Sessions
    {
        public static readonly Guid AwsServerless = Guid.Parse("aaaaaaaa-1111-1111-1111-111111111111");
        public static readonly Guid AwsMl = Guid.Parse("aaaaaaaa-1111-1111-1111-111111111112");
        public static readonly Guid AwsDevRel = Guid.Parse("aaaaaaaa-1111-1111-1111-111111111113");
        public static readonly Guid AwsSecurity = Guid.Parse("aaaaaaaa-1111-1111-1111-111111111114");

        public static readonly Guid VipRoadmap = Guid.Parse("bbbbbbbb-1111-1111-1111-111111111111");
        public static readonly Guid VipMa = Guid.Parse("bbbbbbbb-1111-1111-1111-111111111112");
        public static readonly Guid VipDinner = Guid.Parse("bbbbbbbb-1111-1111-1111-111111111113");

        public static readonly Guid FrontendReact = Guid.Parse("dddddddd-1111-1111-1111-111111111111");
        public static readonly Guid FrontendSolid = Guid.Parse("dddddddd-1111-1111-1111-111111111112");
        public static readonly Guid FrontendWebComponents = Guid.Parse("dddddddd-1111-1111-1111-111111111113");

        public static readonly Guid PastRetro = Guid.Parse("eeeeeeee-1111-1111-1111-111111111111");
        public static readonly Guid PastNetworking = Guid.Parse("eeeeeeee-1111-1111-1111-111111111112");
    }

    internal static class Layouts
    {
        public static readonly Guid AwsLayout = Guid.Parse("cccccccc-cccc-cccc-cccc-000000000001");
        public static readonly Guid VipLayout = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        public static readonly Guid FrontendLayout = Guid.Parse("cccccccc-cccc-cccc-cccc-000000000003");
    }
}
