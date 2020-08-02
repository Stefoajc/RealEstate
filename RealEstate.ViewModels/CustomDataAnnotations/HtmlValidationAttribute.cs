using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RealEstate.ViewModels.CustomDataAnnotations
{
    public class HtmlValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }
            string html = (string) value;
            Regex rRemScript = new Regex(@"<script[^>]*>[\s\S]*?</script>");
            return !rRemScript.IsMatch(html);
        }
    }
}