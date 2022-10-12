using System.ComponentModel.DataAnnotations;

namespace EF_DotNetCore.Utilities
{
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        readonly string _domainName;
        public ValidEmailDomainAttribute (string alloweddomain)
        {
            _domainName = alloweddomain;
        }

        public override bool IsValid(object value)
        {
            string[] strings=value.ToString().Split ("@");

            return strings[1].ToUpper() == this._domainName.ToUpper();
        }
    }
}
