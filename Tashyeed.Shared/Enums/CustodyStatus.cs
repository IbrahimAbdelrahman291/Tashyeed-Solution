using System;
using System.Collections.Generic;
using System.Text;

namespace Tashyeed.Shared.Enums
{
    public enum CustodyStatus
    {
        Pending = 1,    // مدير المشروع بعت الريكويست
        Confirmed = 2,  // الشخص أكد الاستلام
        Rejected = 3    // الشخص رفض
    }
}
