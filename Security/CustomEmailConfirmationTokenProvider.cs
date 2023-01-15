using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EF_DotNetCore.Security
{
    //public class CustomEmailDataProtectionTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    //{
    //    public CustomEmailDataProtectionTokenProvider(IDataProtectionProvider dataProtectionProvider,
    //        IOption<CustomEmailDataProtectionTokenProviderOptions> option) : base(dataProtectionProvider,option)
    //    {

    //    }
    //}

    public class CustomEmailConfirmationTokenProvider<TUser>
    : DataProtectorTokenProvider<TUser> where TUser : class
    {
       // ILogger<CustomEmailConfirmationTokenProvider<TUser>> logger;
        public CustomEmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
                                        IOptions<CustomEmailConfirmationTokenProviderOptions> options,ILogger<CustomEmailConfirmationTokenProvider<TUser>> logger)
            : base(dataProtectionProvider,options,logger)
        { }
    }

}
