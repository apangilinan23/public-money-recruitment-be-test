using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Api.Models;

namespace VacationRental.Api.Services
{
    public class BookingService : IBookingService
    {
        public ResourceIdViewModel CreateBooking(BookingBindingModel model, IDictionary<int, RentalViewModel> rentals, IDictionary<int, BookingViewModel> bookings)
        {
            int unitToAssign = 1;
            for (var i = 0; i < model.Nights; i++)
            {
                var count = 0;
                foreach (var booking in bookings.Values)
                {
                    unitToAssign = booking.Unit;
                    var existingBookingPrepDayStart = booking.Start.AddDays(booking.Nights);
                    var bookingRentalPrepDayCount = rentals[booking.RentalId].PreparationTimeInDays;
                    var bookingPrepDaysRange = GetDateSpan(existingBookingPrepDayStart, bookingRentalPrepDayCount);

                    var paramModelDateRange = Enumerable.Range(0, 0 + model.Start.AddDays(model.Nights).Subtract(model.Start).Days)
                   .Select(offset => model.Start.AddDays(offset))
                   .ToArray();


                    if (IsUnitUnavailableForCurrentDay(model, booking, bookingPrepDaysRange, paramModelDateRange))
                    {
                        count++;
                        if (unitToAssign < rentals[model.RentalId].Units && (booking.Unit != unitToAssign + 1))
                            unitToAssign++;
                    }
                    else
                    {
                        //same unit                        
                        break;
                    }
                }

                if (count >= rentals[model.RentalId].Units)
                    throw new ApplicationException("Not available");
            }

            var key = new ResourceIdViewModel { Id = bookings.Keys.Count + 1 };
            var newBooking = new BookingViewModel
            {
                Id = key.Id,
                Nights = model.Nights,
                RentalId = model.RentalId,
                Start = model.Start.Date,
                Unit = unitToAssign
            };

            bookings.Add(key.Id, newBooking);

            return key;
        }

        private  bool IsUnitUnavailableForCurrentDay(BookingBindingModel model, BookingViewModel booking, DateTime[] bookingPrepDaysRange, DateTime[] paramModelDateRange)
        {
            return booking.RentalId == model.RentalId
                                    && (booking.Start <= model.Start.Date && booking.Start.AddDays(booking.Nights) > model.Start.Date)
                                    || (booking.Start < model.Start.AddDays(model.Nights) && booking.Start.AddDays(booking.Nights) >= model.Start.AddDays(model.Nights))
                                    || (booking.Start > model.Start && booking.Start.AddDays(booking.Nights) < model.Start.AddDays(model.Nights))
                                    || paramModelDateRange.Any(bdr => bookingPrepDaysRange.Select(dd => dd).Contains(bdr));
        }

        private  DateTime[] GetDateSpan(DateTime existingBookingPrepDayStart, int bookingRentalPrepDayCount)
        {
            return bookingRentalPrepDayCount > 1 ? Enumerable.Range(0, 0 + existingBookingPrepDayStart.AddDays(bookingRentalPrepDayCount).Subtract(existingBookingPrepDayStart).Days)
           .Select(offset => existingBookingPrepDayStart.AddDays(offset))
           .ToArray() : Enumerable.Range(0, 1 + existingBookingPrepDayStart.Subtract(existingBookingPrepDayStart).Days)
           .Select(offset => existingBookingPrepDayStart.AddDays(offset))
           .ToArray();
        }
    }
}
