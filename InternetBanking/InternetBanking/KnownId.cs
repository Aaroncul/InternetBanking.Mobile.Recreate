using System;

namespace InternetBanking
{
    public static class KnownId
    {
        public static string IsFirstRunKey = Guid
            .Parse("0F306B9D-D51B-49AA-B69B-A629786ED6CA")
            .ToString();

        public static string IsSignedInKey = Guid
            .Parse("89F96A3D-1C4E-442A-8A1E-FDD59DA42E55")
            .ToString();

        public static string MemberNumberKey = Guid
            .Parse("0a3d36b9-8352-44b4-834d-bd3cb011c857")
            .ToString();

        public static string CustomerIdKey = Guid
            .Parse("0D35A6C7-818A-4ED8-A346-1BCD2639538C")
            .ToString();

        public static string PersonIdKey = Guid
            .Parse("68400980-6859-48EA-9204-C9945492382C")
            .ToString();

        public static string RememberPinKey = Guid
            .Parse("B699391D-C2D1-42BC-B901-7C1CA2460243")
            .ToString();

        public static string NameKey = Guid
            .Parse("36FA54E2-D1F7-47AB-94F7-971BB3CE1EDC")
            .ToString();

        public static string EmailKey = Guid
            .Parse("EE07E2CC-8782-4C9F-991C-1CF082F70180")
            .ToString();

        public static string MobileKey = Guid
            .Parse("4C6B3A47-6AB2-44D4-B3E5-34505E4D9E49")
            .ToString();

        public static string DateOfBirthKey = Guid
            .Parse("645A72A4-16BB-4FC9-9A24-45F61B9171FF")
            .ToString();

        public static string TwoFactorCodeKey = Guid
            .Parse("BA2F0FE5-8B49-4F71-98A3-17674E4CAC47")
            .ToString();

        public static string SleepTimeKey = Guid
            .Parse("50e0538f-f6d7-4319-933f-5c041aa3bb1e")
            .ToString();

        public static string TwoFactorCodeTypeAddTFAId =
            Guid.Parse("095aa81f-e867-414d-bb67-2263e2ff2d4d").ToString();
    }
}