namespace PulseCheckWorkerService.Util
{
    public struct EnumHelper
    {
        public static class Environment
        {
            public static string Production => "prod";
            public static string UAT => "uat";

            public static string Development => "dev";
        }
        //public enum Environment
        //{
        //    sandbox,
        //    production
        //}

        public enum TransacType
        {
            New,
            Delete,
            Redeem
        }
        public enum VoucherStatus
        {
            Active,
            Deactivated,
            Redeemed
        }

        public enum RoleName
        {
            Admin,
            User
        }
        public enum AccountStatus
        {
            Active,
            Deactivated,
            Deleted
        }

        public enum CampaignStatus
        {
            Active,
            Deactivated,
            Freeze
        }

        public enum AllocationStatus
        {
            Free,
            Allocated
            
        }

        public enum ResultStatus
        {
            Success,
            Fail

        }
    }
}
