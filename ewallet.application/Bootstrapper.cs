using System.Web.Mvc;
using ewallet.business.Balance;
using ewallet.business.KYC;
using Unity;
using Unity.AspNet.Mvc;
using ewallet.business.Login;
using ewallet.business.User;
using ewallet.business.Services;
using ewallet.business.Distributor;
using ewallet.business.Gateway;
using ewallet.business.Role;
using ewallet.business.Common;
using ewallet.business.Commission;
using ewallet.business.Bank;
using ewallet.business.Card;
using ewallet.business.Client;
using ewallet.business.DynamicReport;
using ewallet.business.Log;
using ewallet.business.SubDistributor;
using ewallet.business.Mobile;
using ewallet.business.StaticData;
using ewallet.business.AgentManagement;
using ewallet.business.SubAgentManagement;
using ewallet.business.Dashboard;
using ewallet.business.TransactionLimit;
using ewallet.business.DistributorManagement;
using ewallet.business.Client.MobileTopup;
using ewallet.business.Merchant;
using ewallet.business.Notification;
using ewallet.business.SubDistributorManagement;
using ewallet.business.MobileNotification;
using ewallet.business.AppVersionControl;
using ewallet.business.Client.Commission;
using ewallet.business.Agent_Commission;
using ewallet.business.LoadBalance;

namespace ewallet.application
{
    public class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers
            container.RegisterType<ILoginUserBusiness, LoginUserBusiness>();
            container.RegisterType<IDistributorBusiness, DistributorBusiness>();
            container.RegisterType<IUserBusiness, UserBusiness>();
            container.RegisterType<IKycBusiness, KycBusiness>();
            container.RegisterType<IServicesManagementBusiness, ServicesManagementBusiness>();
            container.RegisterType<IRoleBusiness, RoleBusiness>();
            container.RegisterType<IGatewayBusiness, GatewayBusiness>();
            container.RegisterType<ICommonBusiness, CommonBusiness>();
            container.RegisterType<IBalanceBusiness, BalanceBusiness>();
            container.RegisterType<ICommissionBusiness, CommissionBusiness>();
            container.RegisterType<IBankBusiness, BankBusiness>();
            container.RegisterType<IWalletUserBusiness, WalletUserBusiness>();
            container.RegisterType<ISubDistributorBusiness, SubDistributorBusiness>();
            container.RegisterType<ICardBusiness, CardBusiness>();
            container.RegisterType<IMobileTopUpPaymentBusiness, MobileTopUpPaymentBusiness>();
            container.RegisterType<IDynamicReportBusiness, DynamicReportBusiness>();
            container.RegisterType<business.Agent.IAgentBusiness, business.Agent.AgentBusiness>();
            //container.RegisterType<business.NewAgent.IAgentBusiness, business.NewAgent.AgentBusiness>();
            container.RegisterType<IStaticDataBusiness, StaticDataBusiness>();
            container.RegisterType<IActivityLogBusiness, ActivityLogBusiness>();
            container.RegisterType<IMobilePaymentBusiness, MobilePaymentBusiness>();
            container.RegisterType<IClientManagementBusiness, ClientManagementBusiness>();
            container.RegisterType<IAgentManagementBusiness, AgentManagementBusiness>();
            container.RegisterType<ISubAgentManagementBusiness, SubAgentManagementBusiness>();
            container.RegisterType<IDashboardBusiness, DashboardBusiness>();
            container.RegisterType<ITransactionLimitBusiness, TransactionLimitBusiness>();
            container.RegisterType<IAccessLogBusiness, AccessLogBusiness>();
            container.RegisterType<IErrorLogBusiness, ErrorLogBusiness>();
            container.RegisterType<IApiLogBusiness, ApiLogBusiness>();
            container.RegisterType<IDistributorManagementBusiness, DistributorManagementBusiness>();
            container.RegisterType<ISubDistributorManagementBusiness, SubDistributorManagementBusiness>();
            container.RegisterType<IMerchantBusiness, MerchantBusiness>();
            container.RegisterType<INotificationBusiness, NotificationBusiness>();
            container.RegisterType<IMobileNotificationBusiness, MobileNotificationBusiness>();
            container.RegisterType<IAppVersionControlBusiness, AppVersionControlBusiness>();
            container.RegisterType<IClientCommissionBusiness, ClientCommissionBusiness>();
            container.RegisterType<IAgentCommissionBusiness, AgentCommissionBusiness>();
            container.RegisterType<ILoadBalanceBusiness, LoadBalanceBusiness>();




            return container;
        }
    }
}

