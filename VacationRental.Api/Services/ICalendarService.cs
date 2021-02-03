using System;
using System.Collections.Generic;
using VacationRental.Api.Models;

namespace VacationRental.Api.Services
{
    public interface ICalendarService
    {
        CalendarViewModel GetCalendarViewModelResult(IDictionary<int, BookingViewModel> bookings, IDictionary<int, RentalViewModel> rentals, int nights, int rentalId, DateTime start);
    }
}
