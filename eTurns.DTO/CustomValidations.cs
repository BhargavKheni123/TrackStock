namespace eTurns.Validators
{
    //[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    //public class PasswordRequiredAttribute : ValidationAttribute, IClientValidatable
    //{
    //    private long? _ID;
    //    private string _Password, _RePassword;

    //    public long? ID
    //    {
    //        get { return _ID; }
    //        set
    //        {
    //            _ID = value;
    //        }
    //    }

    //    public string Password
    //    {
    //        get { return _Password; }
    //        set
    //        {

    //            _Password = value;
    //        }
    //    }

    //    public string RePassword
    //    {
    //        get { return _RePassword; }
    //        set
    //        {

    //            _RePassword = value;
    //        }
    //    }

    //    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,ControllerContext context)
    //    {
    //        return new[]
    //            {
    //                new ModelClientValidationRangeDateRule(FormatErrorMessage(metadata.GetDisplayName()), ID, Password, RePassword)
    //            };
    //    }

    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        ID = ReflectionHelper.GetPropValue(validationContext.ObjectInstance, "ID") as long?;
    //        Password = ReflectionHelper.GetPropValue(validationContext.ObjectInstance, "Password") as string;
    //        RePassword = ReflectionHelper.GetPropValue(validationContext.ObjectInstance, "ConfirmPassword") as string;
    //        if (!ID.HasValue || ID < 1)
    //        {
    //            if (!string.IsNullOrEmpty(Password))
    //            {
    //                return ValidationResult.Success;
    //            }
    //        }
    //        return new ValidationResult(FormatErrorMessage(string.Empty));           
    //    }
    //}
    //public class ModelClientValidationRangeDateRule
    //    : ModelClientValidationRule
    //{
    //    public ModelClientValidationRangeDateRule(string errorMessage, long? ID, string Password, string RePassword)
    //    {
    //        ErrorMessage = errorMessage;
    //        ValidationType = "Passwordrequired";
    //        ValidationParameters["ID"] = ID;
    //        ValidationParameters["Password"] = Password;
    //        ValidationParameters["RePassword"] = RePassword;
    //    }
    //}
    //public class ReflectionHelper
    //{
    //    public static object GetPropValue(object src, string propName)
    //    {
    //        return GetProperty(src, propName).GetValue(src, null);
    //    }

    //    public static PropertyInfo GetProperty(object src, string propName)
    //    {
    //        var result = src.GetType().GetProperty(propName);
    //        if (result == null)
    //        {
    //            throw new ApplicationException(string.Format("Type: {0} does not contain the Property: {1} ", src.GetType().FullName, propName));
    //        }

    //        return result;
    //    }
    //}
}
