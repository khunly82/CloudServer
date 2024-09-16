using System.ComponentModel.DataAnnotations;

namespace CloudServer.Validators
{
    public class AfterDateAttribute : ValidationAttribute
    {

        private readonly DateTime _dateToCompare;

        public AfterDateAttribute(int nbDaysAfterToday = 0)
        {
            _dateToCompare = DateTime.Now.AddDays(nbDaysAfterToday);
        }

        public override bool IsValid(object? value)
        {
            DateTime? dateToCheck = value as DateTime?;
            if(dateToCheck is null)
            {
                return true;
            }
            return _dateToCompare <= dateToCheck;
        }
    }
}
