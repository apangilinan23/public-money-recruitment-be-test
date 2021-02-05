using System;
using System.Collections.Generic;
using VacationRental.Api.Models;

namespace VacationRental.Api.Services
{
    public class CalendarService : ICalendarService
    {
        public CalendarViewModel GetCalendarViewModelResult(IDictionary<int, BookingViewModel> bookings, IDictionary<int, RentalViewModel> rentals, int nights , int rentalId, DateTime start)
        {
            var result = new CalendarViewModel
            {
                RentalId = rentalId,
                Dates = new List<CalendarDateViewModel>()
            };
            for (var i = 0; i < nights; i++)
            {
                var date = new CalendarDateViewModel
                {
                    Date = start.Date.AddDays(i),
                    Bookings = new List<CalendarBookingViewModel>(),
                    PreparationTimes = new List<PreparationTime>()
                };

                foreach (var booking in bookings.Values)
                {
                    var prepDayCount = rentals[booking.RentalId].PreparationTimeInDays;
                    var bookingEndDate = prepDayCount > 0 ? booking.Start.AddDays(booking.Nights) : new DateTime();
                    var prepDaysEnd = bookingEndDate.AddDays(prepDayCount);

                    if (booking.RentalId == rentalId
                        && booking.Start <= date.Date && booking.Start.AddDays(booking.Nights) > date.Date)
                    {
                        date.Bookings.Add(new CalendarBookingViewModel { Id = booking.Id, Unit = booking.Unit });
                    }
                    else
                    {
                        if (bookingEndDate != new DateTime() && (prepDaysEnd > date.Date && date.Date >= booking.Start))
                        {
                            date.PreparationTimes.Add(new PreparationTime { Unit = booking.Unit });
                        }
                    }
                }

                result.Dates.Add(date);
            }

            return result;
        }
    }
}
