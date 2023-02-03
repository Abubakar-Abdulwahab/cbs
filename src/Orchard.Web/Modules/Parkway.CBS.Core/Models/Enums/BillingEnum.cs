using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models.Enums
{
    public enum DurationType
    {
        None = 0,
        NeverEnds = 1,
        EndsAfter = 2,
        EndsOn = 3,
    }

    public enum WeekDays
    {
        None = 0,
        FirstDayOfTheMonth = 1,
        FirstWeekDayOfTheMonth = 2,
        LastDayOfTheMonth = 3,
        LastDayOfWeekDayOfTheMonth = 4,
        FirstDay = 5,
        SecondDay = 6,
        ThirdDay = 7,
        FourthDay = 8,
        LastDay = 9,
    }
        
    public enum BillingDiscountType
    {
        None = 0,
        Flat = 1,
        Percent = 2,
    }

    public enum PenaltyValueType
    {
        None = 0,
        FlatRate = 1,
        Percentage = 2,
    }

    public enum EffectiveFromType
    {
        None = 0,
        Days = 1,
        Weeks = 2,
        Months = 3,
        Years = 4,
    }
}