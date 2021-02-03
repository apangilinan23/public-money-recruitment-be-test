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
            for (var i = 0; i < model.Nights; i++)
            {
                var count = 0;
                foreach (var booking in bookings.Values)
                {
                    var prepDayStart = booking.Start.AddDays(booking.Nights);
                    var modelPrepDayStart = model.Start.AddDays(model.Nights);
                    var bookingRentalPrepDayCount = rentals[booking.RentalId].PreparationTimeInDays;


                    //prepDay of booking

                    var bookingPrepDaysRange = bookingRentalPrepDayCount > 1 ? Enumerable.Range(0, 0 + prepDayStart.AddDays(bookingRentalPrepDayCount).Subtract(prepDayStart).Days)
                    .Select(offset => prepDayStart.AddDays(offset))
                    .ToArray() : Enumerable.Range(0, 1 + prepDayStart.Subtract(prepDayStart).Days)
                    .Select(offset => prepDayStart.AddDays(offset))
                    .ToArray();

                    //prepDay of model
                    var prepDaysRange = bookingRentalPrepDayCount > 1 ? Enumerable.Range(0, 0 + modelPrepDayStart.AddDays(bookingRentalPrepDayCount).Subtract(modelPrepDayStart).Days)
                    .Select(offset => modelPrepDayStart.AddDays(offset))
                    .ToArray() : Enumerable.Range(0, 1 + modelPrepDayStart.Subtract(modelPrepDayStart).Days)
                    .Select(offset => modelPrepDayStart.AddDays(offset))
                    .ToArray();

                    if (booking.RentalId == model.RentalId
                        && (booking.Start <= model.Start.Date && booking.Start.AddDays(booking.Nights) > model.Start.Date)
                        || (booking.Start < model.Start.AddDays(model.Nights) && booking.Start.AddDays(booking.Nights) >= model.Start.AddDays(model.Nights))
                        || (booking.Start > model.Start && booking.Start.AddDays(booking.Nights) < model.Start.AddDays(model.Nights)))
                    {
                        count++;
                    }

                    var paramModelDateRange = Enumerable.Range(0, 1 + model.Start.AddDays(model.Nights).Subtract(model.Start).Days)
                    .Select(offset => model.Start.AddDays(offset))
                    .ToArray();

                    var paramBookingDateRange = Enumerable.Range(0, 1 + booking.Start.AddDays(booking.Nights).Subtract(booking.Start).Days)
                    .Select(offset => booking.Start.AddDays(offset))
                    .ToArray();

                    //model overlap with booking prep date
                    if (paramModelDateRange.Any(bdr => bookingPrepDaysRange.Select(dd => dd).Contains(bdr)))
                        throw new ApplicationException("Booking overlaps with an existing preparation time");

                    //model prepDay overlap with existing booking
                    if (paramBookingDateRange.Any(bdr => prepDaysRange.Select(dd => dd).Contains(bdr)))
                        throw new ApplicationException("Preptime overlaps with an existing booking");
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
                Unit = key.Id
            };

            if (newBooking.Unit > rentals[model.RentalId].Units)
                throw new ApplicationException("Not available");
            else
                bookings.Add(key.Id, newBooking);

            return key;
        }
    }
}
